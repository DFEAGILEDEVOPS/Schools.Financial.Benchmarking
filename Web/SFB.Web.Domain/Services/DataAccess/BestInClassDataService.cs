using SFB.Web.Common.DataObjects;
using SFB.Web.DAL.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace SFB.Web.Domain.Services.DataAccess
{
    public class BestInClassDataService : IBestInClassDataService
    {
        private readonly IEfficiencyMetricsRepository _efficiencyMetricsRepository;

        public BestInClassDataService(IEfficiencyMetricsRepository efficiencyMetricsRepository)
        {
            _efficiencyMetricsRepository = efficiencyMetricsRepository;
        }

        public List<BestInClassDataObject> GetBestInClassDataObjectsByUrn(int urn)
        {
            return _efficiencyMetricsRepository.GetBestInClassDataObjectsByUrn(urn);
        }

        public BestInClassDataObject GetBestInClassDataObjectByUrnAndPhase(int urn, string phase = null)
        {
            if (phase == null)
            {
                return this.GetBestInClassDataObjectsByUrn(urn).First();
            }
            else
            {
                var keyStage = (phase == "Primary") ? 2 : 4;
                return _efficiencyMetricsRepository.GetBestInClassDataObjectByURNAndKeyStage(urn, keyStage);
            }
        }
    }
}
