using System.Collections.Generic;

namespace SFB.Web.DAL.Repositories
{
    public interface IEdubaseRepository
    {
        dynamic GetSchoolByUrn(string urn);
        dynamic GetSchoolByLaEstab(string laEstab);
        dynamic GetTrustByName(string name);
        dynamic GetMultipleSchoolsByUrns(List<string> urns);
    }
}
