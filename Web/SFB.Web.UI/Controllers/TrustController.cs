using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Services;
using System.Text;
using Microsoft.Ajax.Utilities;
using Microsoft.Azure.Documents;
using SFB.Web.Common;
using SFB.Web.Domain.Models;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Domain.Services.Search;
using SFB.Web.Common.DataObjects;

namespace SFB.Web.UI.Controllers
{
    public class TrustController : Controller
    {
        private readonly ITrustSearchService _trustSearchService;
        private readonly IFinancialDataService _financialDataService;
        private readonly IHistoricalChartBuilder _historicalChartBuilder;
        private readonly IFinancialCalculationsService _fcService;
        private readonly IDownloadCSVBuilder _csvBuilder;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;

        public TrustController(IHistoricalChartBuilder historicalChartBuilder, IFinancialDataService financialDataService, 
            IFinancialCalculationsService fcService, ITrustSearchService trustSearchService, IDownloadCSVBuilder csvBuilder,
            IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
        {
            _historicalChartBuilder = historicalChartBuilder;
            _financialDataService = financialDataService;
            _fcService = fcService;
            _trustSearchService = trustSearchService;
            _csvBuilder = csvBuilder;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
        }

        public async Task<ActionResult> Search(string name, string orderby = "", int page = 1)
        {
            var response = await _trustSearchService.SearchTrustByName(name, (page - 1) * SearchDefaults.RESULTS_PER_PAGE, SearchDefaults.RESULTS_PER_PAGE, orderby, Request.QueryString);

            TrustListViewModel vm = GetTrustViewModelList(response, orderby, page);

            return View("TrustResults", vm);
        }

        public async Task<ActionResult> Index(string matNo, string name, UnitType unit = UnitType.AbsoluteMoney, RevenueGroupType tab = RevenueGroupType.Expenditure, MatFinancingType financing = MatFinancingType.TrustAndAcademies, ChartFormat format = ChartFormat.Charts)
        {
            ChartGroupType chartGroup;
            switch (tab)
            {
                case RevenueGroupType.Expenditure:
                    chartGroup = ChartGroupType.TotalExpenditure;
                    break;
                case RevenueGroupType.Income:
                    chartGroup = ChartGroupType.TotalIncome;
                    break;
                case RevenueGroupType.Balance:
                    chartGroup = ChartGroupType.InYearBalance;
                    break;
                default:
                    chartGroup = ChartGroupType.All;
                    break;
            }
            
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);
            var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
          
            var academies = _financialDataService.GetAcademiesByMatNumber(term, matNo);

            var trustVM = await BuildTrustVMAsync(matNo, name, academies, tab, chartGroup, financing);

            List<string> terms = _financialDataService.GetActiveTermsForMatCentral();
            var latestTerm = terms.First();

            UnitType unitType;
            switch (tab)
            {
                case RevenueGroupType.Workforce:
                    unitType = UnitType.AbsoluteCount;
                    break;
                case RevenueGroupType.Balance:
                    unitType = unit == UnitType.AbsoluteMoney || unit == UnitType.PerPupil || unit == UnitType.PerTeacher ? unit : UnitType.AbsoluteMoney;
                    break;
                default:
                    unitType = unit;
                    break;
            }

            _fcService.PopulateHistoricalChartsWithSchoolData(trustVM.HistoricalCharts, trustVM.HistoricalFinancialDataModels, latestTerm, tab, unitType, EstablishmentType.Academies);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unitType;
            ViewBag.Financing = financing;
            ViewBag.ChartFormat = format;

            return View(trustVM);
        }

        public async Task<PartialViewResult> GetCharts(string matNo, string name, string term, RevenueGroupType revGroup, ChartGroupType chartGroup, UnitType unit, MatFinancingType financing = MatFinancingType.TrustAndAcademies, ChartFormat format = ChartFormat.Charts)
        {
            var dataResponse = _financialDataService.GetAcademiesByMatNumber(term, matNo);

            var trustVM = await BuildTrustVMAsync(matNo, name, dataResponse, revGroup, chartGroup, financing);

            _fcService.PopulateHistoricalChartsWithSchoolData(trustVM.HistoricalCharts, trustVM.HistoricalFinancialDataModels, term, revGroup, unit, EstablishmentType.MAT);

            ViewBag.ChartFormat = format;

            return PartialView("Partials/Chart", trustVM);
        }

        public async Task<ActionResult> Download(string matNo, string name)
        {
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);
            var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);

            var response = _financialDataService.GetAcademiesByMatNumber(term, matNo);

            var trustVM = await BuildTrustVMAsync(matNo, name, response, RevenueGroupType.AllExcludingSchoolPerf, ChartGroupType.All, MatFinancingType.TrustOnly);

            var termsList = _financialDataService.GetActiveTermsForMatCentral();
            _fcService.PopulateHistoricalChartsWithSchoolData(trustVM.HistoricalCharts, trustVM.HistoricalFinancialDataModels, termsList.First(), RevenueGroupType.AllExcludingSchoolPerf, UnitType.AbsoluteMoney, EstablishmentType.MAT);
            
            string csv = _csvBuilder.BuildCSVContentHistorically(trustVM, latestYear);

            return File(Encoding.UTF8.GetBytes(csv),
                         "text/plain",
                         $"HistoricalData-{name}.csv");
        }

        private TrustListViewModel GetTrustViewModelList(dynamic response, string orderBy, int page)
        {
            var trustListVm = new List<TrustViewModel>();
            var vm = new TrustListViewModel(trustListVm, orderBy);
            if (response != null)
            {
                foreach (var result in response.Results)
                {
                    var trustVm = new TrustViewModel(result["MATNumber"], result["TrustOrCompanyName"], null, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());
                    trustListVm.Add(trustVm);
                }

                vm.SchoolComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

                vm.Pagination = new Pagination
                {
                    Start = (SearchDefaults.RESULTS_PER_PAGE * (page - 1)) + 1,
                    Total = response.NumberOfResults,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.RESULTS_PER_PAGE
                };
            }

            return vm;
        }

        private async Task<TrustViewModel> BuildTrustVMAsync(string matNo, string name, List<AcademiesContextualDataObject> academiesList, RevenueGroupType tab, ChartGroupType chartGroup, MatFinancingType matFinancing)
        {
            var comparisonListVM = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            var trustVM = new TrustViewModel(matNo, name, academiesList, comparisonListVM);
            
            trustVM.HistoricalCharts = _historicalChartBuilder.Build(tab, chartGroup, trustVM.EstablishmentType);
            trustVM.ChartGroups = _historicalChartBuilder.Build(tab, trustVM.EstablishmentType).DistinctBy(c => c.ChartGroup).ToList();
            trustVM.Terms = _financialDataService.GetActiveTermsForAcademies();

            trustVM.HistoricalFinancialDataModels = await this.GetFinancialDataHistoricallyAsync(trustVM.MatNo, matFinancing);

            if (trustVM.HistoricalFinancialDataModels.Count > 0)
            {
                trustVM.TotalRevenueIncome = trustVM.HistoricalFinancialDataModels.Last().TotalIncome;
                trustVM.TotalRevenueExpenditure = trustVM.HistoricalFinancialDataModels.Last().TotalExpenditure;
                trustVM.InYearBalance = trustVM.HistoricalFinancialDataModels.Last().InYearBalance;
            }
            return trustVM;
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataHistoricallyAsync(string matCode, MatFinancingType matFinancing)
        {
            var models = new List<FinancialDataModel>();
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);

            var taskList = new List<Task<IEnumerable<SchoolTrustFinancialDataObject>>>();
            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var task = _financialDataService.GetTrustFinancialDataObjectAsync(matCode, term, matFinancing);
                taskList.Add(task);
            }

            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var taskResult = await taskList[ChartHistory.YEARS_OF_HISTORY - 1 - i];
                var resultObject = taskResult?.FirstOrDefault();

                if (resultObject != null && resultObject.DidNotSubmit)
                {
                    var emptyObj = new SchoolTrustFinancialDataObject();
                    emptyObj.DidNotSubmit = true;
                    resultObject = emptyObj;
                }

                models.Add(new FinancialDataModel(matCode, term, resultObject, EstablishmentType.MAT));
            }

            return models;
        }
    }
}