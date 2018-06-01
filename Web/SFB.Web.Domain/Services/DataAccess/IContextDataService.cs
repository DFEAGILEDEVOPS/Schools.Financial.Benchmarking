using System.Collections.Generic;

namespace SFB.Web.Domain.Services.DataAccess
{
    public interface IContextDataService
    {
        dynamic GetSchoolByUrn(string urn);
        bool QuerySchoolByUrn(string urn);
        List<int> GetAllSchoolUrns();
        dynamic GetSchoolByLaEstab(string laEstab);
        dynamic GetSponsorByName(string name);
        dynamic GetMultipleSchoolsByUrns(List<string> urns);        
    }
}
