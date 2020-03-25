using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.Services.Search
{
    public interface ISchoolSearchService
    {
        Task<dynamic> SuggestSchoolByNameAsync(string name, bool openOnly);

        Task<dynamic> SearchSchoolByNameAsync(string name, int skip, int take, string @orderby, NameValueCollection queryParams);

        Task<dynamic> SearchSchoolByLaEstabAsync(string laEstab, int skip, int take, string @orderby, NameValueCollection queryParams);

        Task<dynamic> SearchSchoolByLaCodeAsync(string laCode, int skip, int take, string orderby, NameValueCollection queryParams);

        Task<dynamic> SearchSchoolByLatLonAsync(string lat, string lon, decimal distance, int skip, int take, string orderby, NameValueCollection queryParams);
 
        Task<dynamic> SearchAcademiesByCompanyNoAsync(int companyNo, int skip, int take, string @orderby, NameValueCollection queryParams);
    }
}