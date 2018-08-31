using SFB.Web.Common.DataObjects;
using System.Collections.Generic;

namespace SFB.Web.DAL.Repositories
{
    public interface IEfficiencyMetricsRepository
    {
        List<BestInClassDataObject> GetBestInClassDataObjectsByUrn(int urn);
        BestInClassDataObject GetBestInClassDataObjectByURNAndKeyStage(int urn, int keyStage);
    }
}
