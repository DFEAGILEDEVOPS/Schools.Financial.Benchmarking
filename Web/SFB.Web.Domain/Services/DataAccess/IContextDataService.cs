using SFB.Web.Common.DataObjects;
using System.Collections.Generic;

namespace SFB.Web.Domain.Services.DataAccess
{
    public interface IContextDataService
    {
        EdubaseDataObject GetSchoolDataObjectByUrn(int urn);
        List<int> GetAllSchoolUrns();
        List<EdubaseDataObject> GetSchoolDataObjectByLaEstab(string laEstab, bool openOnly);        
        List<EdubaseDataObject> GetMultipleSchoolDataObjectsByUrns(List<int> urns);
    }
}
