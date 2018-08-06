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

        public BestInBreedDataObject GetBestInBreedDataObjectByUrn(int urn)
        {
            return _efficiencyMetricsRepository.GetBestInBreedDataObjectByUrn(urn);
        }
    }
}
