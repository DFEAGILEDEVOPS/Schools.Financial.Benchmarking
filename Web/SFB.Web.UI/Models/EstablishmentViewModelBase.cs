using System.Collections.Generic;
using System.Linq;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Entities;

namespace SFB.Web.UI.Models
{
    public abstract class EstablishmentViewModelBase : ViewModelBase
    {
        public EstablishmentViewModelBase()
        {
            this.HistoricalCharts = new List<ChartViewModel>();
            this.HistoricalFinancialDataModels = new List<FinancialDataModel>();
        }

        public abstract string Name { get; set; }

        public abstract string Type { get; }

        public abstract long Id { get;}

        public abstract float TotalPupils { get; }

        public int La => ContextData.LACode;        

        public string LaName { get; set; }

        public abstract EstablishmentType EstablishmentType { get; }

        public List<ChartViewModel> HistoricalCharts { get; set; }
        public List<ChartViewModel> FinancialCharts
        {
            get
            {
                return this.HistoricalCharts.Where(h => h.TabType == TabType.Expenditure || h.TabType == TabType.Income || h.TabType == TabType.Balance).ToList();
            }
        }

        public List<ChartViewModel> WorkforceCharts
        {
            get
            {
                return this.HistoricalCharts.Where(h => h.TabType == TabType.Workforce).ToList();
            }
        }

        public TabType Tab { get; set; }

        public List<ChartViewModel> ChartGroups { get; set; }

        public string LatestTerm { get; set; }

        public decimal? TotalRevenueIncome => this.HistoricalFinancialDataModels.Last().TotalIncome;

        public decimal? TotalRevenueExpenditure => this.HistoricalFinancialDataModels.Last().TotalExpenditure;

        public decimal? InYearBalance => this.HistoricalFinancialDataModels.Last().InYearBalance;

        public decimal? RevenueReserve => HistoricalFinancialDataModels.Last().ShareOfRevenueReserve ??
                                          HistoricalFinancialDataModels.Last().RevenueReserve;

        public List<FinancialDataModel> HistoricalFinancialDataModels { get; set; }

        public FinancialDataModel LatestYearFinancialData => HistoricalFinancialDataModels.Count > 0 ? HistoricalFinancialDataModels.Last() : null;
        
        public EdubaseDataObject ContextData { get; set; }

        public bool HasLatestYearFinancialData => LatestYearFinancialData?.FinancialDataObjectModel != null;
        
        public bool HasSomeHistoricalFinancialData => HistoricalFinancialDataModels.Any(f => f.FinancialDataObjectModel != null);

        public bool HasSomeHistoricalWorkforceData => HistoricalFinancialDataModels.Any(f => f.FinancialDataObjectModel != null && f.FinancialDataObjectModel.WorkforcePresent);

        public bool IsReturnsComplete => LatestYearFinancialData.IsReturnsComplete;

        public bool IsReturnsDNS => LatestYearFinancialData.IsReturnsDNS;

        public bool IsReturnsPlaceholder => LatestYearFinancialData.IsPlaceHolder;

        public bool WorkforceDataPresent => LatestYearFinancialData.WorkforceDataPresent;

        public bool HasNoTeacherData => LatestYearFinancialData.TeacherCount == 0m;

        public bool HasNoPupilData => LatestYearFinancialData.PupilCount == 0m;

        public decimal? ProgressScore => LatestYearFinancialData?.ProgressScore;

        public bool IsFederation => ContextData.IsFederation;

        public bool IsUnderReview { get; set; }
        public bool HasCscpUrl { get; set; }
        public bool HasGiasUrl { get; set; }
        
    }
}