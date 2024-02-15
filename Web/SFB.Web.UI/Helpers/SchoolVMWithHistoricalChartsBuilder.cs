using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Helpers;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;

namespace SFB.Web.UI.Helpers
{
    public class SchoolVMWithHistoricalChartsBuilder : ISchoolVMBuilder
    {
        private readonly IContextDataService _contextDataService;
        private readonly IFinancialDataService _financialDataService;
        private readonly IHistoricalChartBuilder _historicalChartBuilder;
        private readonly IFinancialCalculationsService _fcService;
        private readonly ISchoolBenchmarkListService _schoolBenchmarkListService;
        private readonly ILocalAuthoritiesService _laSearchService;

        private SchoolViewModel SchoolVM { get; set; }

        public SchoolVMWithHistoricalChartsBuilder(IHistoricalChartBuilder historicalChartBuilder, IFinancialDataService financialDataService,
            IFinancialCalculationsService fcService, IContextDataService contextDataService, 
            ISchoolBenchmarkListService benchmarkBasketService, ILocalAuthoritiesService laSearchService)
        {
            _historicalChartBuilder = historicalChartBuilder;
            _financialDataService = financialDataService;
            _fcService = fcService;
            _contextDataService = contextDataService;
            _schoolBenchmarkListService = benchmarkBasketService;
            _laSearchService = laSearchService;
        }

        public async Task BuildCoreAsync(long urn)
        {
            SchoolVM = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), _schoolBenchmarkListService.GetSchoolBenchmarkList());
            SchoolVM.LatestTerm = await LatestTermAsync(SchoolVM.EstablishmentType);
        }

        public async Task AddHistoricalChartsAsync(TabType tabType, ChartGroupType chartGroup,
            CentralFinancingType cFinance, UnitType unitType)
        {
            SchoolVM.HistoricalCharts.AddRange(_historicalChartBuilder.Build(tabType, chartGroup,
                SchoolVM.EstablishmentType, unitType));

            // AB#63488: prevent historical data being duplicated for Workforce tab type
            var financialDataModels = await GetFinancialDataHistoricallyAsync(SchoolVM.Id, SchoolVM.EstablishmentType,
                SchoolVM.Tab == TabType.Workforce ? CentralFinancingType.Exclude : cFinance);
            SchoolVM.HistoricalFinancialDataModels.AddRange(financialDataModels.Except(
                SchoolVM.HistoricalFinancialDataModels, new HistoricalFinancialDataModelComparer(tabType)));

            _fcService.PopulateHistoricalChartsWithFinancialData(SchoolVM.HistoricalCharts,
                SchoolVM.HistoricalFinancialDataModels, SchoolVM.LatestTerm, tabType, unitType,
                SchoolVM.EstablishmentType);
        }

        public void SetTab(TabType tabType)
        {
            SchoolVM.Tab = tabType;
        }

        public void SetChartGroups(TabType tabType)
        {
            if (tabType == TabType.Workforce)
            {
                SchoolVM.ChartGroups = _historicalChartBuilder.Build(tabType, ChartGroupType.Workforce, SchoolVM.EstablishmentType).DistinctBy(c => c.ChartGroup).ToList();
            }
            else
            {
                SchoolVM.ChartGroups = _historicalChartBuilder.Build(tabType, SchoolVM.EstablishmentType).DistinctBy(c => c.ChartGroup).ToList();
            }
        }

        public async Task AddLatestYearFinanceAsync()
        {

            SchoolVM.HistoricalFinancialDataModels = new List<FinancialDataModel> 
            {
                await this.GetLatestFinancialDataAsync(SchoolVM.Id, SchoolVM.EstablishmentType, CentralFinancingType.Exclude)
            };
        }

        public void AssignLaName()
        {
            SchoolVM.LaName = _laSearchService.GetLaName(SchoolVM.La.ToString());
        }

        public SchoolViewModel GetResult()
        {
            return SchoolVM;
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataHistoricallyAsync(long urn, EstablishmentType estabType, CentralFinancingType cFinance)
        {
            var models = new List<FinancialDataModel>();
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(estabType);

            var taskList = new List<Task<SchoolTrustFinancialDataObject>>();
            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var task = _financialDataService.GetSchoolFinancialDataObjectAsync(urn, term, estabType, cFinance);
                taskList.Add(task);
            }

            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var resultDataObject = await taskList[ChartHistory.YEARS_OF_HISTORY - 1 - i];

                if (estabType == EstablishmentType.Academies && cFinance == CentralFinancingType.Include && resultDataObject == null)//if nothing found in MAT-Allocs collection try to source it from (non-allocated) Academies data
                {
                    resultDataObject = (await _financialDataService.GetSchoolFinancialDataObjectAsync(urn, term, estabType, CentralFinancingType.Exclude));
                }

                if (resultDataObject != null && resultDataObject.DidNotSubmit)//School did not submit finance, return & display "no data" in the charts
                {
                    resultDataObject = null;
                }

                models.Add(new FinancialDataModel(urn.ToString(), term, resultDataObject, estabType)); 
            }

            return models;
        }

        private async Task<FinancialDataModel> GetLatestFinancialDataAsync(long urn, EstablishmentType estabType, CentralFinancingType cFinance)
        { 
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(estabType);
            var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
            var resultDataObject = await _financialDataService.GetSchoolFinancialDataObjectAsync(urn, term, estabType, cFinance);

            if (estabType == EstablishmentType.Academies && cFinance == CentralFinancingType.Include && resultDataObject == null)//if nothing found in MAT-Allocs collection try to source it from (non-allocated) Academies data
            {
                resultDataObject = (await _financialDataService.GetSchoolFinancialDataObjectAsync(urn, term, estabType, CentralFinancingType.Exclude));
            }

            if (resultDataObject != null && resultDataObject.DidNotSubmit)//School did not submit finance, return & display "no data" in the charts
            {
                resultDataObject = null;
            }

            return new FinancialDataModel(urn.ToString(), term, resultDataObject, estabType);
        }

        private async Task<string> LatestTermAsync(EstablishmentType type)
        {
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(type);
            return SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
        }
        
        /// <summary>
        /// For 'Workforce' TabType only, compare model based on Term rather than Id
        /// </summary>
        private class HistoricalFinancialDataModelComparer : IEqualityComparer<FinancialDataModel>
        {
            private readonly TabType _tabType;

            public HistoricalFinancialDataModelComparer(TabType tabType)
            {
                _tabType = tabType;
            }

            public bool Equals(FinancialDataModel x, FinancialDataModel y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                if (_tabType == TabType.Workforce) return x.Term == y.Term;
                return x.Equals(y);
            }

            public int GetHashCode(FinancialDataModel obj)
            {
                if (_tabType == TabType.Workforce)
                {
                    return obj.Term != null ? obj.Term.GetHashCode() : 0;
                }

                return obj.GetHashCode();
            }
        }
    }
}