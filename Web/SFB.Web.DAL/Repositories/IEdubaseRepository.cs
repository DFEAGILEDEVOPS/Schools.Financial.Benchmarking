using System.Collections.Generic;

namespace SFB.Web.DAL.Repositories
{
    public interface IEdubaseRepository
    {
        dynamic GetSchoolByUrn(int urn);        
        dynamic GetSchoolByLaEstab(string laEstab);        
        dynamic GetMultipleSchoolsByUrns(List<int> urns);
        List<int> GetAllSchoolUrns();
    }
}
