using SFB.Web.Domain.Services;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFB.Web.Common;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services.DataAccess;

namespace SFB.Web.UI.Controllers
{
    public class BenchmarkCriteriaController : BaseController
    {
        private readonly IFinancialDataService _financialDataService;
        private readonly IContextDataService _contextDataService;
        private readonly ILocalAuthoritiesService _laService;
        
        public BenchmarkCriteriaController(ILocalAuthoritiesService laService, IFinancialDataService financialDataService, IContextDataService contextDataService)
        {
            _financialDataService = financialDataService;
            _laService = laService;
            _contextDataService = contextDataService;
        }

        /// <summary>
        /// Step 0
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        public ViewResult ComparisonStrategy(int urn)
        {
            ViewBag.URN = urn;

            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolByUrn(urn.ToString()), null);

            var cookie = base.UpdateSchoolComparisonListCookie(CompareActions.MAKE_DEFAULT_BENCHMARK,
            new BenchmarkSchoolViewModel()
            {
                Name = benchmarkSchool.Name,
                Urn = benchmarkSchool.Id,
                Type = benchmarkSchool.Type,
                FinancialType = benchmarkSchool.FinancialType.ToString()
            });
            Response.Cookies.Add(cookie);

            benchmarkSchool.ComparisonList = base.ExtractSchoolComparisonListFromCookie();

            return View(benchmarkSchool);
        }

        public ViewResult StepOne(int urn, ComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case ComparisonType.Basic:
                    return SelectBasketSize(urn, comparisonType);
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

            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolByUrn(urn.ToString()), base.ExtractSchoolComparisonListFromCookie());
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

            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolByUrn(urn.ToString()), base.ExtractSchoolComparisonListFromCookie());

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
        public ViewResult ChooseRegion(int urn, ComparisonType comparisonType, EstablishmentType estType)
        {
            ViewBag.URN = urn;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.EstType = estType;
            ViewBag.AreaType = ComparisonArea.All;
            ViewBag.Authorities = _laService.GetLocalAuthorities();

            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolByUrn(urn.ToString()), base.ExtractSchoolComparisonListFromCookie());
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
        public ActionResult AdvancedCharacteristics(string urn, ComparisonType comparisonType, EstablishmentType estType, ComparisonArea? areaType, int? lacode,
            BenchmarkCriteria AdvancedCriteria)
        {
            ViewBag.URN = urn;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.EstType = estType;
            ViewBag.AreaType = areaType;
            ViewBag.LaCode = lacode;

            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolByUrn(urn), base.ExtractSchoolComparisonListFromCookie());
            var latestYear = _financialDataService.GetLatestDataYearPerSchoolType(benchmarkSchool.FinancialType);
            var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
            var document = _financialDataService.GetSchoolDataDocument(urn, term, benchmarkSchool.FinancialType);
            benchmarkSchool.HistoricalSchoolDataModels = new List<SchoolDataModel> { new SchoolDataModel(urn, term, document, benchmarkSchool.FinancialType) };
            
            if (!IsAreaFieldsValid(areaType, lacode, benchmarkSchool))
            {
                ViewBag.Authorities = _laService.GetLocalAuthorities();
                return View("ChooseRegion", benchmarkSchool);
            }

            var schoolCharsVM = new SchoolCharacteristicsViewModel(benchmarkSchool, base.ExtractSchoolComparisonListFromCookie(), AdvancedCriteria);
            return View(schoolCharsVM);
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

            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolByUrn(urn.ToString()), base.ExtractSchoolComparisonListFromCookie());

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
        /// <returns></returns>
        public ActionResult OverwriteStrategy(string urn, ComparisonType comparisonType, EstablishmentType estType, BenchmarkCriteriaVM criteria, ComparisonArea areaType, int? lacode)
        {
            ViewBag.URN = urn;
            ViewBag.ComparisonType = comparisonType;
            ViewBag.EstType = estType;
            ViewBag.AreaType = areaType;
            ViewBag.LaCode = lacode;

            var benchmarkList = base.ExtractSchoolComparisonListFromCookie();

            if (!ModelState.IsValid)
            {
                var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolByUrn(urn), benchmarkList);
                var latestYear = _financialDataService.GetLatestDataYearPerSchoolType(benchmarkSchool.FinancialType);
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
                var document = _financialDataService.GetSchoolDataDocument(urn, term, benchmarkSchool.FinancialType);
                benchmarkSchool.HistoricalSchoolDataModels = new List<SchoolDataModel> { new SchoolDataModel(urn, term, document, benchmarkSchool.FinancialType) };
                var schoolCharsVM = new SchoolCharacteristicsViewModel(benchmarkSchool, benchmarkList, new BenchmarkCriteria());
                schoolCharsVM.ErrorMessage = "Validation Error";
                return View("AdvancedCharacteristics", schoolCharsVM);
            }

            if ((benchmarkList.BenchmarkSchools.Count > 1) 
                || (benchmarkList.BenchmarkSchools.Count == 1 && benchmarkList.BenchmarkSchools[0].Urn != benchmarkList.HomeSchoolUrn))
            {
                criteria.ComparisonList = benchmarkList;
                return View(criteria);
            }
            else
            {
                TempData["URN"] = urn;
                TempData["BenchmarkCriteria"] = criteria.AdvancedCriteria;
                TempData["EstType"] = estType;
                TempData["AreaType"] = areaType;
                TempData["LaCode"] = lacode;

                return RedirectToAction("GenerateNewFromAdvancedCriteria", "BenchmarkCharts");
            }
        }
      
        public async Task<int> GenerateCountFromManualCriteria(BenchmarkCriteriaVM criteria, EstablishmentType estType, int? lacode)
        {
            if (!ModelState.IsValid)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new System.ApplicationException("Invalid criteria entered for advanced search!"));
                return 0;
            }

            if (criteria.AdvancedCriteria != null)
            {
                criteria.AdvancedCriteria.LaCode = lacode;
                var result = await _financialDataService.SearchSchoolsCountByCriteriaAsync(criteria.AdvancedCriteria, estType);
                return result;
            }
            return 0;
        }

        private bool IsAreaFieldsValid(ComparisonArea? areaType, int? lacode, SchoolViewModel benchmarkSchool)
        {
            if (areaType == ComparisonArea.All)
            {
                lacode = null;
            }

            if (areaType == null)
            {
                benchmarkSchool.ErrorMessage = "Please select an area";
            }

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
                        benchmarkSchool.ErrorMessage = "Please select a Local authority from the auto-completed list";
                    }
                    break;
            }

            return !benchmarkSchool.HasError();
        }
    }
}