using SFB.Web.ApplicationCore.Models;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.Services.Search
{
    public interface ISchoolSearchService
    {
        Task<dynamic> SuggestSchoolByNameAsync(string name, bool openOnly);

        Task<SearchResultsModel<SchoolSearchResult>> SearchSchoolByNameAsync(string name, int skip, int take, string @orderby, NameValueCollection queryParams);

        Task<SearchResultsModel<SchoolSearchResult>> SearchSchoolByLaEstabAsync(string laEstab, int skip, int take, string @orderby, NameValueCollection queryParams);

        Task<SearchResultsModel<SchoolSearchResult>> SearchSchoolByLaCodeAsync(string laCode, int skip, int take, string orderby, NameValueCollection queryParams);

        Task<SearchResultsModel<SchoolSearchResult>> SearchSchoolByLatLonAsync(string lat, string lon, decimal distance, int skip, int take, string orderby, NameValueCollection queryParams);
 
        Task<SearchResultsModel<SchoolSearchResult>> SearchAcademiesByCompanyNoAsync(int companyNo, int skip, int take, string @orderby, NameValueCollection queryParams);
    }
}