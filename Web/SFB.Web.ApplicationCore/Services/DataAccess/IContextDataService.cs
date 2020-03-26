using SFB.Web.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.Services.DataAccess
{
    public interface IContextDataService
    {
        Task<EdubaseDataObject> GetSchoolDataObjectByUrnAsync(int urn);        
        Task<List<int>> GetAllSchoolUrnsAsync();
        Task<List<EdubaseDataObject>> GetSchoolDataObjectByLaEstabAsync(string laEstab, bool openOnly);        
        Task<List<EdubaseDataObject>> GetMultipleSchoolDataObjectsByUrnsAsync(List<int> urns);
        Task<IEnumerable<EdubaseDataObject>> GetAcademiesByCompanyNumberAsync(int companyNo);
        Task<int> GetAcademiesCountByCompanyNumberAsync(int companyNo);
    }
}
