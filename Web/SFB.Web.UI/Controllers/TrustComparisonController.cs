using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SFB.Web.UI.Models;
using SFB.Web.ApplicationCore.Services.DataAccess;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Attributes;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers;
using SFB.Web.UI.Services;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Helpers.Constants;

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

        public async Task<ActionResult> Manual(int companyNo)
        {
            var benchmarkTrust = new TrustViewModel(companyNo);

            await LoadFinancialDataOfLatestYearAsync(benchmarkTrust);

            await _trustBenchmarkListService.SetTrustAsDefaultAsync(companyNo);

            var trustComparisonList = _trustBenchmarkListService.GetTrustBenchmarkList();

            var vm = new TrustSelectionViewModel(benchmarkTrust, trustComparisonList);

            return View(vm);
        }

        public ViewResult SelectType(int companyNo, string matName)
        {
            var model = new TrustViewModel(companyNo, matName);
            return View(model);
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

            if (criteria.AdvancedCriteria != null && !criteria.AdvancedCriteria.IsAllPropertiesNull())
            {                
                return await _financialDataService.SearchTrustCountByCriteriaAsync(criteria.AdvancedCriteria);                
            }
            return 0;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenerateListFromManualCriteria(BenchmarkCriteriaVM criteria)
        {
            if (!ModelState.IsValid)
            {
                new TelemetryClient().TrackException(new ApplicationException("Invalid criteria entered for advanced search!" + criteria));
                return null;
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

        public PartialViewResult RemoveAllTrusts()
        {
            var vm = _trustBenchmarkListService.ClearTrustBenchmarkList();

            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.CompanyNo != vm.DefaultTrustCompanyNo).ToList());
        }

        private async Task LoadFinancialDataOfLatestYearAsync(TrustViewModel benchmarkTrust)
        {
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.MAT);
            var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
            var financialDataObject = await _financialDataService.GetTrustFinancialDataObjectByCompanyNoAsync(benchmarkTrust.CompanyNo, term, MatFinancingType.TrustAndAcademies);

            benchmarkTrust.HistoricalFinancialDataModels = new List<ApplicationCore.Models.FinancialDataModel>
            {
                new ApplicationCore.Models.FinancialDataModel(benchmarkTrust.CompanyNo.ToString(), term, financialDataObject, EstablishmentType.MAT)
            };
        }
    }
}