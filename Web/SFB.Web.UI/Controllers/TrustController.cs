using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Services;
using System.Text;
using Microsoft.Ajax.Utilities;
using SFB.Web.ApplicationCore;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Services.Search;
using SFB.Web.ApplicationCore.Entities;
using System;
using SFB.Web.UI.Attributes;
using System.Web.Routing;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class TrustController : Controller
    {
        private readonly IFinancialDataService _financialDataService;
        private readonly IContextDataService _contexDataService;
        private readonly IHistoricalChartBuilder _historicalChartBuilder;
        private readonly IFinancialCalculationsService _fcService;
        private readonly IDownloadCSVBuilder _csvBuilder;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;

        public TrustController(IHistoricalChartBuilder historicalChartBuilder, IFinancialDataService financialDataService, 
            IFinancialCalculationsService fcService, IContextDataService contexDataService, IDownloadCSVBuilder csvBuilder,
            IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
        {
            _historicalChartBuilder = historicalChartBuilder;
            _financialDataService = financialDataService;
            _contexDataService = contexDataService;
            _fcService = fcService;
            _csvBuilder = csvBuilder;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
        }

        public async Task<ActionResult> Index(int companyNo, UnitType unit = UnitType.AbsoluteMoney, TabType tab = TabType.Expenditure, MatFinancingType financing = MatFinancingType.TrustAndAcademies, ChartFormat format = ChartFormat.Charts)
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
                default:
                    chartGroup = ChartGroupType.All;
                    break;
            }

            var academies = _financialDataService.GetAcademiesByCompanyNumber(LatestMATTerm(), companyNo).OrderBy(a => a.EstablishmentName).ToList();

            var trustVM = await BuildTrustVMAsync(companyNo, academies, tab, chartGroup, financing);

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

            return View(trustVM);
        }

        public async Task<PartialViewResult> GetCharts(int companyNo, string name, TabType revGroup, ChartGroupType chartGroup, UnitType unit, MatFinancingType financing = MatFinancingType.TrustAndAcademies, ChartFormat format = ChartFormat.Charts)
        {
            var dataResponse = _financialDataService.GetAcademiesByCompanyNumber(LatestMATTerm(), companyNo);

            var trustVM = await BuildTrustVMAsync(companyNo, dataResponse, revGroup, chartGroup, financing);

            _fcService.PopulateHistoricalChartsWithFinancialData(trustVM.HistoricalCharts, trustVM.HistoricalFinancialDataModels, LatestMATTerm(), revGroup, unit, EstablishmentType.MAT);

            ViewBag.ChartFormat = format;
            ViewBag.EstablishmentType = EstablishmentType.MAT;
            ViewBag.UnitType = unit;
            ViewBag.Financing = financing;

            return PartialView("Partials/Chart", trustVM);
        }

        public async Task<ActionResult> Download(int companyNo, string name)
        {
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);
            var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);

            var response = _financialDataService.GetAcademiesByCompanyNumber(term, companyNo);

            var trustVM = await BuildTrustVMAsync(companyNo, response, TabType.AllExcludingSchoolPerf, ChartGroupType.All, MatFinancingType.TrustOnly);

            var termsList = _financialDataService.GetActiveTermsForMatCentral();
            _fcService.PopulateHistoricalChartsWithFinancialData(trustVM.HistoricalCharts, trustVM.HistoricalFinancialDataModels, termsList.First(), TabType.AllExcludingSchoolPerf, UnitType.AbsoluteMoney, EstablishmentType.MAT);
            
            string csv = _csvBuilder.BuildCSVContentHistorically(trustVM, latestYear);

            return File(Encoding.UTF8.GetBytes(csv),
                         "text/plain",
                         $"HistoricalData-{name}.csv");
        }

        private async Task<TrustViewModel> BuildTrustVMAsync(int companyNo, List<AcademiesContextualDataObject> academiesList, TabType tab, ChartGroupType chartGroup, MatFinancingType matFinancing)
        {
            var comparisonListVM = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            var trustVM = new TrustViewModel(companyNo, academiesList, comparisonListVM);
            
            trustVM.HistoricalCharts = _historicalChartBuilder.Build(tab, chartGroup, trustVM.EstablishmentType);
            trustVM.ChartGroups = _historicalChartBuilder.Build(tab, trustVM.EstablishmentType).DistinctBy(c => c.ChartGroup).ToList();
            trustVM.LatestTerm = LatestMATTerm();
            trustVM.AcademiesContextualCount = await _contexDataService.GetAcademiesCountByCompanyNumberAsync(companyNo);

            trustVM.HistoricalFinancialDataModels = await this.GetFinancialDataHistoricallyAsync(trustVM.CompanyNo, matFinancing);

            return trustVM;
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataHistoricallyAsync(int companyNo, MatFinancingType matFinancing)
        {
            var models = new List<FinancialDataModel>();
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);

            var taskList = new List<Task<IEnumerable<SchoolTrustFinancialDataObject>>>();
            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var task = _financialDataService.GetTrustFinancialDataObjectAsync(companyNo, term, matFinancing);
                taskList.Add(task);
            }

            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var taskResult = await taskList[ChartHistory.YEARS_OF_HISTORY - 1 - i];
                var resultObject = taskResult?.FirstOrDefault();

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

        private string LatestMATTerm()
        {
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);
            return SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
        }
    }
}