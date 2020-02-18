using SFB.Web.UI.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Services;
using System.Text;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.ApplicationCore.Services.DataAccess;
using System.Web.UI;//Do not remove. Required in release mode build
using SFB.Web.UI.Attributes;
using SFB.Web.ApplicationCore.Services;
using System;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers;
using System.Threading.Tasks;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class SchoolController : Controller
    {
        private readonly IContextDataService _contextDataService;
        private readonly IFinancialDataService _financialDataService;
        private readonly IHistoricalChartBuilder _historicalChartBuilder;
        private readonly IFinancialCalculationsService _fcService;
        private readonly IDownloadCSVBuilder _csvBuilder;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;
        private readonly ILocalAuthoritiesService _laSearchService;
        private readonly IActiveUrnsService _activeUrnsService;
        private readonly ISchoolVMBuilder _schoolVMBuilder;

        public SchoolController(IHistoricalChartBuilder historicalChartBuilder, IFinancialDataService financialDataService, 
            IFinancialCalculationsService fcService, IContextDataService contextDataService, IDownloadCSVBuilder csvBuilder, 
            IBenchmarkBasketCookieManager benchmarkBasketCookieManager, ILocalAuthoritiesService laSearchService,
            IActiveUrnsService activeUrnsService, ISchoolVMBuilder schoolVMBuilder)
        {
            _historicalChartBuilder = historicalChartBuilder;
            _financialDataService = financialDataService;
            _fcService = fcService;
            _contextDataService = contextDataService;
            _csvBuilder = csvBuilder;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
            _laSearchService = laSearchService;
            _activeUrnsService = activeUrnsService;
            _schoolVMBuilder = schoolVMBuilder;
        }

        #if !DEBUG
        [OutputCache (Duration=28800, VaryByParam= "urn;unit;financing;tab;format", Location = OutputCacheLocation.Server, NoStore=true)]
        #endif
        public async Task<ActionResult> Detail(
            int urn, 
            UnitType unit = UnitType.AbsoluteMoney, 
            CentralFinancingType financing = CentralFinancingType.Include, 
            TabType tab = TabType.Expenditure, 
            ChartFormat format = ChartFormat.Charts)
        {
            OverwriteDefaultUnitTypeForSelectedTab(tab, ref unit);

            _schoolVMBuilder.BuildCore(urn);
            await _schoolVMBuilder.AddHistoricalChartsAsync(tab, DetectDefaultChartGroupFromTabType(tab), financing, unit);
            _schoolVMBuilder.AssignLaName();
            var schoolVM = _schoolVMBuilder.GetResult();

            if (schoolVM.ContextDataModel == null)
            {
                return View("EmptyResult", new SearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), SearchTypes.SEARCH_BY_NAME_ID));
            }

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = schoolVM.HistoricalCharts.First().ChartGroup;
            ViewBag.UnitType = schoolVM.HistoricalCharts.First().ShowValue;
            ViewBag.Financing = financing;
            ViewBag.IsSAT = schoolVM.IsSAT;
            ViewBag.EstablishmentType = schoolVM.EstablishmentType;
            ViewBag.ChartFormat = format;

            return View("Detail", schoolVM);
        }

        public async Task<PartialViewResult> GetCharts(
        int urn,
        TabType revGroup,
        ChartGroupType chartGroup,
        UnitType unit,
        CentralFinancingType financing = CentralFinancingType.Include,
        ChartFormat format = ChartFormat.Charts)
        {
            _schoolVMBuilder.BuildCore(urn);
            await _schoolVMBuilder.AddHistoricalChartsAsync(revGroup, chartGroup, financing, unit);
            var schoolVM = _schoolVMBuilder.GetResult();

            ViewBag.ChartFormat = format;
            ViewBag.Financing = financing;
            ViewBag.IsSat = schoolVM.IsSAT;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.EstablishmentType = schoolVM.EstablishmentType;

            return PartialView("Partials/Chart", schoolVM);
        }

        public async Task<ActionResult> Download(int urn)
        {
            _schoolVMBuilder.BuildCore(urn);
            await _schoolVMBuilder.AddHistoricalChartsAsync(TabType.AllIncludingSchoolPerf, ChartGroupType.All, CentralFinancingType.Include, UnitType.AbsoluteMoney);
            var schoolVM = _schoolVMBuilder.GetResult();
            
            var csv = _csvBuilder.BuildCSVContentHistorically(schoolVM, _financialDataService.GetLatestDataYearPerEstabType(schoolVM.EstablishmentType));

            return File(Encoding.UTF8.GetBytes(csv), "text/plain", $"HistoricalData-{urn}.csv");
        }

        [HttpHead]
        [AllowAnonymous]
        [OutputCache (Duration=28800, VaryByParam= "urn", Location = OutputCacheLocation.Server, NoStore=true)]
        public ActionResult Status(int urn)
        {
            try
            {
                var activeUrns = _activeUrnsService.GetAllActiveUrns();
                var found = activeUrns.Contains(urn);
                return found ? new HttpStatusCodeResult(HttpStatusCode.OK) : new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.ServiceUnavailable);
            }
        }

        public PartialViewResult UpdateBenchmarkBasket(int? urn, CookieActions withAction)
        {          
            if (urn.HasValue)
            {
                var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn.GetValueOrDefault()), null);

                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(withAction,
                    new BenchmarkSchoolModel()
                    {
                        Name = benchmarkSchool.Name,
                        Urn = benchmarkSchool.Id.ToString(),
                        Type = benchmarkSchool.Type,
                        EstabType = benchmarkSchool.EstablishmentType.ToString()
                    });
            }
            else
            {
                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(withAction, null);
            }                       

            return PartialView("Partials/BenchmarkListBanner",
                new SchoolViewModel(null, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie()));
        }
        
        public PartialViewResult UpdateBenchmarkBasketAddMultiple(int[] urns)
        {            
            foreach (var urn in urns)
            {
                var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), null);

                try
                {
                    _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add,
                        new BenchmarkSchoolModel()
                        {
                            Name = benchmarkSchool.Name,
                            Urn = benchmarkSchool.Id.ToString(),
                            Type = benchmarkSchool.Type,
                            EstabType = benchmarkSchool.EstablishmentType.ToString()
                        });
                }
                catch (ApplicationException) { }
            }

            return PartialView("Partials/BenchmarkListBanner",
                new SchoolViewModel(null, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie()));
        }

        public PartialViewResult GetBenchmarkBasket()
        {
            return PartialView("Partials/BenchmarkListBanner", new SchoolViewModel(null, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie()));
        }

        public PartialViewResult GetBenchmarkControls(int urn)
        {
            return PartialView("Partials/BenchmarkControlButtons", new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie()));
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

        private string LatestTerm(EstablishmentType type)
        {
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(type);
            return SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
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