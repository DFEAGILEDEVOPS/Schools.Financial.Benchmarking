using System.Collections.Generic;

namespace SFB.Web.Domain.Services.DataAccess
{
    public interface IContextDataService
    {
        dynamic GetSchoolByUrn(string urn);
        dynamic GetSchoolByLaEstab(string laEstab);
        dynamic GetTrustByName(string name);
        dynamic GetMultipleSchoolsByUrns(List<string> urns);
    }
}
