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
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFB.Web.Common;
using SFB.Web.Domain.Services.DataAccess;

namespace SFB.Web.UI.Controllers
{
    public class BenchmarkChartsController : BaseController
    {
        private readonly IBenchmarkChartBuilder _benchmarkChartBuilder;
        private readonly IFinancialDataService _financialDataService;
        private readonly IFinancialCalculationsService _fcService;
        private readonly ILocalAuthoritiesService _laService;
        private readonly IDownloadCSVBuilder _csvBuilder;
        private readonly IEdubaseDataService _edubaseDataService;
        private readonly IStatisticalCriteriaBuilderService _statisticalCriteriaBuilderService;

        public BenchmarkChartsController(IBenchmarkChartBuilder benchmarkChartBuilder, IFinancialDataService financialDataService, IFinancialCalculationsService fcService, ILocalAuthoritiesService laService, IDownloadCSVBuilder csvBuilder,
            IEdubaseDataService edubaseDataService, IStatisticalCriteriaBuilderService statisticalCriteriaBuilderService)
        {
            _benchmarkChartBuilder = benchmarkChartBuilder;
            _financialDataService = financialDataService;
            _fcService = fcService;
            _laService = laService;
            _csvBuilder = csvBuilder;
            _edubaseDataService = edubaseDataService;
            _statisticalCriteriaBuilderService = statisticalCriteriaBuilderService;
        }

        public async Task<ActionResult> GenerateFromSimpleCriteria(string urn, int basketSize, EstablishmentType estType, SimpleCriteria simpleCriteria)
        {
            var benchmarkSchool = InstantiateBenchmarkSchool(urn);

            var tryCount = 0;

            var benchmarkCriteria = _statisticalCriteriaBuilderService.Build(
                benchmarkSchool,
                simpleCriteria.IncludeFsm.GetValueOrDefault(),
                simpleCriteria.IncludeSen.GetValueOrDefault(),
                simpleCriteria.IncludeEal.GetValueOrDefault(),
                simpleCriteria.IncludeLa.GetValueOrDefault(),
                tryCount);

            var benchmarkSchools = await _financialDataService.SearchSchoolsByCriteria(benchmarkCriteria, estType);

            if (benchmarkSchools.Count > basketSize)//Original query returns more than required. Cut from top by proximity.
            {
                benchmarkSchools = benchmarkSchools.OrderBy(b => Math.Abs(b.GetPropertyValue<int>("No Pupils") - benchmarkSchool.HistoricalSchoolDataModels.Last().PupilCount)).Take(basketSize).ToList();
                benchmarkCriteria.MinNoPupil = benchmarkSchools.Min(s => s.GetPropertyValue<int>("No Pupils"));
                benchmarkCriteria.MaxNoPupil = benchmarkSchools.Max(s => s.GetPropertyValue<int>("No Pupils"));//Update the criteria to reflect the max and min pupil count of the found schools
            }

            while (benchmarkSchools.Count < basketSize)//Original query returns less than required
            {
                if (++tryCount > CriteriaSearchConfig.MAX_TRY_LIMIT)//Max query try reached. Return whatever is found.
                {
                    break;
                }
                benchmarkCriteria = _statisticalCriteriaBuilderService.Build(
                    benchmarkSchool,
                    simpleCriteria.IncludeFsm.GetValueOrDefault(),
                    simpleCriteria.IncludeSen.GetValueOrDefault(),
                    simpleCriteria.IncludeEal.GetValueOrDefault(),
                    simpleCriteria.IncludeLa.GetValueOrDefault(),
                    tryCount);
                benchmarkSchools = await _financialDataService.SearchSchoolsByCriteria(benchmarkCriteria, estType);

                if (benchmarkSchools.Count > basketSize)//Number jumping to more than ideal. Cut from top by proximity.
                {
                    benchmarkSchools = benchmarkSchools.OrderBy(b => Math.Abs(b.GetPropertyValue<int>("No Pupils") - benchmarkSchool.HistoricalSchoolDataModels.Last().PupilCount)).Take(basketSize).ToList();
                    benchmarkCriteria.MinNoPupil = benchmarkSchools.Min(s => s.GetPropertyValue<int>("No Pupils"));
                    benchmarkCriteria.MaxNoPupil = benchmarkSchools.Max(s => s.GetPropertyValue<int>("No Pupils"));//Update the criteria to reflect the max and min pupil count of the found schools
                    break;
                }
            }

            var cookie = base.UpdateSchoolComparisonListCookie(CompareActions.CLEAR_BENCHMARK_LIST, null);
            Response.Cookies.Add(cookie);

            foreach (var schoolDoc in benchmarkSchools)
            {
                var benchmarkSchoolToAdd = new BenchmarkSchoolViewModel()
                {
                    Name = schoolDoc.GetPropertyValue<string>("School Name"),
                    Type = schoolDoc.GetPropertyValue<string>("Type"),
                    FinancialType = schoolDoc.GetPropertyValue<string>("FinanceType") == "A" ? SchoolFinancialType.Academies.ToString() : SchoolFinancialType.Maintained.ToString(),
                    Urn = schoolDoc.GetPropertyValue<string>("URN")
                };
                cookie = base.UpdateSchoolComparisonListCookie(CompareActions.ADD_TO_COMPARISON_LIST, benchmarkSchoolToAdd);
                Response.Cookies.Add(cookie);
            }

            AddDefaultBenchmarkSchoolToList();

            return Index(urn, simpleCriteria, benchmarkCriteria, basketSize, benchmarkSchool.HistoricalSchoolDataModels.Last(), estType, ComparisonType.Basic);
        }

        public async Task<ActionResult> GenerateNewFromAdvancedCriteria()
        {
            var urn = TempData["URN"].ToString();
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

        public async Task<ActionResult> GenerateFromAdvancedCriteria(BenchmarkCriteria criteria, EstablishmentType estType, int? lacode, string urn, ComparisonArea areaType, BenchmarkListOverwriteStrategy overwriteStrategy = BenchmarkListOverwriteStrategy.Overwrite)
        {
            List<Document> benchmarkSchools, limitedList;

            criteria.LaCode = lacode;
            var benchmarkSchool = InstantiateBenchmarkSchool(urn);

            switch (overwriteStrategy)
            {
                case BenchmarkListOverwriteStrategy.Overwrite:
                    benchmarkSchools = await _financialDataService.SearchSchoolsByCriteria(criteria, estType);
                    limitedList = benchmarkSchools.Take(ComparisonListLimit.LIMIT).ToList();

                    var cookie = base.UpdateSchoolComparisonListCookie(CompareActions.CLEAR_BENCHMARK_LIST, null);
                    Response.Cookies.Add(cookie);

                    foreach (var schoolDoc in limitedList)
                    {
                        var benchmarkSchoolToAdd = new BenchmarkSchoolViewModel()
                        {
                            Name = schoolDoc.GetPropertyValue<string>("School Name"),
                            Type = schoolDoc.GetPropertyValue<string>("Type"),
                            FinancialType = schoolDoc.GetPropertyValue<string>("FinanceType") == "A" ? SchoolFinancialType.Academies.ToString() : SchoolFinancialType.Maintained.ToString(),
                            Urn = schoolDoc.GetPropertyValue<string>("URN")
                        };
                        cookie = base.UpdateSchoolComparisonListCookie(CompareActions.ADD_TO_COMPARISON_LIST, benchmarkSchoolToAdd);
                        Response.Cookies.Add(cookie);
                    }
                    break;
                case BenchmarkListOverwriteStrategy.Add:
                    benchmarkSchools = await _financialDataService.SearchSchoolsByCriteria(criteria, estType);
                    var comparisonList = base.ExtractSchoolComparisonListFromCookie();
                    limitedList = benchmarkSchools.Take(ComparisonListLimit.LIMIT - comparisonList.BenchmarkSchools.Count()).ToList();

                    foreach (var schoolDoc in limitedList)
                    {
                        var benchmarkSchoolToAdd = new BenchmarkSchoolViewModel()
                        {
                            Name = schoolDoc.GetPropertyValue<string>("School Name"),
                            Type = schoolDoc.GetPropertyValue<string>("Type"),
                            FinancialType = schoolDoc.GetPropertyValue<string>("FinanceType") == "A" ? SchoolFinancialType.Academies.ToString() : SchoolFinancialType.Maintained.ToString(),
                            Urn = schoolDoc.GetPropertyValue<string>("URN")
                        };
                        cookie = base.UpdateSchoolComparisonListCookie(CompareActions.ADD_TO_COMPARISON_LIST, benchmarkSchoolToAdd);
                        Response.Cookies.Add(cookie);
                    }

                    break;
            }

            AddDefaultBenchmarkSchoolToList();

            return Index(urn, null,
                criteria, ComparisonListLimit.DEFAULT, benchmarkSchool.HistoricalSchoolDataModels.Last(), estType, ComparisonType.Advanced, areaType, lacode.ToString());
        }
        
        public PartialViewResult CustomReport(string json)
        {
            var customSelection = (CustomSelectionListViewModel)JsonConvert.DeserializeObject(json, typeof(CustomSelectionListViewModel));
            var customCharts = ConvertSelectionListToChartList(customSelection.HierarchicalCharts);
            var comparisonList = base.ExtractSchoolComparisonListFromCookie();

            var financialDataModels = this.GetFinancialDataForSchools(comparisonList.BenchmarkSchools, (CentralFinancingType)Enum.Parse(typeof(CentralFinancingType), customSelection.CentralFinance));
            var trimSchoolNames = Request.Browser.IsMobileDevice;
            _fcService.PopulateBenchmarkChartsWithFinancialData(customCharts, financialDataModels, comparisonList.BenchmarkSchools, comparisonList.HomeSchoolUrn, null, trimSchoolNames);

            var academiesTerm = FormatHelpers.FinancialTermFormatAcademies(_financialDataService.GetLatestDataYearPerSchoolType(SchoolFinancialType.Academies));
            var maintainedTerm = FormatHelpers.FinancialTermFormatMaintained(_financialDataService.GetLatestDataYearPerSchoolType(SchoolFinancialType.Maintained));

            var vm = new BenchmarkChartListViewModel(customCharts, base.ExtractSchoolComparisonListFromCookie(), null,
                ComparisonType.Manual, null, null, null, EstablishmentType.All, EstablishmentType.All, null, null, academiesTerm, maintainedTerm, ComparisonArea.All, null, null, ComparisonListLimit.DEFAULT);

            return PartialView("Partials/CustomCharts", vm);
        }
       
        public ActionResult Index( 
            string urn, 
            SimpleCriteria simpleCriteria,
            BenchmarkCriteria benchmarkCriteria,
            int basketSize = ComparisonListLimit.DEFAULT,
            SchoolDataModel benchmarkSchoolData = null,
            EstablishmentType searchedEstabType = EstablishmentType.All,
            ComparisonType comparisonType = ComparisonType.Manual,
            ComparisonArea areaType = ComparisonArea.All,
            string laCode = null,
            RevenueGroupType tab = RevenueGroupType.Expenditure,
            CentralFinancingType financing = CentralFinancingType.Exclude)
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
            var benchmarkCharts = BuildSchoolBenchmarkCharts(tab, chartGroup, defaultUnitType, CentralFinancingType.Exclude);
            var establishmentType = DetectEstablishmentType(base.ExtractSchoolComparisonListFromCookie());

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

            var academiesTerm = FormatHelpers.FinancialTermFormatAcademies(_financialDataService.GetLatestDataYearPerSchoolType(SchoolFinancialType.Academies));
            var maintainedTerm = FormatHelpers.FinancialTermFormatMaintained(_financialDataService.GetLatestDataYearPerSchoolType(SchoolFinancialType.Maintained));

            var vm = new BenchmarkChartListViewModel(benchmarkCharts, base.ExtractSchoolComparisonListFromCookie(), chartGroups, comparisonType, benchmarkCriteria, simpleCriteria, benchmarkSchoolData, establishmentType, searchedEstabType, schoolArea, selectedArea, academiesTerm, maintainedTerm, areaType, laCode, urn, basketSize);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = defaultUnitType;
            ViewBag.HomeSchoolId = vm.SchoolComparisonList.HomeSchoolUrn;
            ViewBag.EstablishmentType = vm.EstablishmentType;
            ViewBag.Financing = financing;

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

            var academiesTerm = FormatHelpers.FinancialTermFormatAcademies(_financialDataService.GetLatestDataYearPerSchoolType(SchoolFinancialType.Academies));
            var maintainedTerm = FormatHelpers.FinancialTermFormatMaintained(_financialDataService.GetLatestDataYearPerSchoolType(SchoolFinancialType.Maintained));

            var vm = new BenchmarkChartListViewModel(benchmarkCharts, null, chartGroups, ComparisonType.Manual, null, null, null, EstablishmentType.MAT, EstablishmentType.MAT, null, null, academiesTerm, maintainedTerm, ComparisonArea.All, null, null, ComparisonListLimit.DEFAULT, base.ExtractTrustComparisonListFromCookie());

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = defaultUnitType;
            ViewBag.HomeSchoolId = vm.TrustComparisonList.DefaultTrustMatNo;
            ViewBag.EstablishmentType = vm.EstablishmentType;
            ViewBag.TrustFinancing = financing;

            return View("Index",vm);
        }

        public PartialViewResult TabChange(EstablishmentType type, UnitType showValue, RevenueGroupType tab = RevenueGroupType.Expenditure, CentralFinancingType financing = CentralFinancingType.Exclude, MatFinancingType trustFinancing = MatFinancingType.TrustAndAcademies)
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
                benchmarkCharts = BuildSchoolBenchmarkCharts(tab, chartGroup, unitType, financing);
            }
            var chartGroups = _benchmarkChartBuilder.Build(tab, EstablishmentType.All).DistinctBy(c => c.ChartGroup).ToList();
            
            var academiesTerm = FormatHelpers.FinancialTermFormatAcademies(_financialDataService.GetLatestDataYearPerSchoolType(SchoolFinancialType.Academies));
            var maintainedTerm = FormatHelpers.FinancialTermFormatMaintained(_financialDataService.GetLatestDataYearPerSchoolType(SchoolFinancialType.Maintained));

            var vm = new BenchmarkChartListViewModel(benchmarkCharts, base.ExtractSchoolComparisonListFromCookie(), chartGroups, ComparisonType.Manual, null, null, null, type, type, null, null, academiesTerm, maintainedTerm,ComparisonArea.All, null, null, ComparisonListLimit.DEFAULT, base.ExtractTrustComparisonListFromCookie());

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unitType;
            ViewBag.EstablishmentType = type;
            ViewBag.Financing = financing;
            ViewBag.TrustFinancing = trustFinancing;
            ViewBag.HomeSchoolId = (type == EstablishmentType.MAT) ? vm.TrustComparisonList.DefaultTrustMatNo : vm.SchoolComparisonList.HomeSchoolUrn;

            return PartialView("Partials/TabContent", vm);
        }

        public PartialViewResult GetCharts(RevenueGroupType revGroup, ChartGroupType chartGroup, UnitType showValue, CentralFinancingType centralFinancing = CentralFinancingType.Exclude, MatFinancingType trustCentralFinancing = MatFinancingType.TrustAndAcademies,EstablishmentType type = EstablishmentType.All)
        {
            List<ChartViewModel> benchmarkCharts;
            if (type == EstablishmentType.MAT)
            {
                benchmarkCharts = BuildTrustBenchmarkCharts(revGroup, chartGroup, showValue, trustCentralFinancing);
                ViewBag.HomeSchoolId = this.ExtractTrustComparisonListFromCookie().DefaultTrustMatNo;
            }
            else
            {
                benchmarkCharts = BuildSchoolBenchmarkCharts(revGroup, chartGroup, showValue, centralFinancing);
                ViewBag.HomeSchoolId = this.ExtractSchoolComparisonListFromCookie().HomeSchoolUrn;
            }

            ViewBag.EstablishmentType = type;
            return PartialView("Partials/Chart", benchmarkCharts);
        }

        public ActionResult Download(EstablishmentType type)
        {
            List<ChartViewModel> benchmarkCharts;
            string csv = null;
            if (type == EstablishmentType.MAT)
            {
                benchmarkCharts = BuildTrustBenchmarkCharts(RevenueGroupType.AllExcludingSchoolPerf, ChartGroupType.All, UnitType.AbsoluteMoney, MatFinancingType.TrustAndAcademies);
                csv = _csvBuilder.BuildCSVContentForTrusts(base.ExtractTrustComparisonListFromCookie(), benchmarkCharts);
            }
            else
            {
                benchmarkCharts = BuildSchoolBenchmarkCharts(RevenueGroupType.AllIncludingSchoolPerf, ChartGroupType.All, null, CentralFinancingType.Exclude);
                csv = _csvBuilder.BuildCSVContentForSchools(base.ExtractSchoolComparisonListFromCookie(), benchmarkCharts);
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
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.FieldName == selection.FieldName).Clone();
                    customChart.ShowValue = UnitType.AbsoluteMoney;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.PerPupilSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.FieldName == selection.FieldName).Clone();
                    customChart.ShowValue = UnitType.PerPupil;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.PerTeacherSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.FieldName == selection.FieldName).Clone();
                    customChart.ShowValue = UnitType.PerTeacher;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.PercentageSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.FieldName == selection.FieldName).Clone();
                    customChart.ShowValue = UnitType.PercentageOfTotal;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.AbsoluteCountSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.FieldName == selection.FieldName).Clone();
                    customChart.ShowValue = UnitType.AbsoluteCount;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.HeadCountPerFTESelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.FieldName == selection.FieldName).Clone();
                    customChart.ShowValue = UnitType.HeadcountPerFTE;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.PercentageOfWorkforceSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.FieldName == selection.FieldName).Clone();
                    customChart.ShowValue = UnitType.FTERatioToTotalFTE;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
                if (selection.NumberOfPupilsPerMeasureSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.First(c => c.FieldName == selection.FieldName).Clone();
                    customChart.ShowValue = UnitType.NoOfPupilsPerMeasure;
                    customChart.ChartType = ChartType.CustomReport;
                    customChartList.Add(customChart);
                }
            }

            return customChartList;
        }

        private List<ChartViewModel> BuildSchoolBenchmarkCharts(RevenueGroupType revGroup, ChartGroupType chartGroup, UnitType? showValue, CentralFinancingType cFinancing)
        {
            var comparisonList = base.ExtractSchoolComparisonListFromCookie();
            var establishmentType = DetectEstablishmentType(comparisonList);
            var benchmarkCharts = _benchmarkChartBuilder.Build(revGroup, chartGroup, establishmentType);
            RemoveIrrelevantCharts(showValue.GetValueOrDefault(), benchmarkCharts);
            var financialDataModels = this.GetFinancialDataForSchools(comparisonList.BenchmarkSchools, cFinancing);
            var trimSchoolNames = false;
            if (Request.Browser != null)
            {
                trimSchoolNames = Request.Browser.IsMobileDevice;
            }
            _fcService.PopulateBenchmarkChartsWithFinancialData(benchmarkCharts, financialDataModels, comparisonList.BenchmarkSchools, comparisonList.HomeSchoolUrn, showValue, trimSchoolNames);
            return benchmarkCharts;
        }

        private EstablishmentType DetectEstablishmentType(ComparisonListModel comparisonList)
        {
            var schoolTypes = comparisonList.BenchmarkSchools
                .Select(bs => (SchoolFinancialType) Enum.Parse(typeof(SchoolFinancialType), bs.FinancialType)).Distinct()
                .ToList();
            EstablishmentType establishmentType;
            switch (schoolTypes.Count)
            {
                case 0:
                    establishmentType = EstablishmentType.Academy;
                    break;
                case 1:
                    establishmentType = schoolTypes.First() == SchoolFinancialType.Academies
                        ? EstablishmentType.Academy
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
            var cookie = Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
            var comparisonList = JsonConvert.DeserializeObject<TrustComparisonViewModel>(cookie.Value);
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
                    chartList.RemoveAll(c => c.Name == "Teachers with QTS (%)");
                    break;
                case UnitType.FTERatioToTotalFTE:
                    chartList.RemoveAll(c => c.Name == "Teachers with QTS (%)");
                    chartList.RemoveAll(c => c.Name == "School workforce (FTE)");
                    chartList.RemoveAll(c => c.Name == "School workforce (headcount)");
                    break;
                case UnitType.NoOfPupilsPerMeasure:
                    chartList.RemoveAll(c => c.Name == "Teachers with QTS (%)");
                    break;
            }
        }
        
        private List<SchoolDataModel> GetFinancialDataForTrusts(List<TrustToCompareViewModel> trusts, MatFinancingType matFinancing = MatFinancingType.TrustAndAcademies)
        {
            var models = new List<SchoolDataModel>();
            
            var terms = _financialDataService.GetActiveTermsForMatCentral();

            foreach (var trust in trusts){
                var financialDataModel = _financialDataService.GetMATDataDocument(trust.MatNo, terms.First(), matFinancing);
                models.Add(new SchoolDataModel(trust.MatNo, terms.First(), financialDataModel, SchoolFinancialType.Academies));
            }

            return models;
        }

        private List<SchoolDataModel> GetFinancialDataForSchools(List<BenchmarkSchoolViewModel> schools, CentralFinancingType centralFinancing = CentralFinancingType.Exclude)
        {
            var models = new List<SchoolDataModel>();

            foreach (var school in schools)
            {
                var latestYear = _financialDataService.GetLatestDataYearPerSchoolType((SchoolFinancialType)Enum.Parse(typeof(SchoolFinancialType), school.FinancialType));
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);

                var financialDataModel = _financialDataService.GetSchoolDataDocument(school.Urn, term, (SchoolFinancialType)Enum.Parse(typeof(SchoolFinancialType), school.FinancialType), centralFinancing);
                models.Add(new SchoolDataModel(school.Urn, term, financialDataModel, (SchoolFinancialType)Enum.Parse(typeof(SchoolFinancialType), school.FinancialType)));
            }

            return models;
        }

        private SchoolViewModel InstantiateBenchmarkSchool(string urn)
        {
            var benchmarkSchool = new SchoolViewModel(_edubaseDataService.GetSchoolByUrn(urn), base.ExtractSchoolComparisonListFromCookie());
            var latestYear = _financialDataService.GetLatestDataYearPerSchoolType(benchmarkSchool.FinancialType);
            var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
            var document = _financialDataService.GetSchoolDataDocument(urn, term, benchmarkSchool.FinancialType);
            benchmarkSchool.HistoricalSchoolDataModels = new List<SchoolDataModel> { new SchoolDataModel(urn, term, document, benchmarkSchool.FinancialType) };
            return benchmarkSchool;
        }

        private void AddDefaultBenchmarkSchoolToList()
        {
            var cookieObject = ExtractSchoolComparisonListFromCookie();
            var defaultBenchmarkSchool = new BenchmarkSchoolViewModel()
            {
                Name = cookieObject.HomeSchoolName,
                Type = cookieObject.HomeSchoolType,
                FinancialType = cookieObject.HomeSchoolFinancialType,
                Urn = cookieObject.HomeSchoolUrn
            };
            var cookie = base.UpdateSchoolComparisonListCookie(CompareActions.ADD_TO_COMPARISON_LIST, defaultBenchmarkSchool);
            Response.Cookies.Add(cookie);
        }

    }
}