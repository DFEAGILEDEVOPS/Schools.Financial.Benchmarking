using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services.Search;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFB.Web.Infrastructure.SearchEngine
{
    public class AzureTrustSearchService : ITrustSearchService
    {
        private readonly string _key;
        private readonly string _searchInstance;
        private readonly string _index;
        private readonly SearchIndexClient _indexClient;

        public AzureTrustSearchService(string searchInstance, string key, string index)
        {
            _key = key;
            _searchInstance = searchInstance;
            _index = index;
            _indexClient = new SearchIndexClient(_searchInstance, _index, new SearchCredentials(_key));
        }

        public async Task<dynamic> SearchTrustByNameAsync(string name, int skip, int take, string orderby, NameValueCollection queryParams)
        {
            if (name.Length > 2)
            {
                var exactMatches = await ExecuteSearchAsync($"{name}*", SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME, null, orderby, skip, take);

                return exactMatches;
            }

            return new SearchResultsModel<TrustSearchResult>(0, null, new List<TrustSearchResult>(), 0, 0);
        }

        public dynamic SuggestTrustByName(string name)
        {
            var indexClient = new SearchIndexClient(_searchInstance, _index, new SearchCredentials(_key));

            throw new NotImplementedException();
        }

        private async Task<SearchResultsModel<TrustSearchResult>> ExecuteSearchAsync(string query, string searchFields, string filter, string orderBy, int skip, int take)
        {
            var parameters = new SearchParameters();
            var searchFieldsArray = searchFields.Split(',');
            var orderByField = string.IsNullOrEmpty(orderBy) && searchFieldsArray.Length > 0 ? searchFieldsArray[0] : orderBy;

            parameters = new SearchParameters()
            {
                OrderBy = new[] { $"{orderByField} asc" },
                SearchFields = searchFieldsArray,
                Filter = filter,
                Skip = skip,
                Top = take,
                IncludeTotalResultCount = true,
                //QueryType = QueryType.Full,
                //SearchMode = SearchMode.Any
            };

            DocumentSearchResult<TrustSearchResult> results;
            try
            {
                results = await _indexClient.Documents.SearchAsync<TrustSearchResult>(query, parameters);
            }
            catch (Exception exc)
            {
                throw new ApplicationException($"Azure Search trust search error: {exc.Message}");
            }

            var facetsModel = MapResponseFacetsToFacetsModel(results);
            return new SearchResultsModel<TrustSearchResult>((int)results.Count, null, results.Results.Select(r => r.Document), take, skip);
        }

        private Dictionary<string, FacetResultModel[]> MapResponseFacetsToFacetsModel(DocumentSearchResult<TrustSearchResult> response)
        {
            var facetsModel = new Dictionary<string, FacetResultModel[]>();
            if (response.Facets != null)
            {
                foreach (var facet in response.Facets)
                {
                    facetsModel.Add(facet.Key, facet.Value.Select(fv => new FacetResultModel(fv.Value.ToString(), (long)fv.From, (long)fv.To, fv.Count.GetValueOrDefault())).ToArray());
                }
            }

            return facetsModel;
        }

        public Task<dynamic> SuggestTrustByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        //private static void RunQueries(ISearchIndexClient indexClient)
        //{
        //    SearchParameters parameters;
        //    DocumentSearchResult<dynamic> results;

        //    Console.WriteLine("Search the entire index for the term 'motel' and return only the HotelName field:\n");

        //    parameters =
        //        new SearchParameters()
        //        {
        //            Select = new[] { "HotelName" }
        //        };

        //    results = indexClient.Documents.Search<dynamic>("motel", parameters);


        //    Console.Write("Apply a filter to the index to find hotels with a room cheaper than $100 per night, ");
        //    Console.WriteLine("and return the hotelId and description:\n");

        //    parameters =
        //        new SearchParameters()
        //        {
        //            Filter = "Rooms/any(r: r/BaseRate lt 100)",
        //            Select = new[] { "HotelId", "Description" }
        //        };

        //    results = indexClient.Documents.Search<dynamic>("*", parameters);


        //    Console.Write("Search the entire index, order by a specific field (lastRenovationDate) ");
        //    Console.Write("in descending order, take the top two results, and show only hotelName and ");
        //    Console.WriteLine("lastRenovationDate:\n");

        //    parameters =
        //        new SearchParameters()
        //        {
        //            OrderBy = new[] { "LastRenovationDate desc" },
        //            Select = new[] { "HotelName", "LastRenovationDate" },
        //            Top = 2
        //        };

        //    results = indexClient.Documents.Search<dynamic>("*", parameters);


        //    Console.WriteLine("Search the hotel names for the term 'hotel':\n");

        //    parameters = new SearchParameters()
        //    {
        //        SearchFields = new[] { "HotelName" }
        //    };
        //    results = indexClient.Documents.Search<dynamic>("hotel", parameters);

        //}
    }
}
