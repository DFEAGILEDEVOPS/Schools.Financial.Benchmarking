using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Services;
using System.Text;
using Microsoft.Ajax.Utilities;
using SFB.Web.Common;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.Domain.Services.DataAccess;
using System.Threading.Tasks;
using SFB.Web.DAL;
using SFB.Web.Domain.Models;
using System.Web.UI;//Do not remove. Required in release mode build
using SFB.Web.Common.DataObjects;
using SFB.Web.Domain.ApiWrappers;

namespace SFB.Web.UI.Controllers
{
    [Authorize]
    public class SchoolController : Controller
    {
        private readonly IContextDataService _contextDataService;
        private readonly IFinancialDataService _financialDataService;
        private readonly IHistoricalChartBuilder _historicalChartBuilder;
        private readonly IFinancialCalculationsService _fcService;
        private readonly IDownloadCSVBuilder _csvBuilder;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;
        private readonly IApiRequest _apiRequest;

        public SchoolController(IHistoricalChartBuilder historicalChartBuilder, IFinancialDataService financialDataService, 
            IFinancialCalculationsService fcService, IContextDataService contextDataService, IDownloadCSVBuilder csvBuilder, 
            IBenchmarkBasketCookieManager benchmarkBasketCookieManager, IApiRequest apiRequest)
        {
            _historicalChartBuilder = historicalChartBuilder;
            _financialDataService = financialDataService;
            _fcService = fcService;
            _contextDataService = contextDataService;
            _csvBuilder = csvBuilder;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
            _apiRequest = apiRequest;
        }

        #if !DEBUG
            [OutputCache (Duration=14400, VaryByParam= "urn;unit;financing;tab;format", Location = OutputCacheLocation.Server, NoStore=true)]
        #endif
        public async Task<ActionResult> Detail(int urn, UnitType unit = UnitType.AbsoluteMoney, CentralFinancingType financing = CentralFinancingType.Include, RevenueGroupType tab = RevenueGroupType.Expenditure, ChartFormat format = ChartFormat.Charts)
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
                case RevenueGroupType.Workforce:
                    chartGroup = ChartGroupType.Workforce;
                    break;
                default:
                    chartGroup = ChartGroupType.All;
                    break;
            }

            var schoolDetailsFromEdubase = _contextDataService.GetSchoolDataObjectByUrn(urn);

            if (schoolDetailsFromEdubase == null)
            {
                return View("EmptyResult", new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), SearchTypes.SEARCH_BY_NAME_ID));
            }

            SchoolViewModel schoolVM = await BuildSchoolVMAsync(tab, chartGroup, financing, schoolDetailsFromEdubase);

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

            _fcService.PopulateHistoricalChartsWithSchoolData(schoolVM.HistoricalCharts, schoolVM.HistoricalFinancialDataModels, (BuildTermsList(schoolVM.EstablishmentType)).First(), tab, unitType, schoolVM.EstablishmentType);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unitType;
            ViewBag.Financing = financing;
            ViewBag.IsSAT = schoolVM.IsSAT;
            ViewBag.EstablishmentType = schoolVM.EstablishmentType;
            ViewBag.ChartFormat = format;
            ViewBag.SptReportExists = SptReportExists(schoolVM.Id);

            return View("Detail", schoolVM);
        }

        [HttpHead]
        public ActionResult Status(int urn)
        { 
            var urns = (List<int>)HttpContext.Cache.Get("SFBActiveURNList");
            var found = urns.Contains(urn);         
            return found ? new HttpStatusCodeResult(HttpStatusCode.OK) : new HttpStatusCodeResult(HttpStatusCode.NoContent);                               
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

                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add,
                    new BenchmarkSchoolModel()
                    {
                        Name = benchmarkSchool.Name,
                        Urn = benchmarkSchool.Id.ToString(),
                        Type = benchmarkSchool.Type,
                        EstabType = benchmarkSchool.EstablishmentType.ToString()
                    });
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

        public async Task<PartialViewResult> GetCharts(int urn, string term, RevenueGroupType revGroup, ChartGroupType chartGroup, UnitType unit, CentralFinancingType financing = CentralFinancingType.Include, ChartFormat format = ChartFormat.Charts)
        {
            financing = revGroup == RevenueGroupType.Workforce ? CentralFinancingType.Exclude : financing;

            var schoolDetailsFromEdubase = _contextDataService.GetSchoolDataObjectByUrn(urn);

            SchoolViewModel schoolVM = await BuildSchoolVMAsync(revGroup, chartGroup, financing, schoolDetailsFromEdubase, unit);

            _fcService.PopulateHistoricalChartsWithSchoolData(schoolVM.HistoricalCharts, schoolVM.HistoricalFinancialDataModels, term, revGroup, unit, schoolVM.EstablishmentType);

            ViewBag.ChartFormat = format;
            ViewBag.Financing = financing;
            ViewBag.IsSat = schoolVM.IsSAT;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.EstablishmentType = schoolVM.EstablishmentType;

            return PartialView("Partials/Chart", schoolVM);
        }

        public async Task<ActionResult> Download(int urn)
        {
            var schoolDetailsFromEdubase = _contextDataService.GetSchoolDataObjectByUrn(urn);

            SchoolViewModel schoolVM = await BuildSchoolVMAsync(RevenueGroupType.AllIncludingSchoolPerf, ChartGroupType.All, CentralFinancingType.Include, schoolDetailsFromEdubase);

            var termsList = BuildTermsList(schoolVM.EstablishmentType);
            _fcService.PopulateHistoricalChartsWithSchoolData(schoolVM.HistoricalCharts, schoolVM.HistoricalFinancialDataModels, termsList.First(), RevenueGroupType.AllExcludingSchoolPerf, UnitType.AbsoluteMoney, schoolVM.EstablishmentType);

            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(schoolVM.EstablishmentType);

            var csv = _csvBuilder.BuildCSVContentHistorically(schoolVM, latestYear);

            return File(Encoding.UTF8.GetBytes(csv),
                         "text/plain",
                         $"HistoricalData-{urn}.csv");
        }

        private async Task<SchoolViewModel> BuildSchoolVMAsync(RevenueGroupType revenueGroup, ChartGroupType chartGroup, CentralFinancingType cFinance, EdubaseDataObject schoolDetailsData, UnitType unit = UnitType.AbsoluteCount)
        {
            var schoolVM = new SchoolViewModel(schoolDetailsData, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());

            schoolVM.HistoricalCharts = _historicalChartBuilder.Build(revenueGroup, chartGroup, schoolVM.EstablishmentType, unit);
            schoolVM.ChartGroups = _historicalChartBuilder.Build(revenueGroup, schoolVM.EstablishmentType).DistinctBy(c => c.ChartGroup).ToList();
            schoolVM.Terms = BuildTermsList(schoolVM.EstablishmentType);
            schoolVM.Tab = revenueGroup;

            cFinance = revenueGroup == RevenueGroupType.Workforce ? CentralFinancingType.Exclude : cFinance;//TODO: Remove this rule after WF data is distributed

            schoolVM.HistoricalFinancialDataModels = await this.GetFinancialDataHistoricallyAsync(schoolVM.Id, schoolVM.EstablishmentType, cFinance);

            schoolVM.TotalRevenueIncome = schoolVM.HistoricalFinancialDataModels.Last().TotalIncome;
            schoolVM.TotalRevenueExpenditure = schoolVM.HistoricalFinancialDataModels.Last().TotalExpenditure;
            schoolVM.InYearBalance = schoolVM.HistoricalFinancialDataModels.Last().InYearBalance;

            return schoolVM;
        }

        private List<string> BuildTermsList(EstablishmentType type)
        {
            var years = new List<string>();
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(type);
            for (int i = 0; i < ChartHistory.YEARS_OF_HISTORY; i++)
            {                
                years.Add(FormatHelpers.FinancialTermFormatAcademies(latestYear - i));
            }

            return years;
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataHistoricallyAsync(int urn, EstablishmentType estabType, CentralFinancingType cFinance)
        {
            var models = new List<FinancialDataModel>();
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(estabType);
            
            var taskList = new List<Task<IEnumerable<SchoolTrustFinancialDataObject>>>();
            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var task = _financialDataService.GetSchoolFinancialDataObjectAsync(urn, term, estabType, cFinance);
                taskList.Add(task);
            }

            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var taskResult = await taskList[ChartHistory.YEARS_OF_HISTORY - 1 - i];
                var resultDataObject = taskResult?.FirstOrDefault();
                var dataGroup = estabType.ToDataGroup(cFinance);

                if (dataGroup == DataGroups.MATAllocs && resultDataObject == null)//if nothing found in MAT-Allocs collection try to source it from (non-allocated) Academies data
                {
                    resultDataObject = (await _financialDataService.GetSchoolFinancialDataObjectAsync(urn, term, estabType, CentralFinancingType.Exclude))
                        ?.FirstOrDefault();
                }
                
                if (resultDataObject != null && resultDataObject.DidNotSubmit)//School did not submit finance, return & display "no data" in the charts
                {
                    resultDataObject = null;
                }

                models.Add(new FinancialDataModel(urn.ToString(), term, resultDataObject, estabType));
            }
            
            return models;
        }

        private bool SptReportExists(int urn)
        {
            return _apiRequest.Head("/estab-details/", new List<string> { urn.ToString() }).statusCode == HttpStatusCode.OK;
        }
    }
}