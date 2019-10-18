using SFB.Web.UI.Models;
using System.Collections.Generic;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers.Enums;

namespace SFB.Web.UI.Services
{
    public interface IFinancialCalculationsService
    {
        void PopulateHistoricalChartsWithSchoolData(List<ChartViewModel> historicalCharts, List<FinancialDataModel> financialDataModels, string term, RevenueGroupType revgroup, UnitType unit, EstablishmentType estabType);
        void PopulateBenchmarkChartsWithFinancialData(List<ChartViewModel> benchmarkCharts, List<FinancialDataModel> financialDataModels, IEnumerable<CompareEntityBase> bmEntities, string homeSchoolId, UnitType? unit);
    }
}
