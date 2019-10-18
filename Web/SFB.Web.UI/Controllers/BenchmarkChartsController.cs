using System;
using SFB.Web.UI.Models;
using System.Linq;
using System.Web.Mvc;
using SFB.Web.UI.Helpers;
using System.Collections.Generic;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Services;
using System.Text;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Enums;
using System.Threading.Tasks;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Services.Comparison;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Attributes;
using System.Net;
using SFB.Web.ApplicationCore.Helpers.Enums;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class BenchmarkChartsController : Controller
    {
        private readonly IContextDataService _contextDataService;
        private readonly IFinancialDataService _financialDataService;
        private readonly IFinancialCalculationsService _fcService;
        private readonly ILocalAuthoritiesService _laService;
        private readonly IBenchmarkChartBuilder _benchmarkChartBuilder;
        private readonly IDownloadCSVBuilder _csvBuilder;
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


        public async Task<ActionResult> GenerateFromSavedBasket(string urns, string companyNumbers, BenchmarkListOverwriteStrategy? overwriteStrategy, ComparisonType comparison = ComparisonType.Manual)
        {
            List<int> urnList = null;
            List<int> companyNoList = null;
            try
            {
                urnList = urns?.Split('-').Select(urn => int.Parse(urn)).ToList();
                companyNoList = companyNumbers?.Split('-').Select(cn => int.Parse(cn)).ToList();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (overwriteStrategy == null)
            {
                var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

                if (comparisonList?.BenchmarkSchools?.Count > 0 && comparisonList.BenchmarkSchools.Count + urnList.Count <= ComparisonListLimit.LIMIT)
                {
                    return Redirect($"SaveOverwriteStrategy?savedUrns={urns}");
                }
                if (comparisonList?.BenchmarkSchools?.Count > 0 && comparisonList.BenchmarkSchools.Count + urnList.Count > ComparisonListLimit.LIMIT)
                {
                    return Redirect($"ReplaceWithSavedBasket?savedUrns={urns}");
                }
            }

            if (overwriteStrategy == BenchmarkListOverwriteStrategy.Add)
            {
                var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

                if (comparisonList.BenchmarkSchools.Count + urnList.Count > ComparisonListLimit.LIMIT)
                {
                    var vm = new SaveOverwriteViewModel()
                    {
                        ComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(),
                        SavedUrns = urns,
                        ErrorMessage = ErrorMessages.BMBasketLimitExceed
                    };

                    return View("SaveOverwriteStrategy", vm);
                }
                else
                {
                    try
                    {
                        AddSchoolDataObjectsToBasket(comparison, urnList);
                    }
                    catch (ApplicationException)
                    {
                        //ignore double addition attempts
                    }

                    return await Index(null, null, null, null, comparison);
                }
            }
            
            if (urnList != null)
            {
                EmptyBenchmarkList();

                AddSchoolDataObjectsToBasket(comparison, urnList);

                return await Index(null, null, null, null, comparison);
            }

            else if (companyNoList != null)
            {
                var trustDataObjects = _financialDataService.GetMultipleTrustDataObjectsByCompanyNumbers(companyNoList);

                _benchmarkBasketCookieManager.UpdateTrustComparisonListCookie(CookieActions.RemoveAll, null);

                foreach (var trustData in trustDataObjects)
                {
                    _benchmarkBasketCookieManager.UpdateTrustComparisonListCookie(CookieActions.Add, trustData.CompanyNumber, trustData.TrustOrCompanyName);
                }

                return Mats();
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        public ActionResult SaveOverwriteStrategy(string savedUrns)
        {
            var vm = new SaveOverwriteViewModel()
            {
                ComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(),
                SavedUrns = savedUrns
            };

            return View("SaveOverwriteStrategy", vm);
        }

        [HttpGet]
        public ActionResult ReplaceWithSavedBasket(string savedUrns)
        {
            var vm = new SaveOverwriteViewModel()
            {
                ComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(),
                SavedUrns = savedUrns
            };

            return View("ReplaceWithSavedBasket", vm);
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

            EmptyBenchmarkList();

            AddSchoolsToBenchmarkList(comparisonResult);

            AddDefaultBenchmarkSchoolToList(benchmarkSchool);

            return await Index(urn, simpleCriteria, comparisonResult.BenchmarkCriteria, null, ComparisonType.Basic, basketSize, benchmarkSchool.LatestYearFinancialData, estType);
        }

        [HttpGet]
        public async Task<ActionResult> GenerateFromBicCriteria(int urn)
        {
            var benchmarkSchool = InstantiateBenchmarkSchool(urn);
            var bmFinancialData = benchmarkSchool.LatestYearFinancialData;

            var bicCriteria = new BestInClassCriteria()
            {
                EstablishmentType = EstablishmentType.All,
                OverallPhase = benchmarkSchool.OverallPhase,
                UrbanRural = bmFinancialData.UrbanRural,
                NoPupilsMin = WithinPositiveLimits((bmFinancialData.PupilCount.GetValueOrDefault() - CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_PUPIL_COUNT_TOPUP) * (1 - CriteriaSearchConfig.BIC_DEFAULT_FLEX_PUPIL_COUNT)),
                NoPupilsMax = (bmFinancialData.PupilCount.GetValueOrDefault() + CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_PUPIL_COUNT_TOPUP) * (1 + CriteriaSearchConfig.BIC_DEFAULT_FLEX_PUPIL_COUNT),
                PerPupilExpMin = 0,
                PerPupilExpMax = bmFinancialData.PerPupilTotalExpenditure.GetValueOrDefault() + CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_EXP_PP_TOPUP,
                PercentageFSMMin = WithinPercentLimits(bmFinancialData.PercentageOfEligibleFreeSchoolMeals.GetValueOrDefault() - CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_FSM_TOPUP) * (1 - CriteriaSearchConfig.BIC_DEFAULT_FLEX_FSM),
                PercentageFSMMax = WithinPercentLimits(bmFinancialData.PercentageOfEligibleFreeSchoolMeals.GetValueOrDefault() + CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_FSM_TOPUP) * (1 + CriteriaSearchConfig.BIC_DEFAULT_FLEX_FSM),
                PercentageSENMin = bmFinancialData.PercentageOfPupilsWithSen.GetValueOrDefault() * (1 - CriteriaSearchConfig.BIC_DEFAULT_FLEX_SEN),
                PercentageSENMax = WithinPercentLimits(bmFinancialData.PercentageOfPupilsWithSen.GetValueOrDefault() * (1 + CriteriaSearchConfig.BIC_DEFAULT_FLEX_SEN)),
                Ks2ProgressScoreMin = bmFinancialData.SchoolOverallPhase == "Secondary" ? (decimal?)null : 0,
                Ks2ProgressScoreMax = bmFinancialData.SchoolOverallPhase == "Secondary" ? (decimal?)null : 20,
                Ks4ProgressScoreMin = bmFinancialData.SchoolOverallPhase == "Secondary" ? 0 : (decimal?)null,
                Ks4ProgressScoreMax = bmFinancialData.SchoolPhase == "Secondary" ? +5 : (decimal?)null,
                RRPerIncomeMin = CriteriaSearchConfig.RR_PER_INCOME_TRESHOLD,
                LondonWeighting = bmFinancialData.LondonWeighting == "Neither" ? new[] { "Neither" } : new[] { "Inner", "Outer" }
            };

            return await GenerateFromBicCriteria(urn, bicCriteria);
        }

        [HttpPost]
        public async Task<ActionResult> GenerateFromBicCriteria(int urn, BestInClassCriteria bicCriteria)
        {
            var benchmarkSchool = InstantiateBenchmarkSchool(urn);

            var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromBicComparisonCriteria(benchmarkSchool.LatestYearFinancialData, bicCriteria);

            var comparisonResult = await _comparisonService.GenerateBenchmarkListWithBestInClassComparisonAsync(
                bicCriteria.EstablishmentType,
                benchmarkCriteria,
                bicCriteria,
                benchmarkSchool.LatestYearFinancialData);

            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.RemoveAll, null);

            foreach (var schoolData in comparisonResult.BenchmarkSchools)
            {
                var benchmarkSchoolToAdd = new BenchmarkSchoolModel()
                {
                    Name = schoolData.SchoolName,
                    Type = schoolData.Type,
                    EstabType = schoolData.FinanceType,
                    Urn = schoolData.URN.ToString(),
                    ProgressScore = schoolData.OverallPhase == "Secondary" ?
                        schoolData.Progress8Measure
                        : decimal.Round(schoolData.Ks2Progress.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero)

                };
                try { 
                    _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, benchmarkSchoolToAdd);
                }
                catch (ApplicationException) { }
            }

            AddDefaultBenchmarkSchoolToList(benchmarkSchool);

            return await Index(urn, null, comparisonResult.BenchmarkCriteria, bicCriteria, ComparisonType.BestInClass, ComparisonListLimit.DEFAULT, benchmarkSchool.LatestYearFinancialData, bicCriteria.EstablishmentType);
        }

        [HttpGet]
        public async Task<ViewResult> OneClickReport(int urn)
        {
            var benchmarkSchool = InstantiateBenchmarkSchool(urn);

            var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromOneClickComparisonCriteria(benchmarkSchool.LatestYearFinancialData);

            var comparisonResult = await _comparisonService.GenerateBenchmarkListWithOneClickComparisonAsync(benchmarkCriteria, EstablishmentType.All, ComparisonListLimit.ONE_CLICK, benchmarkSchool.LatestYearFinancialData);

            EmptyBenchmarkList();

            AddSchoolsToBenchmarkList(comparisonResult);

            SetSchoolAsDefault(benchmarkSchool);

            AddDefaultBenchmarkSchoolToList(benchmarkSchool);

            var benchmarkCharts = await BuildSchoolBenchmarkChartsAsync(RevenueGroupType.Custom, ChartGroupType.Custom, null, CentralFinancingType.Include);

            var vm = new BenchmarkChartListViewModel(
                benchmarkCharts,
                _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(),
                null,
                ComparisonType.OneClick,
                comparisonResult.BenchmarkCriteria,
                null,
                null,
                benchmarkSchool.LatestYearFinancialData,
                benchmarkSchool.EstablishmentType,
                EstablishmentType.All,
                null, null,
                FormatHelpers.FinancialTermFormatAcademies(_financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.Academies)),
                FormatHelpers.FinancialTermFormatMaintained(_financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.Maintained)),
                ComparisonArea.All, null, urn,
                ComparisonListLimit.ONE_CLICK);

            ViewBag.ChartFormat = ChartFormat.Charts;
            ViewBag.HomeSchoolId = vm.SchoolComparisonList.HomeSchoolUrn;
            ViewBag.Financing = CentralFinancingType.Include;


            return View(vm);
        }

        [HttpGet]
        public async Task<ActionResult> GenerateNewFromAdvancedCriteria()
        {
            int urn;
            BenchmarkCriteria usedCriteria;
            try
            {
                urn = (int)TempData["URN"];
                usedCriteria = TempData["BenchmarkCriteria"] as BenchmarkCriteria;
            }
            catch (Exception)
            {
                return new RedirectResult("/Errors/InvalidRequest");
            }

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

            bool excludePartial = false;
            if(TempData["ExcludePartial"] != null)
            {
                excludePartial = Convert.ToBoolean(TempData["ExcludePartial"]);
            }

            return await GenerateFromAdvancedCriteria(usedCriteria, searchedEstabType, laCode, urn, areaType, BenchmarkListOverwriteStrategy.Overwrite, excludePartial);
        }

        [HttpGet]
        public ActionResult GenerateFromAdvancedCriteria()
        {
            return new RedirectResult("/Errors/InvalidRequest");
        }

        [HttpPost]
        public async Task<ActionResult> GenerateFromAdvancedCriteria(BenchmarkCriteria criteria, EstablishmentType estType, int? lacode, int urn, ComparisonArea areaType, 
            BenchmarkListOverwriteStrategy overwriteStrategy = BenchmarkListOverwriteStrategy.Overwrite, bool excludePartial = false)
        {
            criteria.LocalAuthorityCode = lacode;
            var benchmarkSchool = InstantiateBenchmarkSchool(urn);

            switch (overwriteStrategy)
            {
                case BenchmarkListOverwriteStrategy.Overwrite:
                    var result = await _comparisonService.GenerateBenchmarkListWithAdvancedComparisonAsync(criteria, estType, excludePartial, 30);

                    EmptyBenchmarkList();

                    AddSchoolsToBenchmarkList(result);

                    break;
                case BenchmarkListOverwriteStrategy.Add:
                    var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
                    var comparisonResult = await _comparisonService.GenerateBenchmarkListWithAdvancedComparisonAsync(criteria, estType, excludePartial, ComparisonListLimit.LIMIT - comparisonList.BenchmarkSchools.Count);

                    if (comparisonList.BenchmarkSchools.Count + comparisonResult.BenchmarkSchools.Count > ComparisonListLimit.LIMIT)
                    {
                        ViewBag.URN = urn;
                        ViewBag.HomeSchoolName = comparisonList.HomeSchoolName;
                        ViewBag.ComparisonType = ComparisonType.Advanced;
                        ViewBag.EstType = estType;
                        ViewBag.AreaType = areaType;
                        ViewBag.LaCode = lacode;
                        ViewBag.ExcludePartial = excludePartial.ToString();
                        return View("~/Views/BenchmarkCriteria/OverwriteStrategy.cshtml",
                            new BenchmarkCriteriaVM(criteria)
                            {
                                ComparisonList = comparisonList,
                                ErrorMessage = ErrorMessages.BMBasketLimitExceed
                            });
                    }

                    foreach (var schoolDoc in comparisonResult.BenchmarkSchools)
                    {
                        var benchmarkSchoolToAdd = new BenchmarkSchoolModel()
                        {
                            Name = schoolDoc.SchoolName,
                            Type = schoolDoc.Type,
                            EstabType = schoolDoc.FinanceType,
                            Urn = schoolDoc.URN.ToString()
                        };
                        try
                        {
                            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, benchmarkSchoolToAdd);
                        }
                        catch (ApplicationException) { }
                    }

                    break;
            }

            AddDefaultBenchmarkSchoolToList(benchmarkSchool);

            return await Index(urn, null, criteria, null, ComparisonType.Advanced, ComparisonListLimit.DEFAULT, 
                benchmarkSchool.LatestYearFinancialData, estType, areaType, lacode.ToString(), excludePartial);
        }

        public async Task<PartialViewResult> CustomReport(string json, ChartFormat format)
        {
            var customSelection = (CustomSelectionListViewModel)JsonConvert.DeserializeObject(json, typeof(CustomSelectionListViewModel));
            var customCharts = ConvertSelectionListToChartList(customSelection.HierarchicalCharts);
            var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

            var financialDataModels = await this.GetFinancialDataForSchoolsAsync(comparisonList.BenchmarkSchools, (CentralFinancingType)Enum.Parse(typeof(CentralFinancingType), customSelection.CentralFinance));
            _fcService.PopulateBenchmarkChartsWithFinancialData(customCharts, financialDataModels, comparisonList.BenchmarkSchools, comparisonList.HomeSchoolUrn, null);

            var academiesTerm = FormatHelpers.FinancialTermFormatAcademies(_financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.Academies));
            var maintainedTerm = FormatHelpers.FinancialTermFormatMaintained(_financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.Maintained));

            var vm = new BenchmarkChartListViewModel(customCharts, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), null,
                ComparisonType.Manual, null, null, null, null, EstablishmentType.All, EstablishmentType.All, null, null, academiesTerm, maintainedTerm, ComparisonArea.All, null, 0, ComparisonListLimit.DEFAULT);

            ViewBag.ChartFormat = format;
            ViewBag.HomeSchoolId = vm.SchoolComparisonList.HomeSchoolUrn;

            return PartialView("Partials/CustomCharts", vm);
        }

        public async Task<ActionResult> Index(
            int? urn,
            SimpleCriteria simpleCriteria,
            BenchmarkCriteria advancedCriteria,
            BestInClassCriteria bicCriteria = null,
            ComparisonType comparisonType = ComparisonType.Manual,
            int basketSize = ComparisonListLimit.DEFAULT,
            FinancialDataModel benchmarkSchoolData = null,
            EstablishmentType searchedEstabType = EstablishmentType.All,
            ComparisonArea areaType = ComparisonArea.All,
            string laCode = null,
            bool excludePartial = false,
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

            var defaultUnitType = tab == RevenueGroupType.Workforce ?
                UnitType.AbsoluteCount :
                comparisonType == ComparisonType.BestInClass ? UnitType.PerPupil : UnitType.AbsoluteMoney;
            var benchmarkCharts = await BuildSchoolBenchmarkChartsAsync(tab, chartGroup, defaultUnitType, financing);
            var establishmentType = DetectEstablishmentType(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());

            var chartGroups = _benchmarkChartBuilder.Build(tab, establishmentType).DistinctBy(c => c.ChartGroup).ToList();

            string selectedArea = "";
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

            var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

            var bicComparisonSchools = PopulateBicSchoolsForBestInBreedTab(comparisonType, comparisonList);

            var vm = new BenchmarkChartListViewModel(benchmarkCharts, comparisonList, chartGroups, comparisonType, advancedCriteria, simpleCriteria, 
                bicCriteria, benchmarkSchoolData, establishmentType, searchedEstabType, schoolArea, selectedArea, academiesTerm, maintainedTerm, areaType, 
                laCode, urn.GetValueOrDefault(), basketSize, null, bicComparisonSchools, excludePartial);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = defaultUnitType;
            ViewBag.HomeSchoolId = vm.SchoolComparisonList.HomeSchoolUrn;
            ViewBag.EstablishmentType = vm.EstablishmentType;
            ViewBag.Financing = financing;
            ViewBag.ChartFormat = ChartFormat.Charts;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.BicComparisonOverallPhase = bicComparisonSchools?.FirstOrDefault()?.OverallPhaseInFinancialSubmission;

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

            var vm = new BenchmarkChartListViewModel(benchmarkCharts, null, chartGroups, ComparisonType.Manual, null, null, null, null, EstablishmentType.MAT, 
                EstablishmentType.MAT, null, null, academiesTerm, maintainedTerm, ComparisonArea.All, null, 0, ComparisonListLimit.DEFAULT, 
                _benchmarkBasketCookieManager.ExtractTrustComparisonListFromCookie());

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = defaultUnitType;
            ViewBag.HomeSchoolId = vm.TrustComparisonList.DefaultTrustCompanyNo;
            ViewBag.EstablishmentType = vm.EstablishmentType;
            ViewBag.TrustFinancing = financing;

            return View("Index", vm);
        }

        public async Task<PartialViewResult> TabChange(EstablishmentType type, UnitType showValue, RevenueGroupType tab = RevenueGroupType.Expenditure,
            CentralFinancingType financing = CentralFinancingType.Include, MatFinancingType trustFinancing = MatFinancingType.TrustAndAcademies,
            ChartFormat format = ChartFormat.Charts, ComparisonType comparisonType = ComparisonType.Manual, string bicComparisonOverallPhase = "Primary",
            bool excludePartial = false)
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

            var vm = new BenchmarkChartListViewModel(benchmarkCharts, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), chartGroups, 
                ComparisonType.Manual, null, null, null, null, type, type, null, null, academiesTerm, maintainedTerm, ComparisonArea.All, null, 0, 
                ComparisonListLimit.DEFAULT, _benchmarkBasketCookieManager.ExtractTrustComparisonListFromCookie(), null, excludePartial);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unitType;
            ViewBag.EstablishmentType = type;
            ViewBag.Financing = financing;
            ViewBag.TrustFinancing = trustFinancing;
            ViewBag.HomeSchoolId = (type == EstablishmentType.MAT) ? vm.TrustComparisonList.DefaultTrustCompanyNo.ToString() : vm.SchoolComparisonList.HomeSchoolUrn;
            ViewBag.ChartFormat = format;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.BicComparisonOverallPhase = bicComparisonOverallPhase;

            return PartialView("Partials/TabContent", vm);
        }

        public async Task<PartialViewResult> GetCharts(RevenueGroupType revGroup, ChartGroupType chartGroup, UnitType showValue,
            CentralFinancingType centralFinancing = CentralFinancingType.Include, MatFinancingType trustCentralFinancing = MatFinancingType.TrustAndAcademies,
            EstablishmentType type = EstablishmentType.All, ChartFormat format = ChartFormat.Charts,
            ComparisonType comparisonType = ComparisonType.Manual, string bicComparisonOverallPhase = "Primary")
        {
            List<ChartViewModel> benchmarkCharts;
            if (type == EstablishmentType.MAT)
            {
                benchmarkCharts = BuildTrustBenchmarkCharts(revGroup, chartGroup, showValue, trustCentralFinancing);
                ViewBag.HomeSchoolId = _benchmarkBasketCookieManager.ExtractTrustComparisonListFromCookie().DefaultTrustCompanyNo;
            }
            else
            {
                benchmarkCharts = await BuildSchoolBenchmarkChartsAsync(revGroup, chartGroup, showValue, centralFinancing);
                ViewBag.HomeSchoolId = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie().HomeSchoolUrn;
            }

            ViewBag.EstablishmentType = type;
            ViewBag.ChartFormat = format;
            ViewBag.UnitType = showValue;
            ViewBag.Financing = centralFinancing;
            ViewBag.TrustFinancing = trustCentralFinancing;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.BicComparisonOverallPhase = bicComparisonOverallPhase;

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

        private List<SchoolViewModel> PopulateBicSchoolsForBestInBreedTab(ComparisonType comparisonType, SchoolComparisonListModel comparisonList)
        {
            if (comparisonType == ComparisonType.BestInClass)
            {
                var bicSchools = new List<SchoolViewModel>();

                foreach (var school in comparisonList.BenchmarkSchools)
                {
                    var bicSchool = InstantiateBenchmarkSchool(int.Parse(school.Urn));
                    bicSchool.LaName = _laService.GetLaName(bicSchool.La.ToString());
                    bicSchools.Add(bicSchool);
                }

                return bicSchools;
            }
            return null;
        }

        private List<ChartViewModel> ConvertSelectionListToChartList(List<HierarchicalChartViewModel> customChartSelection)
        {
            List<ChartViewModel> customChartList = new List<ChartViewModel>();
            var allAvailableCharts = _benchmarkChartBuilder.Build(RevenueGroupType.AllExcludingSchoolPerf, ChartGroupType.All, EstablishmentType.All);
            foreach (var selection in customChartSelection.SelectMany(ccs => ccs.Charts))
            {
                if (selection.AbsoluteMoneySelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Name == selection.Name)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.AbsoluteMoney;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.PerPupilSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Name == selection.Name)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.PerPupil;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.PerTeacherSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Name == selection.Name)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.PerTeacher;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.PercentageSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Name == selection.Name)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.PercentageOfTotal;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.AbsoluteCountSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Name == selection.Name)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.AbsoluteCount;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.HeadCountPerFTESelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Name == selection.Name)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.HeadcountPerFTE;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.PercentageOfWorkforceSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Name == selection.Name)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.FTERatioToTotalFTE;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.NumberOfPupilsPerMeasureSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Name == selection.Name)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.NoOfPupilsPerMeasure;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
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

            _fcService.PopulateBenchmarkChartsWithFinancialData(benchmarkCharts, financialDataModels, comparisonList.BenchmarkSchools, comparisonList.HomeSchoolUrn, showValue);
            return benchmarkCharts;
        }

        private EstablishmentType DetectEstablishmentType(SchoolComparisonListModel comparisonList)
        {
            var schoolTypes = comparisonList.BenchmarkSchools
                .Select(bs => (EstablishmentType)Enum.Parse(typeof(EstablishmentType), bs.EstabType)).Distinct()
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
            _fcService.PopulateBenchmarkChartsWithFinancialData(benchmarkCharts, financialDataModels, comparisonList.Trusts, comparisonList.DefaultTrustCompanyNo.ToString(), showValue);
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

            foreach (var trust in trusts)
            {
                var financialDataModel = _financialDataService.GetTrustFinancialDataObject(trust.CompanyNo, terms.First(), matFinancing);
                models.Add(new FinancialDataModel(trust.CompanyNo.ToString(), terms.First(), financialDataModel, EstablishmentType.MAT));
            }

            return models;
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataForSchoolsAsync(List<BenchmarkSchoolModel> schools, CentralFinancingType centralFinancing = CentralFinancingType.Include)
        {
            var models = new List<FinancialDataModel>();

            var taskList = new List<Task<IEnumerable<SchoolTrustFinancialDataObject>>>();
            foreach (var school in schools)
            {
                var estabType = (EstablishmentType)Enum.Parse(typeof(EstablishmentType), school.EstabType);
                var latestYear = _financialDataService.GetLatestDataYearPerEstabType(estabType);
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);

                var task = _financialDataService.GetSchoolFinancialDataObjectAsync(Int32.Parse(school.Urn), term, estabType, centralFinancing);
                taskList.Add(task);
            }

            for (var i = 0; i < schools.Count; i++)
            {
                var estabType = (EstablishmentType)Enum.Parse(typeof(EstablishmentType), schools[i].EstabType);
                var latestYear = _financialDataService.GetLatestDataYearPerEstabType(estabType);
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
                var taskResult = await taskList[i];
                var resultDocument = taskResult?.FirstOrDefault();

                if (estabType == EstablishmentType.Academies && centralFinancing == CentralFinancingType.Include && resultDocument == null)//if nothing found in -Allocs collection try to source it from (non-allocated) Academies data
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

        private void AddDefaultBenchmarkSchoolToList(SchoolViewModel bmSchool)
        {
            var cookieObject = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            var defaultBenchmarkSchool = new BenchmarkSchoolModel()
            {
                Name = cookieObject.HomeSchoolName,
                Type = cookieObject.HomeSchoolType,
                EstabType = cookieObject.HomeSchoolFinancialType,
                Urn = cookieObject.HomeSchoolUrn,
                ProgressScore = bmSchool.ProgressScore
            };
            try
            {
                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, defaultBenchmarkSchool);
            }
            catch (ApplicationException) { }
        }

        private void SetSchoolAsDefault(SchoolViewModel benchmarkSchool)
        {
            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.SetDefault,
            new BenchmarkSchoolModel()
            {
                Name = benchmarkSchool.Name,
                Urn = benchmarkSchool.Id.ToString(),
                Type = benchmarkSchool.Type,
                EstabType = benchmarkSchool.EstablishmentType.ToString()
            });
        }

        private void EmptyBenchmarkList()
        {
            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.RemoveAll, null);
        }

        private void AddSchoolsToBenchmarkList(ComparisonResult comparisonResult)
        {
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
        }

        private decimal WithinPercentLimits(decimal percent)
        {
            if (percent > 100)
            {
                return 100;
            }
            if (percent < 0)
            {
                return 0;
            }
            else return percent;
        }

        private decimal WithinPositiveLimits(decimal value)
        {
            if (value < 0)
            {
                return 0;
            }
            else return value;
        }

        private void AddSchoolDataObjectsToBasket(ComparisonType comparison, List<int> urnList)
        {
            var benchmarkSchoolDataObjects = _contextDataService.GetMultipleSchoolDataObjectsByUrns(urnList);

            foreach (var schoolContextData in benchmarkSchoolDataObjects)
            {
                var benchmarkSchoolToAdd = new BenchmarkSchoolModel()
                {
                    Name = schoolContextData.EstablishmentName,
                    Type = schoolContextData.TypeOfEstablishment,
                    EstabType = schoolContextData.FinanceType,
                    Urn = schoolContextData.URN.ToString()
                };
                if (comparison == ComparisonType.BestInClass)
                {
                    var schoolFinancialData = _financialDataService.GetSchoolsLatestFinancialDataModel(int.Parse(benchmarkSchoolToAdd.Urn), (EstablishmentType)Enum.Parse(typeof(EstablishmentType), benchmarkSchoolToAdd.EstabType));
                    benchmarkSchoolToAdd.ProgressScore = schoolFinancialData.SchoolOverallPhase == "Secondary" ? schoolFinancialData.P8Mea
                        : decimal.Round(schoolFinancialData.Ks2Progress.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                }
                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, benchmarkSchoolToAdd);
            }
        }
    }
}