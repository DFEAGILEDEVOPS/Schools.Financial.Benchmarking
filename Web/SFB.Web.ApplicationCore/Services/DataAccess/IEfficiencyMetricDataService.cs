using SFB.Web.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.Services.DataAccess
{
    public interface IEfficiencyMetricDataService
    {
        Task<EfficiencyMetricDataObject> GetSchoolDataObjectByUrnAsync(int urn);

        Task<List<EfficiencyMetricDataObject>> GetMultipleSchoolDataObjectsByUrnsAsync(List<int> urns);
    }
}
