using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SFB.Web.Domain.Services.Search
{
    public interface ISchoolSearchService
    {
        Task<dynamic> SuggestSchoolByName(string name, bool openOnly);

        Task<dynamic> SearchSchoolByName(string name, int skip, int take, string @orderby,
            NameValueCollection queryParams);

        Task<dynamic> SearchSchoolByLaEstab(string laEstab, int skip, int take, string @orderby,
            NameValueCollection queryParams);

        Task<dynamic> SearchSchoolByLaCode(string laCode, int skip, int take, string orderby,
            NameValueCollection queryParams);

        Task<dynamic> SearchSchoolByLatLon(string lat, string lon, decimal distance, int skip, int take,
            string orderby, NameValueCollection queryParams);
 
        Task<dynamic> SearchSchoolByCompanyNoAsync(int companyNo, int skip, int take, string @orderby, NameValueCollection queryParams);
    }
}