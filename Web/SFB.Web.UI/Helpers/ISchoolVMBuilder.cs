using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System.Threading.Tasks;

namespace SFB.Web.UI.Helpers
{
    public interface ISchoolVMBuilder
    {
        Task BuildCoreAsync(int urn);
        Task AddHistoricalChartsAsync(TabType tabType, ChartGroupType chartGroup, CentralFinancingType cFinance, UnitType unitType);
        Task AddLatestYearFinanceAsync();
        void AssignLaName();
        SchoolViewModel GetResult();
    }
}
