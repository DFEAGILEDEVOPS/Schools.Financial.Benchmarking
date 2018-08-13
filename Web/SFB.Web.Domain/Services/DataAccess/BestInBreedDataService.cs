using SFB.Web.Common.DataObjects;
using SFB.Web.DAL.Repositories;

namespace SFB.Web.Domain.Services.DataAccess
{
    public class BestInBreedDataService : IBestInBreedDataService
    {
        private readonly IEfficiencyMetricsRepository _efficiencyMetricsRepository;

        public BestInBreedDataService(IEfficiencyMetricsRepository efficiencyMetricsRepository)
        {
            _efficiencyMetricsRepository = efficiencyMetricsRepository;
        }

        public BestInBreedDataObject GetBestInClassDataObjectByUrnAndPhase(int urn, string phase = null)
        {
            if (string.IsNullOrEmpty(phase))
            {
                return _efficiencyMetricsRepository.GetBestInClassDataObjectByUrn(urn);
            }
            else
            {
                var dataGroup = (phase == "Primary") ? "KS2" : "KS4";
                return _efficiencyMetricsRepository.GetBestInClassDataObjectByURNAndDataGroup(urn, dataGroup);
            }
        }
    }
}
