using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Services;
using System.Text;
using Microsoft.Ajax.Utilities;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.UI.Attributes;
using System.Web.Routing;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers;
using System;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class TrustController : Controller
    {
        private readonly IFinancialDataService _financialDataService;
        private readonly IContextDataService _contextDataService;
        private readonly IHistoricalChartBuilder _historicalChartBuilder;
        private readonly IFinancialCalculationsService _fcService;
        private readonly ITrustHistoryService _trustHistoryService;
        private readonly IDownloadCSVBuilder _csvBuilder;
        private readonly ISchoolBenchmarkListService _benchmarkBasketService;
        private readonly IGiasLookupService _giasLookupService;
        private readonly ICscpLookupService _cscpLookupService;

        public TrustController(IHistoricalChartBuilder historicalChartBuilder, IFinancialDataService financialDataService, 
            IFinancialCalculationsService fcService, IContextDataService contextDataService, IDownloadCSVBuilder csvBuilder,
            ISchoolBenchmarkListService benchmarkBasketService, ITrustHistoryService trustHistoryService,
            IGiasLookupService giasLookupService,
            ICscpLookupService cscpLookupService)
        {
            _historicalChartBuilder = historicalChartBuilder;
            _financialDataService = financialDataService;
            _contextDataService = contextDataService;
            _fcService = fcService;
            _csvBuilder = csvBuilder;
            _benchmarkBasketService = benchmarkBasketService;
            _trustHistoryService = trustHistoryService;
            _giasLookupService = giasLookupService;
            _cscpLookupService = cscpLookupService;
        }
        
        private string[] exclusionPhaseList =  { "Nursery", "Pupil referral unit", "Special" , "16 plus"};

        public async Task<ActionResult> Index(int? companyNo, int? uid = null, UnitType unit = UnitType.AbsoluteMoney, TabType tab = TabType.Expenditure, MatFinancingType financing = MatFinancingType.TrustAndAcademies, ChartFormat format = ChartFormat.Charts)
        {
            if (companyNo == null && uid.HasValue)
            {
                var trustFinance = await _financialDataService.GetTrustFinancialDataObjectByUidAsync(uid.GetValueOrDefault(), await LatestMATTermAsync());
                companyNo = trustFinance.CompanyNumber;
            }
            return RedirectToActionPermanent("Detail", "Trust", new RouteValueDictionary { 
                { "companyNo", companyNo },
                { "unit",  unit},
                { "tab",  tab},
                { "financing", financing},
                { "format",  format}
            });
        }

        public async Task<ActionResult> Detail(int? companyNo, int? uid = null, UnitType unit = UnitType.AbsoluteMoney, TabType tab = TabType.Expenditure, MatFinancingType financing = MatFinancingType.TrustAndAcademies, ChartFormat format = ChartFormat.Charts)
        {
            if (companyNo == null && uid.HasValue)
            {
                var trustFinance = await _financialDataService.GetTrustFinancialDataObjectByUidAsync(uid.GetValueOrDefault(), await LatestMATTermAsync());
                companyNo = trustFinance.CompanyNumber;
                return RedirectToActionPermanent("Detail", "Trust", new RouteValueDictionary {
                    { "companyNo", companyNo },
                    { "unit",  unit},
                    { "tab",  tab},
                    { "financing", financing},
                    { "format",  format}
                });
            }

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
                default:
                    chartGroup = ChartGroupType.All;
                    break;
            }

            var trustVM = await BuildFullTrustVMAsync(companyNo.GetValueOrDefault(), tab, chartGroup, financing);
            var hasGiasUrl = await _giasLookupService.GiasHasPage(trustVM.UID.GetValueOrDefault(), true);
            var hasCscpUrl = await _cscpLookupService.CscpHasPage(trustVM.UID.GetValueOrDefault(), true);
           

            trustVM.HasCscpUrl = hasCscpUrl;
            trustVM.HasGiasUrl = hasGiasUrl;
            if (!trustVM.HasLatestYearFinancialData)
            {
                if (trustVM.AcademiesInFinanceList.Count == 1)
                {
                    return RedirectToActionPermanent("Detail", "School", new RouteValueDictionary { { "urn", trustVM.AcademiesInFinanceList.First().URN } });
                }
                return RedirectToActionPermanent("SuggestTrust", "TrustSearch", new RouteValueDictionary { { "trustNameId", companyNo } });
            }

            UnitType unitType;
            switch (tab)
            {
                case TabType.Workforce:
                    unitType = UnitType.AbsoluteCount;
                    break;
                case TabType.Balance:
                    unitType = unit == UnitType.AbsoluteMoney || unit == UnitType.PerPupil || unit == UnitType.PerTeacher ? unit : UnitType.AbsoluteMoney;
                    break;
                default:
                    unitType = unit;
                    break;
            }

            _fcService.PopulateHistoricalChartsWithFinancialData(trustVM.HistoricalCharts, trustVM.HistoricalFinancialDataModels, trustVM.LatestTerm, tab, unitType, EstablishmentType.Academies);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unitType;
            ViewBag.Financing = financing;
            ViewBag.ChartFormat = format;
            ViewBag.EstablishmentType = EstablishmentType.MAT;
            
            var trustSchoolsPhases = trustVM.AcademiesInContextList.Select(x => x.OverallPhase).ToList();
            
            var hasDashboardPhases = trustSchoolsPhases.Count(x => exclusionPhaseList.All(y => y != x)) > 0;
            ViewBag.ShouldShowDashBoard = trustVM.LatestYearFinancialData?.InYearBalance != null && hasDashboardPhases;
            return View("Detail", trustVM);
        }

        public async Task<PartialViewResult> GetCharts(int companyNo, TabType revGroup, ChartGroupType chartGroup, UnitType unit, MatFinancingType financing = MatFinancingType.TrustAndAcademies, ChartFormat format = ChartFormat.Charts)
        {
            var trustVM = await BuildFinancialTrustVMAsync(companyNo, revGroup, chartGroup, financing);

            _fcService.PopulateHistoricalChartsWithFinancialData(trustVM.HistoricalCharts, trustVM.HistoricalFinancialDataModels, await LatestMATTermAsync(), revGroup, unit, EstablishmentType.MAT);

            ViewBag.ChartFormat = format;
            ViewBag.EstablishmentType = EstablishmentType.MAT;
            ViewBag.UnitType = unit;
            ViewBag.Financing = financing;
            ViewBag.IsSchoolPage = false;

            return PartialView("Partials/Chart", trustVM);
        }

        public async Task<ActionResult> Download(int companyNo, string name)
        {
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.MAT);

            var trustVM = await BuildFinancialTrustVMAsync(companyNo, TabType.AllExcludingSchoolPerf, ChartGroupType.All, MatFinancingType.TrustOnly);

            var termsList = await _financialDataService.GetActiveTermsForMatCentralAsync();
            _fcService.PopulateHistoricalChartsWithFinancialData(trustVM.HistoricalCharts, trustVM.HistoricalFinancialDataModels, termsList.First(), TabType.AllExcludingSchoolPerf, UnitType.AbsoluteMoney, EstablishmentType.MAT);
            
            string csv = _csvBuilder.BuildCSVContentHistorically(trustVM, latestYear);

            return File(Encoding.UTF8.GetBytes(csv),
                         "text/plain",
                         $"HistoricalData-{name}.csv");
        }

        private async Task<TrustViewModel> BuildFinancialTrustVMAsync(int companyNo, TabType tab, ChartGroupType chartGroup, MatFinancingType matFinancing)
        {
            var comparisonListVM = _benchmarkBasketService.GetSchoolBenchmarkList();
            var trustVM = new TrustViewModel(companyNo, comparisonListVM);
            
            trustVM.HistoricalCharts = _historicalChartBuilder.Build(tab, chartGroup, trustVM.EstablishmentType);
            trustVM.ChartGroups = _historicalChartBuilder.Build(tab, trustVM.EstablishmentType).DistinctBy(c => c.ChartGroup).ToList();
            trustVM.LatestTerm = await LatestMATTermAsync();            

            trustVM.HistoricalFinancialDataModels = await this.GetFinancialDataHistoricallyAsync(trustVM.CompanyNo, matFinancing);

            trustVM.AcademiesInFinanceList = (await _financialDataService.GetAcademiesByCompanyNumberAsync(await LatestMATTermAsync(), companyNo)).OrderBy(a => a.EstablishmentName).ToList();

            return trustVM;
        }

        private async Task<TrustViewModel> BuildFullTrustVMAsync(int companyNo, TabType tab, ChartGroupType chartGroup, MatFinancingType matFinancing)
        {
            var trustVM = await BuildFinancialTrustVMAsync(companyNo, tab, chartGroup, matFinancing);
            
            trustVM.AcademiesInContextList = (await _contextDataService.GetAcademiesByUidAsync(trustVM.UID.GetValueOrDefault())).OrderBy(a => a.EstablishmentName).ToList();

            if (trustVM.UID != null)
            {
                try
                {
                    trustVM.TrustHistory = await _trustHistoryService.GetTrustHistoryModelAsync(trustVM.UID.GetValueOrDefault());
                }
                catch (NullReferenceException)
                { 
                    //Do not load trust history if missing 
                }
            }
            return trustVM;
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataHistoricallyAsync(int companyNo, MatFinancingType matFinancing)
        {
            var models = new List<FinancialDataModel>();
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.MAT);

            var taskList = new List<Task<SchoolTrustFinancialDataObject>>();
            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var task = _financialDataService.GetTrustFinancialDataObjectByCompanyNoAsync(companyNo, term, matFinancing);
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

                models.Add(new FinancialDataModel(companyNo.ToString(), term, resultObject, EstablishmentType.MAT));
            }

            return models;
        }

        private async Task<string> LatestMATTermAsync()
        {
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.MAT);
            return SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
        }
    }
}