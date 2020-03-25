//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Dynamic;
//using System.Linq;
//using System.Threading.Tasks;
//using RedDog.Search;
//using RedDog.Search.Http;
//using RedDog.Search.Model;
//using SFB.Web.ApplicationCore.Helpers.Constants;
//using SFB.Web.ApplicationCore.Models;

//namespace SFB.Web.ApplicationCore.Services.Search
//{
//    public class RedDogTrustSearchService : ITrustSearchService
//    {
//        private readonly string _key;
//        private readonly string _searchInstance;
//        private readonly string _index;

//        public RedDogTrustSearchService(string searchInstance, string key, string index)
//        {
//            _key = key;
//            _searchInstance = searchInstance;
//            _index = index;
//        }

//        public async Task<dynamic> SuggestTrustByNameAsync(string name)
//        {
//            var connection = ApiConnection.Create(_searchInstance, _key);
//            var client = new IndexQueryClient(connection);

//            Func<SuggestionResultRecord, ExpandoObject> processResult = r =>
//            {
//                dynamic retVal = new ExpandoObject();
//                retVal.Id = r.Properties[$"{SchoolTrustFinanceDataFieldNames.COMPANY_NUMBER}"]?.ToString();
//                retVal.Text = r.Properties[$"{SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME}"] as string;
//                return retVal;
//            };

//            var response = await client.SuggestAsync(_index, new SuggestionQuery(name)
//                .SuggesterName("namesuggest")
//                .Fuzzy(false)
//                .Select($"{SchoolTrustFinanceDataFieldNames.COMPANY_NUMBER}")
//                .Select($"{SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME}")
//                .SearchField($"{SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME}")
//                .Top(10));

//            if (!response.IsSuccess)
//            {
//                throw new ApplicationException($"Azure Search trust suggestion error {response.Error.Code}: {response.Error.Message}");
//            }

//            var results = response.Body.Records;

//            var matches = (from r in results
//                           select processResult(r));

//            dynamic ret = new ExpandoObject();
//            ret.Matches = matches;
//            return ret;
//        }

//        public async Task<dynamic> SearchTrustByNameAsync(string name, int skip, int take, string @orderby, NameValueCollection queryParams)
//        {
//            if (name.Length > 2)
//            {
//                name += "*";
//                var exactMatches = await ExecuteSearch(_index, $"{name}", $"{SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME}", null, orderby, skip, take);

//                return exactMatches;
//            }

//            return new QueryResultsModel(0, null, new List<IDictionary<string, object>>(), 0, 0);
//        }

//        private async Task<QueryResultsModel> ExecuteSearch(string index, string query, string searchFields, string filter, string orderBy, int skip, int take)
//        {
//            var connection = ApiConnection.Create(_searchInstance, _key);
//            var searchFieldsArray = searchFields.Split(',');
//            var orderByField = string.IsNullOrEmpty(orderBy) && searchFieldsArray.Length > 0 ? searchFieldsArray[0] : orderBy;

//            var client = new IndexQueryClient(connection);
//            var searchQueryModel = new SearchQuery(query);
//            searchQueryModel.SearchFields = searchFields;
//            if (!string.IsNullOrWhiteSpace(filter))
//            {
//                searchQueryModel = searchQueryModel.Filter(filter);
//            }

//            //searchQueryModel.Facets = new[] { "TypeOfEstablishment", "OverallPhase", "ReligiousCharacter", "OfstedRating" };
//            searchQueryModel.OrderBy = orderByField;
//            searchQueryModel = searchQueryModel.Mode(SearchMode.All).Count(true).Skip(skip).Top(take);

//            var response = await client.SearchAsync(index, searchQueryModel);

//            if (!response.IsSuccess)
//            {
//                throw new ApplicationException($"Azure Search trust search error {response.Error.Code}: {response.Error.Message}");
//            }

//            var facetsModel = MapResponseFacetsToFacetsModel(response);
//            return new QueryResultsModel(response.Body.Count, facetsModel, response.Body.Records.Select(x => x.Properties), take, skip);
//        }

//        private Dictionary<string, FacetResultModel[]> MapResponseFacetsToFacetsModel(IApiResponse<SearchQueryResult> response)
//        {
//            var facetsModel = new Dictionary<string, FacetResultModel[]>();
//            if (response.Body.Facets != null)
//            {
//                foreach (var facet in response.Body.Facets)
//                {
//                    facetsModel.Add(facet.Key, facet.Value.Select(fv => new FacetResultModel(fv.Value, fv.From, fv.To, fv.Count)).ToArray());
//                }
//            }

//            return facetsModel;
//        }
//    }
//}