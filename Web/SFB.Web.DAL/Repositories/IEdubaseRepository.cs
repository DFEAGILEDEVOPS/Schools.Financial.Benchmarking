﻿using SFB.Web.Common.DataObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.DAL.Repositories
{
    public interface IEdubaseRepository
    {
        EdubaseDataObject GetSchoolDataObjectByUrn(int urn);
        List<EdubaseDataObject> GetSchoolsByLaEstab(string laEstab, bool openOnly);        
        List<EdubaseDataObject> GetMultipleSchoolDataObjectsByUrns(List<int> urns);
        List<int> GetAllSchoolUrns();
        Task<IEnumerable<EdubaseDataObject>> GetSchoolsByCompanyNoAsync(int companyNo);
    }
}
