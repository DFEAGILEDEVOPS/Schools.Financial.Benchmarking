using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services.Search;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace SFB.Web.Infrastructure.SearchEngine
{
    public class AzureSchoolSearchService : ISchoolSearchService
    {
        private readonly string _key;
        private readonly string _searchInstance;
        private readonly string _index;
        private readonly SearchIndexClient _indexClient;

        public AzureSchoolSearchService(string searchInstance, string key, string index)
        {
            _key = key;
            _searchInstance = searchInstance;
            _index = index;
            _indexClient = new SearchIndexClient(_searchInstance, _index, new SearchCredentials(_key));
        }

        public Task<dynamic> SearchAcademiesByCompanyNoAsync(int companyNo, int skip, int take, string orderby, NameValueCollection queryParams)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> SearchSchoolByLaCode(string laCode, int skip, int take, string orderby, NameValueCollection queryParams)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> SearchSchoolByLaEstab(string laEstab, int skip, int take, string orderby, NameValueCollection queryParams)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> SearchSchoolByLatLon(string lat, string lon, decimal distance, int skip, int take, string orderby, NameValueCollection queryParams)
        {
            return await FindNearestSchools(lat, lon, distance, skip, take, orderby, queryParams);
        }

        public Task<dynamic> SearchSchoolByName(string name, int skip, int take, string orderby, NameValueCollection queryParams)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> SuggestSchoolByName(string name, bool openOnly)
        {
            try
            {
                var parameters = new SuggestParameters()
                {
                    UseFuzzyMatching = false,
                    Top = 10,
                };

                var response = await _indexClient.Documents.SuggestAsync(name, "nameSuggester", parameters);

                IEnumerable<SuggestResult<Document>> results = response.Results;

                if (openOnly)
                {
                    results = response.Results.Where(s => s.Document[EdubaseDataFieldNames.ESTAB_STATUS]?.ToString() != "Closed");
                }

                var matches =  results.Select (r =>
                               {
                                   dynamic retVal = new ExpandoObject();
                                   var postCode = r.Document[EdubaseDataFieldNames.POSTCODE] as string;
                                   var town = r.Document[EdubaseDataFieldNames.TOWN] as string;
                                   var schoolName = r.Document[EdubaseDataFieldNames.ESTAB_NAME] as string;
                                   retVal.Id = r.Document[EdubaseDataFieldNames.URN]?.ToString();

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
                                   if (r.Document[EdubaseDataFieldNames.ESTAB_STATUS]?.ToString() == "Closed")
                                   {
                                       retVal.Text += " (Closed)";
                                   }

                                   return retVal;
                               });

                dynamic ret = new ExpandoObject();
                ret.Matches = matches;
                return ret;
            }
            catch(Exception exc)
            {
                throw new ApplicationException(
                    $"Edubase school suggestion error: {exc.Message}");
            }
        }

        public async Task<QueryResultsModel> FindNearestSchools(string lat, string lon, decimal distance, int skip,
            int take, string orderby, NameValueCollection queryParams)
        {
            //var formattedLat = double.Parse(lat).ToString();
            //var formattedLon = double.Parse(lon).ToString();
            //return await ExecuteGeoSpatialSearch(_index, formattedLat, formattedLon,
            //    distance, ConstructApiFilterParams(queryParams), orderby, skip, take);
            return null;
        }

    //    public async Task<QueryResultsModel> ExecuteGeoSpatialSearch(string index, string lat, string lon,
    //decimal distance, string filter, string orderBy, int skip, int take)
    //    {
    //        const string search = "*";
    //        var latitude = double.Parse(lat);
    //        var longitude = double.Parse(lon);

    //        var filterBuilder = new StringBuilder();
    //        var orderByField = string.IsNullOrEmpty(orderBy)
    //            ? string.Format(GeoDistanceLocationOrderFormat, latitude, longitude)
    //            : orderBy;

    //        filterBuilder.AppendFormat(GeoDistanceLocationSearchFormat, latitude, longitude, distance);
    //        if (!string.IsNullOrEmpty(filter))
    //        {
    //            filterBuilder.AppendFormat(" and " + filter);
    //        }

    //        var connection = ApiConnection.Create(_searchInstance, _key);

    //        var client = new IndexQueryClient(connection);

    //        var response = await client.SearchAsync(index, new SearchQuery(search)
    //            .OrderBy(orderByField)
    //            .Facet($"{EdubaseDataFieldNames.TYPE_OF_ESTAB}, count:25")
    //            .Facet($"{EdubaseDataFieldNames.OVERALL_PHASE}")
    //            .Facet($"{EdubaseDataFieldNames.RELIGIOUS_CHARACTER}")
    //            .Facet($"{EdubaseDataFieldNames.OFSTED_RATING}")
    //            .Facet($"{EdubaseDataFieldNames.ESTAB_STATUS}")
    //            .Count(true)
    //            .Filter(filterBuilder.ToString())
    //            .Skip(skip)
    //            .Top(take));

    //        if (!response.IsSuccess)
    //        {
    //            Console.WriteLine("{0}: {1}", response.Error.Code, response.Error.Message);
    //        }

    //        var facetsModel = MapResponseFacetsToFacetsModel(response);

    //        var results = new QueryResultsModel(response.Body.Count, facetsModel,
    //            CalcDistance(response.Body.Records.Select(x => x.Properties), lat, lon), take, skip)
    //        {
    //            QueryLat = latitude.ToString(),
    //            QueryLong = longitude.ToString()
    //        };

    //        return results;
    //    }

    //    private static IEnumerable<IDictionary<string, object>> CalcDistance(
    //IEnumerable<IDictionary<string, object>> results, string lat, string lng)
    //    {
    //        var calcDistance = results as IDictionary<string, object>[] ?? results.ToArray();
    //        foreach (var result in calcDistance)
    //        {
    //            if (result.ContainsKey($"{EdubaseDataFieldNames.LOCATION}"))
    //            {
    //                dynamic location = result[$"{EdubaseDataFieldNames.LOCATION}"];
    //                if (location != null)
    //                {
    //                    var coordinates = location.coordinates;
    //                    result.Add("distanceInMeters",
    //                        GeoCodeCalc.CalcDistance(coordinates[1].ToString(), coordinates[0].ToString(), lat, lng));
    //                }
    //            }
    //        }

    //        return calcDistance;
    //    }

    //    private bool AliasFound(string query, string alias)
    //    {
    //        return query != null && (query.StartsWith(alias) || query.Contains(" " + alias));
    //    }

    //    private string ConstructApiFilterParams(NameValueCollection parameters)
    //    {
    //        if (parameters == null)
    //        {
    //            return string.Empty;
    //        }

    //        var queryFilter = new List<string>();

    //        if (parameters["searchtype"] != null && (parameters["searchtype"] == "search-by-trust-location" || parameters["searchtype"] == "search-by-trust-la-code-name"))
    //        {
    //            queryFilter.Add($"{EdubaseDataFieldNames.FINANCE_TYPE} eq 'A'");
    //            queryFilter.Add($"{EdubaseDataFieldNames.COMPANY_NUMBER} ne null and {EdubaseDataFieldNames.COMPANY_NUMBER} ne '0'");
    //            queryFilter.Add($"{EdubaseDataFieldNames.ESTAB_STATUS} eq 'Open' or {EdubaseDataFieldNames.ESTAB_STATUS} eq 'Open, but proposed to close'");
    //        }

    //        if (parameters["openOnly"] != null)
    //        {
    //            var openOnly = false;
    //            bool.TryParse(parameters["openOnly"], out openOnly);
    //            if (openOnly)
    //            {
    //                queryFilter.Add($"{EdubaseDataFieldNames.ESTAB_STATUS} eq 'Open' or {EdubaseDataFieldNames.ESTAB_STATUS} eq 'Open, but proposed to close'");
    //            }
    //        }

    //        if (parameters["schoollevel"] != null)
    //        {
    //            string[] values = ExtractValues(parameters["schoollevel"]);
    //            queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDataFieldNames.OVERALL_PHASE} eq '" + x + "'")));
    //        }

    //        if (parameters["schooltype"] != null)
    //        {
    //            string[] values = ExtractValues(parameters["schooltype"]);
    //            queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDataFieldNames.TYPE_OF_ESTAB} eq '" + x + "'")));
    //        }

    //        if (parameters["ofstedrating"] != null)
    //        {
    //            string[] values = ExtractValues(parameters["ofstedrating"]);
    //            values = values.Select(x => x == "6" ? "" : x).ToArray();
    //            queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDataFieldNames.OFSTED_RATING} eq '" + x + "'")));
    //        }

    //        if (parameters["faith"] != null)
    //        {
    //            string[] values = ExtractValues(parameters["faith"]);
    //            queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDataFieldNames.RELIGIOUS_CHARACTER} eq '" + x + "'")));
    //        }

    //        if (parameters["gender"] != null)
    //        {
    //            string[] values = ExtractValues(parameters["gender"]);
    //            queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDataFieldNames.GENDER} eq '" + x + "'")));
    //        }

    //        if (parameters["establishmentStatus"] != null)
    //        {
    //            string[] values = ExtractValues(parameters["establishmentStatus"]);
    //            queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDataFieldNames.ESTAB_STATUS} eq '" + x + "'")));
    //        }

    //        if (parameters["EstablishmentStatusInLatestAcademicYear"] != null)
    //        {
    //            string[] values = ExtractValues(parameters["EstablishmentStatusInLatestAcademicYear"]);
    //            queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDataFieldNames.ESTAB_STATUS_IN_YEAR} eq '" + x + "'")));
    //        }

    //        if (parameters["financeType"] != null)
    //        {
    //            string[] values = ExtractValues(parameters["financeType"]);
    //            queryFilter.Add(string.Join(" or ", values.Select(x => $"{EdubaseDataFieldNames.FINANCE_TYPE} eq '" + x + "'")));
    //        }

    //        //queryFilter.Add("OverallPhase ne '0'");
    //        //queryFilter.Add("EstablishmentStatus eq 'Open' or EstablishmentStatus eq 'Open, but proposed to close'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Higher Education Institutions'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'LA Nursery School'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Other Independent School'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Other Independent Special School'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Welsh Establishment'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Special Post 16 Institution'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Sixth Form Centres'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Service Childrens Education'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Secure Units'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Offshore Schools'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Institution funded by other Government Department'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Free Schools - 16-19'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'British Schools Overseas'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Academy 16-19 Sponsor Led'");
    //        //queryFilter.Add("TypeOfEstablishment ne 'Academy 16-19 Converter'");
    //        //queryFilter.Add("StatutoryLowAge ne '16'");
    //        //queryFilter.Add("StatutoryLowAge ne '17'");
    //        //queryFilter.Add("StatutoryLowAge ne '18'");
    //        //queryFilter.Add("StatutoryLowAge ne '19'");

    //        //queryFilter.Add("StatutoryHighAge ne '1'");
    //        //queryFilter.Add("StatutoryHighAge ne '2'");
    //        //queryFilter.Add("StatutoryHighAge ne '3'");
    //        //queryFilter.Add("StatutoryHighAge ne '4'");
    //        //queryFilter.Add("StatutoryHighAge ne '5'");//Todo: Remove .Do not filter out nurseries.

    //        return string.Join(" and ", queryFilter.Select(x => "(" + x + ")"));
    //    }

    //    private string[] ExtractValues(string commaSeparatedValues)
    //    {
    //        if (commaSeparatedValues == "Open, but proposed to close")
    //        {
    //            return new string[] { commaSeparatedValues };
    //        }
    //        else
    //        {
    //            return commaSeparatedValues.Split(',');
    //        }
    //    }
    }
}
