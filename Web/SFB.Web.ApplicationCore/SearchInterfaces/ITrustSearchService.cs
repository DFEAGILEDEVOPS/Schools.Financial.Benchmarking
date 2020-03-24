using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.Services.Search
{
    public interface ITrustSearchService
    {
        Task<dynamic> SuggestTrustByNameAsync(string name);

        Task<dynamic> SearchTrustByNameAsync(string name, int skip, int take, string @orderby, NameValueCollection queryParams);

        dynamic SuggestTrustByName(string name);
    }
}