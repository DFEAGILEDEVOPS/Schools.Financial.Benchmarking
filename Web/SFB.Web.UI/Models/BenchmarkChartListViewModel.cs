using System.Collections.Generic;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.Domain.Models;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public class BenchmarkChartListViewModel : ViewModelListBase<ChartViewModel>
    {
        public string SchoolArea { get; set; }
        public string SelectedArea { get; set; }
        public List<ChartViewModel> ChartGroups { get; set; }
        public ComparisonType ComparisonType { get; set; }
        public BenchmarkCriteria AdvancedCriteria { get; set; }
        public SimpleCriteria SimpleCriteria { get; set; }
        public FinancialDataModel BenchmarkSchoolData { get; set; }    
        public EstablishmentType EstablishmentType { get; set; }
        public EstablishmentType SearchedEstablishmentType { get; set; }
        public TrustComparisonListModel TrustComparisonList { get; set; }
        public string LatestTermAcademies { get; set; }
        public string LatestTermMaintained { get; set; }
        public ComparisonArea AreaType { get; }
        public string LaCode { get; }
        public string URN { get; }

        public bool HasIncompleteFinancialData
        {
            get
            {
                if (this.EstablishmentType == EstablishmentType.MAT)
                {
                    return ModelList.SelectMany(m => m.BenchmarkData).Any(d => !d.IsCompleteYear)
                           || ModelList.SelectMany(m => m.BenchmarkData).Any(d => d.PartialYearsPresentInSubSchools);
                }
                return ModelList.SelectMany(m => m.BenchmarkData).Any(d => !d.IsCompleteYear);
            }
        }

        public bool HasIncompleteWorkforceData
        {
            get
            {
                if (this.EstablishmentType == EstablishmentType.MAT)
                {
                    return false;
                }
                else
                {
                    return ModelList.SelectMany(m => m.BenchmarkData).Any(d => !d.IsWFDataPresent);
                }
            }
        }

        public bool HasNoTeacherData => ModelList.SelectMany(m => m.BenchmarkData).Any(d => d.TeacherCount == 0d);

        public bool NoResultsForSimpleSearch => (ComparisonType == ComparisonType.Basic && ComparisonListCount < 2);
        public int BasketSize { get; set; }

        public BenchmarkChartListViewModel(List<ChartViewModel> modelList, SchoolComparisonListModel comparisonList, List<ChartViewModel> chartGroups, ComparisonType comparisonType, BenchmarkCriteria advancedCriteria, SimpleCriteria simpleCriteria, FinancialDataModel benchmarkSchoolData, EstablishmentType estabType, EstablishmentType searchedEstabType, string schoolArea, string selectedArea, string latestTermAcademies, string latestTermMaintained, ComparisonArea areaType, string laCode, string urn, int basketSize, TrustComparisonListModel trustComparisonList = null)
        {
            base.SchoolComparisonList = comparisonList;
            base.ModelList = modelList;
            this.ChartGroups = chartGroups;
            this.AdvancedCriteria = advancedCriteria;
            this.SimpleCriteria = simpleCriteria;
            this.ComparisonType = comparisonType;
            this.BenchmarkSchoolData = benchmarkSchoolData;
            this.EstablishmentType = estabType;
            this.SearchedEstablishmentType = searchedEstabType;
            this.SchoolArea = schoolArea;
            this.SelectedArea = selectedArea;
            this.TrustComparisonList = trustComparisonList;
            this.LatestTermAcademies = latestTermAcademies;
            this.LatestTermMaintained = latestTermMaintained;
            this.AreaType = areaType;
            this.LaCode = laCode;
            this.URN = urn;
            this.BasketSize = basketSize;
        }
    }
}