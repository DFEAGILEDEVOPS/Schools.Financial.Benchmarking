using System.Collections.Generic;

namespace SFB.Web.DAL.Repositories
{
    public interface IEdubaseRepository
    {
        dynamic GetSchoolByUrn(string urn);
        bool QuerySchoolByUrn(string urn);
        dynamic GetSchoolByLaEstab(string laEstab);
        dynamic GetSponsorByName(string name);
        dynamic GetMultipleSchoolsByUrns(List<string> urns);
        List<int> GetAllSchoolUrns();
    }
}
