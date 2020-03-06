using SFB.Web.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.DataAccess
{
    public interface IEdubaseRepository
    {
        EdubaseDataObject GetSchoolDataObjectByUrn(int urn);
        List<EdubaseDataObject> GetSchoolsByLaEstab(string laEstab, bool openOnly);        
        List<EdubaseDataObject> GetMultipleSchoolDataObjectsByUrns(List<int> urns);
        List<int> GetAllSchoolUrns();
        Task<IEnumerable<EdubaseDataObject>> GetAcademiesByCompanyNoAsync(int companyNo);
        Task<int> GetAcademiesCountByCompanyNoAsync(int companyNo);
    }
}
