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
        private readonly IFinancialCalculationsService _fcService;
        private readonly IDownloadCSVBuilder _csvBuilder;
        private readonly ISchoolBenchmarkListService _benchmarkBasketService;
        private readonly IActiveEstablishmentsService _activeEstabService;
        private readonly ISchoolVMBuilder _schoolVMBuilder;
        private IGiasLookupService _giasLookupService;
        private ICscpLookupService _cscpLookupService;

        public SchoolController(IFinancialDataService financialDataService, 
            IFinancialCalculationsService fcService, IContextDataService contextDataService, IDownloadCSVBuilder csvBuilder, 
            ISchoolBenchmarkListService benchmarkBasketService,
            IActiveEstablishmentsService activeEstabService, ISchoolVMBuilder schoolVMBuilder,
            IGiasLookupService giasLookupService,
            ICscpLookupService cscpLookupService)
        {
            _financialDataService = financialDataService;
            _fcService = fcService;
            _contextDataService = contextDataService;
            _csvBuilder = csvBuilder;
            _benchmarkBasketService = benchmarkBasketService;
            _activeEstabService = activeEstabService;
            _schoolVMBuilder = schoolVMBuilder;
            _giasLookupService = giasLookupService;
            _cscpLookupService = cscpLookupService;
        }

        public async Task<ActionResult> Index(long urn,
            UnitType unit = UnitType.AbsoluteMoney,
            CentralFinancingType financing = CentralFinancingType.Include,
            TabType tab = TabType.Expenditure,
            ChartFormat format = ChartFormat.Charts)
        {
            

            OverwriteDefaultUnitTypeForSelectedTab(tab, ref unit);

            await _schoolVMBuilder.BuildCoreAsync(urn);
            _schoolVMBuilder.SetTab(tab);
            await _schoolVMBuilder.AddHistoricalChartsAsync(tab, DetectDefaultChartGroupFromTabType(tab), financing, unit);

            // prevent duplicate chart population from Workforce tab
            if (tab != TabType.Workforce) 
            {
                await _schoolVMBuilder.AddHistoricalChartsAsync(TabType.Workforce, DetectDefaultChartGroupFromTabType(TabType.Workforce), CentralFinancingType.Include, UnitType.AbsoluteCount);
            }

            _schoolVMBuilder.SetChartGroups(tab);
            _schoolVMBuilder.AssignLaName();
            var schoolVM = _schoolVMBuilder.GetResult();

            var hasCscpUrl = await _cscpLookupService.CscpHasPage((int)urn, false);
            var hasGiasUrl = await _giasLookupService.GiasHasPage((int)urn, false);
            
            schoolVM.HasCscpUrl = hasCscpUrl;
            schoolVM.HasGiasUrl = hasGiasUrl;

            if (schoolVM.IsFederation)
            {
                return Redirect("/federation/detail?fuid=" + schoolVM.Id);
            }

            if (schoolVM.ContextData == null)
            {
                return View("EmptyResult", new SearchViewModel(_benchmarkBasketService.GetSchoolBenchmarkList(), SearchTypes.SEARCH_BY_NAME_ID));
            }

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = schoolVM.HistoricalCharts.First().ChartGroup;
            ViewBag.UnitType = schoolVM.HistoricalCharts.First().ShowValue;
            ViewBag.Financing = financing;
            ViewBag.IsSATinLatestFinance = schoolVM.IsSATinLatestFinance;
            ViewBag.EstablishmentType = schoolVM.EstablishmentType;
            ViewBag.ChartFormat = format;
            ViewBag.ShouldShowDashBoard = schoolVM.LatestYearFinancialData?.TotalExpenditure != null;

            // AB#63763: use CFR instead of GIAS pupil count
            if (schoolVM.LatestYearFinancialData?.PupilCount.GetValueOrDefault() > 0)
            {
                schoolVM.ContextData.NumberOfPupils = Convert.ToSingle(schoolVM.LatestYearFinancialData.PupilCount);
            }

            return View("Index", schoolVM);
        }

        //#if !DEBUG
        //[OutputCache (Duration=28800, VaryByParam= "urn;unit;financing;tab;format", Location = OutputCacheLocation.Server, NoStore=true)]
        //#endif
        public ActionResult Detail(long urn)
        {
            return RedirectToActionPermanent("Index", new { urn = urn });
        }

        [Route("school/start-benchmarking")]
        public async Task<ViewResult> StartBenchmarking(long urn)
        {
            await _schoolVMBuilder.BuildCoreAsync(urn);
            await _schoolVMBuilder.AddLatestYearFinanceAsync();
            var schoolVM = _schoolVMBuilder.GetResult();
            return View(schoolVM);
        }

        public async Task<PartialViewResult> GetCharts(
        long urn,
        TabType revGroup,
        ChartGroupType chartGroup,
        UnitType unit,
        CentralFinancingType financing = CentralFinancingType.Include,
        ChartFormat format = ChartFormat.Charts,
        Boolean isSchoolPage = false)
        {
            await _schoolVMBuilder.BuildCoreAsync(urn);
            _schoolVMBuilder.SetTab(revGroup);
            await _schoolVMBuilder.AddHistoricalChartsAsync(revGroup, chartGroup, financing, unit);
            _schoolVMBuilder.SetChartGroups(revGroup);
            var schoolVM = _schoolVMBuilder.GetResult();

            ViewBag.ChartFormat = format;
            ViewBag.Financing = financing;
            ViewBag.IsSatInLatestFinance = schoolVM.IsSATinLatestFinance;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.EstablishmentType = schoolVM.EstablishmentType;
            ViewBag.IsSchoolPage = isSchoolPage;

            return PartialView("Partials/Chart", schoolVM);
        }

        public async Task<ActionResult> Download(long urn)
        {
            await _schoolVMBuilder.BuildCoreAsync(urn);
            _schoolVMBuilder.SetTab(TabType.AllIncludingSchoolPerf);
            await _schoolVMBuilder.AddHistoricalChartsAsync(TabType.AllIncludingSchoolPerf, ChartGroupType.All, CentralFinancingType.Include, UnitType.AbsoluteMoney);
            _schoolVMBuilder.SetChartGroups(TabType.AllIncludingSchoolPerf);
            var schoolVM = _schoolVMBuilder.GetResult();
            
            var csv = _csvBuilder.BuildCSVContentHistorically(schoolVM, await _financialDataService.GetLatestDataYearPerEstabTypeAsync(schoolVM.EstablishmentType));

            return File(Encoding.UTF8.GetBytes(csv), "text/plain", $"HistoricalData-{urn}.csv");
        }

        //[HttpHead]
        //[AllowAnonymous]
        //[OutputCache (Duration=28800, VaryByParam= "urn", Location = OutputCacheLocation.Server, NoStore=true)]
        //public async Task<ActionResult> Status(long urn)
        //{
        //    try
        //    {
        //        var activeUrns = await _activeEstabService.GetAllActiveUrnsAsync();
        //        var found = activeUrns.Contains(urn);
        //        return found ? new HttpStatusCodeResult(HttpStatusCode.OK) : new HttpStatusCodeResult(HttpStatusCode.NoContent);
        //    }
        //    catch
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.ServiceUnavailable);
        //    }
        //}

        public async Task<PartialViewResult> UpdateBenchmarkBasket(long? urn, CookieActions withAction)
        {          
            if (urn.HasValue)
            {
                switch (withAction)
                {
                    case CookieActions.SetDefault:
                        await _benchmarkBasketService.SetSchoolAsDefaultAsync(urn.GetValueOrDefault());
                        break;
                    case CookieActions.Add:
                        await _benchmarkBasketService.AddSchoolToBenchmarkListAsync(urn.GetValueOrDefault());
                        break;
                    case CookieActions.Remove:
                        await _benchmarkBasketService.RemoveSchoolFromBenchmarkListAsync(urn.GetValueOrDefault());
                        break;
                    case CookieActions.UnsetDefault:
                        _benchmarkBasketService.UnsetDefaultSchool();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                _benchmarkBasketService.ClearSchoolBenchmarkList();
            }                       

            return PartialView("Partials/BenchmarkListBanner",
                new SchoolViewModel(_benchmarkBasketService.GetSchoolBenchmarkList()));
        }
        
        public async Task<PartialViewResult> UpdateBenchmarkBasketAddMultiple(int[] urns)
        {            
            foreach (var urn in urns)
            {
                await _benchmarkBasketService.TryAddSchoolToBenchmarkListAsync(urn);
            }

            return PartialView("Partials/BenchmarkListBanner",
                new SchoolViewModel(_benchmarkBasketService.GetSchoolBenchmarkList()));
        }

        public PartialViewResult GetBenchmarkBasket()
        {
            return PartialView("Partials/BenchmarkListBanner", new SchoolViewModel(_benchmarkBasketService.GetSchoolBenchmarkList()));
        }

        public async Task<PartialViewResult> GetBenchmarkControls(long urn)
        {
            return PartialView("Partials/BenchmarkControlButtons", new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), _benchmarkBasketService.GetSchoolBenchmarkList()));
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

        private async Task<string> LatestTermAsync(EstablishmentType type)
        {
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(type);
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