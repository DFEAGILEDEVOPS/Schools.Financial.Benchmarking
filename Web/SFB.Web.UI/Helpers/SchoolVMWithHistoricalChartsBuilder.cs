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

        public async Task BuildCoreAsync(int urn)
        {
            SchoolVM = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), _schoolBenchmarkListService.GetSchoolBenchmarkList());
        }

        public async Task AddHistoricalChartsAsync(TabType tabType, ChartGroupType chartGroup, CentralFinancingType cFinance, UnitType unitType)
        {
            SchoolVM.HistoricalCharts = _historicalChartBuilder.Build(tabType, chartGroup, SchoolVM.EstablishmentType, unitType);
            SchoolVM.ChartGroups = _historicalChartBuilder.Build(tabType, SchoolVM.EstablishmentType).DistinctBy(c => c.ChartGroup).ToList();
            SchoolVM.LatestTerm = await LatestTermAsync(SchoolVM.EstablishmentType);
            SchoolVM.Tab = tabType;
            
            SchoolVM.HistoricalFinancialDataModels = await this.GetFinancialDataHistoricallyAsync(SchoolVM.Id, SchoolVM.EstablishmentType, SchoolVM.Tab == TabType.Workforce ? CentralFinancingType.Exclude : cFinance);
            
            _fcService.PopulateHistoricalChartsWithFinancialData(SchoolVM.HistoricalCharts, SchoolVM.HistoricalFinancialDataModels, SchoolVM.LatestTerm, SchoolVM.Tab, unitType, SchoolVM.EstablishmentType);
           
        }

        public void AssignLaName()
        {
            SchoolVM.LaName = _laSearchService.GetLaName(SchoolVM.La.ToString());
        }

        public SchoolViewModel GetResult()
        {
            return SchoolVM;
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataHistoricallyAsync(int urn, EstablishmentType estabType, CentralFinancingType cFinance)
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

        private async Task<string> LatestTermAsync(EstablishmentType type)
        {
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(type);
            return SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
        }
    }
}