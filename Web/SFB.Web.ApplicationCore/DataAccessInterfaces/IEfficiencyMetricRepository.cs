using SFB.Web.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.DataAccess
{
    public interface IEfficiencyMetricRepository
    {
        Task<EfficiencyMetricDataObject> GetEfficiencyMetricDataObjectByUrnAsync(int urn);
    }
}
