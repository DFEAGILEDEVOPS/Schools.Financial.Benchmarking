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
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Attributes;
using System.Net;
using SFB.Web.ApplicationCore.Helpers.Enums;
using System.Web.UI;
using SFB.Web.ApplicationCore.Helpers;

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
        private readonly IBicComparisonResultCachingService _bicComparisonResultCachingService;
        private readonly IEfficiencyMetricDataService _efficiencyMetricDataService;
        private readonly ISchoolBenchmarkListService _schoolBenchmarkListService;
        private readonly ITrustBenchmarkListService _trustBenchmarkListService;

        public BenchmarkChartsController(
            IBenchmarkChartBuilder benchmarkChartBuilder,
            IFinancialDataService financialDataService,
            IFinancialCalculationsService fcService,
            ILocalAuthoritiesService laService,
            IDownloadCSVBuilder csvBuilder,
            IContextDataService contextDataService,
            IBenchmarkCriteriaBuilderService benchmarkCriteriaBuilderService,
            IComparisonService comparisonService,
            IBicComparisonResultCachingService bicComparisonResultCachingService,
            IEfficiencyMetricDataService efficiencyMetricDataService,
            ISchoolBenchmarkListService benchmarkBasketService,
            ITrustBenchmarkListService trustBenchmarkListService)
        {
            _benchmarkChartBuilder = benchmarkChartBuilder;
            _financialDataService = financialDataService;
            _fcService = fcService;
            _laService = laService;
            _csvBuilder = csvBuilder;
            _contextDataService = contextDataService;
            _benchmarkCriteriaBuilderService = benchmarkCriteriaBuilderService;
            _comparisonService = comparisonService;
            _bicComparisonResultCachingService = bicComparisonResultCachingService;
            _efficiencyMetricDataService = efficiencyMetricDataService;
            _schoolBenchmarkListService = benchmarkBasketService;
            _trustBenchmarkListService = trustBenchmarkListService;
        }

        public async Task<ActionResult> GenerateFromSavedBasket(string urns, string companyNumbers, int? @default, BenchmarkListOverwriteStrategy? overwriteStrategy, ComparisonType comparison = ComparisonType.Manual)
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

            switch (overwriteStrategy)
            {
                case null:
                    {
                        if (urnList != null)
                        {
                            var comparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
                            if (comparisonList?.BenchmarkSchools?.Count == 0)
                            {
                                return await GenerateFromSavedBasket(urns, companyNumbers, @default, BenchmarkListOverwriteStrategy.Add, comparison);
                            }
                            else if (comparisonList?.BenchmarkSchools?.Count > 0 && comparisonList.BenchmarkSchools.Count + urnList.Count <= ComparisonListLimit.LIMIT)
                            {
                                var redirectUrl = $"SaveOverwriteStrategy?savedUrns={urns}";
                                if(@default.HasValue)
                                {
                                    redirectUrl += $"&default={@default}";
                                }
                                return Redirect(redirectUrl);
                            }
                            else if (comparisonList?.BenchmarkSchools?.Count > 0 && comparisonList.BenchmarkSchools.Count + urnList.Count > ComparisonListLimit.LIMIT)
                            {
                                var redirectUrl = $"ReplaceWithSavedBasket?savedUrns={urns}";
                                if (@default.HasValue)
                                {
                                    redirectUrl += $"&default={@default}";
                                }
                                return Redirect(redirectUrl);
                            }
                        }
                        else if (companyNoList != null)
                        {
                            var trustComparisonList = _trustBenchmarkListService.GetTrustBenchmarkList();

                            if (trustComparisonList?.Trusts?.Count == 0)
                            {
                                return await GenerateFromSavedBasket(urns, companyNumbers, @default, BenchmarkListOverwriteStrategy.Add, comparison);
                            }
                            else if (trustComparisonList?.Trusts?.Count > 0 && trustComparisonList.Trusts.Count + companyNoList.Count <= ComparisonListLimit.MAT_LIMIT)
                            {
                                var redirectUrl = $"SaveOverwriteStrategy?savedCompanyNos={companyNumbers}";
                                if(@default.HasValue)
                                {
                                    redirectUrl += $"&default={@default}";
                                }
                                return Redirect(redirectUrl);
                            }
                            else if (trustComparisonList?.Trusts?.Count > 0 && trustComparisonList.Trusts.Count + companyNoList.Count > ComparisonListLimit.MAT_LIMIT)
                            {
                                var redirectUrl = $"ReplaceWithSavedBasket?savedCompanyNos={companyNumbers}";
                                if (@default.HasValue)
                                {
                                    redirectUrl += $"&default={@default}";
                                }
                                return Redirect(redirectUrl);
                            }
                        }
                    }
                    break;
                case BenchmarkListOverwriteStrategy.Add:
                    {
                        if (urnList != null)
                        {
                            var comparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();

                            if (comparisonList.BenchmarkSchools.Count + urnList.Count > ComparisonListLimit.LIMIT)
                            {
                                var vm = new SaveOverwriteViewModel()
                                {
                                    ComparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList(),
                                    SavedUrns = urns,
                                    DefaultUrn = @default.GetValueOrDefault(),
                                    ErrorMessage = ErrorMessages.BMBasketLimitExceed
                                };

                                return View("SaveOverwriteStrategy", vm);
                            }
                            else
                            {
                                 await _schoolBenchmarkListService.AddSchoolsToBenchmarkListAsync(comparison, urnList);

                                if (@default.HasValue)
                                {
                                    await _schoolBenchmarkListService.SetSchoolAsDefaultAsync(@default.GetValueOrDefault());
                                }

                                return await Index(null, null, null, null, comparison);
                            }
                        }
                        else if (companyNoList != null)
                        {
                            var trustDataObjects = await _financialDataService.GetMultipleTrustDataObjectsByCompanyNumbersAsync(companyNoList);

                            foreach (var trustData in trustDataObjects)
                            {
                                _trustBenchmarkListService.TryAddTrustToBenchmarkList(trustData.CompanyNumber.GetValueOrDefault(), trustData.TrustOrCompanyName);

                                if (@default.HasValue)
                                {
                                    await _trustBenchmarkListService.SetTrustAsDefaultAsync(@default.GetValueOrDefault());
                                }

                            }

                            return await Mats();
                        }
                    }
                    break;
                case BenchmarkListOverwriteStrategy.Overwrite:
                    {
                        if (urnList != null)
                        {
                            _schoolBenchmarkListService.ClearSchoolBenchmarkList();

                            await _schoolBenchmarkListService.AddSchoolsToBenchmarkListAsync(comparison, urnList);
                            if (@default.HasValue)
                            {
                                await _schoolBenchmarkListService.SetSchoolAsDefaultAsync(@default.GetValueOrDefault());
                            }

                            return await Index(null, null, null, null, comparison);
                        }
                        else if (companyNoList != null)
                        {
                            var trustDataObjects = await _financialDataService.GetMultipleTrustDataObjectsByCompanyNumbersAsync(companyNoList);

                            _trustBenchmarkListService.ClearTrustBenchmarkList();

                            foreach (var trustData in trustDataObjects)
                            {
                                _trustBenchmarkListService.AddTrustToBenchmarkList(trustData.CompanyNumber.GetValueOrDefault(), trustData.TrustOrCompanyName);
                            }

                            if (@default.HasValue)
                            {
                                await _trustBenchmarkListService.SetTrustAsDefaultAsync(@default.GetValueOrDefault());
                            }

                            return await Mats();
                        }
                    }
                    break;
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        public ActionResult SaveOverwriteStrategy(string savedUrns, string savedCompanyNos, int? @default)
        {
            var vm = new SaveOverwriteViewModel()
            {
                TrustComparisonList = _trustBenchmarkListService.GetTrustBenchmarkList(),
                ComparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList(),
                SavedUrns = savedUrns,
                DefaultUrn = @default.GetValueOrDefault(),
                SavedCompanyNos = savedCompanyNos
            };

            return View("SaveOverwriteStrategy", vm);
        }

        [HttpGet]
        public ActionResult ReplaceWithSavedBasket(string savedUrns, string savedCompanyNos, int? @default)
        {
            var vm = new SaveOverwriteViewModel()
            {
                TrustComparisonList = _trustBenchmarkListService.GetTrustBenchmarkList(),
                ComparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList(),
                SavedUrns = savedUrns,
                DefaultUrn = @default.GetValueOrDefault(),
                SavedCompanyNos = savedCompanyNos
            };

            return View("ReplaceWithSavedBasket", vm);
        }

        [HttpGet]
        public async Task<ActionResult> GenerateFromSimpleCriteria(int urn, EstablishmentType estType, SimpleCriteria simpleCriteria, int basketSize = ComparisonListLimit.DEFAULT)
        {
            var benchmarkSchool = await InstantiateBenchmarkSchoolAsync(urn);

            var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromSimpleComparisonCriteria(benchmarkSchool.LatestYearFinancialData, simpleCriteria);

            var comparisonResult = await _comparisonService.GenerateBenchmarkListWithSimpleComparisonAsync(benchmarkCriteria, estType, basketSize, simpleCriteria, benchmarkSchool.LatestYearFinancialData);

            _schoolBenchmarkListService.ClearSchoolBenchmarkList();

            _schoolBenchmarkListService.AddSchoolsToBenchmarkList(comparisonResult);

            _schoolBenchmarkListService.TryAddSchoolToBenchmarkList(benchmarkSchool);

            _schoolBenchmarkListService.SetSchoolAsDefault(benchmarkSchool);

            return await Index(urn, simpleCriteria, comparisonResult.BenchmarkCriteria, null, ComparisonType.Basic, basketSize, benchmarkSchool.LatestYearFinancialData, estType);
        }

        [HttpGet]
        public async Task<ActionResult> SpecialsComparison(int urn, bool? similarPupils)
        {
            var benchmarkSchool = await InstantiateBenchmarkSchoolAsync(urn);

            var specialCriteria = new SpecialCriteria();
            specialCriteria.SimilarPupils = similarPupils.GetValueOrDefault();
            specialCriteria.TopSenCriteria = new List<SenCriterion>();
            for (int i=0; i<benchmarkSchool.TopSenCharacteristics.Count; i++)
            {
                var senCriterion = new SenCriterion(i, benchmarkSchool.TopSenCharacteristics[i].Definition, benchmarkSchool.TopSenCharacteristics[i].DataName, benchmarkSchool.TopSenCharacteristics[i].Value);
                specialCriteria.TopSenCriteria.Add(senCriterion);
            }

            var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromSpecialComparisonCriteria(benchmarkSchool.LatestYearFinancialData, specialCriteria);

            var comparisonResult = await _comparisonService.GenerateBenchmarkListWithSpecialComparisonAsync(benchmarkCriteria, specialCriteria, benchmarkSchool.LatestYearFinancialData);

            _schoolBenchmarkListService.ClearSchoolBenchmarkList();

            _schoolBenchmarkListService.AddSchoolsToBenchmarkList(comparisonResult);

            _schoolBenchmarkListService.TryAddSchoolToBenchmarkList(benchmarkSchool);

            return await Index(urn, null, comparisonResult.BenchmarkCriteria, null, ComparisonType.Specials, ComparisonListLimit.SPECIALS, benchmarkSchool.LatestYearFinancialData, EstablishmentType.All);
        }

        public async Task<ActionResult> GenerateFromBicCriteria(int urn)
        {
            var benchmarkSchool = await InstantiateBenchmarkSchoolAsync(urn);            

            var bicCriteria = new BestInClassCriteria()
            {
                EstablishmentType = EstablishmentType.All,
                OverallPhase = benchmarkSchool.OverallPhase,
                UrbanRural = benchmarkSchool.LatestYearFinancialData.UrbanRural,
                NoPupilsMin = WithinPositiveLimits((benchmarkSchool.LatestYearFinancialData.PupilCount.GetValueOrDefault() - CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_PUPIL_COUNT_TOPUP) * (1 - CriteriaSearchConfig.BIC_DEFAULT_FLEX_PUPIL_COUNT)),
                NoPupilsMax = (benchmarkSchool.LatestYearFinancialData.PupilCount.GetValueOrDefault() + CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_PUPIL_COUNT_TOPUP) * (1 + CriteriaSearchConfig.BIC_DEFAULT_FLEX_PUPIL_COUNT),
                PerPupilExpMin = 0,
                PerPupilExpMax = benchmarkSchool.LatestYearFinancialData.PerPupilTotalExpenditure.GetValueOrDefault() + CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_EXP_PP_TOPUP,
                PercentageFSMMin = WithinPercentLimits(benchmarkSchool.LatestYearFinancialData.PercentageOfEligibleFreeSchoolMeals.GetValueOrDefault() - CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_FSM_TOPUP) * (1 - CriteriaSearchConfig.BIC_DEFAULT_FLEX_FSM),
                PercentageFSMMax = WithinPercentLimits(benchmarkSchool.LatestYearFinancialData.PercentageOfEligibleFreeSchoolMeals.GetValueOrDefault() + CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_FSM_TOPUP) * (1 + CriteriaSearchConfig.BIC_DEFAULT_FLEX_FSM),
                PercentageSENMin = benchmarkSchool.LatestYearFinancialData.PercentageOfPupilsWithSen.GetValueOrDefault() * (1 - CriteriaSearchConfig.BIC_DEFAULT_FLEX_SEN),
                PercentageSENMax = WithinPercentLimits(benchmarkSchool.LatestYearFinancialData.PercentageOfPupilsWithSen.GetValueOrDefault() * (1 + CriteriaSearchConfig.BIC_DEFAULT_FLEX_SEN)),
                Ks2ProgressScoreMin = benchmarkSchool.BicProgressScoreType == BicProgressScoreType.P8 ? (decimal?)null : 0,
                Ks2ProgressScoreMax = benchmarkSchool.BicProgressScoreType == BicProgressScoreType.P8 ? (decimal?)null : 20,
                Ks4ProgressScoreMin = benchmarkSchool.BicProgressScoreType == BicProgressScoreType.P8 ? 0 : (decimal?)null,
                Ks4ProgressScoreMax = benchmarkSchool.BicProgressScoreType == BicProgressScoreType.P8 ? +5 : (decimal?)null,
                RRPerIncomeMin = CriteriaSearchConfig.RR_PER_INCOME_TRESHOLD,
                LondonWeighting = benchmarkSchool.LatestYearFinancialData.LondonWeighting == "Neither" ? new[] { "Neither" } : new[] { "Inner", "Outer" }
            };

            return await GenerateFromBicCriteria(urn, bicCriteria, false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenerateFromBicCriteria(int urn, BestInClassCriteria bicCriteria, bool isEditedCriteria = true)
        {
            var benchmarkSchool = await InstantiateBenchmarkSchoolAsync(urn);

            var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromBicComparisonCriteria(benchmarkSchool.LatestYearFinancialData, bicCriteria);

            ComparisonResult comparisonResult = null;

            if (!isEditedCriteria)
            {
                comparisonResult = _bicComparisonResultCachingService.GetBicComparisonResultByUrn(urn);
            }

            if (comparisonResult == null)
            {
                comparisonResult = await _comparisonService.GenerateBenchmarkListWithBestInClassComparisonAsync(bicCriteria.EstablishmentType, benchmarkCriteria,
                                         bicCriteria, benchmarkSchool.LatestYearFinancialData);

                if (!isEditedCriteria)
                {
                    _bicComparisonResultCachingService.StoreBicComparisonResultByUrn(urn, comparisonResult);
                }
            }

            _schoolBenchmarkListService.ClearSchoolBenchmarkList();

            foreach (var schoolData in comparisonResult.BenchmarkSchools)
            {
                var benchmarkSchoolToAdd = new BenchmarkSchoolModel(schoolData)
                {
                      ProgressScore = benchmarkSchool.BicProgressScoreType== BicProgressScoreType.P8 ?
                        decimal.Round(schoolData.Progress8Measure.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero)
                        : decimal.Round(schoolData.Ks2Progress.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero)

                };

                 _schoolBenchmarkListService.TryAddSchoolToBenchmarkList(benchmarkSchoolToAdd);

            }

            _schoolBenchmarkListService.TryAddSchoolToBenchmarkList(benchmarkSchool);

            return await Index(urn, null, comparisonResult.BenchmarkCriteria, bicCriteria, ComparisonType.BestInClass, ComparisonListLimit.DEFAULT, benchmarkSchool.LatestYearFinancialData, bicCriteria.EstablishmentType);
        }

        [HttpGet]
        [OutputCache (Duration=28800, VaryByParam= "urn", Location = OutputCacheLocation.Server, NoStore=true)]        
        public async Task<ViewResult> OneClickReport(int urn)
        {
            var benchmarkSchool = await InstantiateBenchmarkSchoolAsync(urn);

            var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromOneClickComparisonCriteria(benchmarkSchool.LatestYearFinancialData);

            var comparisonResult = await _comparisonService.GenerateBenchmarkListWithOneClickComparisonAsync(benchmarkCriteria, EstablishmentType.All, ComparisonListLimit.ONE_CLICK, benchmarkSchool.LatestYearFinancialData);

            _schoolBenchmarkListService.ClearSchoolBenchmarkList();

            _schoolBenchmarkListService.AddSchoolsToBenchmarkList(comparisonResult);

            _schoolBenchmarkListService.SetSchoolAsDefault(benchmarkSchool);

            _schoolBenchmarkListService.TryAddSchoolToBenchmarkList(benchmarkSchool);

            var benchmarkCharts = await BuildSchoolBenchmarkChartsAsync(TabType.Custom, ChartGroupType.Custom, null, CentralFinancingType.Include);

            var vm = new BenchmarkChartListViewModel(
                benchmarkCharts,
                _schoolBenchmarkListService.GetSchoolBenchmarkList(),
                null,
                ComparisonType.OneClick,
                comparisonResult.BenchmarkCriteria,
                null,
                null,
                benchmarkSchool.LatestYearFinancialData,
                benchmarkSchool.EstablishmentType,
                EstablishmentType.All,
                null, null,
                SchoolFormatHelpers.FinancialTermFormatAcademies(await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Academies)),
                SchoolFormatHelpers.FinancialTermFormatMaintained(await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Maintained)),
                ComparisonArea.All, null, urn,
                ComparisonListLimit.ONE_CLICK);

            ViewBag.ChartFormat = ChartFormat.Charts;
            ViewBag.HomeSchoolId = vm.SchoolComparisonList.HomeSchoolUrn;
            ViewBag.Financing = CentralFinancingType.Include;

            return View(vm);
        }

        [HttpGet]
        public async Task<ActionResult> GenerateFromEfficiencyMetricsTop(int urn)
        {
            var defaultSchool = await _efficiencyMetricDataService.GetSchoolDataObjectByUrnAsync(urn);
            var neighbourSchools = (await _efficiencyMetricDataService.GetSchoolDataObjectByUrnAsync(urn)).Neighbours;
            var topNeighbourSchoolURNs = neighbourSchools.OrderBy(s => s.EfficiencyDecileNeighbour).Take(15).Select(n => n.Urn).ToList();

            _schoolBenchmarkListService.ClearSchoolBenchmarkList();

            await _schoolBenchmarkListService.AddSchoolsToBenchmarkListAsync(ComparisonType.EfficiencyTop, topNeighbourSchoolURNs);

            await _schoolBenchmarkListService.SetSchoolAsDefaultAsync(urn);

            return await Index(urn, null, null, comparisonType: ComparisonType.EfficiencyTop);            
        }

        [HttpPost]
        public async Task<ActionResult> GenerateFromEfficiencyMetricsManual(int urn, string neighbourURNs)
        {
            var neighbourUrnList = neighbourURNs?.Split(',').Select(u => int.Parse(u)).ToList();

            _schoolBenchmarkListService.ClearSchoolBenchmarkList();

            await _schoolBenchmarkListService.AddSchoolsToBenchmarkListAsync(ComparisonType.EfficiencyManual, neighbourUrnList);

            await _schoolBenchmarkListService.SetSchoolAsDefaultAsync(urn);

            return await Index(urn, null, null, comparisonType: ComparisonType.EfficiencyManual);
        }

        [HttpGet]
        public async Task<ActionResult> GenerateNewFromAdvancedCriteria()
        {
            int? urn;
            BenchmarkCriteria usedCriteria;
            try
            {
                urn = (int?)TempData["URN"];
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenerateFromAdvancedCriteria(BenchmarkCriteria criteria, EstablishmentType estType, int? lacode, int? urn, ComparisonArea areaType, 
            BenchmarkListOverwriteStrategy? overwriteStrategy, bool excludePartial = false)
        {
            criteria.LocalAuthorityCode = lacode;
            SchoolViewModel benchmarkSchoolVM;
            if (urn.HasValue)
            {
                benchmarkSchoolVM = await InstantiateBenchmarkSchoolAsync(urn.Value);
            }
            else
            {
                benchmarkSchoolVM = new SchoolViewModelWithNoDefaultSchool();
            }

            switch (overwriteStrategy)
            {
                case null:
                    var comparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
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
                            ErrorMessage = ErrorMessages.SelectOverwriteStrategy
                        });
                case BenchmarkListOverwriteStrategy.Overwrite:
                    var result = await _comparisonService.GenerateBenchmarkListWithAdvancedComparisonAsync(criteria, estType, excludePartial, 30);

                    _schoolBenchmarkListService.ClearSchoolBenchmarkList();

                    _schoolBenchmarkListService.AddSchoolsToBenchmarkList(result);

                    break;
                case BenchmarkListOverwriteStrategy.Add:
                    comparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
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
                     _schoolBenchmarkListService.AddSchoolsToBenchmarkList(comparisonResult);
                    break;
            }

            if (!(benchmarkSchoolVM is SchoolViewModelWithNoDefaultSchool))
            {
                _schoolBenchmarkListService.TryAddSchoolToBenchmarkList(benchmarkSchoolVM);
            }

            return await Index(urn, null, criteria, null, ComparisonType.Advanced, ComparisonListLimit.DEFAULT, 
                benchmarkSchoolVM.LatestYearFinancialData, estType, areaType, lacode.ToString(), excludePartial);
        }

        public async Task<PartialViewResult> CustomReport(string json, ChartFormat format)
        {
            var customSelection = (CustomSelectionListViewModel)JsonConvert.DeserializeObject(json, typeof(CustomSelectionListViewModel));
            var customCharts = ConvertSelectionListToChartList(customSelection.HierarchicalCharts);
            var comparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();

            var financialDataModels = await this.GetFinancialDataForSchoolsAsync(comparisonList.BenchmarkSchools, (CentralFinancingType)Enum.Parse(typeof(CentralFinancingType), customSelection.CentralFinance));
            _fcService.PopulateBenchmarkChartsWithFinancialData(customCharts, financialDataModels, comparisonList.BenchmarkSchools, comparisonList.HomeSchoolUrn, null);

            var academiesTerm = SchoolFormatHelpers.FinancialTermFormatAcademies(await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Academies));
            var maintainedTerm = SchoolFormatHelpers.FinancialTermFormatMaintained(await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Maintained));

            var vm = new BenchmarkChartListViewModel(customCharts, _schoolBenchmarkListService.GetSchoolBenchmarkList(), null,
                ComparisonType.Manual, null, null, null, null, EstablishmentType.All, EstablishmentType.All, null, null, academiesTerm, maintainedTerm, ComparisonArea.All, null, null, ComparisonListLimit.DEFAULT);

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
            TabType tab = TabType.Expenditure,
            CentralFinancingType financing = CentralFinancingType.Include)
        {
            var chartGroup = DetermineDefaultChartGroup(tab);

            var defaultUnitType = DetermineDefaultUnitType(comparisonType, tab);
            var benchmarkCharts = await BuildSchoolBenchmarkChartsAsync(tab, chartGroup, defaultUnitType, financing);
            var establishmentType = DetectEstablishmentType(_schoolBenchmarkListService.GetSchoolBenchmarkList());

            var chartGroups = _benchmarkChartBuilder.Build(tab, establishmentType).DistinctBy(c => c.ChartGroup).ToList();

            string selectedArea = "";
            switch (areaType)
            {
                case ComparisonArea.All:
                    selectedArea = "All England";
                    break;
                case ComparisonArea.LaCodeName:
                     selectedArea = _laService.GetLaName(laCode);
                    break;
            }

            string schoolArea = "";
            if (benchmarkSchoolData != null)
            {
                schoolArea = _laService.GetLaName(benchmarkSchoolData.LaNumber.ToString());
            }

            var academiesTerm = SchoolFormatHelpers.FinancialTermFormatAcademies(await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Academies));
            var maintainedTerm = SchoolFormatHelpers.FinancialTermFormatMaintained(await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Maintained));

            var comparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();

            var comparisonSchools = await PopulateSchoolsListForComparisonTableAsync(comparisonList);

            var vm = new BenchmarkChartListViewModel(benchmarkCharts, comparisonList, chartGroups, comparisonType, advancedCriteria, simpleCriteria,
                bicCriteria, benchmarkSchoolData, establishmentType, searchedEstabType, schoolArea, selectedArea, academiesTerm, maintainedTerm, areaType,
                laCode, urn, basketSize, null, comparisonSchools, excludePartial);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = defaultUnitType;
            ViewBag.HomeSchoolId = vm.SchoolComparisonList.HomeSchoolUrn;
            ViewBag.EstablishmentType = vm.EstablishmentType;
            ViewBag.Financing = financing;
            ViewBag.ChartFormat = ChartFormat.Charts;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.BicComparisonOverallPhase = comparisonSchools?.FirstOrDefault()?.OverallPhaseInFinancialSubmission;

            return View("Index", vm);
        }

        public async Task<ActionResult> Mats(TabType tab = TabType.Expenditure, MatFinancingType financing = MatFinancingType.TrustAndAcademies)
        {
            var chartGroup = DetermineDefaultChartGroup(tab);
            var defaultUnitType = DetermineDefaultUnitType(ComparisonType.Basic, tab);
            var benchmarkCharts = await BuildTrustBenchmarkChartsAsync(tab, chartGroup, UnitType.AbsoluteMoney, financing);
            var chartGroups = _benchmarkChartBuilder.Build(tab, EstablishmentType.MAT).DistinctBy(c => c.ChartGroup).ToList();

            var academiesTerm = SchoolFormatHelpers.FinancialTermFormatAcademies(await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Academies));
            var maintainedTerm = SchoolFormatHelpers.FinancialTermFormatMaintained(await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Maintained));

            var vm = new BenchmarkChartListViewModel(benchmarkCharts, null, chartGroups, ComparisonType.Manual, null, null, null, null, EstablishmentType.MAT, 
                EstablishmentType.MAT, null, null, academiesTerm, maintainedTerm, ComparisonArea.All, null, 0, ComparisonListLimit.DEFAULT,
                _trustBenchmarkListService.GetTrustBenchmarkList());

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = defaultUnitType;
            ViewBag.HomeSchoolId = vm.TrustComparisonList.DefaultTrustCompanyNo;
            ViewBag.EstablishmentType = vm.EstablishmentType;
            ViewBag.TrustFinancing = financing;

            return View("Index", vm);
        }

        public async Task<PartialViewResult> TabChange(EstablishmentType type, UnitType showValue, TabType tab = TabType.Expenditure,
            CentralFinancingType financing = CentralFinancingType.Include, MatFinancingType trustFinancing = MatFinancingType.TrustAndAcademies,
            ChartFormat format = ChartFormat.Charts, ComparisonType comparisonType = ComparisonType.Manual, string bicComparisonOverallPhase = "Primary",
            bool excludePartial = false)
        {
            var chartGroup = DetermineDefaultChartGroup(tab);

            var unitType = DetermineUnitTypeWhenTabChanged(showValue, tab);

            List<ChartViewModel> benchmarkCharts;
            if (type == EstablishmentType.MAT)
            {
                benchmarkCharts = await BuildTrustBenchmarkChartsAsync(tab, chartGroup, unitType, trustFinancing);
            }
            else
            {
                benchmarkCharts = await BuildSchoolBenchmarkChartsAsync(tab, chartGroup, unitType, financing);
            }
            var chartGroups = _benchmarkChartBuilder.Build(tab, EstablishmentType.All).DistinctBy(c => c.ChartGroup).ToList();

            var academiesTerm = SchoolFormatHelpers.FinancialTermFormatAcademies(await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Academies));
            var maintainedTerm = SchoolFormatHelpers.FinancialTermFormatMaintained(await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.Maintained));

            var vm = new BenchmarkChartListViewModel(benchmarkCharts, _schoolBenchmarkListService.GetSchoolBenchmarkList(), chartGroups,
                ComparisonType.Manual, null, null, null, null, type, type, null, null, academiesTerm, maintainedTerm, ComparisonArea.All, null, 0,
                ComparisonListLimit.DEFAULT, _trustBenchmarkListService.GetTrustBenchmarkList(), null, excludePartial);

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

        public async Task<PartialViewResult> GetCharts(TabType revGroup, 
            ChartGroupType chartGroup, UnitType showValue,
            CentralFinancingType centralFinancing = CentralFinancingType.Include, 
            MatFinancingType trustCentralFinancing = MatFinancingType.TrustAndAcademies,
            EstablishmentType type = EstablishmentType.All, ChartFormat format = ChartFormat.Charts,
            ComparisonType comparisonType = ComparisonType.Manual,
            string bicComparisonOverallPhase = "Primary")
        {
            List<ChartViewModel> benchmarkCharts;
            if (type == EstablishmentType.MAT)
            {
                benchmarkCharts = await BuildTrustBenchmarkChartsAsync(revGroup, chartGroup, showValue, trustCentralFinancing);
                ViewBag.HomeSchoolId = _trustBenchmarkListService.GetTrustBenchmarkList().DefaultTrustCompanyNo;
            }
            else
            {
                benchmarkCharts = await BuildSchoolBenchmarkChartsAsync(revGroup, chartGroup, showValue, centralFinancing);
                ViewBag.HomeSchoolId = _schoolBenchmarkListService.GetSchoolBenchmarkList().HomeSchoolUrn;
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
            string csv;
            if (type == EstablishmentType.MAT)
            {
                benchmarkCharts = await BuildTrustBenchmarkChartsAsync(TabType.AllExcludingSchoolPerf, ChartGroupType.All, UnitType.AbsoluteMoney, MatFinancingType.TrustAndAcademies);
                csv = _csvBuilder.BuildCSVContentForTrusts(_trustBenchmarkListService.GetTrustBenchmarkList(), benchmarkCharts);
            }
            else
            {
                benchmarkCharts = await BuildSchoolBenchmarkChartsAsync(TabType.AllIncludingSchoolPerf, ChartGroupType.All, null, CentralFinancingType.Exclude);
                csv = _csvBuilder.BuildCSVContentForSchools(_schoolBenchmarkListService.GetSchoolBenchmarkList(), benchmarkCharts);
            }

            return File(Encoding.UTF8.GetBytes(csv),
                         "text/plain",
                         "BenchmarkData.csv");
        }

        private async Task<List<SchoolViewModel>> PopulateSchoolsListForComparisonTableAsync(SchoolComparisonListModel comparisonList)
        {
            var comparisonSchools = new List<SchoolViewModel>();

            foreach (var school in comparisonList.BenchmarkSchools)
            {
                var bmSchool = await InstantiateBenchmarkSchoolAsync(int.Parse(school.Urn));
                bmSchool.LaName = _laService.GetLaName(bmSchool.La.ToString());
                comparisonSchools.Add(bmSchool);
            }

            return comparisonSchools;
        }

        private List<ChartViewModel> ConvertSelectionListToChartList(List<HierarchicalChartViewModel> customChartSelection)
        {
            List<ChartViewModel> customChartList = new List<ChartViewModel>();
            var allAvailableCharts = _benchmarkChartBuilder.Build(TabType.AllExcludingSchoolPerf, ChartGroupType.All, EstablishmentType.All);
            foreach (var selection in customChartSelection.SelectMany(ccs => ccs.Charts))
            {
                if (selection.AbsoluteMoneySelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Id == selection.Id)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.AbsoluteMoney;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.PerPupilSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Id == selection.Id)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.PerPupil;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.PerTeacherSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Id == selection.Id)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.PerTeacher;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.PercentageExpenditureSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Id == selection.Id)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.PercentageOfTotalExpenditure;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.PercentageIncomeSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Id == selection.Id)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.PercentageOfTotalIncome;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.AbsoluteCountSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Id == selection.Id)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.AbsoluteCount;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.HeadCountPerFTESelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Id == selection.Id)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.HeadcountPerFTE;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.PercentageOfWorkforceSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Id == selection.Id)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.FTERatioToTotalFTE;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
                if (selection.NumberOfPupilsPerMeasureSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Id == selection.Id)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.NoOfPupilsPerMeasure;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }

                if (selection.PercentageTeachersSelected)
                {
                    var customChart = (ChartViewModel)allAvailableCharts.FirstOrDefault(c => c.Id == selection.Id)?.Clone();
                    if (customChart != null)
                    {
                        customChart.ShowValue = UnitType.PercentageTeachers;
                        customChart.ChartType = ChartType.CustomReport;
                        customChartList.Add(customChart);
                    }
                }
            }

            return customChartList;
        }

        private async Task<List<ChartViewModel>> BuildSchoolBenchmarkChartsAsync(TabType revGroup, ChartGroupType chartGroup, UnitType? showValue, CentralFinancingType cFinancing)
        {
            var comparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
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

        private async Task<List<ChartViewModel>> BuildTrustBenchmarkChartsAsync(TabType revGroup, ChartGroupType chartGroup, UnitType showValue, MatFinancingType mFinancing)
        {
            var comparisonList = _trustBenchmarkListService.GetTrustBenchmarkList();
            var benchmarkCharts = _benchmarkChartBuilder.Build(revGroup, chartGroup, EstablishmentType.MAT);
            var financialDataModels = await this.GetFinancialDataForTrustsAsync(comparisonList.Trusts, mFinancing);
            _fcService.PopulateBenchmarkChartsWithFinancialData(benchmarkCharts, financialDataModels, comparisonList.Trusts, comparisonList.DefaultTrustCompanyNo.ToString(), showValue);
            return benchmarkCharts;
        }

        private void RemoveIrrelevantCharts(UnitType unit, List<ChartViewModel> chartList)
        {
            switch (unit)
            {
                case UnitType.PercentageOfTotalExpenditure:
                    chartList.RemoveAll(c => c.Id == 1);//Total Expenditure
                    break;
                case UnitType.PercentageOfTotalIncome:
                    chartList.RemoveAll(c => c.Id == 33);//Total Income
                    break;
                case UnitType.HeadcountPerFTE:
                    chartList.RemoveAll(c => c.Id == 57);//School workforce (headcount)
                    chartList.RemoveAll(c => c.Id == 52);//Teachers with Qualified Teacher Status (%)
                    break;
                case UnitType.FTERatioToTotalFTE:
                    chartList.RemoveAll(c => c.Id == 52);//Teachers with Qualified Teacher Status (%)
                    chartList.RemoveAll(c => c.Id == 50);//School workforce (Full Time Equivalent)
                    chartList.RemoveAll(c => c.Id == 57);//School workforce (headcount)
                    break;
                case UnitType.NoOfPupilsPerMeasure:
                    chartList.RemoveAll(c => c.Id == 52);//Teachers with Qualified Teacher Status (%)
                    break;
            }
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataForTrustsAsync(List<BenchmarkTrustModel> trusts, MatFinancingType matFinancing = MatFinancingType.TrustAndAcademies)
        {
            var models = new List<FinancialDataModel>();

            var terms = await _financialDataService.GetActiveTermsForMatCentralAsync();

            foreach (var trust in trusts)
            {
                var financialDataModel = await _financialDataService.GetTrustFinancialDataObjectByCompanyNoAsync(trust.CompanyNo, terms.First(), matFinancing);
                models.Add(new FinancialDataModel(trust.CompanyNo.ToString(), terms.First(), financialDataModel, EstablishmentType.MAT));
            }

            return models;
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataForSchoolsAsync(List<BenchmarkSchoolModel> schools, CentralFinancingType centralFinancing = CentralFinancingType.Include)
        {
            var schoolSearchModels = schools.Select(s => new SchoolSearchModel(s.Urn, s.EstabType)).ToList();
            return await _financialDataService.GetFinancialDataForSchoolsAsync(schoolSearchModels, centralFinancing);
        }

        private async Task<SchoolViewModel> InstantiateBenchmarkSchoolAsync(int urn)
        {
            var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), _schoolBenchmarkListService.GetSchoolBenchmarkList());
            var schoolsLatestFinancialDataModel = await _financialDataService.GetSchoolsLatestFinancialDataModelAsync(benchmarkSchool.Id, benchmarkSchool.EstablishmentType);
            benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel> { schoolsLatestFinancialDataModel };
            return benchmarkSchool;
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

        private UnitType DetermineDefaultUnitType(ComparisonType comparisonType, TabType tab)
        {
            UnitType defaultUnitType;
            switch (tab)
            {
                case TabType.Workforce:
                    defaultUnitType = UnitType.AbsoluteCount;
                    break;
                case TabType.Salary:
                    defaultUnitType = UnitType.PercentageTeachers;
                    break;
                default:
                    defaultUnitType = comparisonType == ComparisonType.BestInClass || comparisonType == ComparisonType.Specials
                                      ? UnitType.PerPupil
                                      : UnitType.AbsoluteMoney;
                    break;
            }

            return defaultUnitType;
        }

        private UnitType DetermineUnitTypeWhenTabChanged(UnitType showValue, TabType tab)
        {
            UnitType unitType;
            switch (tab)
            {
                case TabType.Workforce:
                    unitType = UnitType.AbsoluteCount;
                    break;
                case TabType.Balance:
                    unitType = showValue == UnitType.PercentageOfTotalIncome || showValue == UnitType.PercentageOfTotalExpenditure || showValue == UnitType.PerPupil || showValue == UnitType.PerTeacher ? showValue : UnitType.AbsoluteMoney;
                    break;
                case TabType.Salary:
                    unitType = UnitType.PercentageTeachers;
                    break;
                case TabType.Income:
                    unitType = showValue == UnitType.PercentageOfTotalExpenditure ? UnitType.PercentageOfTotalExpenditure : showValue;
                    break;
                case TabType.Expenditure:
                    unitType = showValue == UnitType.PercentageOfTotalIncome ? UnitType.PercentageOfTotalIncome : showValue;
                    break;
                default:
                    unitType = UnitType.AbsoluteMoney;
                    break;
            }

            return unitType;
        }

        private ChartGroupType DetermineDefaultChartGroup(TabType tab)
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
                case TabType.Salary:
                    chartGroup = ChartGroupType.Salary;
                    break;
                default:
                    chartGroup = ChartGroupType.All;
                    break;
            }

            return chartGroup;
        }


    }
}