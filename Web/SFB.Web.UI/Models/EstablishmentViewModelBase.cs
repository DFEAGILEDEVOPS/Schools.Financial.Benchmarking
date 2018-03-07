using System.Collections.Generic;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public abstract class EstablishmentViewModelBase : DynamicViewModelBase
    {
        public abstract string Name { get; set; }

        public abstract string Type { get; }

        public abstract SchoolFinancialType FinancialType { get; }

        public List<ChartViewModel> HistoricalCharts { get; set; }

        public RevenueGroupType Tab { get; set; }

        public List<ChartViewModel> ChartGroups { get; set; }

        public List<string> Terms { get; set; }

        public List<SchoolDataModel> HistoricalSchoolDataModels { get; set; }

        public decimal TotalRevenueIncome { get; set; }

        public decimal TotalRevenueExpenditure { get; set; }

        public decimal InYearBalance { get; set; }

        public bool IsReturnsComplete => HistoricalSchoolDataModels.Last().PeriodCoveredByReturn == 12;

        public bool WorkforceDataPresent => HistoricalSchoolDataModels.Last().WorkforceDataPresent;

        public bool HasNoTeacherData => HistoricalSchoolDataModels.Last().TeacherCount == 0d;

        public bool HasNoPupilData => HistoricalSchoolDataModels.Last().PupilCount == 0d;
    }
}