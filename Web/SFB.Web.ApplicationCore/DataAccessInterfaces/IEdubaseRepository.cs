using SFB.Web.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.DataAccess
{
    public interface IEdubaseRepository
    {
        Task<EdubaseDataObject> GetSchoolDataObjectByUrnAsync(int urn);
        Task<List<EdubaseDataObject>> GetSchoolsByLaEstabAsync(string laEstab, bool openOnly);        
        Task<List<EdubaseDataObject>> GetMultipleSchoolDataObjectsByUrnsAsync(List<int> urns);
        Task<List<int>> GetAllSchoolUrnsAsync();
        Task<IEnumerable<EdubaseDataObject>> GetAcademiesByCompanyNoAsync(int companyNo);
        Task<int> GetAcademiesCountByCompanyNoAsync(int companyNo);
    }
}
