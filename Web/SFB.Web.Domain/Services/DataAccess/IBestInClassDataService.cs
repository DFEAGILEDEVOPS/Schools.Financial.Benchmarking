using SFB.Web.Common.DataObjects;
using System.Collections.Generic;

namespace SFB.Web.Domain.Services.DataAccess
{
    public interface IBestInClassDataService
    {
        List<BestInClassDataObject> GetBestInClassDataObjectsByUrn(int urn);
        BestInClassDataObject GetBestInClassDataObjectByUrnAndPhase(int urn, string phase);
    }
}
