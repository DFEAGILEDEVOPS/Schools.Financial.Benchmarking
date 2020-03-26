using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Models;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.Services.DataAccess
{
    public interface IEfficiencyMetricDataService
    {
        Task<EfficiencyMetricDataObject> GetSchoolDataObjectByUrnAsync(int urn);
    }
}
