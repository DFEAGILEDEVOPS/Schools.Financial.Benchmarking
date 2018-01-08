using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SFB.Web.Domain.Services
{
    public interface ISchoolApiService
    {
        //dynamic SearchSchoolByName(string name, int skip, int take, string orderby, NameValueCollection queryParams);
        //dynamic SearchSchoolByLocation(string location, decimal? distance, int skip, int take, string orderby, NameValueCollection queryParams);
        //dynamic SearchSchoolByLocation(string lat, string lon, decimal? distance, int skip, int take, string orderby, NameValueCollection queryParams);
        //dynamic SearchSchoolByLaCode(string laCode, int skip, int take, string orderby, NameValueCollection queryParams);
        //dynamic SearchSchoolByLaEstab(string laEstab, int skip, int take, string orderby, NameValueCollection queryParams);
        //dynamic SuggestSchoolByName(string name);

        dynamic GetLocalAuthorities();//Leave this as an SPT API call

        //dynamic GetSchoolByUrn(string urn);
        //dynamic GetMultipleSchoolsByUrns(List<string> urns);
        //Task<ApiResponse> GetSchoolByUrnAsync(string urn);
        //dynamic GetSponsorByCode(int code);
        
    }
}
