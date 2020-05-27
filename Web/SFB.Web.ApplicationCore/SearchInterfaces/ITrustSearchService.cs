using SFB.Web.ApplicationCore.Models;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.Services.Search
{
    public interface ITrustSearchService
    {
        Task<dynamic> SuggestTrustByNameAsync(string name);

        Task<SearchResultsModel<TrustSearchResult>> SearchTrustByNameAsync(string name, int skip, int take, string @orderby, NameValueCollection queryParams);
        Task<SearchResultsModel<TrustSearchResult>> SearchTrustByCompanyNoAsync(string companyNo, int skip, int take, string @orderby, NameValueCollection queryParams);
    }
}