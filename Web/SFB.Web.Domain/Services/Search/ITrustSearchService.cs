using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SFB.Web.Domain.Services.Search
{
    public interface ITrustSearchService
    {
        Task<dynamic> SuggestTrustByName(string name);

        Task<dynamic> SearchTrustByName(string name, int skip, int take, string @orderby,
            NameValueCollection queryParams);
    }
}