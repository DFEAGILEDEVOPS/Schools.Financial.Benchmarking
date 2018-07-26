using System;
using SFB.Web.UI.Models;
using System.Linq;
using System.Web.Mvc;
using SFB.Web.UI.Helpers;
using System.Collections.Generic;
using SFB.Web.Domain.Services;
using SFB.Web.UI.Services;
using System.Text;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using SFB.Web.Domain.Models;
using SFB.Web.UI.Helpers.Enums;
using System.Threading.Tasks;
using SFB.Web.Common;
using SFB.Web.DAL;
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.Domain.Services.Comparison;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Common.DataObjects;

namespace SFB.Web.UI.Controllers
{
    public class BenchmarkChartsController : Controller
    {
        private readonly IBenchmarkChartBuilder _benchmarkChartBuilder;
        private readonly IFinancialDataService _financialDataService;
        private readonly IFinancialCalculationsService _fcService;
        private readonly ILocalAuthoritiesService _laService;
        private readonly IDownloadCSVBuilder _csvBuilder;
        private readonly IContextDataService _contextDataService;
        private readonly IBenchmarkCriteriaBuilderService _benchmarkCriteriaBuilderService;
        private readonly IComparisonService _comparisonService;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;

        public BenchmarkChartsController(IBenchmarkChartBuilder benchmarkChartBuilder, IFinancialDataService financialDataService, IFinancialCalculationsService fcService, ILocalAuthoritiesService laService, IDownloadCSVBuilder csvBuilder,
            IContextDataService contextDataService, IBenchmarkCriteriaBuilderService benchmarkCriteriaBuilderService, IComparisonService comparisonService,
            IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
        {
            _benchmarkChartBuilder = benchmarkChartBuilder;
            _financialDataService = financialDataService;
            _fcService = fcService;
            _laService = laService;
            _csvBuilder = csvBuilder;
            _contextDataService = contextDataService;
            _benchmarkCriteriaBuilderService = benchmarkCriteriaBuilderService;
            _comparisonService = comparisonService;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
        }

        [HttpGet]
        public ActionResult GenerateFromSimpleCriteria()
        {
            return new RedirectResult("/Errors/InvalidRequest");
        }

        [HttpPost]
        public async Task<ActionResult> GenerateFromSimpleCriteria(int urn, int basketSize, EstablishmentType estType, SimpleCriteria simpleCriteria)
        {
            var benchmarkSchool = InstantiateBenchmarkSchool(urn);
            
            var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromSimpleComparisonCriteria(benchmarkSchool.LatestYearFinancialData, simpleCriteria);

            var comparisonResult = await _comparisonService.GenerateBenchmarkListWithSimpleComparisonAsync(benchmarkCriteria, estType, basketSize, simpleCriteria, benchmarkSchool.LatestYearFinancialData);

            var benchmarkSchools = comparisonResult.BenchmarkSchools;
            benchmarkCriteria = comparisonResult.BenchmarkCriteria;

            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.RemoveAll, null);

            foreach (var schoolDoc in benchmarkSchools)
            {
                var benchmarkSchoolToAdd = new BenchmarkSchoolModel()
                {
                    Name = schoolDoc.SchoolName,
                    Type = schoolDoc.Type,
                    EstabType = schoolDoc.FinanceType,
                    Urn = schoolDoc.URN.ToString()
                };
                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, benchmarkSchoolToAdd);                
            }

            AddDefaultBenchmarkSchoolToList();

            return await Index(urn, simpleCriteria, benchmarkCriteria, ComparisonType.Basic, basketSize, benchmarkSchool.LatestYearFinancialData, estType);
        }

        [HttpGet]
        public async Task<ActionResult> GenerateForBestInClass()
        {
            if (this.Request.UrlReferrer == null || TempData["URN"] == null)
            {
                return new RedirectResult("/Errors/InvalidRequest");
            }

            var urn = (int)TempData["URN"];

            var schoolsContextualData = _comparisonService.GenerateBenchmarkListWithBestInBreedComparison(urn);

            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.RemoveAll, null);

            foreach (var schoolContextualData in schoolsContextualData)
            {
                var benchmarkSchoolToAdd = new BenchmarkSchoolModel()
                {
                    Name = schoolContextualData.EstablishmentName,
                    Type = schoolContextualData.TypeOfEstablishment,
                    EstabType = schoolContextualData.FinanceType,
                    Urn = schoolContextualData.URN.ToString()
                };
                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, benchmarkSchoolToAdd);
            }

            AddDefaultBenchmarkSchoolToList();

            return await Index(urn, null, null, ComparisonType.BestInBreed);
        }

        [HttpGet]
        public async Task<ActionResult> GenerateNewFromAdvancedCriteria()
        {
            if(this.Request.UrlReferrer == null)
            {
                return new RedirectResult("/Errors/InvalidRequest");
            }

            var urn = (int)TempData["URN"];
            var usedCriteria = TempData["BenchmarkCriteria"] as BenchmarkCriteria;
            var searchedEstabType = EstablishmentType.All;

            if (TempData["EstType"] != null)
            {
                searchedEstabType = (EstablishmentType)Enum.Parse(typeof(EstablishmentType), TempData["EstType"].ToString());
            }

            var areaType = ComparisonArea.All;
            if (TempData["AreaType"] != null)
            {
                Enum.TryParse(TempData["AreaType"].ToString(), out areaType);
            }

            int? laCode = null;
            if (TempData["LaCode"] != null)
            {
                laCode = Convert.ToInt32(TempData["LaCode"]);
            }

            return await GenerateFromAdvancedCriteria(usedCriteria, searchedEstabType, laCode, urn, areaType);
        }

        [HttpGet]
        public ActionResult GenerateFromAdvancedCriteria()
        {
            return new RedirectResult("/Errors/InvalidRequest");
        }

        [HttpPost]
        public async Task<ActionResult> GenerateFromAdvancedCriteria(BenchmarkCriteria criteria, EstablishmentType estType, int? lacode, int urn, ComparisonArea areaType, BenchmarkListOverwriteStrategy overwriteStrategy = BenchmarkListOverwriteStrategy.Overwrite)
        {
            criteria.LocalAuthorityCode = lacode;
            var benchmarkSchool = InstantiateBenchmarkSchool(urn);

            switch (overwriteStrategy)
            {
                case BenchmarkListOverwriteStrategy.Overwrite:
                    var result = await _comparisonService.GenerateBenchmarkListWithAdvancedComparisonAsync(criteria, estType);

                    _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.RemoveAll, null);
                    
                    foreach (var schoolDoc in result.BenchmarkSchools)
                    {
                        var benchmarkSchoolToAdd = new BenchmarkSchoolModel()
                        {
                            Name = schoolDoc.SchoolName,
                            Type = schoolDoc.Type,
                            EstabType = schoolDoc.FinanceType,
                            Urn = schoolDoc.URN.ToString()
                        };
                        _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, benchmarkSchoolToAdd);                        
                    }
                    break;
                case BenchmarkListOverwriteStrategy.Add:
                    var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
                    var comparisonResult = await _comparisonService.GenerateBenchmarkListWithAdvancedComparisonAsync(criteria, estType, ComparisonListLimit.LIMIT - comparisonList.BenchmarkSchools.Count);

                    foreach (var schoolDoc in comparisonResult.BenchmarkSchools)
                    {
                        var benchmarkSchoolToAdd = new BenchmarkSchoolModel()
                        {
                            Name = schoolDoc.SchoolName,
                            Type = schoolDoc.Type,
                            EstabType = schoolDoc.FinanceType,
                            Urn = schoolDoc.URN.ToString()
                        };
                        _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, benchmarkSchoolToAdd);
                    }

                    break;
            }

            AddDefaultBenchmarkSchoolToList();

            return await Index(urn, null,
                criteria, ComparisonType.Advanced, ComparisonListLimit.DEFAULT, benchmarkSchool.HistoricalFinancialDataModels.Last(), estType, areaType, lacode.ToString());
        }

        public async Task<PartialViewResult> CustomReport(string json, ChartFormat format)
        {
            var customSelection = (CustomSelectionListViewModel)JsonConvert.DeserializeObject(json, typeof(CustomSelectionListViewModel));
            var customCharts = ConvertSelectionListToChartList(customSelection.HierarchicalCharts);
            var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

            var financialDataModels = await this.GetFinancialDataForSchoolsAsync(comparisonList.BenchmarkSchools, (CentralFinancingType)Enum.Parse(typeof(CentralFinancingType), customSelection.CentralFinance));
            var trimSchoolNames = Request.Browser.IsMobileDevice;
            _fcService.PopulateBenchmarkChartsWithFinancialData(customCharts, financialDataModels, comparisonList.BenchmarkSchools, comparisonList.HomeSchoolUrn, null, trimSchoolNames);

            var academiesTerm = FormatHelpers.FinancialTermFormatAcademies(_financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.Academies));
            var maintainedTerm = FormatHelpers.FinancialTermFormatMaintained(_financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.Maintained));

            var vm = new BenchmarkChartListViewModel(customCharts, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), null,
                ComparisonType.Manual, null, null, null, EstablishmentType.All, EstablishmentType.All, null, null, academiesTerm, maintainedTerm, ComparisonArea.All, null, 0, ComparisonListLimit.DEFAULT);

            ViewBag.ChartFormat = format;

            return PartialView("Partials/CustomCharts", vm);
        }
       
        public async Task<ActionResult> Index( 
            int? urn, 
            SimpleCriteria simpleCriteria,
            BenchmarkCriteria benchmarkCriteria,
            ComparisonType comparisonType = ComparisonType.Manual,
            int basketSize = ComparisonListLimit.DEFAULT,
            FinancialDataModel benchmarkSchoolData = null,
            EstablishmentType searchedEstabType = EstablishmentType.All,
            ComparisonArea areaType = ComparisonArea.All,
            string laCode = null,
            RevenueGroupType tab = RevenueGroupType.Expenditure,
            CentralFinancingType financing = CentralFinancingType.Include)
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

            var defaultUnitType = tab == RevenueGroupType.Workforce ? UnitType.AbsoluteCount : UnitType.AbsoluteMoney;
            var benchmarkCharts = await BuildSchoolBenchmarkChartsAsync(tab, chartGroup, defaultUnitType, financing);
            var establishmentType = DetectEstablishmentType(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());

            var chartGroups = _benchmarkChartBuilder.Build(tab, establishmentType).DistinctBy(c => c.ChartGroup).ToList();
            
            string  selectedArea = "";
            switch (areaType)
            {
                case ComparisonArea.All:
                    selectedArea = "All England";
                    break;
                case ComparisonArea.LaCode:
                case ComparisonArea.LaName:
                    selectedArea = _laService.GetLaName(laCode);
                    break;
            }

            string schoolArea = "";
            if (benchmarkSchoolData != null)
            {
                schoolArea = _laService.GetLaName(benchmarkSchoolData.LaNumber.ToString());
            }

            var academiesTerm = FormatHelpers.FinancialTermFormatAcademies(_financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.Academies));
            var maintainedTerm = FormatHelpers.FinancialTermFormatMaintained(_financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.Maintained));

            var vm = new BenchmarkChartListViewModel(benchmarkCharts, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), chartGroups, comparisonType, benchmarkCriteria, simpleCriteria, benchmarkSchoolData, establishmentType, searchedEstabType, schoolArea, selectedArea, academiesTerm, maintainedTerm, areaType, laCode, urn.GetValueOrDefault(), basketSize);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = defaultUnitType;
            ViewBag.HomeSchoolId = vm.SchoolComparisonList.HomeSchoolUrn;
            ViewBag.EstablishmentType = vm.EstablishmentType;
            ViewBag.Financing = financing;
            ViewBag.ChartFormat = ChartFormat.Charts;

            return View("Index", vm);
        }

        public ActionResult Mats(RevenueGroupType tab = RevenueGroupType.Expenditure, MatFinancingType financing = MatFinancingType.TrustAndAcademies)
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

            var defaultUnitType = tab == RevenueGroupType.Workforce ? UnitType.AbsoluteCount : UnitType.AbsoluteMoney;
            var benchmarkCharts = BuildTrustBenchmarkCharts(tab, chartGroup, UnitType.AbsoluteMoney, financing);
            var chartGroups = _benchmarkChartBuilder.Build(tab, EstablishmentType.MAT).DistinctBy(c => c.ChartGroup).ToList();

            var academiesTerm = FormatHelpers.FinancialTermFormatAcademies(_financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.Academies));
            var maintainedTerm = FormatHelpers.FinancialTermFormatMaintained(_financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.Maintained));

            var vm = new BenchmarkChartListViewModel(benchmarkCharts, null, chartGroups, ComparisonType.Manual, null, null, null, EstablishmentType.MAT, EstablishmentType.MAT, null, null, academiesTerm, maintainedTerm, ComparisonArea.All, null, 0, ComparisonListLimit.DEFAULT, _benchmarkBasketCookieManager.ExtractTrustComparisonListFromCookie());

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = defaultUnitType;
            ViewBag.HomeSchoolId = vm.TrustComparisonList.DefaultTrustMatNo;
            ViewBag.EstablishmentType = vm.EstablishmentType;
            ViewBag.TrustFinancing = financing;

            return View("Index",vm);
        }

        public async Task<PartialViewResult> TabChange(EstablishmentType type, UnitType showValue, RevenueGroupType tab = RevenueGroupType.Expenditure, CentralFinancingType financing = CentralFinancingType.Include, MatFinancingType trustFinancing = MatFinancingType.TrustAndAcademies, ChartFormat format = ChartFormat.Charts)
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

            UnitType unitType;
            switch (tab)
            {
                case RevenueGroupType.Workforce:
                    unitType = UnitType.AbsoluteCount;
                    break;
                case RevenueGroupType.Balance:
                    unitType = showValue == UnitType.AbsoluteMoney || showValue == UnitType.PerPupil || showValue == UnitType.PerTeacher ? showValue : UnitType.AbsoluteMoney;
                    break;
                 default:
                     unitType = showValue;
                     break;
            }

            List<ChartViewModel> benchmarkCharts;
            if (type == EstablishmentType.MAT)
            {
                benchmarkCharts = BuildTrustBenchmarkCharts(tab, chartGroup, unitType, trustFinancing);
            }
            else
            {
                benchmarkCharts = await BuildSchoolBenchmarkChartsAsync(tab, chartGroup, unitType, financing);
            }
            var chartGroups = _benchmarkChartBuilder.Build(tab, EstablishmentType.All).DistinctBy(c => c.ChartGroup).ToList();
            
            var academiesTerm = FormatHelpers.FinancialTermFormatAcademies(_financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.Academies));
            var maintainedTerm = FormatHelpers.FinancialTermFormatMaintained(_financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.Maintained));

            var vm = new BenchmarkChartListViewModel(benchmarkCharts, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), chartGroups, ComparisonType.Manual, null, null, null, type, type, null, null, academiesTerm, maintainedTerm,ComparisonArea.All, null, 0, ComparisonListLimit.DEFAULT, _benchmarkBasketCookieManager.ExtractTrustComparisonListFromCookie());

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unitType;
            ViewBag.EstablishmentType = type;
            ViewBag.Financing = financing;
            ViewBag.TrustFinancing = trustFinancing;
            ViewBag.HomeSchoolId = (type == EstablishmentType.MAT) ? vm.TrustComparisonList.DefaultTrustMatNo : vm.SchoolComparisonList.HomeSchoolUrn;
            ViewBag.ChartFormat = format;

            return PartialView("Partials/TabContent", vm);
        }

        public async Task<PartialViewResult> GetCharts(RevenueGroupType revGroup, ChartGroupType chartGroup, UnitType showValue, CentralFinancingType centralFinancing = CentralFinancingType.Include, MatFinancingType trustCentralFinancing = MatFinancingType.TrustAndAcademies,EstablishmentType type = EstablishmentType.All, ChartFormat format = ChartFormat.Charts)
        {
            List<ChartViewModel> benchmarkCharts;
            if (type == EstablishmentType.MAT)
            {
                benchmarkCharts = BuildTrustBenchmarkCharts(revGroup, chartGroup, showValue, trustCentralFinancing);
                ViewBag.HomeSchoolId = _benchmarkBasketCookieManager.ExtractTrustComparisonListFromCookie().DefaultTrustMatNo;
            }
            else
            {
                benchmarkCharts = await BuildSchoolBenchmarkChartsAsync(revGroup, chartGroup, showValue, centralFinancing);
                ViewBag.HomeSchoolId = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie().HomeSchoolUrn;
            }

            ViewBag.EstablishmentType = type;
            ViewBag.ChartFormat = format;
            ViewBag.UnitType = showValue;

            return PartialView("Partials/Chart", benchmarkCharts);
        }

        public async Task<ActionResult> Download(EstablishmentType type)
        {
            List<ChartViewModel> benchmarkCharts;
            string csv = null;
            if (type == EstablishmentType.MAT)
            {
                benchmarkCharts = BuildTrustBenchmarkCharts(RevenueGroupType.AllExcludingSchoolPerf, ChartGroupType.All, UnitType.AbsoluteMoney, MatFinancingType.TrustAndAcademies);
                csv = _csvBuilder.BuildCSVContentForTrusts(_benchmarkBasketCookieManager.ExtractTrustComparisonListFromCookie(), benchmarkCharts);
            }
            else
            {
                benchmarkCharts = await BuildSchoolBenchmarkChartsAsync(RevenueGroupType.AllIncludingSchoolPerf, ChartGroupType.All, null, CentralFinancingType.Exclude);
                csv = _csvBuilder.BuildCSVContentForSchools(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), benchmarkCharts);
            }

            return File(Encoding.UTF8.GetBytes(csv),
                         "text/plain",
                         "BenchmarkData.csv");
        }

        private List<ChartViewModel> ConvertSelectionListToChartList(List<HierarchicalChartViewModel> customChartSelection)
        {
            List<ChartViewModel> customChartList = new List<ChartViewModel>();
            var allAvailableCharts = _benchmarkChartBuilder.Build(RevenueGroupType.AllExcludingSchoolPerf, ChartGroupType.All, EstablishmentType.All);
            foreach (var selection in customChartSelection.SelectMany(ccs => ccs.Charts))
            {
                if (selection.AbsoluteMoneySelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.Name == selection.Name).Clone();
                    customChart.ShowValue = UnitType.AbsoluteMoney;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.PerPupilSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.Name == selection.Name).Clone();
                    customChart.ShowValue = UnitType.PerPupil;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.PerTeacherSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.Name == selection.Name).Clone();
                    customChart.ShowValue = UnitType.PerTeacher;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.PercentageSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.Name == selection.Name).Clone();
                    customChart.ShowValue = UnitType.PercentageOfTotal;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.AbsoluteCountSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.Name == selection.Name).Clone();
                    customChart.ShowValue = UnitType.AbsoluteCount;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.HeadCountPerFTESelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.Name == selection.Name).Clone();
                    customChart.ShowValue = UnitType.HeadcountPerFTE;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.PercentageOfWorkforceSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.Name == selection.Name).Clone();
                    customChart.ShowValue = UnitType.FTERatioToTotalFTE;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.NumberOfPupilsPerMeasureSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.Name == selection.Name).Clone();
                    customChart.ShowValue = UnitType.NoOfPupilsPerMeasure;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
            }

            return customChartList;
        }

        private async Task<List<ChartViewModel>> BuildSchoolBenchmarkChartsAsync(RevenueGroupType revGroup, ChartGroupType chartGroup, UnitType? showValue, CentralFinancingType cFinancing)
        {
            var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            var establishmentType = DetectEstablishmentType(comparisonList);
            var benchmarkCharts = _benchmarkChartBuilder.Build(revGroup, chartGroup, establishmentType);
            RemoveIrrelevantCharts(showValue.GetValueOrDefault(), benchmarkCharts);

            var financialDataModels = await this.GetFinancialDataForSchoolsAsync(comparisonList.BenchmarkSchools, cFinancing);

            var trimSchoolNames = false;
            if (Request.Browser != null)
            {
                trimSchoolNames = Request.Browser.IsMobileDevice;
            }
            _fcService.PopulateBenchmarkChartsWithFinancialData(benchmarkCharts, financialDataModels, comparisonList.BenchmarkSchools, comparisonList.HomeSchoolUrn, showValue, trimSchoolNames);
            return benchmarkCharts;
        }

        private EstablishmentType DetectEstablishmentType(SchoolComparisonListModel comparisonList)
        {
            var schoolTypes = comparisonList.BenchmarkSchools
                .Select(bs => (EstablishmentType) Enum.Parse(typeof(EstablishmentType), bs.EstabType)).Distinct()
                .ToList();
            EstablishmentType establishmentType;
            switch (schoolTypes.Count)
            {
                case 0:
                    establishmentType = EstablishmentType.Academies;
                    break;
                case 1:
                    establishmentType = schoolTypes.First() == EstablishmentType.Academies
                        ? EstablishmentType.Academies
                        : EstablishmentType.Maintained;
                    break;
                default:
                    establishmentType = EstablishmentType.All;
                    break;
            }
            return establishmentType;
        }

        private List<ChartViewModel> BuildTrustBenchmarkCharts(RevenueGroupType revGroup, ChartGroupType chartGroup, UnitType showValue, MatFinancingType mFinancing)
        {
            var comparisonList = _benchmarkBasketCookieManager.ExtractTrustComparisonListFromCookie();
            var benchmarkCharts = _benchmarkChartBuilder.Build(revGroup, chartGroup, EstablishmentType.MAT);
            var financialDataModels = this.GetFinancialDataForTrusts(comparisonList.Trusts, mFinancing);
            var trimSchoolNames = Request.Browser.IsMobileDevice;
            _fcService.PopulateBenchmarkChartsWithFinancialData(benchmarkCharts, financialDataModels, comparisonList.Trusts, comparisonList.DefaultTrustMatNo, showValue, trimSchoolNames);
            return benchmarkCharts;
        }

        private void RemoveIrrelevantCharts(UnitType unit, List<ChartViewModel> chartList)
        {
            switch (unit)
            {
                case UnitType.PercentageOfTotal:
                    chartList.RemoveAll(c => c.Name == "Total expenditure");
                    chartList.RemoveAll(c => c.Name == "Total income");
                    break;
                case UnitType.HeadcountPerFTE:
                    chartList.RemoveAll(c => c.Name == "School workforce (headcount)");
                    chartList.RemoveAll(c => c.Name == "Teachers with Qualified Teacher Status (%)");
                    break;
                case UnitType.FTERatioToTotalFTE:
                    chartList.RemoveAll(c => c.Name == "Teachers with Qualified Teacher Status (%)");
                    chartList.RemoveAll(c => c.Name == "School workforce (Full Time Equivalent)");
                    chartList.RemoveAll(c => c.Name == "School workforce (headcount)");
                    break;
                case UnitType.NoOfPupilsPerMeasure:
                    chartList.RemoveAll(c => c.Name == "Teachers with Qualified Teacher Status (%)");
                    break;
            }
        }
        
        private List<FinancialDataModel> GetFinancialDataForTrusts(List<BenchmarkTrustModel> trusts, MatFinancingType matFinancing = MatFinancingType.TrustAndAcademies)
        {
            var models = new List<FinancialDataModel>();
            
            var terms = _financialDataService.GetActiveTermsForMatCentral();

            foreach (var trust in trusts){
                var financialDataModel = _financialDataService.GetTrustFinancialDataObject(trust.MatNo, terms.First(), matFinancing);
                models.Add(new FinancialDataModel(trust.MatNo, terms.First(), financialDataModel, EstablishmentType.MAT));
            }

            return models;
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataForSchoolsAsync(List<BenchmarkSchoolModel> schools, CentralFinancingType centralFinancing = CentralFinancingType.Include)
        {
            var models = new List<FinancialDataModel>();

            var taskList = new List<Task<IEnumerable<SchoolTrustFinancialDataObject>>>();
            foreach (var school in schools)
            {
                var estabType = (EstablishmentType) Enum.Parse(typeof(EstablishmentType), school.EstabType);
                var latestYear = _financialDataService.GetLatestDataYearPerEstabType(estabType);
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);

                var task = _financialDataService.GetSchoolFinancialDataObjectAsync(Int32.Parse(school.Urn), term, estabType, centralFinancing);
                taskList.Add(task);
            }

            for (var i=0; i < schools.Count; i++)
            {
                var estabType = (EstablishmentType)Enum.Parse(typeof(EstablishmentType), schools[i].EstabType);
                var latestYear = _financialDataService.GetLatestDataYearPerEstabType(estabType);
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
                var taskResult = await taskList[i];
                var resultDocument = taskResult?.FirstOrDefault();
                var dataGroup = schools[i].EstabType;

                if (estabType == EstablishmentType.Academies)
                {
                    dataGroup = (centralFinancing == CentralFinancingType.Include) ? DataGroups.MATAllocs : DataGroups.Academies;
                }

                if (dataGroup == DataGroups.MATAllocs && resultDocument == null)//if nothing found in -Allocs collection try to source it from (non-allocated) Academies data
                {
                    resultDocument = (await _financialDataService.GetSchoolFinancialDataObjectAsync(Int32.Parse(schools[i].Urn), term, estabType, CentralFinancingType.Exclude))
                        ?.FirstOrDefault();
                }
                
                if (resultDocument != null && resultDocument.DidNotSubmit)//School did not submit finance, return & display "no data" in the charts
                {
                    resultDocument = null;
                }

                models.Add(new FinancialDataModel(schools[i].Urn, term, resultDocument, (EstablishmentType)Enum.Parse(typeof(EstablishmentType), schools[i].EstabType)));
            }

            return models;
        }

        private SchoolViewModel InstantiateBenchmarkSchool(int urn)
        {
            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());            
            var schoolsLatestFinancialDataModel = _financialDataService.GetSchoolsLatestFinancialDataModel(benchmarkSchool.Id, benchmarkSchool.EstablishmentType);
            benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel> { schoolsLatestFinancialDataModel };
            return benchmarkSchool;
        }

        private void AddDefaultBenchmarkSchoolToList()
        {
            var cookieObject = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            var defaultBenchmarkSchool = new BenchmarkSchoolModel()
            {
                Name = cookieObject.HomeSchoolName,
                Type = cookieObject.HomeSchoolType,
                EstabType = cookieObject.HomeSchoolFinancialType,
                Urn = cookieObject.HomeSchoolUrn
            };
            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, defaultBenchmarkSchool);            
        }

    }
}