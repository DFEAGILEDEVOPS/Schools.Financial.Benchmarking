using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SFB.Web.UI.Controllers
{
    public class TrustController : BaseController
    {
        private readonly ITrustSearchService _trustSearchService;
        private readonly IFinancialDataService _financialDataService;
        private readonly IHistoricalChartBuilder _historicalChartBuilder;
        private readonly IFinancialCalculationsService _fcService;
        private readonly IDownloadCSVBuilder _csvBuilder;

        public TrustController(IHistoricalChartBuilder historicalChartBuilder, IFinancialDataService financialDataService, IFinancialCalculationsService fcService, ITrustSearchService trustSearchService, IDownloadCSVBuilder csvBuilder)
        {
            _historicalChartBuilder = historicalChartBuilder;
            _financialDataService = financialDataService;
            _fcService = fcService;
            _trustSearchService = trustSearchService;
            _csvBuilder = csvBuilder;
        }

        public async Task<ActionResult> Search(string name, string orderby = "", int page = 1)
        {
            var response = await _trustSearchService.SearchTrustByName(name, (page - 1) * SearchDefaults.RESULTS_PER_PAGE, SearchDefaults.RESULTS_PER_PAGE, orderby, Request.QueryString);

            SponsorListViewModel vm = GetTrustViewModelList(response, orderby, page);

            return View("TrustResults", vm);
        }

        private SponsorListViewModel GetTrustViewModelList(dynamic response, string orderBy, int page)
        {
            var sponsorListVm = new List<SponsorViewModel>();
            var vm = new SponsorListViewModel(sponsorListVm, null, orderBy);
            if (response != null)
            {
                foreach (var result in response.Results)
                {
                    var sponsorVm = new SponsorViewModel(result["MATNumber"], result["TrustOrCompanyName"], null, base.ExtractSchoolComparisonListFromCookie());
                    sponsorListVm.Add(sponsorVm);
                }

                vm.SchoolComparisonList = base.ExtractSchoolComparisonListFromCookie();

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
            
            var latestYear = _financialDataService.GetLatestDataYearForTrusts();
            var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
          
            var dataResponse = _financialDataService.GetAcademiesByMatNumber(term, matNo);

            var sponsorVM = await BuildSponsorVMAsync(matNo, name, dataResponse, tab, chartGroup, financing);

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

            _fcService.PopulateHistoricalChartsWithSchoolData(sponsorVM.HistoricalCharts, sponsorVM.HistoricalSchoolFinancialDataModels, latestTerm, tab, unitType, SchoolFinancialType.Academies);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unitType;
            ViewBag.Financing = financing;
            ViewBag.ChartFormat = format;

            return View(sponsorVM);
        }

        public async Task<PartialViewResult> GetCharts(string matNo, string name, string term, RevenueGroupType revGroup, ChartGroupType chartGroup, UnitType unit, MatFinancingType financing = MatFinancingType.TrustAndAcademies, ChartFormat format = ChartFormat.Charts)
        {
            var dataResponse = _financialDataService.GetAcademiesByMatNumber(term, matNo);

            var sponsorVM = await BuildSponsorVMAsync(matNo, name, dataResponse, revGroup, chartGroup, financing);

            _fcService.PopulateHistoricalChartsWithSchoolData(sponsorVM.HistoricalCharts, sponsorVM.HistoricalSchoolFinancialDataModels, term, revGroup, unit, SchoolFinancialType.Academies);

            ViewBag.ChartFormat = format;

            return PartialView("Partials/Chart", sponsorVM);
        }

        public async Task<ActionResult> Download(string matNo, string name)
        {
            var latestYear = _financialDataService.GetLatestDataYearForTrusts();
            var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);

            var response = _financialDataService.GetAcademiesByMatNumber(term, matNo);

            var sponsorVM = await BuildSponsorVMAsync(matNo, name, response, RevenueGroupType.AllExcludingSchoolPerf, ChartGroupType.All, MatFinancingType.TrustOnly);

            var termsList = _financialDataService.GetActiveTermsForMatCentral();
            _fcService.PopulateHistoricalChartsWithSchoolData(sponsorVM.HistoricalCharts, sponsorVM.HistoricalSchoolFinancialDataModels, termsList.First(), RevenueGroupType.AllExcludingSchoolPerf, UnitType.AbsoluteMoney, SchoolFinancialType.Academies);
            
            string csv = _csvBuilder.BuildCSVContentHistorically(sponsorVM, latestYear);

            return File(Encoding.UTF8.GetBytes(csv),
                         "text/plain",
                         $"HistoricalData-{name}.csv");
        }

        private async Task<SponsorViewModel> BuildSponsorVMAsync(string matNo, string name, dynamic response, RevenueGroupType tab, ChartGroupType chartGroup, MatFinancingType matFinancing)
        {
            var schoolListVM = new List<SchoolViewModel>();

            foreach (var result in response.Results)
            {
                schoolListVM.Add(new SchoolViewModel(result, null));
            }

            var comparisonListVM = base.ExtractSchoolComparisonListFromCookie();
            var sponsorVM = new SponsorViewModel(matNo, name, new SchoolListViewModel(schoolListVM, comparisonListVM), comparisonListVM);
            
            sponsorVM.HistoricalCharts = _historicalChartBuilder.Build(tab, chartGroup, sponsorVM.FinancialType);
            sponsorVM.ChartGroups = _historicalChartBuilder.Build(tab, sponsorVM.FinancialType).DistinctBy(c => c.ChartGroup).ToList();
            sponsorVM.Terms = _financialDataService.GetActiveTermsForAcademies();

            sponsorVM.HistoricalSchoolFinancialDataModels = await this.GetFinancialDataHistoricallyAsync(sponsorVM.MatNo, matFinancing);

            if (sponsorVM.HistoricalSchoolFinancialDataModels.Count > 0)
            {
                sponsorVM.TotalRevenueIncome = sponsorVM.HistoricalSchoolFinancialDataModels.Last().TotalIncome;
                sponsorVM.TotalRevenueExpenditure = sponsorVM.HistoricalSchoolFinancialDataModels.Last().TotalExpenditure;
                sponsorVM.InYearBalance = sponsorVM.HistoricalSchoolFinancialDataModels.Last().InYearBalance;
            }
            return sponsorVM;
        }

        private async Task<List<SchoolFinancialDataModel>> GetFinancialDataHistoricallyAsync(string matCode, MatFinancingType matFinancing)
        {
            var models = new List<SchoolFinancialDataModel>();
            var latestYear = _financialDataService.GetLatestDataYearForTrusts();
            
            var taskList = new List<Task<IEnumerable<Document>>>();
            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var task = _financialDataService.GetMATDataDocumentAsync(matCode, term, matFinancing);
                taskList.Add(task);
            }

            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var taskResult = await taskList[ChartHistory.YEARS_OF_HISTORY - 1 - i];
                var resultDocument = taskResult?.FirstOrDefault();

                if (resultDocument != null && resultDocument.GetPropertyValue<bool>("DNS"))
                {
                    var emptyDoc = new Document();
                    emptyDoc.SetPropertyValue("DNS", true);
                    resultDocument = emptyDoc;
                }

                models.Add(new SchoolFinancialDataModel(matCode, term, resultDocument, SchoolFinancialType.Academies));
            }

            return models;
        }
    }
}