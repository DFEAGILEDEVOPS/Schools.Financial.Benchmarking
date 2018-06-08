using SFB.Web.UI.Models;
using System.Collections.Generic;
using SFB.Web.Common;
using SFB.Web.Domain.Models;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Services
{
    public interface IFinancialCalculationsService
    {
        void PopulateHistoricalChartsWithSchoolData(List<ChartViewModel> historicalCharts, List<FinancialDataModel> financialDataModels, string term, RevenueGroupType revgroup, UnitType unit, EstablishmentType estabType);
        void PopulateBenchmarkChartsWithFinancialData(List<ChartViewModel> benchmarkCharts, List<FinancialDataModel> financialDataModels, IEnumerable<CompareEntityBase> bmEntities, string homeSchoolId, UnitType? unit, bool trimSchoolNames = false);        
    }
}
