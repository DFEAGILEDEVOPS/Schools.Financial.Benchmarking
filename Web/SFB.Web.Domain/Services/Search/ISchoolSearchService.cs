using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SFB.Web.Domain.Services.Search
{
    public interface ISchoolSearchService
    {
        Task<dynamic> SuggestSchoolByName(string name);

        Task<dynamic> SearchSchoolByName(string name, int skip, int take, string @orderby,
            NameValueCollection queryParams);

        Task<dynamic> SearchSchoolByLaCode(string laCode, int skip, int take, string orderby,
            NameValueCollection queryParams);

        Task<dynamic> SearchSchoolByLocation(string location, decimal distance, int skip, int take, string @orderby,
            NameValueCollection queryParams);

        Task<dynamic> SearchSchoolByLocation(string lat, string lon, decimal distance, int skip, int take,
            string orderby, NameValueCollection queryParams);

        Task<dynamic> SearchSchoolByMatNo(string matNo, int skip, int take, string @orderby,
            NameValueCollection queryParams);
    }
}