using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Entities;

namespace SFB.Web.ApplicationCore.Services.DataAccess
{
    public class EfficiencyMetricDataService : IEfficiencyMetricDataService
    {
        private readonly IEfficiencyMetricRepository _efficiencyMetricRepository;

        public EfficiencyMetricDataService(IEfficiencyMetricRepository efficiencyMetricRepository)
        {
            _efficiencyMetricRepository = efficiencyMetricRepository;
        }

        public Task<EfficiencyMetricDataObject> GetSchoolDataObjectByUrnAsync(int urn)
        {
            return _efficiencyMetricRepository.GetEfficiencyMetricDataObjectByUrnAsync(urn);
        }

        public Task<List<EfficiencyMetricDataObject>> GetMultipleSchoolDataObjectsByUrnsAsync(List<int> urns)
        {
            return _efficiencyMetricRepository.GetMultipleSchoolDataObjectsByUrnsAsync(urns);
        }
    }
}
