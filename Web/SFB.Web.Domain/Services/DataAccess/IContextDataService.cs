using SFB.Web.Common.DataObjects;
using System.Collections.Generic;

namespace SFB.Web.Domain.Services.DataAccess
{
    public interface IContextDataService
    {
        EdubaseDataObject GetSchoolDataObjectByUrn(int urn);
        List<int> GetAllSchoolUrns();
        EdubaseDataObject GetSchoolDataObjectByLaEstab(string laEstab);        
        List<EdubaseDataObject> GetMultipleSchoolDataObjectsByUrns(List<int> urns);
    }
}
