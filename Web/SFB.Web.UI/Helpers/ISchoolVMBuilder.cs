using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System.Threading.Tasks;

namespace SFB.Web.UI.Helpers
{
    public interface ISchoolVMBuilder
    {
        void BuildCore(int urn);
        Task<SchoolViewModel> AddHistoricalChartsAsync(TabType tabType, ChartGroupType chartGroup, CentralFinancingType cFinance, UnitType unitType);
        void AssignLaName();
        SchoolViewModel GetResult();
    }
}
