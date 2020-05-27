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
    public class AzureTrustSearchService : BaseSearchService<TrustSearchResult>, ITrustSearchService
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

        public async Task<SearchResultsModel<TrustSearchResult>> SearchTrustByNameAsync(string name, int skip, int take, string orderby, NameValueCollection queryParams)
        {
            if (name.Length > 2)
            {
                var exactMatches = await ExecuteSearchAsync($"{name}*", SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME, null, orderby, skip, take);

                return exactMatches;
            }

            return new SearchResultsModel<TrustSearchResult>(0, null, new List<TrustSearchResult>(), 0, 0);
        }

        public async Task<SearchResultsModel<TrustSearchResult>> SearchTrustByCompanyNoAsync(string companyNo, int skip, int take, string orderby, NameValueCollection queryParams)
        {
            var isNumber = int.TryParse(companyNo, out _);
            if (!isNumber || companyNo.Length < SearchParameterValidLengths.COMPANY_NO_LENGTH_MIN || companyNo.Length > SearchParameterValidLengths.COMPANY_NO_LENGTH_MAX)
            {
                return new SearchResultsModel<TrustSearchResult>(0, null, new List<TrustSearchResult>(), 0, 0);
            }
            else
            {
                var exactMatches = await ExecuteSearchAsync($"{companyNo}*", SchoolTrustFinanceDataFieldNames.COMPANY_NUMBER, null, orderby, skip, take);
                return exactMatches;
            }

        }

        public async Task<dynamic> SuggestTrustByNameAsync(string name)
        {
            Func<SuggestResult<Document>, ExpandoObject> processResult = r =>
            {
                dynamic retVal = new ExpandoObject();
                retVal.Id = r.Document[SchoolTrustFinanceDataFieldNames.COMPANY_NUMBER]?.ToString();
                retVal.Text = r.Document[SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME] as string;
                return retVal;
            };

            try
            {
                var parameters = new SuggestParameters()
                {
                    UseFuzzyMatching = false,
                    Top = 10,
                };

                var response = await _indexClient.Documents.SuggestAsync(name, "namesuggest", parameters);

                var results = response.Results;
                
                var matches = results.Select(r =>
                {
                    dynamic retVal = new ExpandoObject();
                    retVal.Id = r.Document[SchoolTrustFinanceDataFieldNames.COMPANY_NUMBER]?.ToString();
                    retVal.Text = r.Document[SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME] as string;
                    return retVal;
                });

                dynamic ret = new ExpandoObject();
                ret.Matches = matches;
                return ret;

            }
            catch(Exception exc)
            {
                throw new ApplicationException($"Azure Search trust suggestion error: {exc.Message}");
            }
        }

        private async Task<SearchResultsModel<TrustSearchResult>> ExecuteSearchAsync(string query, string searchFields, string filter, string orderBy, int skip, int take)
        {
            var parameters = new SearchParameters();
            var searchFieldsArray = searchFields.Split(',');
            var orderByField = string.IsNullOrEmpty(orderBy) && searchFieldsArray.Length > 0 ? searchFieldsArray[0] : orderBy;

            parameters = new SearchParameters()
            {
                OrderBy = new[] { $"{orderByField}" },
                SearchFields = searchFieldsArray,
                Filter = filter,
                Skip = skip,
                Top = take,
                IncludeTotalResultCount = true,
                SearchMode = SearchMode.All
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
    }
}
