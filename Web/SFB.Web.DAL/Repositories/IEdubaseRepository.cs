using SFB.Web.Common.DataObjects;
using System.Collections.Generic;

namespace SFB.Web.DAL.Repositories
{
    public interface IEdubaseRepository
    {
        EdubaseDataObject GetSchoolDataObjectByUrn(int urn);
        EdubaseDataObject GetSchoolByLaEstab(string laEstab);        
        List<EdubaseDataObject> GetMultipleSchoolDataObjectsByUrns(List<int> urns);
        List<int> GetAllSchoolUrns();
    }
}
