using System.Collections.Generic;
using System.Linq;
using SFB.Web.ApplicationCore;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public abstract class EstablishmentViewModelBase : ViewModelBase
    {
        public abstract string Name { get; set; }

        public abstract string Type { get; }

        public abstract EstablishmentType EstablishmentType { get; }

        public List<ChartViewModel> HistoricalCharts { get; set; }

        public TabType Tab { get; set; }

        public List<ChartViewModel> ChartGroups { get; set; }

        public string LatestTerm { get; set; }

        public decimal? TotalRevenueIncome => this.HistoricalFinancialDataModels.Last().TotalIncome;

        public decimal? TotalRevenueExpenditure => this.HistoricalFinancialDataModels.Last().TotalExpenditure;

        public decimal? InYearBalance => this.HistoricalFinancialDataModels.Last().InYearBalance;

        public List<FinancialDataModel> HistoricalFinancialDataModels { get; set; }

        public FinancialDataModel LatestYearFinancialData => HistoricalFinancialDataModels.Last();

        public bool IsReturnsComplete => LatestYearFinancialData.IsReturnsComplete;

        public bool IsReturnsDNS => LatestYearFinancialData.IsReturnsDNS;

        public bool IsReturnsPlaceholder => LatestYearFinancialData.IsPlaceHolder;

        public bool WorkforceDataPresent => LatestYearFinancialData.WorkforceDataPresent;

        public bool HasNoTeacherData => LatestYearFinancialData.TeacherCount == 0m;

        public bool HasNoPupilData => LatestYearFinancialData.PupilCount == 0m;
    }
}