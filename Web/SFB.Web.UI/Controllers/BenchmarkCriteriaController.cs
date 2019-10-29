using System;
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
using SFB.Web.UI.Services;
using SFB.Web.ApplicationCore.Services.Comparison;
using SFB.Web.UI.Attributes;
using SFB.Web.ApplicationCore.Helpers.Enums;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class BenchmarkCriteriaController : Controller
    {
        private readonly IFinancialDataService _financialDataService;
        private readonly IContextDataService _contextDataService;
        private readonly ILocalAuthoritiesService _laService;
        private readonly ILaSearchService _laSearchService;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;
        private readonly IComparisonService _comparisonService;

        public BenchmarkCriteriaController(ILocalAuthoritiesService laService, IFinancialDataService financialDataService, 
            IContextDataService contextDataService, ILaSearchService laSearchService, IBenchmarkBasketCookieManager benchmarkBasketCookieManager, IComparisonService comparisonService)
        {
            _financialDataService = financialDataService;
            _laService = laService;
            _contextDataService = contextDataService;
            _laSearchService = laSearchService;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
            _comparisonService = comparisonService;
        }

        /// <summary>
        /// Step 0
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        public ViewResult ComparisonStrategy(int urn)
        {
            ViewBag.URN = urn;

            var benchmarkSchool = InstantiateBenchmarkSchool(urn);

            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.SetDefault,            
            new BenchmarkSchoolModel()
            {
                Name = benchmarkSchool.Name,
                Urn = benchmarkSchool.Id.ToString(),
                Type = benchmarkSchool.Type,
                EstabType = benchmarkSchool.EstablishmentType.ToString()
            });
            
            benchmarkSchool.ComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
                        
            return View(benchmarkSchool);
        }

        public ActionResult StepOne(int urn, ComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case ComparisonType.BestInClass:
                    return HighestProgressSchoolsBenchmarking(urn);
                case ComparisonType.Basic:
                    return SelectBasketSize(urn, comparisonType);
                case ComparisonType.Manual:
                    return RedirectToAction("Index", "ManualComparison");
                case ComparisonType.Advanced:
                default:
                    return SelectSchoolType(urn, comparisonType, null, null);
            }
        }

        /// <summary>
        /// Step 1 - Basic
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public ViewResult SelectBasketSize(int urn, ComparisonType comparisonType)
        {
            ViewBag.URN = urn;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.BasketSize = ComparisonListLimit.DEFAULT;

            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());
            return View("SelectBasketSize", benchmarkSchool);
        }

        /// <summary>
        /// Step 1 - Advanced
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="comparisonType"></param>
        /// <param name="estType"></param>
        /// <returns></returns>
        public ViewResult SelectSchoolType(int urn, ComparisonType comparisonType, EstablishmentType? estType, int? basketSize)
        {
            ViewBag.URN = urn;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.EstType = estType;
            ViewBag.BasketSize = basketSize;

            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());

            if ((ViewBag.ComparisonType == ComparisonType.Basic) && ((!basketSize.HasValue) || (basketSize.Value < 5 || basketSize.Value > 30)))
            {
                benchmarkSchool.ErrorMessage = "Please enter a number between 5 and 30";
                return View("SelectBasketSize", benchmarkSchool);
            }

            return View("SelectSchoolType", benchmarkSchool);
        }

        /// <summary>
        /// Step 2 - Advanced
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="comparisonType"></param>
        /// <param name="estType"></param>
        /// <returns></returns>
        public ViewResult ChooseRegion(int urn, ComparisonType comparisonType, EstablishmentType estType, bool excludePartial = false)
        {
            ViewBag.URN = urn;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.EstType = estType;
            ViewBag.AreaType = ComparisonArea.All;
            ViewBag.Authorities = _laService.GetLocalAuthorities();
            ViewBag.ExcludePartial = excludePartial.ToString();

            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());
            return View("ChooseRegion", benchmarkSchool);
        }

        /// <summary>
        /// Step 3 - Advanced
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="estType"></param>
        /// <param name="comparisonType"></param>
        /// <param name="areaType"></param>
        /// <param name="lacode"></param>
        /// <returns></returns>
        public ActionResult AdvancedCharacteristics(int urn, ComparisonType comparisonType, EstablishmentType estType, ComparisonArea? areaType, int? lacode,
            string laNameText, BenchmarkCriteria AdvancedCriteria, bool excludePartial = false)
        {
            if (areaType == ComparisonArea.LaName && !string.IsNullOrEmpty(laNameText) && lacode == null)
            {
                var exactLaMatch = _laSearchService.SearchExactMatch(laNameText);
                if (exactLaMatch != null)
                {
                    lacode = Int32.Parse(exactLaMatch.id);
                }
            }

            ViewBag.URN = urn;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.EstType = estType;
            ViewBag.EstTypeDescription = estType.GetDescription();
            ViewBag.AreaType = areaType;
            ViewBag.AreaTypeDescription = lacode == null ? "All of England" : string.IsNullOrEmpty(laNameText) ? _laService.GetLaName(lacode.ToString()) : laNameText;
            ViewBag.LaCode = lacode;
            ViewBag.ExcludePartial = excludePartial.ToString();

            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());

            var schoolsLatestFinancialDataModel = _financialDataService.GetSchoolsLatestFinancialDataModel(benchmarkSchool.Id, benchmarkSchool.EstablishmentType);
            benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel> { schoolsLatestFinancialDataModel };

            if (!IsAreaFieldsValid(areaType, lacode, benchmarkSchool))
            {
                ViewBag.Authorities = _laService.GetLocalAuthorities();
                return View("ChooseRegion", benchmarkSchool);
            }

            var schoolCharsVM = new SchoolCharacteristicsViewModel(benchmarkSchool, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), AdvancedCriteria);
            return View(schoolCharsVM);
        }

        /// <summary>
        /// Step 1 - Best in class interstitial
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        public ActionResult HighestProgressSchoolsBenchmarking(int urn)
        {
            ViewBag.URN = urn;
            return View("HighestProgressSchoolsBenchmarking");
        }

        /// <summary>
        /// Step 4 - Best in class edit
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        public ActionResult BestInClassCharacteristics(int urn, BestInClassCriteria bicCriteria)
        {                                
            var benchmarkSchool = InstantiateBenchmarkSchool(urn);

            var schoolCharsVM = new BestInClassCharacteristicsViewModel(benchmarkSchool, bicCriteria);

            return View("BestInClassCharacteristics", schoolCharsVM);
        }

        /// <summary>
        /// Step 3 - Simple
        /// </summary>
        /// <param name="urn"></param>
        /// <param name="comparisonType"></param>
        /// <param name="basketSize"></param>
        /// <param name="estType"></param>
        /// <param name="simpleCriteria"></param>
        /// <returns></returns>
        public ActionResult SimpleCharacteristics(int urn, ComparisonType comparisonType, int basketSize, EstablishmentType estType, SimpleCriteria SimpleCriteria)
        {
            ViewBag.URN = urn;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.BasketSize = basketSize;
            ViewBag.EstType = estType;

            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());

            var schoolCharsVM = new SimpleCharacteristicsViewModel(benchmarkSchool, SimpleCriteria);
            return View(schoolCharsVM);
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
        public ActionResult OverwriteStrategy(int urn, ComparisonType comparisonType, EstablishmentType estType, BenchmarkCriteriaVM criteria, 
            ComparisonArea areaType, int? lacode, string schoolName, bool excludePartial = false)
        {
            ViewBag.URN = urn;
            ViewBag.HomeSchoolName = schoolName;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.EstType = estType;
            ViewBag.AreaType = areaType;
            ViewBag.LaCode = lacode;
            ViewBag.ExcludePartial = excludePartial.ToString();

            var benchmarkList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

            if (!ModelState.IsValid)
            {
                var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), benchmarkList);                               
                var schoolsLatestFinancialDataModel = _financialDataService.GetSchoolsLatestFinancialDataModel(benchmarkSchool.Id, benchmarkSchool.EstablishmentType);
                benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel> { schoolsLatestFinancialDataModel };
                var schoolCharsVM = new SchoolCharacteristicsViewModel(benchmarkSchool, benchmarkList, new BenchmarkCriteria());
                schoolCharsVM.ErrorMessage = "Validation Error";
                return View("AdvancedCharacteristics", schoolCharsVM);
            }

            if ((benchmarkList.BenchmarkSchools.Count > 1) 
                || (benchmarkList.BenchmarkSchools.Count == 1 && benchmarkList.BenchmarkSchools[0].Urn != benchmarkList.HomeSchoolUrn))
            {
                criteria.ComparisonList = benchmarkList;
                if(criteria.AdvancedCriteria == null)
                {
                    criteria.AdvancedCriteria = new BenchmarkCriteria();
                }
                return View(criteria);
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

        private bool IsAreaFieldsValid(ComparisonArea? areaType, int? lacode, SchoolViewModel benchmarkSchool)
        {
            switch (areaType)
            {
                case ComparisonArea.LaCode:
                    if (lacode == null)
                    {
                        benchmarkSchool.ErrorMessage = "Please enter a valid Local authority code";
                    }
                    break;
                case ComparisonArea.LaName:
                    if (lacode == null)
                    {
                        benchmarkSchool.ErrorMessage = "Please select a local authority from the auto-completed list";
                    }
                    break;
                case null:
                    benchmarkSchool.ErrorMessage = "Please select an area";
                    break;
            }

            return !benchmarkSchool.HasError();
        }

        private SchoolViewModel InstantiateBenchmarkSchool(int urn)
        {
            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());
            var schoolsLatestFinancialDataModel = _financialDataService.GetSchoolsLatestFinancialDataModel(benchmarkSchool.Id, benchmarkSchool.EstablishmentType);
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
    }
}