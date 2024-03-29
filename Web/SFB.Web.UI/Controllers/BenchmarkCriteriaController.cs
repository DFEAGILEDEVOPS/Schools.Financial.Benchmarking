﻿using System;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Services.Comparison;
using SFB.Web.UI.Attributes;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Services.LocalAuthorities;
using SFB.Web.UI.Helpers.Constants;
using System.Text.RegularExpressions;
using SFB.Web.UI.Services;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class BenchmarkCriteriaController : Controller
    {
        private readonly IFinancialDataService _financialDataService;
        private readonly IContextDataService _contextDataService;
        private readonly ILocalAuthoritiesService _laService;
        private readonly ILaSearchService _laSearchService;
        private readonly ISchoolBenchmarkListService _benchmarkBasketService;
        private readonly IComparisonService _comparisonService;
        private readonly IValidationService _valService;

        public BenchmarkCriteriaController(
            ILocalAuthoritiesService laService, 
            IFinancialDataService financialDataService, 
            IContextDataService contextDataService, 
            ILaSearchService laSearchService,
            ISchoolBenchmarkListService benchmarkBasketService, 
            IComparisonService comparisonService,
            IValidationService valService)
        {
            _financialDataService = financialDataService;
            _laService = laService;
            _contextDataService = contextDataService;
            _laSearchService = laSearchService;
            _benchmarkBasketService = benchmarkBasketService;
            _comparisonService = comparisonService;
            _valService = valService;
        }

        /// <summary>
        /// Step 0
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        public async Task<ViewResult> ComparisonStrategy(long urn)
        {
            ViewBag.URN = urn;

            var benchmarkSchool = await InstantiateBenchmarkSchoolAsync(urn);

            _benchmarkBasketService.SetSchoolAsDefault(benchmarkSchool);            
            
            benchmarkSchool.ComparisonList = _benchmarkBasketService.GetSchoolBenchmarkList();
                        
            return View(benchmarkSchool);
        }

        private async Task<ActionResult> ErrorViewForComparisonStrategy(long urn)
        {
            ViewBag.URN = urn;

            var benchmarkSchool = await InstantiateBenchmarkSchoolAsync(urn);

            _benchmarkBasketService.SetSchoolAsDefault(benchmarkSchool);

            benchmarkSchool.ComparisonList = _benchmarkBasketService.GetSchoolBenchmarkList();

            benchmarkSchool.ErrorMessage = ErrorMessages.SelectComparisonType;

            return View("ComparisonStrategy", benchmarkSchool);
        
        }

        public async Task<ActionResult> StepOne(long? urn, long? fuid, ComparisonType? comparisonType, bool similarPupils = false)
        {
            switch (comparisonType)
            {
                case ComparisonType.BestInClass:
                    return HighestProgressSchoolsBenchmarking(urn.Value);                    
                case ComparisonType.Manual:
                    if (urn.HasValue)
                    {
                        return RedirectToAction("Index", "ManualComparison");
                    }
                    else
                    {
                        return RedirectToAction("WithoutBaseSchool", "ManualComparison");
                    }
                case ComparisonType.Basic:
                case ComparisonType.FederationBasic:
                case ComparisonType.Advanced:
                    return await SelectSchoolType(urn, fuid,comparisonType.Value, null, null);
                case ComparisonType.Specials:
                    return await HowWeCalculateSpecials(urn.GetValueOrDefault(), similarPupils);
                case null:
                default:
                    if (urn.HasValue) {
                        return await ErrorViewForComparisonStrategy(urn.Value);
                    }
                    else
                    {
                        return ErrorViewForHomePage(SearchTypes.COMPARE_WITHOUT_DEFAULT_SCHOOL, ErrorMessages.SelectComparisonType, _benchmarkBasketService.GetSchoolBenchmarkList());
                    }

            }
        }

        private async Task<ViewResult> HowWeCalculateSpecials(long urn, bool similarPupils)
        {
            ViewBag.URN = urn;
            ViewBag.SimilarPupils = similarPupils;

            var benchmarkSchool = await InstantiateBenchmarkSchoolAsync(urn);

            return View("HowWeCalculateSpecials", benchmarkSchool);           
        }

        /// <summary>
        /// Step 1 - Basic
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public async Task<ViewResult> SelectBasketSize(long urn, ComparisonType comparisonType)
        {
            ViewBag.URN = urn;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.BasketSize = ComparisonListLimit.DEFAULT;

            var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), _benchmarkBasketService.GetSchoolBenchmarkList());
            return View("SelectBasketSize", benchmarkSchool);
        }

        /// <summary>
        /// Step 1 - Advanced
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="comparisonType"></param>
        /// <param name="estType"></param>
        /// <returns></returns>
        public async Task<ViewResult> SelectSchoolType(long? urn, long? fuid, ComparisonType comparisonType, EstablishmentType? estType, int? basketSize)
        {
            ViewBag.URN = urn;
            ViewBag.Fuid = fuid;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.EstType = estType;
            ViewBag.BasketSize = basketSize;

            if (fuid.HasValue)
            {
                var benchmarkSchool = new FederationViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(fuid.Value), _benchmarkBasketService.GetSchoolBenchmarkList());
                _benchmarkBasketService.SetFederationAsDefault(benchmarkSchool);
                return View("SelectSchoolType", benchmarkSchool);
            }
            else if (urn.HasValue)
            {
                var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn.Value), _benchmarkBasketService.GetSchoolBenchmarkList());

                return View("SelectSchoolType", benchmarkSchool);
            }
            else
            {
                _benchmarkBasketService.UnsetDefaultSchool();
                return View("SelectSchoolType", new SchoolViewModelWithNoDefaultSchool());
            }
        }

        /// <summary>
        /// Step 2 - Advanced
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="comparisonType"></param>
        /// <param name="estType"></param>
        /// <returns></returns>
        public async Task<ViewResult> ChooseRegion(long? urn, ComparisonType comparisonType, EstablishmentType? estType, bool excludePartial = false)
        {
            ViewBag.URN = urn;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.EstType = estType;
            //ViewBag.AreaType = ComparisonArea.All;
            ViewBag.Authorities = _laService.GetLocalAuthorities();
            ViewBag.ExcludePartial = excludePartial.ToString();

            if (urn.HasValue)
            {
                var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn.Value), _benchmarkBasketService.GetSchoolBenchmarkList());

                if (estType.HasValue)
                {
                    return View("ChooseRegion", benchmarkSchool);
                }

                benchmarkSchool.ErrorMessage = ErrorMessages.SelectSchoolType;

                ViewBag.URN = urn;
                ViewBag.ComparisonType = comparisonType;

                return View("SelectSchoolType", benchmarkSchool);

            }
            else
            {
                if (estType.HasValue)
                {
                    return View("ChooseRegion", new SchoolViewModelWithNoDefaultSchool());
                }

                var bs = new SchoolViewModelWithNoDefaultSchool
                {
                    ErrorMessage = ErrorMessages.SelectSchoolType
                };

                return View("SelectSchoolType", bs);

            }
        }

        /// <summary>
        /// Step 3 - Advanced
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="estType"></param>
        /// <param name="comparisonType"></param>
        /// <param name="areaType"></param>
        /// <param name="laCodeName"></param>
        /// <returns></returns>
        public async Task<ActionResult> AdvancedCharacteristics(
            long? urn, 
            ComparisonType comparisonType, 
            EstablishmentType estType, 
            ComparisonArea? areaType, 
            string laCodeName,
            BenchmarkCriteria AdvancedCriteria, 
            bool excludePartial = false,
            int? laCode = null)
        {

            if (!areaType.HasValue)
            {
                SchoolViewModel vm = null;
                if (urn.HasValue)
                {
                    vm = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn.Value), _benchmarkBasketService.GetSchoolBenchmarkList());
                }
                else
                {
                    vm = new SchoolViewModelWithNoDefaultSchool();
                }
                vm.ErrorMessage = ErrorMessages.SelectAreaType;
                ViewBag.Authorities = _laService.GetLocalAuthorities();
                ViewBag.URN = urn;
                ViewBag.ComparisonType = comparisonType;
                ViewBag.EstType = estType;
                ViewBag.AreaType = areaType;
                ViewBag.ExcludePartial = excludePartial.ToString();
                return View("ChooseRegion", vm);
            }


            if (areaType == ComparisonArea.LaCodeName && laCode == null)
            {
                string errorMessage = _valService.ValidateLaCodeNameParameter(laCodeName);
                
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (IsNumeric(laCodeName))
                    {
                        laCode = int.Parse(laCodeName);
                        if (!_laSearchService.LaCodesContain(laCode.Value))
                        {
                            errorMessage = SearchErrorMessages.NO_LA_RESULTS;
                        }
                    }
                    else
                    {
                        var exactLaMatch = _laSearchService.SearchExactMatch(laCodeName);
                        if (exactLaMatch == null)
                        {
                            errorMessage = SearchErrorMessages.NO_LA_RESULTS;
                        }
                        else
                        {
                            laCode = int.Parse(exactLaMatch.Id);
                        }
                    }
                }

                if (errorMessage != null)
                {
                    SchoolViewModel vm = null;
                    if (urn.HasValue)
                    {
                        vm = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn.Value), _benchmarkBasketService.GetSchoolBenchmarkList());
                    }
                    else
                    {
                        vm = new SchoolViewModelWithNoDefaultSchool();
                    }
                    vm.ErrorMessage = errorMessage;
                    ViewBag.Authorities = _laService.GetLocalAuthorities();
                    ViewBag.URN = urn;
                    ViewBag.ComparisonType = comparisonType;
                    ViewBag.EstType = estType;
                    ViewBag.AreaType = areaType;
                    ViewBag.ExcludePartial = excludePartial.ToString();
                    return View("ChooseRegion", vm);
                }
            }

            SchoolViewModel benchmarkSchoolVM;
            if (urn.HasValue)
            {
                benchmarkSchoolVM = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn.Value), _benchmarkBasketService.GetSchoolBenchmarkList());

                var schoolsLatestFinancialDataModel = await _financialDataService.GetSchoolsLatestFinancialDataModelAsync(benchmarkSchoolVM.Id, benchmarkSchoolVM.EstablishmentType);
                benchmarkSchoolVM.HistoricalFinancialDataModels = new List<FinancialDataModel> { schoolsLatestFinancialDataModel };
            }
            else
            {
                benchmarkSchoolVM = new SchoolViewModelWithNoDefaultSchool();
            }

            var schoolCharsVM = new SchoolCharacteristicsViewModel(benchmarkSchoolVM, _benchmarkBasketService.GetSchoolBenchmarkList(), AdvancedCriteria);

            ViewBag.URN = urn;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.EstType = estType;
            ViewBag.EstTypeDescription = estType.GetDescription();
            ViewBag.ExcludePartial = excludePartial.ToString();
            ViewBag.AreaType = areaType;
            ViewBag.AreaTypeDescription = areaType == ComparisonArea.All ? "All of England" : _laService.GetLaName(laCode.ToString());
            ViewBag.LaCode = laCode;

            return View(schoolCharsVM);
        }

        /// <summary>
        /// Step 1 - Best in class interstitial
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        public ActionResult HighestProgressSchoolsBenchmarking(long urn)
        {
            ViewBag.URN = urn;
            return View("HighestProgressSchoolsBenchmarking");
        }

        /// <summary>
        /// Step 4 - Best in class edit
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BestInClassCharacteristics(long urn, BestInClassCriteria bicCriteria)
        {
            ViewBag.ModelState = ModelState;

            var benchmarkSchool = await InstantiateBenchmarkSchoolAsync(urn);

            var schoolCharsVM = new BestInClassCharacteristicsViewModel(benchmarkSchool, bicCriteria);

            return View("BestInClassCharacteristics", schoolCharsVM);
        }

        /// <summary>
        /// Step 3 - Simple
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="fuid"></param>
        /// <param name="comparisonType"></param>
        /// <param name="estType"></param>
        /// <param name="simpleCriteria"></param>
        /// <returns></returns>
        public async Task<ActionResult> SimpleCharacteristics(long? urn, long? fuid, ComparisonType comparisonType, EstablishmentType? estType, SimpleCriteria SimpleCriteria)
        {
            if (estType.HasValue)
            {
                ViewBag.URN = urn;
                ViewBag.Fuid = fuid;
                ViewBag.ComparisonType = comparisonType;
                ViewBag.EstType = estType;

                EstablishmentViewModelBase benchmarkSchool;

                if (fuid.HasValue) 
                {
                    benchmarkSchool = new FederationViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(fuid.Value), _benchmarkBasketService.GetSchoolBenchmarkList());
                }
                else
                {
                    benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn.GetValueOrDefault()), _benchmarkBasketService.GetSchoolBenchmarkList());
                }

                var schoolCharsVM = new SimpleCharacteristicsViewModel(benchmarkSchool, SimpleCriteria);
                return View(schoolCharsVM);
            }
            else
            {
                ViewBag.URN = urn;
                ViewBag.Fuid = fuid;
                ViewBag.ComparisonType = comparisonType;

                EstablishmentViewModelBase benchmarkSchool;
                if (fuid.HasValue)
                {
                    benchmarkSchool = new FederationViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(fuid.Value), _benchmarkBasketService.GetSchoolBenchmarkList());
                }
                else
                {
                    benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn.GetValueOrDefault()), _benchmarkBasketService.GetSchoolBenchmarkList());
                }

                benchmarkSchool.ErrorMessage = ErrorMessages.SelectSchoolType;

                return View("SelectSchoolType", benchmarkSchool);
            }
        }

        /// <summary>
        /// Step 4 - Advanced
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="comparisonType"></param>
        /// <param name="estType"></param>
        /// <param name="criteria"></param>
        /// <param name="areaType"></param>
        /// <param name="lacode"></param>
        /// <param name="schoolName"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OverwriteStrategy(long? urn, ComparisonType comparisonType, EstablishmentType estType, BenchmarkCriteriaVM criteria, 
            ComparisonArea areaType, int? lacode, string schoolName, int basketCount, bool excludePartial = false)
        {
            ViewBag.URN = urn;
            ViewBag.HomeSchoolName = schoolName;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.EstType = estType;
            ViewBag.AreaType = areaType;
            ViewBag.LaCode = lacode;
            ViewBag.ExcludePartial = excludePartial.ToString();

            var benchmarkList = _benchmarkBasketService.GetSchoolBenchmarkList();

            if (!ModelState.IsValid)
            {
                if (urn.HasValue)
                {
                    var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn.Value), benchmarkList);
                    var schoolsLatestFinancialDataModel = await _financialDataService.GetSchoolsLatestFinancialDataModelAsync(benchmarkSchool.Id, benchmarkSchool.EstablishmentType);
                    benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel> { schoolsLatestFinancialDataModel };
                    var schoolCharsVM = new SchoolCharacteristicsViewModel(benchmarkSchool, benchmarkList, new BenchmarkCriteria());
                    schoolCharsVM.ErrorMessage = "Validation Error";
                    return View("AdvancedCharacteristics", schoolCharsVM);
                }
                else
                {
                    var schoolCharsVM = new SchoolCharacteristicsViewModel(new SchoolViewModelWithNoDefaultSchool(), benchmarkList, new BenchmarkCriteria());
                    schoolCharsVM.ErrorMessage = "Validation Error";
                    return View("AdvancedCharacteristics", schoolCharsVM);
                }
            }

            if ((benchmarkList.BenchmarkSchools.Count > 1) || 
                (benchmarkList.BenchmarkSchools.Count == 1 && benchmarkList.BenchmarkSchools[0].Urn != benchmarkList.HomeSchoolUrn))
            {
                criteria.ComparisonList = benchmarkList;
                if(criteria.AdvancedCriteria == null)
                {
                    criteria.AdvancedCriteria = new BenchmarkCriteria();
                }
                if (benchmarkList.BenchmarkSchools.Count + basketCount > ComparisonListLimit.LIMIT)
                {
                    return View("OverwriteReplace", criteria);
                }
                else
                {
                    return View(criteria);
                }
            }
            else
            {
                TempData["URN"] = urn;
                TempData["BenchmarkCriteria"] = criteria.AdvancedCriteria;
                TempData["EstType"] = estType;
                TempData["AreaType"] = areaType;
                TempData["LaCode"] = lacode;
                TempData["ExcludePartial"] = excludePartial;

                return RedirectToAction("GenerateNewFromAdvancedCriteria", "BenchmarkCharts");
            }
        }

        public async Task<int> GenerateCountFromAdvancedCriteria(BenchmarkCriteriaVM criteria, EstablishmentType estType, int? lacode, bool excludePartial = false)
        {
            if (!ModelState.IsValid)
            {
                return 0;
            }

            if (criteria.AdvancedCriteria == null)
            {
                criteria.AdvancedCriteria = new BenchmarkCriteria();
            }
            criteria.AdvancedCriteria.LocalAuthorityCode = lacode;
            var result = await _financialDataService.SearchSchoolsCountByCriteriaAsync(criteria.AdvancedCriteria, estType, excludePartial);
            return result;
        }

        private async Task<SchoolViewModel> InstantiateBenchmarkSchoolAsync(long urn)
        {
            var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), _benchmarkBasketService.GetSchoolBenchmarkList());
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
        private bool IsNumeric(string field) => field != null ? Regex.IsMatch(field, @"^\d+$") : false;
        private ActionResult ErrorViewForHomePage(string searchType, string errorMessage, SchoolComparisonListModel schoolComparisonList)
        {
            var searchVM = new SearchViewModel(schoolComparisonList, searchType)
            {
                SearchType = searchType,
                ErrorMessage = errorMessage,
                Authorities = _laService.GetLocalAuthorities()
            };

            return View("~/Views/Home/index.cshtml", searchVM);
        }
    }
}