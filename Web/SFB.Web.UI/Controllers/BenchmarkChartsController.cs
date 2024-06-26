﻿using System;
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
using SFB.Web.ApplicationCore.Entities;

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
            List<long> urnList = null;
            List<int> companyNoList = null;
            try
            {
                urnList = urns?.Split('-').Select(urn => long.Parse(urn)).ToList();
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
        public async Task<ActionResult> GenerateFromSimpleCriteria(long? urn, long? fuid, EstablishmentType estType, SimpleCriteria simpleCriteria, int basketSize = ComparisonListLimit.DEFAULT)
        {
            if (fuid.HasValue)
            {
                var benchmarkFed = await InstantiateBenchmarkSchoolOrFedAsync(fuid.GetValueOrDefault());

                var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromSimpleComparisonCriteria(benchmarkFed.LatestYearFinancialData, simpleCriteria);

                var comparisonResult = await _comparisonService.GenerateBenchmarkListWithSimpleComparisonAsync(benchmarkCriteria, estType, basketSize, simpleCriteria, benchmarkFed.LatestYearFinancialData, false);

                if (comparisonResult.BenchmarkSchools.Count < 15)
                {
                    var openCriteria = new SimpleCriteria() { IncludeEal = false, IncludeFsm = false, IncludeSen = false };
                    benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromSimpleComparisonCriteria(benchmarkFed.LatestYearFinancialData, openCriteria);
                    benchmarkCriteria.MinNoPupil = (benchmarkFed.LatestYearFinancialData.PupilCount / 100 * 50);
                    benchmarkCriteria.MaxNoPupil = (benchmarkFed.LatestYearFinancialData.PupilCount / 100 * 150);
                    comparisonResult = await _comparisonService.GenerateBenchmarkListWithSimpleComparisonAsync(benchmarkCriteria, EstablishmentType.All, ComparisonListLimit.DEFAULT, openCriteria, benchmarkFed.LatestYearFinancialData, false);
                }
                
                _schoolBenchmarkListService.ClearSchoolBenchmarkList();

                _schoolBenchmarkListService.AddSchoolsToBenchmarkList(comparisonResult);

                _schoolBenchmarkListService.TryAddFederationToBenchmarkList((FederationViewModel)benchmarkFed);

                _schoolBenchmarkListService.SetFederationAsDefault((FederationViewModel)benchmarkFed);

                return await Index(fuid, simpleCriteria, comparisonResult.BenchmarkCriteria, null, ComparisonType.FederationBasic, basketSize, benchmarkFed.LatestYearFinancialData, estType);
            }
            else
            {
                var benchmarkSchool = await InstantiateBenchmarkSchoolOrFedAsync(urn.GetValueOrDefault());

                var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromSimpleComparisonCriteria(benchmarkSchool.LatestYearFinancialData, simpleCriteria);

                var comparisonResult = await _comparisonService.GenerateBenchmarkListWithSimpleComparisonAsync(benchmarkCriteria, estType, basketSize, simpleCriteria, benchmarkSchool.LatestYearFinancialData);

                if (comparisonResult.BenchmarkSchools.Count < 15)
                {
                    var openCriteria = new SimpleCriteria() { IncludeEal = false, IncludeFsm = false, IncludeSen = false };
                    benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromSimpleComparisonCriteria(benchmarkSchool.LatestYearFinancialData, openCriteria);
                    benchmarkCriteria.MinNoPupil = (benchmarkSchool.LatestYearFinancialData.PupilCount / 100 * 50);
                    benchmarkCriteria.MaxNoPupil = (benchmarkSchool.LatestYearFinancialData.PupilCount / 100 * 150);
                    comparisonResult = await _comparisonService.GenerateBenchmarkListWithSimpleComparisonAsync(benchmarkCriteria, EstablishmentType.All, ComparisonListLimit.DEFAULT, openCriteria, benchmarkSchool.LatestYearFinancialData, false);
                }
                
                _schoolBenchmarkListService.ClearSchoolBenchmarkList();

                _schoolBenchmarkListService.AddSchoolsToBenchmarkList(comparisonResult);

                _schoolBenchmarkListService.TryAddSchoolToBenchmarkList((SchoolViewModel)benchmarkSchool);

                _schoolBenchmarkListService.SetSchoolAsDefault((SchoolViewModel)benchmarkSchool);

                return await Index(urn, simpleCriteria, comparisonResult.BenchmarkCriteria, null, ComparisonType.Basic, basketSize, benchmarkSchool.LatestYearFinancialData, estType);
            }
        }

        [HttpGet]
        public async Task<ActionResult> SpecialsComparison(long urn, bool? similarPupils)
        {
            var benchmarkSchool = await InstantiateBenchmarkSchoolOrFedAsync(urn);

            var specialCriteria = new SpecialCriteria();
            specialCriteria.SimilarPupils = similarPupils.GetValueOrDefault();
            specialCriteria.TopSenCriteria = new List<SenCriterion>();
            for (int i=0; i< (benchmarkSchool as SchoolViewModel).TopSenCharacteristics.Count; i++)
            {
                var senCriterion = new SenCriterion(i, (benchmarkSchool as SchoolViewModel).TopSenCharacteristics[i].Definition, (benchmarkSchool as SchoolViewModel).TopSenCharacteristics[i].DataName, (benchmarkSchool as SchoolViewModel).TopSenCharacteristics[i].Value);
                specialCriteria.TopSenCriteria.Add(senCriterion);
            }

            var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromSpecialComparisonCriteria(benchmarkSchool.LatestYearFinancialData, specialCriteria);

            var comparisonResult = await _comparisonService.GenerateBenchmarkListWithSpecialComparisonAsync(benchmarkCriteria, specialCriteria, benchmarkSchool.LatestYearFinancialData);

            _schoolBenchmarkListService.ClearSchoolBenchmarkList();

            _schoolBenchmarkListService.AddSchoolsToBenchmarkList(comparisonResult);

            _schoolBenchmarkListService.TryAddSchoolToBenchmarkList((benchmarkSchool as SchoolViewModel));

            return await Index(urn, null, comparisonResult.BenchmarkCriteria, null, ComparisonType.Specials, ComparisonListLimit.SPECIALS, benchmarkSchool.LatestYearFinancialData, EstablishmentType.All);
        }

        public async Task<ActionResult> GenerateFromBicCriteria(long urn)
        {
            ViewBag.ModelState = ModelState;

            var benchmarkSchool = await InstantiateBenchmarkSchoolOrFedAsync(urn);            

            var bicCriteria = new BestInClassCriteria()
            {
                EstablishmentType = EstablishmentType.All,
                OverallPhase = (benchmarkSchool as SchoolViewModel).OverallPhase,
                UrbanRural = benchmarkSchool.LatestYearFinancialData.UrbanRural,
                NoPupilsMin = WithinPositiveLimits((benchmarkSchool.LatestYearFinancialData.PupilCount.GetValueOrDefault() - CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_PUPIL_COUNT_TOPUP) * (1 - CriteriaSearchConfig.BIC_DEFAULT_FLEX_PUPIL_COUNT)),
                NoPupilsMax = (benchmarkSchool.LatestYearFinancialData.PupilCount.GetValueOrDefault() + CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_PUPIL_COUNT_TOPUP) * (1 + CriteriaSearchConfig.BIC_DEFAULT_FLEX_PUPIL_COUNT),
                PerPupilExpMin = 0,
                PerPupilExpMax = benchmarkSchool.LatestYearFinancialData.PerPupilTotalExpenditure.GetValueOrDefault() + CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_EXP_PP_TOPUP,
                PercentageFSMMin = WithinPercentLimits(benchmarkSchool.LatestYearFinancialData.PercentageOfEligibleFreeSchoolMeals.GetValueOrDefault() - CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_FSM_TOPUP) * (1 - CriteriaSearchConfig.BIC_DEFAULT_FLEX_FSM),
                PercentageFSMMax = WithinPercentLimits(benchmarkSchool.LatestYearFinancialData.PercentageOfEligibleFreeSchoolMeals.GetValueOrDefault() + CriteriaSearchConfig.BIC_DEFAULT_CONSTANT_FSM_TOPUP) * (1 + CriteriaSearchConfig.BIC_DEFAULT_FLEX_FSM),
                PercentageSENMin = benchmarkSchool.LatestYearFinancialData.PercentageOfPupilsWithSen.GetValueOrDefault() * (1 - CriteriaSearchConfig.BIC_DEFAULT_FLEX_SEN),
                PercentageSENMax = WithinPercentLimits(benchmarkSchool.LatestYearFinancialData.PercentageOfPupilsWithSen.GetValueOrDefault() * (1 + CriteriaSearchConfig.BIC_DEFAULT_FLEX_SEN)),
                Ks2ProgressScoreMin = (benchmarkSchool as SchoolViewModel).BicProgressScoreType == BicProgressScoreType.P8 ? (decimal?)null : 0,
                Ks2ProgressScoreMax = (benchmarkSchool as SchoolViewModel).BicProgressScoreType == BicProgressScoreType.P8 ? (decimal?)null : 20,
                Ks4ProgressScoreMin = (benchmarkSchool as SchoolViewModel).BicProgressScoreType == BicProgressScoreType.P8 ? 0 : (decimal?)null,
                Ks4ProgressScoreMax = (benchmarkSchool as SchoolViewModel).BicProgressScoreType == BicProgressScoreType.P8 ? +5 : (decimal?)null,
                RRPerIncomeMin = CriteriaSearchConfig.RR_PER_INCOME_TRESHOLD,
                LondonWeighting = benchmarkSchool.LatestYearFinancialData.LondonWeighting == "Neither" ? new[] { "Neither" } : new[] { "Inner", "Outer" }
            };

            return await GenerateFromBicCriteria(urn, bicCriteria, false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenerateFromBicCriteria(long urn, BestInClassCriteria bicCriteria, bool isEditedCriteria = true)
        {
            ViewBag.ModelState = ModelState;

            async Task<SchoolViewModel> InstantiateBenchmarkSchoolAsync(long id)
            {
                var bmSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(id), _schoolBenchmarkListService.GetSchoolBenchmarkList());
                var schoolsLatestFinancialDataModel = await _financialDataService.GetSchoolsLatestFinancialDataModelAsync(bmSchool.Id, bmSchool.EstablishmentType);
                bmSchool.HistoricalFinancialDataModels = new List<FinancialDataModel> { schoolsLatestFinancialDataModel };
                return bmSchool;
            }

            if (!ModelState.IsValid) {
                    return View("~/Views/BenchmarkCriteria/BestInClassCharacteristics.cshtml", 
                    new BestInClassCharacteristicsViewModel(await InstantiateBenchmarkSchoolAsync(urn), bicCriteria) {
                    ErrorMessage = "Validation error"
                });
            }

            var benchmarkSchool = await InstantiateBenchmarkSchoolOrFedAsync(urn);

            var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromBicComparisonCriteria(benchmarkSchool.LatestYearFinancialData, bicCriteria);

            ComparisonResult comparisonResult = null;

            if (!isEditedCriteria)
            {
                comparisonResult = _bicComparisonResultCachingService.GetBicComparisonResultByUrn(urn);
            }

            if (comparisonResult is null)
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
                      ProgressScore = (benchmarkSchool as SchoolViewModel).BicProgressScoreType== BicProgressScoreType.P8 ?
                        decimal.Round(schoolData.Progress8Measure.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero)
                        : decimal.Round(schoolData.Ks2Progress.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero)

                };

                 _schoolBenchmarkListService.TryAddSchoolToBenchmarkList(benchmarkSchoolToAdd);

            }

            _schoolBenchmarkListService.TryAddSchoolToBenchmarkList((benchmarkSchool as SchoolViewModel));
            await _schoolBenchmarkListService.SetSchoolAsDefaultAsync(urn);

            return await Index(urn, null, comparisonResult.BenchmarkCriteria, bicCriteria, ComparisonType.BestInClass, ComparisonListLimit.DEFAULT, benchmarkSchool.LatestYearFinancialData, bicCriteria.EstablishmentType);
        }

        [HttpGet]
        public async Task<ActionResult> GenerateFromEfficiencyMetricsTop(long urn)
        {
            var defaultSchool = await _efficiencyMetricDataService.GetSchoolDataObjectByUrnAsync(urn);
            var neighbourSchools = (await _efficiencyMetricDataService.GetSchoolDataObjectByUrnAsync(urn)).Neighbours;
            var topNeighbourSchoolURNs = neighbourSchools.OrderBy(s => s.Rank).Take(15).Select(n => n.Urn).ToList();

            _schoolBenchmarkListService.ClearSchoolBenchmarkList();

            await _schoolBenchmarkListService.AddSchoolsToBenchmarkListAsync(ComparisonType.EfficiencyTop, topNeighbourSchoolURNs);

            await _schoolBenchmarkListService.SetSchoolAsDefaultAsync(urn);

            return await Index(urn, null, null, comparisonType: ComparisonType.EfficiencyTop);            
        }

        [HttpPost]
        public async Task<ActionResult> GenerateFromEfficiencyMetricsManual(long urn, string neighbourURNs)
        {

            var neighbourUrnList = neighbourURNs?.Split(',').Select(u => long.Parse(u)).ToList();

            _schoolBenchmarkListService.ClearSchoolBenchmarkList();

            await _schoolBenchmarkListService.AddSchoolsToBenchmarkListAsync(ComparisonType.EfficiencyManual, neighbourUrnList);

            await _schoolBenchmarkListService.SetSchoolAsDefaultAsync(urn);

            return await Index(urn, null, null, comparisonType: ComparisonType.EfficiencyManual);
        }

        [HttpGet]
        public async Task<ActionResult> GenerateNewFromAdvancedCriteria()
        {
            long? urn;
            BenchmarkCriteria usedCriteria;
            try
            {
                urn = (long?)TempData["URN"];
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
        public async Task<ActionResult> GenerateFromAdvancedCriteria(BenchmarkCriteria criteria, EstablishmentType estType, int? lacode, long? urn, ComparisonArea areaType, 
            BenchmarkListOverwriteStrategy? overwriteStrategy, bool excludePartial = false)
        {
            if(criteria is null)
            {
                criteria = new BenchmarkCriteria();
            }

            criteria.LocalAuthorityCode = lacode;
            SchoolViewModel benchmarkSchoolVM;
            if (urn.HasValue)
            {
                benchmarkSchoolVM = await InstantiateBenchmarkSchoolOrFedAsync(urn.Value) as SchoolViewModel;
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
            long? urn,
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
            if (string.IsNullOrWhiteSpace(vm.SchoolComparisonList.HomeSchoolUrn) &&
                comparisonType == ComparisonType.Specials && urn.HasValue)
            {
                vm.SchoolComparisonList.HomeSchoolUrn = urn.Value.ToString();
                vm.SchoolComparisonList.HomeSchoolName = vm.SchoolComparisonList.BenchmarkSchools
                    .Where(b => b.Urn.Equals(vm.SchoolComparisonList.HomeSchoolUrn)).Select(b => b.Name)
                    .FirstOrDefault();
            }

            ViewBag.HomeSchoolId = vm.SchoolComparisonList.HomeSchoolUrn;
            ViewBag.EstablishmentType = vm.EstablishmentType;
            ViewBag.Financing = financing;
            ViewBag.ChartFormat = ChartFormat.Charts;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.BicComparisonOverallPhase = (comparisonSchools?.FirstOrDefault() as SchoolViewModel)?.OverallPhaseInFinancialSubmission;

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
            
            var usedCriteria = TempData["BenchmarkCriteria"] as BenchmarkCriteria;
            var comparisonType = TempData["ComparisonType"] as ComparisonType? ?? ComparisonType.Manual;
            var vm = new BenchmarkChartListViewModel(benchmarkCharts, null, chartGroups, comparisonType, usedCriteria, null, null, null, EstablishmentType.MAT, 
                EstablishmentType.MAT, null, null, academiesTerm, maintainedTerm, ComparisonArea.All, null, 0, ComparisonListLimit.DEFAULT,
                _trustBenchmarkListService.GetTrustBenchmarkList());

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = defaultUnitType;
            ViewBag.HomeSchoolId = vm.TrustComparisonList.DefaultTrustCompanyNo;
            ViewBag.EstablishmentType = vm.EstablishmentType;
            ViewBag.TrustFinancing = financing;
            ViewBag.ComparisonType = comparisonType;

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

        [HttpGet]
        public async Task<JsonResult> GetSchoolListFromSimpleCriteria(long id)
        {
            var benchmarkSchool = await InstantiateBenchmarkSchoolOrFedAsync(id);

            ComparisonResult comparisonResult;
            if (benchmarkSchool is SchoolViewModel && (benchmarkSchool as SchoolViewModel).OverallPhase == "Special")
            {
                var specialCriteria = new SpecialCriteria();
                specialCriteria.TopSenCriteria = new List<SenCriterion>();
                for (int i = 0; i < (benchmarkSchool as SchoolViewModel).TopSenCharacteristics.Count; i++)
                {
                    var senCriterion = new SenCriterion(i, (benchmarkSchool as SchoolViewModel).TopSenCharacteristics[i].Definition, (benchmarkSchool as SchoolViewModel).TopSenCharacteristics[i].DataName, (benchmarkSchool as SchoolViewModel).TopSenCharacteristics[i].Value);
                    specialCriteria.TopSenCriteria.Add(senCriterion);
                }

                var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromSpecialComparisonCriteria(benchmarkSchool.LatestYearFinancialData, specialCriteria);

                comparisonResult = await _comparisonService.GenerateBenchmarkListWithSpecialComparisonAsync(benchmarkCriteria, specialCriteria, benchmarkSchool.LatestYearFinancialData);
            }
            else
            {
                var simpleCriteria = new SimpleCriteria() { IncludeEal = true, IncludeFsm = true, IncludeSen = true };
                var benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromSimpleComparisonCriteria(benchmarkSchool.LatestYearFinancialData, simpleCriteria);
                comparisonResult = await _comparisonService.GenerateBenchmarkListWithSimpleComparisonAsync(benchmarkCriteria, benchmarkSchool.EstablishmentType, ComparisonListLimit.DEFAULT, simpleCriteria, benchmarkSchool.LatestYearFinancialData, false);

                if (comparisonResult.BenchmarkSchools.Count < 15)
                {
                    var openCriteria = new SimpleCriteria() { IncludeEal = false, IncludeFsm = false, IncludeSen = false };
                    benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromSimpleComparisonCriteria(benchmarkSchool.LatestYearFinancialData, openCriteria);
                    benchmarkCriteria.MinNoPupil = (benchmarkSchool.LatestYearFinancialData.PupilCount / 100 * 50);
                    benchmarkCriteria.MaxNoPupil = (benchmarkSchool.LatestYearFinancialData.PupilCount / 100 * 150);
                    comparisonResult = await _comparisonService.GenerateBenchmarkListWithSimpleComparisonAsync(benchmarkCriteria, EstablishmentType.All, ComparisonListLimit.DEFAULT, openCriteria, benchmarkSchool.LatestYearFinancialData, false);
                }
            }
            //For cases where school names are the same
            foreach (var school in comparisonResult.BenchmarkSchools.Where(s1 => comparisonResult.BenchmarkSchools.Any(s2 => s2.SchoolName == s1.SchoolName && s2.URN != s1.URN)))
            {
                school.SchoolName += " ";
            }

            return Json(new
            {
                count = comparisonResult.BenchmarkSchools.Count,
                schools = comparisonResult.BenchmarkSchools.Select(s => new { s.URN, s.SchoolName, s.FederationName, s.Type, s.FinanceType }),
                criteria = comparisonResult.BenchmarkCriteria
            }
            , JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<PartialViewResult> GetQCChart(
            long id,
            string chartName, 
            ChartGroupType chartGroup, 
            ChartFormat format,
            List<SchoolTrustFinancialDataObject> schools)
        {
            var comparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
            comparisonList.BenchmarkSchools = schools.Select(school => new BenchmarkSchoolModel(school)).ToList();
            var benchmarkChart = (await BuildSchoolBenchmarkChartAsync(id, comparisonList, TabType.Expenditure, chartGroup, chartName, UnitType.PerPupil, CentralFinancingType.Include)).FirstOrDefault();
            
            ViewBag.HomeSchoolId = id.ToString();
            ViewBag.ChartFormat = format;
            ViewBag.UnitType = UnitType.PerPupil;
            ViewBag.Financing = CentralFinancingType.Include;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.ComparisonType = ComparisonType.Basic;
            ViewBag.EstablishmentType = Enum.Parse(typeof(EstablishmentType), schools.First()?.FinanceType);

            if (format == ChartFormat.Charts)
            {
                return PartialView("Partials/QCDashboardChart", benchmarkChart);
            }
            else
            {
                return PartialView("Partials/QCChartTable", benchmarkChart);
            }
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

        private async Task<List<EstablishmentViewModelBase>> PopulateSchoolsListForComparisonTableAsync(SchoolComparisonListModel comparisonList)
        {
            var comparisonSchools = new List<EstablishmentViewModelBase>();

            foreach (var school in comparisonList.BenchmarkSchools)
            {
                var bmSchool = await InstantiateBenchmarkSchoolOrFedAsync(long.Parse(school.Urn));
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

        private async Task<List<ChartViewModel>> BuildSchoolBenchmarkChartAsync(TabType revGroup, ChartGroupType chartGroup, string chartName, UnitType? showValue, CentralFinancingType cFinancing)
        {
            var comparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();

            var establishmentType = DetectEstablishmentType(comparisonList);

            var benchmarkCharts = _benchmarkChartBuilder.Build(revGroup, chartGroup, establishmentType).Where(bc => bc.Name == chartName).ToList();

            var financialDataModels = await this.GetFinancialDataForSchoolsAsync(comparisonList.BenchmarkSchools, cFinancing);

            _fcService.PopulateBenchmarkChartsWithFinancialData(benchmarkCharts, financialDataModels, comparisonList.BenchmarkSchools, comparisonList.HomeSchoolUrn, showValue);
            
            return benchmarkCharts;
        }

        private async Task<List<ChartViewModel>> BuildSchoolBenchmarkChartAsync(long comparisonSchoolId, SchoolComparisonListModel comparisonList, TabType revGroup, ChartGroupType chartGroup, string chartName, UnitType? showValue, CentralFinancingType cFinancing)
        {
            var establishmentType = DetectEstablishmentType(comparisonList);

            var benchmarkCharts = _benchmarkChartBuilder.Build(revGroup, chartGroup, establishmentType).Where(bc => bc.Name == chartName).ToList();

            var financialDataModels = await this.GetFinancialDataForSchoolsAsync(comparisonList.BenchmarkSchools, cFinancing);

            _fcService.PopulateBenchmarkChartsWithFinancialData(benchmarkCharts, financialDataModels, comparisonList.BenchmarkSchools, comparisonSchoolId.ToString(), showValue);
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

        private async Task<EstablishmentViewModelBase> InstantiateBenchmarkSchoolOrFedAsync(long urn)
        {
            var contextData = await _contextDataService.GetSchoolDataObjectByUrnAsync(urn);
            EstablishmentViewModelBase benchmarkSchool;
            if (contextData.IsFederation) { 
                benchmarkSchool = new FederationViewModel(contextData, _schoolBenchmarkListService.GetSchoolBenchmarkList());
            }
            else
            {
                benchmarkSchool = new SchoolViewModel(contextData, _schoolBenchmarkListService.GetSchoolBenchmarkList());
            }
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
                case TabType.Income:
                    unitType = showValue == UnitType.PercentageOfTotalIncome || showValue == UnitType.PercentageOfTotalExpenditure || showValue == UnitType.PerPupil || showValue == UnitType.PerTeacher ? showValue : UnitType.AbsoluteMoney;
                    break;
                case TabType.Expenditure:
                    unitType = showValue == UnitType.PercentageOfTotalIncome || showValue == UnitType.PercentageOfTotalExpenditure || showValue == UnitType.PerPupil || showValue == UnitType.PerTeacher ? showValue : UnitType.AbsoluteMoney;
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
                default:
                    chartGroup = ChartGroupType.All;
                    break;
            }

            return chartGroup;
        }

    }
}