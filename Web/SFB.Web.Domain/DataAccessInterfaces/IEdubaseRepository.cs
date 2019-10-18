using SFB.Web.Common.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.Domain.DataAccessInterfaces
{
    public interface IEdubaseRepository
    {
        EdubaseDataObject GetSchoolDataObjectByUrn(int urn);
        List<EdubaseDataObject> GetSchoolsByLaEstab(string laEstab, bool openOnly);        
        List<EdubaseDataObject> GetMultipleSchoolDataObjectsByUrns(List<int> urns);
        List<int> GetAllSchoolUrns();
        Task<IEnumerable<EdubaseDataObject>> GetSchoolsByCompanyNoAsync(int companyNo);
    }
}
