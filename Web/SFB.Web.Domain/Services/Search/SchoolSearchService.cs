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

namespace SFB.Web.Domain.Services.Search
{
    public class SchoolSearchService : ISchoolSearchService
    {
        private readonly string _key;
        private readonly string _searchInstance;
        private readonly string _index;
        private readonly string _googleApiKey;

        private readonly string[][] _aliases = new[]
        {
            new[] {"st. ", "st ", "saint "}
        };

        private const string GoogleGeocodingUrl = "https://maps.googleapis.com";

        private const string GoogleGeoCodingQUeryFormat =
            "/maps/api/geocode/json?address={0}&components=administrative_area_level_1:ENG|country:GB&key=";

        private const string GeoDistanceLocationSearchFormat =
            "geo.distance(Location,geography'POINT({1} {0})') le {2}";

        private const string GeoDistanceLocationOrderFormat = "geo.distance(Location,geography'POINT({1} {0})') asc";

        public SchoolSearchService(string searchInstance, string key, string index, string googleApiKey)
        {
            this._searchInstance = searchInstance;
            this._key = key;
            this._googleApiKey = googleApiKey;
            this._index = index;
        }

        public async Task<dynamic> SuggestSchoolByName(string name)
        {
            var connection = ApiConnection.Create(_searchInstance, _key);
            var client = new IndexQueryClient(connection);

            Func<SuggestionResultRecord, ExpandoObject> processResult = r =>
            {
                dynamic retVal = new ExpandoObject();
                var postCode = r.Properties["Postcode"] as string;
                var town = r.Properties["Town"] as string;
                var schoolName = r.Properties["EstablishmentName"] as string;
                retVal.Id = r.Properties["URN"]?.ToString();

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
                if (r.Properties["EstablishmentStatus"].ToString() == "Closed")
                {
                    retVal.Text += " (Closed)";
                }

                return retVal;
            };

            var response = await client.SuggestAsync(_index, new SuggestionQuery(name)
                .SuggesterName("nameSuggester")
                .Fuzzy(false)
                .Select("EstablishmentName")
                .Select("URN")
                .Select("Town")
                .Select("Postcode")
                .Select("EstablishmentStatus")
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
                .SearchField("EstablishmentName")
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
                var facets = new[] {"TypeOfEstablishment", "OverallPhase", "ReligiousCharacter", "OfstedRating"};
                var exactMatches = await ExecuteSearch(_index, $"{name}", "EstablishmentName",
                    ConstructApiFilterParams(queryParams), orderby, skip, take, facets);
                return exactMatches;
            }

            return new QueryResultsModel(0, null, new List<IDictionary<string, object>>(), 0, 0);
        }

        public async Task<dynamic> SearchSchoolByLaCode(string laCode, int skip, int take, string orderby,
            NameValueCollection queryParams)
        {
            var facets = new[] { "TypeOfEstablishment", "OverallPhase", "ReligiousCharacter", "OfstedRating" };
            var exactMatches = await ExecuteSearch(_index, $"{laCode}", "LACode", ConstructApiFilterParams(queryParams),
                orderby, skip, take, facets);

            return exactMatches;
        }

        public async Task<dynamic> SearchSchoolByLocation(string location, decimal distance, int skip, int take,
            string orderby, NameValueCollection queryParams)
        {
            var locations = ExecuteSuggestLocationName(location);

            if (locations.Matches.Count == 0)
            {
                dynamic obj = new ExpandoObject();
                obj.NumberOfResults = 0;
                return obj;
            }

            return await FindNearestSchools(locations.Matches.First().Lat, locations.Matches.First().Long, distance, skip, take, orderby, queryParams);
        }

        public async Task<dynamic> SearchSchoolByLocation(string lat, string lon, decimal distance, int skip, int take,
            string orderby,
            NameValueCollection queryParams)
        {
            return await FindNearestSchools(lat, lon, distance, skip, take, orderby, queryParams);
        }

        public async Task<dynamic> SearchSchoolByMatNo(string matNo, int skip, int take, string @orderby, NameValueCollection queryParams)
        {
            var facets = new[] { "OverallPhase", "OfstedRating", "Gender" };
            var exactMatches = await ExecuteSearch(_index, $"{matNo}", "MATNumber",
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

        private SuggestionQueryResult ExecuteSuggestLocationName(string query)
        {
            var client = new RestClient(GoogleGeocodingUrl);

            var request = new RestRequest(string.Format(GoogleGeoCodingQUeryFormat, query) + _googleApiKey);
            var response = client.Execute(request);

            dynamic content = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);

            return new SuggestionQueryResult
            {
                Matches = Map(content.results),
            };
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
                ? searchFieldsArray[0].Replace("EstablishmentName", "EstablishmentNameUpperCase")
                : orderBy.Replace("EstablishmentName", "EstablishmentNameUpperCase");

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
                .Facet("TypeOfEstablishment")
                .Facet("OverallPhase")
                .Facet("ReligiousCharacter")
                .Facet("OfstedRating")
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
                if (result.ContainsKey("Location"))
                {
                    dynamic location = result["Location"];
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

            if (parameters["schoollevel"] != null)
            {
                string[] values = ExtractValues(parameters["schoollevel"]);
                queryFilter.Add(string.Join(" or ", values.Select(x => "OverallPhase eq '" + x + "'")));
            }

            if (parameters["schooltype"] != null)
            {
                string[] values = ExtractValues(parameters["schooltype"]);
                queryFilter.Add(string.Join(" or ", values.Select(x => "TypeOfEstablishment eq '" + x + "'")));
            }

            if (parameters["ofstedrating"] != null)
            {
                string[] values = ExtractValues(parameters["ofstedrating"]);
                values = values.Select(x => x == "6" ? "" : x).ToArray();
                queryFilter.Add(string.Join(" or ", values.Select(x => "OfstedRating eq '" + x + "'")));
            }

            if (parameters["faith"] != null)
            {
                string[] values = ExtractValues(parameters["faith"]);
                queryFilter.Add(string.Join(" or ", values.Select(x => "ReligiousCharacter eq '" + x + "'")));
            }

            if (parameters["gender"] != null)
            {
                string[] values = ExtractValues(parameters["gender"]);
                queryFilter.Add(string.Join(" or ", values.Select(x => "Gender eq '" + x + "'")));
            }

            return string.Join(" and ", queryFilter.Select(x => "(" + x + ")"));
        }

        private string[] ExtractValues(string commaSeparatedValues)
        {
            return commaSeparatedValues.Split(',');
        }

        private List<Disambiguation> Map(dynamic results)
        {
            var disambiguationList = new List<Disambiguation>();
            foreach (var result in results)
            {
                disambiguationList.Add(new Disambiguation
                {
                    Id = result.place_id,
                    Text = result.formatted_address,
                    Lat = result.geometry.location.lat,
                    Long = result.geometry.location.lng
                });
            }

            return disambiguationList;
        }
    }
}