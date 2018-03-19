using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Services;
using System.Text;
using Microsoft.Ajax.Utilities;
using SFB.Web.Common;
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

        public ActionResult Index(string matNo, string name, UnitType unit = UnitType.AbsoluteMoney, RevenueGroupType tab = RevenueGroupType.Expenditure, MatFinancingType financing = MatFinancingType.TrustAndAcademies)
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

            var sponsorVM = BuildSponsorVM(matNo, name, dataResponse, tab, chartGroup, financing);

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

            _fcService.PopulateHistoricalChartsWithSchoolData(sponsorVM.HistoricalCharts, sponsorVM.HistoricalSchoolDataModels, latestTerm, tab, unitType, SchoolFinancialType.Academies);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unitType;
            ViewBag.Financing = financing;

            return View(sponsorVM);
        }

        public PartialViewResult GetCharts(string matNo, string name, string term, RevenueGroupType revGroup, ChartGroupType chartGroup, UnitType unit, MatFinancingType financing = MatFinancingType.TrustAndAcademies)
        {
            var dataResponse = _financialDataService.GetAcademiesByMatNumber(term, matNo);

            var sponsorVM = BuildSponsorVM(matNo, name, dataResponse, revGroup, chartGroup, financing);

            _fcService.PopulateHistoricalChartsWithSchoolData(sponsorVM.HistoricalCharts, sponsorVM.HistoricalSchoolDataModels, term, revGroup, unit, SchoolFinancialType.Academies);

            return PartialView("Partials/Chart", sponsorVM);
        }

        public ActionResult Download(string matNo, string name)
        {
            var latestYear = _financialDataService.GetLatestDataYearForTrusts();
            var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);

            var response = _financialDataService.GetAcademiesByMatNumber(term, matNo);

            var sponsorVM = BuildSponsorVM(matNo, name, response, RevenueGroupType.AllExcludingSchoolPerf, ChartGroupType.All, MatFinancingType.TrustOnly);

            var termsList = _financialDataService.GetActiveTermsForMatCentral();
            _fcService.PopulateHistoricalChartsWithSchoolData(sponsorVM.HistoricalCharts, sponsorVM.HistoricalSchoolDataModels, termsList.First(), RevenueGroupType.AllExcludingSchoolPerf, UnitType.AbsoluteMoney, SchoolFinancialType.Academies);
            
            string csv = _csvBuilder.BuildCSVContentHistorically(sponsorVM, latestYear);

            return File(Encoding.UTF8.GetBytes(csv),
                         "text/plain",
                         $"HistoricalData-{name}.csv");
        }

        private SponsorViewModel BuildSponsorVM(string matNo, string name, dynamic response, RevenueGroupType tab, ChartGroupType chartGroup, MatFinancingType matFinancing)
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
            
            sponsorVM.HistoricalSchoolDataModels = this.GetFinancialDataHistorically(sponsorVM.MatNo, matFinancing);

            if (sponsorVM.HistoricalSchoolDataModels.Count > 0)
            {
                sponsorVM.TotalRevenueIncome = sponsorVM.HistoricalSchoolDataModels.Last().TotalIncome;
                sponsorVM.TotalRevenueExpenditure = sponsorVM.HistoricalSchoolDataModels.Last().TotalExpenditure;
                sponsorVM.InYearBalance = sponsorVM.HistoricalSchoolDataModels.Last().InYearBalance;
            }
            return sponsorVM;
        }

        private List<SchoolDataModel> GetFinancialDataHistorically(string matCode, MatFinancingType matFinancing)
        {
            var models = new List<SchoolDataModel>();
            var latestYear = _financialDataService.GetLatestDataYearForTrusts();
            //TODO: Can this be done in parallel threads asynchronously?
            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var dataModel = _financialDataService.GetMATDataDocument(matCode, term, matFinancing);

                models.Add(new SchoolDataModel(matCode, term, dataModel, SchoolFinancialType.Academies));
            }

            return models;
        }
    }
}