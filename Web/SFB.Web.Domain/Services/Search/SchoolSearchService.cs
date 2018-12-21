using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDog.Search;
using RedDog.Search.Http;
using RedDog.Search.Model;
using RestSharp;
using SFB.Web.Domain.Helpers;
using SFB.Web.Domain.Models;
using SFB.Web.Common;

namespace SFB.Web.Domain.Services.Search
{
    public class SchoolSearchService : ISchoolSearchService
    {
        private readonly string _key;
        private readonly string _searchInstance;
        private readonly string _index;

        private readonly string[][] _aliases = new[]
        {
            new[] {"st. ", "st ", "saint "}
        };

        private const string GeoDistanceLocationSearchFormat =
            "geo.distance(Location,geography'POINT({1} {0})') le {2}";

        private const string GeoDistanceLocationOrderFormat = "geo.distance(Location,geography'POINT({1} {0})') asc";

        public SchoolSearchService(string searchInstance, string key, string index)
        {
            this._searchInstance = searchInstance;
            this._key = key;
            this._index = index;
        }

        public async Task<dynamic> SuggestSchoolByName(string name)
        {
            var connection = ApiConnection.Create(_searchInstance, _key);
            var client = new IndexQueryClient(connection);

            Func<SuggestionResultRecord, ExpandoObject> processResult = r =>
            {
                dynamic retVal = new ExpandoObject();
                var postCode = r.Properties[$"{EdubaseDBFieldNames.POSTCODE}"] as string;
                var town = r.Properties[$"{EdubaseDBFieldNames.TOWN}"] as string;
                var schoolName = r.Properties[$"{EdubaseDBFieldNames.ESTAB_NAME}"] as string;
                retVal.Id = r.Properties[$"{EdubaseDBFieldNames.URN}"]?.ToString();

                if (!string.IsNullOrWhiteSpace(postCode) && !string.IsNullOrWhiteSpace(town)) // town and postcode
                {
                    retVal.Text = $"{schoolName} ({town}, {postCode})";
                }
                else if (!string.IsNullOrWhiteSpace(postCode) && string.IsNullOrWhiteSpace(town)) // just postcode
                {
                    retVal.Text = $"{schoolName} ({postCode})";
                }
                else if (string.IsNullOrWhiteSpace(postCode) && !string.IsNullOrWhiteSpace(town)) // just town
                {
                    retVal.Text = $"{schoolName} ({town})";
                }
                else if (string.IsNullOrWhiteSpace(postCode) && string.IsNullOrWhiteSpace(town)
                ) // neither town nor post code
                {
                    retVal.Text = schoolName;
                }
                if (r.Properties[$"{EdubaseDBFieldNames.ESTAB_STATUS}"]?.ToString() == "Closed")
                {
                    retVal.Text += " (Closed)";
                }

                return retVal;
            };

            var response = await client.SuggestAsync(_index, new SuggestionQuery(name)
                .SuggesterName("nameSuggester")
                .Fuzzy(false)
                .Select($"{EdubaseDBFieldNames.ESTAB_NAME}")
                .Select($"{EdubaseDBFieldNames.URN}")
                .Select($"{EdubaseDBFieldNames.TOWN}")
                .Select($"{EdubaseDBFieldNames.POSTCODE}")
                .Select($"{EdubaseDBFieldNames.ESTAB_STATUS}")
                //.Filter("EstablishmentStatus eq 'Open'" +
                //        " and TypeOfEstablishment ne 'Higher Education Institutions'" +
                //        " and TypeOfEstablishment ne 'LA Nursery School'" +
                //        " and TypeOfEstablishment ne 'Other Independent School'" +
                //        " and TypeOfEstablishment ne 'Other Independent Special School'" +
                //        " and TypeOfEstablishment ne 'Welsh Establishment'" +
                //        " and TypeOfEstablishment ne 'Special Post 16 Institution'" +
                //        " and TypeOfEstablishment ne 'Sixth Form Centres'" +
                //        " and TypeOfEstablishment ne 'Service Childrens Education'" +
                //        " and TypeOfEstablishment ne 'Secure Units'" +
                //        " and TypeOfEstablishment ne 'Offshore Schools'" +
                //        " and TypeOfEstablishment ne 'Institution funded by other Government Department'" +
                //        " and TypeOfEstablishment ne 'Free Schools - 16-19'" +
                //        " and TypeOfEstablishment ne 'British Schools Overseas'" +
                //        " and TypeOfEstablishment ne 'Academy 16-19 Sponsor Led'" +
                //        " and TypeOfEstablishment ne 'Academy 16-19 Converter'" +
                //        " and StatutoryLowAge ne '16'" +
                //        " and StatutoryLowAge ne '17'" +
                //        " and StatutoryLowAge ne '18'" +
                //        " and StatutoryLowAge ne '19'")
                //.Filter("StatutoryHighAge ne '1'" +
                //        " and StatutoryHighAge ne '2'" +
                //        " and StatutoryHighAge ne '3'" +
                //        " and StatutoryHighAge ne '4'" +
                //        " and StatutoryHighAge ne '5'")//Todo: Remove .Do not filter out nurseries.
                .SearchField($"{EdubaseDBFieldNames.ESTAB_NAME}")
                .Top(10));

            if (!response.IsSuccess)
            {
                throw new ApplicationException(
                    $"Edubase school suggestion error {response.Error.Code}: {response.Error.Message}");
            }
            var results = response.Body.Records;

            var matches = (from r in results
                select processResult(r));

            dynamic ret = new ExpandoObject();
            ret.Matches = matches;
            return ret;
        }

        public async Task<dynamic> SearchSchoolByName(string name, int skip, int take, string @orderby,
            NameValueCollection queryParams)
        {
            if (name.Length > 2)
            {
                var facets = new[] {$"{EdubaseDBFieldNames.TYPE_OF_ESTAB}", $"{EdubaseDBFieldNames.OVERALL_PHASE}", $"{EdubaseDBFieldNames.RELIGIOUS_CHARACTER}", $"{EdubaseDBFieldNames.OFSTED_RATING}"};
                var exactMatches = await ExecuteSearch(_index, $"{name}", $"{EdubaseDBFieldNames.ESTAB_NAME}",
                    ConstructApiFilterParams(queryParams), orderby, skip, take, facets);
                return exactMatches;
            }

            return new QueryResultsModel(0, null, new List<IDictionary<string, object>>(), 0, 0);
        }

        public async Task<dynamic> SearchSchoolByLaCode(string laCode, int skip, int take, string orderby,
            NameValueCollection queryParams)
        {
            var facets = new[] { $"{EdubaseDBFieldNames.TYPE_OF_ESTAB}", $"{EdubaseDBFieldNames.OVERALL_PHASE}", $"{EdubaseDBFieldNames.RELIGIOUS_CHARACTER}", $"{EdubaseDBFieldNames.OFSTED_RATING}" };
            var exactMatches = await ExecuteSearch(_index, $"{laCode}", $"{EdubaseDBFieldNames.LA_CODE}", ConstructApiFilterParams(queryParams),
                orderby, skip, take, facets);

            return exactMatches;
        }

        public async Task<dynamic> SearchSchoolByLatLon(string lat, string lon, decimal distance, int skip, int take,
            string orderby,
            NameValueCollection queryParams)
        {
            return await FindNearestSchools(lat, lon, distance, skip, take, orderby, queryParams);
        }

        public async Task<dynamic> SearchSchoolByMatNo(string matNo, int skip, int take, string @orderby, NameValueCollection queryParams)
        {
            var facets = new[] { $"{EdubaseDBFieldNames.OVERALL_PHASE}", $"{EdubaseDBFieldNames.OFSTED_RATING}", $"{EdubaseDBFieldNames.GENDER}" };
            var exactMatches = await ExecuteSearch(_index, $"{matNo}", $"{EdubaseDBFieldNames.MAT_NUMBER}",
                ConstructApiFilterParams(queryParams), orderby, skip, take, facets);
            return exactMatches;
        }

        public async Task<QueryResultsModel> FindNearestSchools(string lat, string lon, decimal distance, int skip,
            int take, string orderby, NameValueCollection queryParams)
        {
            var formattedLat = double.Parse(lat).ToString();
            var formattedLon = double.Parse(lon).ToString();
            return await ExecuteGeoSpatialSearch(_index, formattedLat, formattedLon,
                distance, ConstructApiFilterParams(queryParams), orderby, skip, take);
        }

        private async Task<QueryResultsModel> ExecuteSearch(string index, string query, string searchFields,
            string filter, string orderBy, int skip, int take, IEnumerable<string> facets)
        {
            var set = _aliases.FirstOrDefault(x => x.Any(a => AliasFound(query, a)));
            if (set != null)
            {
                var word = set.FirstOrDefault(x => AliasFound(query, x));
                query = query.Replace(word, string.Concat("(", string.Join("|", set), ") "), true);
            }

            var connection = ApiConnection.Create(_searchInstance, _key);
            var searchFieldsArray = searchFields.Split(',');
            var orderByField = string.IsNullOrEmpty(orderBy) && searchFieldsArray.Length > 0
                ? searchFieldsArray[0].Replace($"{EdubaseDBFieldNames.ESTAB_NAME}", $"{EdubaseDBFieldNames.ESTAB_NAME_UPPERCASE}")
                : orderBy.Replace($"{EdubaseDBFieldNames.ESTAB_NAME}", $"{EdubaseDBFieldNames.ESTAB_NAME_UPPERCASE}");

            var client = new IndexQueryClient(connection);
            var searchQueryModel = new SearchQuery(query);
            searchQueryModel.SearchFields = searchFields;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                searchQueryModel = searchQueryModel.Filter(filter);
            }
            searchQueryModel.Facets = facets;
            searchQueryModel.OrderBy = orderByField;
            searchQueryModel = searchQueryModel.Mode(SearchMode.All).Count(true).Skip(skip).Top(take);

            var response = await client.SearchAsync(index, searchQueryModel);

            if (!response.IsSuccess)
            {
                throw new ApplicationException(
                    $"Edubase school search error {response.Error.Code}: {response.Error.Message}");
            }

            return new QueryResultsModel(response.Body.Count, response.Body.Facets,
                response.Body.Records.Select(x => x.Properties), take, skip);
        }

        public async Task<QueryResultsModel> ExecuteGeoSpatialSearch(string index, string lat, string lon,
            decimal distance, string filter, string orderBy, int skip, int take)
        {
            const string search = "*";
            var latitude = double.Parse(lat);
            var longitude = double.Parse(lon);

            var filterBuilder = new StringBuilder();
            var orderByField = string.IsNullOrEmpty(orderBy)
                ? string.Format(GeoDistanceLocationOrderFormat, latitude, longitude)
                : orderBy;

            filterBuilder.AppendFormat(GeoDistanceLocationSearchFormat, latitude, longitude, distance);
            if (!string.IsNullOrEmpty(filter))
            {
                filterBuilder.AppendFormat(" and " + filter);
            }

            var connection = ApiConnection.Create(_searchInstance, _key);

            var client = new IndexQueryClient(connection);

            var response = await client.SearchAsync(index, new SearchQuery(search)
                .OrderBy(orderByField)
                .Facet($"{EdubaseDBFieldNames.TYPE_OF_ESTAB}")
                .Facet($"{EdubaseDBFieldNames.OVERALL_PHASE}")
                .Facet($"{EdubaseDBFieldNames.RELIGIOUS_CHARACTER}")
                .Facet($"{EdubaseDBFieldNames.OFSTED_RATING}")
                .Count(true)
                .Filter(filterBuilder.ToString())
                .Skip(skip)
                .Top(take));

            if (!response.IsSuccess)
            {
                Console.WriteLine("{0}: {1}", response.Error.Code, response.Error.Message);
            }

            var results = new QueryResultsModel(response.Body.Count, response.Body.Facets,
                CalcDistance(response.Body.Records.Select(x => x.Properties), lat, lon), take, skip)
            {
                QueryLat = latitude.ToString(),
                QueryLong = longitude.ToString()
            };
            return results;
        }

        private static IEnumerable<IDictionary<string, object>> CalcDistance(
            IEnumerable<IDictionary<string, object>> results, string lat, string lng)
        {
            var calcDistance = results as IDictionary<string, object>[] ?? results.ToArray();
            foreach (var result in calcDistance)
            {
                if (result.ContainsKey($"{EdubaseDBFieldNames.LOCATION}"))
                {
                    dynamic location = result[$"{EdubaseDBFieldNames.LOCATION}"];
                    if (location != null)
                    {
                        var coordinates = location.coordinates;
                        result.Add("distanceInMeters",
                            GeoCodeCalc.CalcDistance(coordinates[1].ToString(), coordinates[0].ToString(), lat, lng));
                    }
                }
            }

            return calcDistance;
        }

        private bool AliasFound(string query, string alias)
        {
            return query != null && (query.StartsWith(alias) || query.Contains(" " + alias));
        }

        private string ConstructApiFilterParams(NameValueCollection parameters)
        {
            if (parameters == null)
            {
                return string.Empty;
            }

            var queryFilter = new List<string>();

            //queryFilter.Add("OverallPhase ne '0'");
            //queryFilter.Add("EstablishmentStatus eq 'Open' or EstablishmentStatus eq 'Open, but proposed to close'");
            //queryFilter.Add("TypeOfEstablishment ne 'Higher Education Institutions'");
            //queryFilter.Add("TypeOfEstablishment ne 'LA Nursery School'");
            //queryFilter.Add("TypeOfEstablishment ne 'Other Independent School'");
            //queryFilter.Add("TypeOfEstablishment ne 'Other Independent Special School'");
            //queryFilter.Add("TypeOfEstablishment ne 'Welsh Establishment'");
            //queryFilter.Add("TypeOfEstablishment ne 'Special Post 16 Institution'");
            //queryFilter.Add("TypeOfEstablishment ne 'Sixth Form Centres'");
            //queryFilter.Add("TypeOfEstablishment ne 'Service Childrens Education'");
            //queryFilter.Add("TypeOfEstablishment ne 'Secure Units'");
            //queryFilter.Add("TypeOfEstablishment ne 'Offshore Schools'");
            //queryFilter.Add("TypeOfEstablishment ne 'Institution funded by other Government Department'");
            //queryFilter.Add("TypeOfEstablishment ne 'Free Schools - 16-19'");
            //queryFilter.Add("TypeOfEstablishment ne 'British Schools Overseas'");
            //queryFilter.Add("TypeOfEstablishment ne 'Academy 16-19 Sponsor Led'");
            //queryFilter.Add("TypeOfEstablishment ne 'Academy 16-19 Converter'");
            //queryFilter.Add("StatutoryLowAge ne '16'");
            //queryFilter.Add("StatutoryLowAge ne '17'");
            //queryFilter.Add("StatutoryLowAge ne '18'");
            //queryFilter.Add("StatutoryLowAge ne '19'");

            //queryFilter.Add("StatutoryHighAge ne '1'");
            //queryFilter.Add("StatutoryHighAge ne '2'");
            //queryFilter.Add("StatutoryHighAge ne '3'");
            //queryFilter.Add("StatutoryHighAge ne '4'");
            //queryFilter.Add("StatutoryHighAge ne '5'");//Todo: Remove .Do not filter out nurseries.

            if (parameters["openOnly"] != null)
            {
                var openOnly = false;
                bool.TryParse(parameters["openOnly"], out openOnly);
                if (openOnly)
                {
                    queryFilter.Add($"{EdubaseDBFieldNames.ESTAB_STATUS} eq 'Open' or {EdubaseDBFieldNames.ESTAB_STATUS} eq 'Open, but proposed to close'");
                }
            }

            if (parameters["schoollevel"] != null)
            {
                string[] values = ExtractValues(parameters["schoollevel"]);
                queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDBFieldNames.OVERALL_PHASE} eq '" + x + "'")));
            }

            if (parameters["schooltype"] != null)
            {
                string[] values = ExtractValues(parameters["schooltype"]);
                queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDBFieldNames.TYPE_OF_ESTAB} eq '" + x + "'")));
            }

            if (parameters["ofstedrating"] != null)
            {
                string[] values = ExtractValues(parameters["ofstedrating"]);
                values = values.Select(x => x == "6" ? "" : x).ToArray();
                queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDBFieldNames.OFSTED_RATING} eq '" + x + "'")));
            }

            if (parameters["faith"] != null)
            {
                string[] values = ExtractValues(parameters["faith"]);
                queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDBFieldNames.RELIGIOUS_CHARACTER} eq '" + x + "'")));
            }

            if (parameters["gender"] != null)
            {
                string[] values = ExtractValues(parameters["gender"]);
                queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDBFieldNames.GENDER} eq '" + x + "'")));
            }

            return string.Join(" and ", queryFilter.Select(x => "(" + x + ")"));
        }

        private string[] ExtractValues(string commaSeparatedValues)
        {
            return commaSeparatedValues.Split(',');
        }

    }
}