using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System.Threading.Tasks;

namespace SFB.Web.UI.Helpers
{
    public interface ISchoolVMBuilder
    {
        Task BuildCoreAsync(long urn);
        Task AddHistoricalChartsAsync(TabType tabType, ChartGroupType chartGroup, CentralFinancingType cFinance, UnitType unitType);
        void SetChartGroups(TabType tabType);
        void SetTab(TabType tabType);
        Task AddLatestYearFinanceAsync();
        void AssignLaName();
        SchoolViewModel GetResult();
    }
}
