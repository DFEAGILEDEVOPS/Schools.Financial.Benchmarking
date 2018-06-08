using System.Collections.Generic;

namespace SFB.Web.Domain.Services.DataAccess
{
    public interface IContextDataService
    {
        dynamic GetSchoolByUrn(int urn);        
        List<int> GetAllSchoolUrns();
        dynamic GetSchoolByLaEstab(string laEstab);        
        dynamic GetMultipleSchoolsByUrns(List<int> urns);        
    }
}
