using System.Collections.Generic;

namespace SFB.Web.Domain.Services.DataAccess
{
    public interface IEdubaseDataService
    {
        dynamic GetSchoolByUrn(string urn);
        dynamic GetSchoolByLaEstab(string laEstab);
        dynamic GetSponsorByName(string name);
        dynamic GetMultipleSchoolsByUrns(List<string> urns);
    }
}
