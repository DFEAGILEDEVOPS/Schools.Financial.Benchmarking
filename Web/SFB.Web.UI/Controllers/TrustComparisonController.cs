using System;
using System.Linq;
using System.Web.Mvc;
using SFB.Web.UI.Models;
using SFB.Web.ApplicationCore.Services.DataAccess;
using System.Threading.Tasks;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Attributes;
using SFB.Web.UI.Services;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Helpers.Constants;
using Microsoft.ApplicationInsights;
using SFB.Web.ApplicationCore.Models;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class TrustComparisonController : Controller
    {
        private readonly IFinancialDataService _financialDataService;
        private readonly ITrustBenchmarkListService _trustBenchmarkListService;

        public TrustComparisonController(IFinancialDataService financialDataService, ITrustBenchmarkListService trustBenchmarkListService)
        {
            _financialDataService = financialDataService;
            _trustBenchmarkListService = trustBenchmarkListService;
        }

        public async Task<ActionResult> Advanced(int companyNo, BenchmarkCriteria advancedCriteria)
        {
            var benchmarkTrust =  await _trustBenchmarkListService.SetTrustAsDefaultAsync(companyNo);

            var vm = new TrustCharacteristicsViewModel(benchmarkTrust, advancedCriteria);

            return View(vm);
        }


        public async Task<ActionResult> Manual(int companyNo)
        {
            var benchmarkTrust = await _trustBenchmarkListService.SetTrustAsDefaultAsync(companyNo);

            var trustComparisonList = _trustBenchmarkListService.GetTrustBenchmarkList();

            var vm = new TrustSelectionViewModel(benchmarkTrust, trustComparisonList);

            return View(vm);
        }

        public ViewResult SelectType(int companyNo, string matName)
        {
            var model = new TrustViewModel(companyNo, matName);
            return View(model);
        }
        
        [HttpGet]
        public ActionResult SelectComparisonType(int uid, int companyNo, string matName)
        {
            var vm = new TrustViewModel(companyNo)
            {
                UID = uid,
                Name = matName
            };
            return View(vm);
        }

        public ActionResult StepOne(int companyNo, string matName, ComparisonType? comparisonType)
        {
            switch (comparisonType)
            {
                case ComparisonType.Advanced:
                    return Redirect($"/TrustComparison/Advanced?companyNo={companyNo}");
                case ComparisonType.Manual:
                    return Redirect($"/TrustComparison/Manual?companyNo={companyNo}");
                default:
                    var model = new TrustViewModel(companyNo, matName);
                    model.ErrorMessage = ErrorMessages.SelectComparisonType;
                    return View("SelectType", model);
            } 
        }

        public async Task<int> GenerateCountFromAdvancedCriteria(BenchmarkCriteriaVM criteria)
        {
            if (!ModelState.IsValid)
            {
                return 0;
            }
              
            return await _financialDataService.SearchTrustCountByCriteriaAsync(criteria?.AdvancedCriteria);                
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenerateListFromAdvancedCriteria(BenchmarkCriteriaVM criteria, ComparisonType? comparison = null)
        {
            if (!ModelState.IsValid)
            {
                new TelemetryClient().TrackException(new ApplicationException("Invalid criteria entered for advanced search!" + criteria));
                throw new ApplicationException();
            }

            if (criteria.AdvancedCriteria != null && !criteria.AdvancedCriteria.IsAllPropertiesNull())
            {
                _trustBenchmarkListService.ClearTrustBenchmarkList();
                _trustBenchmarkListService.AddDefaultTrustToBenchmarkList();
                var trustDocs = await _financialDataService.SearchTrustsByCriteriaAsync(criteria.AdvancedCriteria);
                foreach (var doc in trustDocs)
                {
                    _trustBenchmarkListService.TryAddTrustToBenchmarkList(doc.CompanyNumber.GetValueOrDefault(), doc.TrustOrCompanyName);
                }
            }
            
            TempData["BenchmarkCriteria"] = criteria.AdvancedCriteria;
            TempData["ComparisonType"] = comparison;
            return Redirect("/BenchmarkCharts/Mats");
        }

        public PartialViewResult AddTrust(int companyNo, string matName)
        {
            TrustComparisonListModel vm;
            try
            {
                vm = _trustBenchmarkListService.AddTrustToBenchmarkList(companyNo, matName);
            }catch(ApplicationException ex)
            {
                vm = _trustBenchmarkListService.GetTrustBenchmarkList();
                ViewBag.Error = ex.Message;
            }

            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.CompanyNo != vm.DefaultTrustCompanyNo).ToList());
        }

        public PartialViewResult RemoveTrust(int companyNo)
        {
            var vm = _trustBenchmarkListService.RemoveTrustFromBenchmarkList(companyNo);

            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.CompanyNo != vm.DefaultTrustCompanyNo).ToList());
        }

        //public PartialViewResult RemoveAllTrusts()
        //{
        //    var vm = _trustBenchmarkListService.ClearTrustBenchmarkList();

        //    return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.CompanyNo != vm.DefaultTrustCompanyNo).ToList());
        //}
    }
}