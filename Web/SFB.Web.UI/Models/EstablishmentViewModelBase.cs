using System.Collections.Generic;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.Domain.Models;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public abstract class EstablishmentViewModelBase : ViewModelBase
    {
        public abstract string Name { get; set; }

        public abstract string Type { get; }

        public abstract EstablishmentType EstablishmentType { get; }

        public List<ChartViewModel> HistoricalCharts { get; set; }

        public RevenueGroupType Tab { get; set; }

        public List<ChartViewModel> ChartGroups { get; set; }

        public List<string> Terms { get; set; }

        public decimal? TotalRevenueIncome { get; set; }

        public decimal? TotalRevenueExpenditure { get; set; }

        public decimal? InYearBalance { get; set; }

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