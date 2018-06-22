using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using RedDog.Search;
using RedDog.Search.Http;
using RedDog.Search.Model;
using SFB.Web.Domain.Models;

namespace SFB.Web.Domain.Services.Search
{
    //TODO: Use constant DB field names here
    public class TrustSearchService : ITrustSearchService
    {
        private readonly string _key;
        private readonly string _searchInstance;
        private readonly string _index;

        public TrustSearchService(string searchInstance, string key, string index)
        {
            _key = key;
            _searchInstance = searchInstance;
            _index = index;
        }

        public async Task<dynamic> SuggestTrustByName(string name)
        {
            var connection = ApiConnection.Create(_searchInstance, _key);
            var client = new IndexQueryClient(connection);

            Func<SuggestionResultRecord, ExpandoObject> processResult = r =>
            {
                dynamic retVal = new ExpandoObject();
                retVal.Id = r.Properties["MATNumber"]?.ToString();
                retVal.Text = r.Properties["TrustOrCompanyName"] as string;
                return retVal;
            };

            var response = await client.SuggestAsync(_index, new SuggestionQuery(name)
                .SuggesterName("namesuggest")
                .Fuzzy(false)
                .Select("MATNumber")
                .Select("TrustOrCompanyName")
                .SearchField("TrustOrCompanyName")
                .Top(10));

            if (!response.IsSuccess)
            {
                throw new ApplicationException($"Azure Search trust suggestion error {response.Error.Code}: {response.Error.Message}");
            }

            var results = response.Body.Records;

            var matches = (from r in results
                           select processResult(r));

            dynamic ret = new ExpandoObject();
            ret.Matches = matches;
            return ret;
        }

        public async Task<dynamic> SearchTrustByName(string name, int skip, int take, string @orderby, NameValueCollection queryParams)
        {
            if (name.Length > 2)
            {
                var exactMatches = await ExecuteSearch(_index, $"{name}", "TrustOrCompanyName", null, orderby, skip, take);

                return exactMatches;
            }

            return new QueryResultsModel(0, null, new List<IDictionary<string, object>>(), 0, 0);
        }

        private async Task<QueryResultsModel> ExecuteSearch(string index, string query, string searchFields, string filter, string orderBy, int skip, int take)
        {
            var connection = ApiConnection.Create(_searchInstance, _key);
            var searchFieldsArray = searchFields.Split(',');
            var orderByField = string.IsNullOrEmpty(orderBy) && searchFieldsArray.Length > 0 ? searchFieldsArray[0] : orderBy;

            var client = new IndexQueryClient(connection);
            var searchQueryModel = new SearchQuery(query);
            searchQueryModel.SearchFields = searchFields;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                searchQueryModel = searchQueryModel.Filter(filter);
            }

            //searchQueryModel.Facets = new[] { "TypeOfEstablishment", "OverallPhase", "ReligiousCharacter", "OfstedRating" };
            searchQueryModel.OrderBy = orderByField;
            searchQueryModel = searchQueryModel.Mode(SearchMode.All).Count(true).Skip(skip).Top(take);

            var response = await client.SearchAsync(index, searchQueryModel);

            if (!response.IsSuccess)
            {
                throw new ApplicationException($"Azure Search trust search error {response.Error.Code}: {response.Error.Message}");
            }

            return new QueryResultsModel(response.Body.Count, response.Body.Facets, response.Body.Records.Select(x => x.Properties), take, skip);
        }
    }
}