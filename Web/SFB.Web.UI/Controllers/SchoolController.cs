using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Services;
using System.Text;
using System.Web;
using System.Web.UI;
using Microsoft.Ajax.Utilities;
using SFB.Web.Common;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.Domain.Services.DataAccess;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFB.Web.DAL;
using SFB.Web.Domain.Models;

namespace SFB.Web.UI.Controllers
{
    public class SchoolController : BaseController
    {
        private readonly IContextDataService _contextDataService;
        private readonly IFinancialDataService _financialDataService;
        private readonly IHistoricalChartBuilder _historicalChartBuilder;
        private readonly IFinancialCalculationsService _fcService;
        private readonly IDownloadCSVBuilder _csvBuilder;

        public SchoolController(IHistoricalChartBuilder historicalChartBuilder, IFinancialDataService financialDataService, IFinancialCalculationsService fcService, IContextDataService contextDataService, IDownloadCSVBuilder csvBuilder)
        {
            _historicalChartBuilder = historicalChartBuilder;
            _financialDataService = financialDataService;
            _fcService = fcService;
            _contextDataService = contextDataService;
            _csvBuilder = csvBuilder;
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
 
            var schoolDetailsFromEdubase = _contextDataService.GetSchoolByUrn(urn.ToString());
            
            if (schoolDetailsFromEdubase == null)
            {
                return View("EmptyResult", new SchoolSearchViewModel(base.ExtractSchoolComparisonListFromCookie(), SearchTypes.SEARCH_BY_NAME_ID));
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

            _fcService.PopulateHistoricalChartsWithSchoolData(schoolVM.HistoricalCharts, schoolVM.HistoricalFinancialDataModels, (BuildTermsList(schoolVM.EstabType)).First(), tab, unitType, schoolVM.EstabType);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unitType;
            ViewBag.Financing = financing;
            ViewBag.ChartFormat = format;

            return View("Detail", schoolVM);
        }

        [HttpHead]
        public ActionResult Status(int urn)
        {
            var schoolDetailsFromEdubase = _contextDataService.GetSchoolByUrn(urn.ToString());
            return schoolDetailsFromEdubase == null ? new HttpStatusCodeResult(HttpStatusCode.NotFound) : new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public PartialViewResult UpdateBenchmarkBasket(int? urn, string withAction)
        {
            HttpCookie cookie;

            if (urn.HasValue)
            {
                var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolByUrn(urn.ToString()), null);

                cookie = base.UpdateSchoolComparisonListCookie(withAction,
                    new BenchmarkSchoolModel()
                    {
                        Name = benchmarkSchool.Name,
                        Urn = benchmarkSchool.Id,
                        Type = benchmarkSchool.Type,
                        EstabType = benchmarkSchool.EstabType.ToString()
                    });
            }
            else
            {
                cookie = base.UpdateSchoolComparisonListCookie(withAction, null);
            }

            if (cookie != null)
            {
                Response.Cookies.Add(cookie);
            }

            return PartialView("Partials/BenchmarkListBanner",
                new SchoolViewModel(null, base.ExtractSchoolComparisonListFromCookie()));
        }
        
        public PartialViewResult UpdateBenchmarkBasketAddMultiple(string[] urns)
        {
            HttpCookie cookie = null;

            foreach (var urn in urns)
            {
                var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolByUrn(urn), null);

                cookie = base.UpdateSchoolComparisonListCookie(CompareActions.ADD_TO_COMPARISON_LIST,
                    new BenchmarkSchoolModel()
                    {
                        Name = benchmarkSchool.Name,
                        Urn = benchmarkSchool.Id,
                        Type = benchmarkSchool.Type,
                        EstabType = benchmarkSchool.EstabType.ToString()
                    });
            }

            if (cookie != null)
            {
                Response.Cookies.Add(cookie);
            }

            return PartialView("Partials/BenchmarkListBanner",
                new SchoolViewModel(null, base.ExtractSchoolComparisonListFromCookie()));
        }

        public PartialViewResult GetBenchmarkBasket()
        {
            return PartialView("Partials/BenchmarkListBanner", new SchoolViewModel(null, base.ExtractSchoolComparisonListFromCookie()));
        }

        public PartialViewResult GetBenchmarkControls(int urn)
        {
            return PartialView("Partials/BenchmarkControlButtons", new SchoolViewModel(_contextDataService.GetSchoolByUrn(urn.ToString()), base.ExtractSchoolComparisonListFromCookie()));
        }

        public async Task<PartialViewResult> GetCharts(int urn, string term, RevenueGroupType revGroup, ChartGroupType chartGroup, UnitType unit, CentralFinancingType financing = CentralFinancingType.Include, ChartFormat format = ChartFormat.Charts)
        {
            financing = revGroup == RevenueGroupType.Workforce ? CentralFinancingType.Exclude : financing;

            var schoolDetailsFromEdubase = _contextDataService.GetSchoolByUrn(urn.ToString());

            SchoolViewModel schoolVM = await BuildSchoolVMAsync(revGroup, chartGroup, financing, schoolDetailsFromEdubase, unit);

            _fcService.PopulateHistoricalChartsWithSchoolData(schoolVM.HistoricalCharts, schoolVM.HistoricalFinancialDataModels, term, revGroup, unit, schoolVM.EstabType);

            ViewBag.ChartFormat = format;

            return PartialView("Partials/Chart", schoolVM);
        }

        public async Task<ActionResult> Download(int urn)
        {
            var schoolDetailsFromEdubase = _contextDataService.GetSchoolByUrn(urn.ToString());

            SchoolViewModel schoolVM = await BuildSchoolVMAsync(RevenueGroupType.AllIncludingSchoolPerf, ChartGroupType.All, CentralFinancingType.Include, schoolDetailsFromEdubase);

            var termsList = BuildTermsList(schoolVM.EstabType);
            _fcService.PopulateHistoricalChartsWithSchoolData(schoolVM.HistoricalCharts, schoolVM.HistoricalFinancialDataModels, termsList.First(), RevenueGroupType.AllExcludingSchoolPerf, UnitType.AbsoluteMoney, schoolVM.EstabType);

            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(schoolVM.EstabType);

            var csv = _csvBuilder.BuildCSVContentHistorically(schoolVM, latestYear);

            return File(Encoding.UTF8.GetBytes(csv),
                         "text/plain",
                         $"HistoricalData-{urn}.csv");
        }

        private async Task<SchoolViewModel> BuildSchoolVMAsync(RevenueGroupType revenueGroup, ChartGroupType chartGroup, CentralFinancingType cFinance, dynamic schoolDetailsData, UnitType unit = UnitType.AbsoluteCount)
        {
            var schoolVM = new SchoolViewModel(schoolDetailsData, base.ExtractSchoolComparisonListFromCookie());

            schoolVM.HistoricalCharts = _historicalChartBuilder.Build(revenueGroup, chartGroup, schoolVM.EstabType, unit);
            schoolVM.ChartGroups = _historicalChartBuilder.Build(revenueGroup, schoolVM.EstabType).DistinctBy(c => c.ChartGroup).ToList();
            schoolVM.Terms = BuildTermsList(schoolVM.EstabType);
            schoolVM.Tab = revenueGroup;

            schoolVM.HistoricalFinancialDataModels = await this.GetFinancialDataHistoricallyAsync(schoolVM.Id, schoolVM.EstabType, cFinance);

            schoolVM.TotalRevenueIncome = schoolVM.HistoricalFinancialDataModels.Last().TotalIncome;
            schoolVM.TotalRevenueExpenditure = schoolVM.HistoricalFinancialDataModels.Last().TotalExpenditure;
            schoolVM.InYearBalance = schoolVM.HistoricalFinancialDataModels.Last().InYearBalance;

            return schoolVM;
        }

        private List<string> BuildTermsList(EstabType type)
        {
            var years = new List<string>();
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(type);
            for (int i = 0; i < ChartHistory.YEARS_OF_HISTORY; i++)
            {                
                years.Add(FormatHelpers.FinancialTermFormatAcademies(latestYear - i));
            }

            return years;
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataHistoricallyAsync(string urn, EstabType estabType, CentralFinancingType cFinance)
        {
            var models = new List<FinancialDataModel>();
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(estabType);
            
            var taskList = new List<Task<IEnumerable<Document>>>();
            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var task = _financialDataService.GetSchoolDataDocumentAsync(urn, term, estabType, cFinance);
                taskList.Add(task);
            }

            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var taskResult = await taskList[ChartHistory.YEARS_OF_HISTORY - 1 - i];
                var resultDocument = taskResult?.FirstOrDefault();
                var dataGroup = estabType.ToDataGroup(cFinance);

                if (dataGroup == DataGroups.MATAllocs && resultDocument == null)//if nothing found in -Distributed collection try to source it from (non-distributed) Academies data
                {
                    resultDocument = (await _financialDataService.GetSchoolDataDocumentAsync(urn, term, estabType, CentralFinancingType.Exclude))
                        ?.FirstOrDefault();
                }
                
                if (resultDocument != null && resultDocument.GetPropertyValue<bool>("DNS"))//School did not submit finance, return & display "no data" in the charts
                {
                    resultDocument = null;
                }

                models.Add(new FinancialDataModel(urn, term, resultDocument, estabType));
            }
            
            return models;
        }
    }
}