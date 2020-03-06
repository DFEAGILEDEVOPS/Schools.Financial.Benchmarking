using SFB.Web.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.Services.DataAccess
{
    public interface IContextDataService
    {
        EdubaseDataObject GetSchoolDataObjectByUrn(int urn);
        List<int> GetAllSchoolUrns();
        List<EdubaseDataObject> GetSchoolDataObjectByLaEstab(string laEstab, bool openOnly);        
        List<EdubaseDataObject> GetMultipleSchoolDataObjectsByUrns(List<int> urns);
        Task<IEnumerable<EdubaseDataObject>> GetAcademiesByCompanyNumberAsync(int companyNo);
        Task<int> GetAcademiesCountByCompanyNumberAsync(int companyNo);
    }
}
