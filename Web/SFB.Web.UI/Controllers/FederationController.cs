using Microsoft.Ajax.Utilities;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Helpers;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Services.LocalAuthorities;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SFB.Web.UI.Controllers
{
    public class FederationController : Controller
    {

        private readonly IFinancialDataService _financialDataService;
        private readonly IContextDataService _contextDataService;
        private readonly IHistoricalChartBuilder _historicalChartBuilder;
        private readonly IFinancialCalculationsService _fcService;
        private readonly ILocalAuthoritiesService _laService;
        private readonly IDownloadCSVBuilder _csvBuilder;
        private readonly ISchoolBenchmarkListService _benchmarkBasketService;

        public FederationController(IHistoricalChartBuilder historicalChartBuilder, IFinancialDataService financialDataService,
            IFinancialCalculationsService fcService, IContextDataService contextDataService, ILocalAuthoritiesService laService, 
            IDownloadCSVBuilder csvBuilder, ISchoolBenchmarkListService benchmarkBasketService)
        {
            _historicalChartBuilder = historicalChartBuilder;
            _financialDataService = financialDataService;
            _contextDataService = contextDataService;
            _fcService = fcService;
            _laService = laService;
            _csvBuilder = csvBuilder;
            _benchmarkBasketService = benchmarkBasketService;
        }

        public async Task<ActionResult> Index(long fuid,
            UnitType unit = UnitType.AbsoluteMoney,
            TabType tab = TabType.Expenditure,
            ChartFormat format = ChartFormat.Charts)
        {
            //TODO: Uncomment for production
            //if (FeatureManager.IsEnabled(Features.RevisedSchoolPage))
            //{
            //    return Redirect($"/federation/detail?fuid={fuid}");
            //}

            OverwriteDefaultUnitTypeForSelectedTab(tab, ref unit);
            var chartGroup = DetectDefaultChartGroupFromTabType(tab);

            var vm = await BuildFederationViewModelAsync(fuid, tab, chartGroup, unit);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unit;
            ViewBag.ChartFormat = format;
            ViewBag.EstablishmentType = EstablishmentType.Federation;

            return View(vm);
        }

        public async Task<ActionResult> Detail(long fuid,
            UnitType unit = UnitType.AbsoluteMoney,
            TabType tab = TabType.Expenditure,
            ChartFormat format = ChartFormat.Charts)
        {
            if (FeatureManager.IsDisabled(Features.RevisedSchoolPage))
            {
                return Redirect($"/federation?fuid={fuid}");
            }

            OverwriteDefaultUnitTypeForSelectedTab(tab, ref unit);
            var chartGroup = DetectDefaultChartGroupFromTabType(tab);

            var vm = await BuildFederationViewModelAsync(fuid, tab, chartGroup, unit);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unit;
            ViewBag.ChartFormat = format;
            ViewBag.EstablishmentType = EstablishmentType.Federation;

            return View("Detail", vm);
        }

        public async Task<PartialViewResult> GetCharts(
            long fuid,
            TabType revGroup,
            ChartGroupType chartGroup,
            UnitType unit,
            ChartFormat format = ChartFormat.Charts)
        {
            var vm = await BuildFederationViewModelAsync(fuid, revGroup, chartGroup, unit);

            ViewBag.Tab = revGroup;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unit;
            ViewBag.ChartFormat = format;
            ViewBag.EstablishmentType = EstablishmentType.Federation;

            return PartialView("Partials/Chart", vm);
        }

        public async Task<JsonResult> GetMapData(long fuid)
        {
            var context = await _contextDataService.GetSchoolDataObjectByUrnAsync(fuid);
            var schoolsInFederation = (await _contextDataService.GetMultipleSchoolDataObjectsByUrnsAsync(context.FederationMembers.ToList())).Select(d => new SchoolViewModel(d));

            var results = new List<SchoolSummaryViewModel>();
            foreach (var school in schoolsInFederation)
            {
                var schoolVm = new SchoolSummaryViewModel(school);
                results.Add(schoolVm);
            }

            return Json(new { count = results.Count, results = results }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Download(long fuid)
        {

            var vm = await BuildFederationViewModelAsync(fuid, TabType.AllExcludingSchoolPerf, ChartGroupType.All, UnitType.AbsoluteMoney);

            var csv = _csvBuilder.BuildCSVContentHistorically(vm, await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Federation));

            return File(Encoding.UTF8.GetBytes(csv), "text/plain", $"HistoricalData-{fuid}.csv");
        }

        [Route("federation/start-benchmarking")]
        public ViewResult StartBenchmarking(long fuid)
        {
            ViewBag.fuid = fuid;
            return View();
        }

        private async Task<SchoolTrustFinancialDataObject> GetLatestFinance(long fuid)
        {
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Federation);
            var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
            var finance = await _financialDataService.GetFederationFinancialDataObjectByFuidAsync(fuid, term);
            return finance;
        }

        private async Task<FederationViewModel> BuildFederationViewModelAsync(long fuid, TabType tab, ChartGroupType chartGroup, UnitType unitType)
        {
            var vm = new FederationViewModel(fuid, _benchmarkBasketService.GetSchoolBenchmarkList());

            vm.HistoricalCharts = _historicalChartBuilder.Build(tab, chartGroup, vm.EstablishmentType, unitType);
            vm.ChartGroups = _historicalChartBuilder.Build(tab, vm.EstablishmentType).DistinctBy(c => c.ChartGroup).ToList();
            vm.LatestTerm = await LatestFederationTermAsync();
            vm.Tab = tab;

            vm.ContextData = await _contextDataService.GetSchoolDataObjectByUrnAsync(fuid);
            vm.HistoricalFinancialDataModels = await GetFinancialDataHistoricallyAsync(fuid);
            _fcService.PopulateHistoricalChartsWithFinancialData(vm.HistoricalCharts, vm.HistoricalFinancialDataModels, vm.LatestTerm, vm.Tab, unitType, vm.EstablishmentType);

            vm.SchoolsInFederation = await _contextDataService.GetMultipleSchoolDataObjectsByUrnsAsync(vm.FederationMembersURNs.ToList());
            
            vm.LaName = _laService.GetLaName(vm.La.ToString());

            return vm;
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataHistoricallyAsync(long fuid)
        {
            var models = new List<FinancialDataModel>();
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Federation);

            var taskList = new List<Task<SchoolTrustFinancialDataObject>>();
            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var task = _financialDataService.GetFederationFinancialDataObjectByFuidAsync(fuid, term);
                taskList.Add(task);
            }

            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var taskResult = await taskList[ChartHistory.YEARS_OF_HISTORY - 1 - i];
                var resultObject = taskResult;

                if (resultObject != null && resultObject.DidNotSubmit)
                {
                    var emptyObj = new SchoolTrustFinancialDataObject();
                    emptyObj.DidNotSubmit = true;
                    resultObject = emptyObj;
                }

                models.Add(new FinancialDataModel(fuid.ToString(), term, resultObject, EstablishmentType.Federation));
            }

            return models;
        }

        private async Task<string> LatestFederationTermAsync()
        {
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Federation);
            return SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
        }

        private void OverwriteDefaultUnitTypeForSelectedTab(TabType tabType, ref UnitType unitType)
        {
            switch (tabType)
            {
                case TabType.Workforce:
                    unitType = UnitType.AbsoluteCount;
                    break;
                case TabType.Balance:
                    unitType = unitType == UnitType.PercentageOfTotalIncome || unitType == UnitType.PercentageOfTotalExpenditure || unitType == UnitType.PerPupil || unitType == UnitType.PerTeacher ? unitType : UnitType.AbsoluteMoney;
                    break;
                case TabType.Income:
                    unitType = unitType == UnitType.PercentageOfTotalExpenditure ? UnitType.PercentageOfTotalExpenditure : unitType;
                    break;
                case TabType.Expenditure:
                    unitType = unitType == UnitType.PercentageOfTotalIncome ? UnitType.PercentageOfTotalIncome : unitType;
                    break;
            }
        }

        private ChartGroupType DetectDefaultChartGroupFromTabType(TabType tab)
        {
            ChartGroupType chartGroup;
            switch (tab)
            {
                case TabType.Expenditure:
                    chartGroup = ChartGroupType.TotalExpenditure;
                    break;
                case TabType.Income:
                    chartGroup = ChartGroupType.TotalIncome;
                    break;
                case TabType.Balance:
                    chartGroup = ChartGroupType.InYearBalance;
                    break;
                case TabType.Workforce:
                    chartGroup = ChartGroupType.Workforce;
                    break;
                default:
                    chartGroup = ChartGroupType.All;
                    break;
            }

            return chartGroup;
        }
    }
}