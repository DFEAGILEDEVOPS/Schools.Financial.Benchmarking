using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SFB.Web.UI.Models;
using SFB.Web.ApplicationCore.Services.DataAccess;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Attributes;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers;
using SFB.Web.UI.Services;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class TrustComparisonController : Controller
    {
        private readonly IFinancialDataService _financialDataService;
        private readonly IBenchmarkBasketService _benchmarkBasketService;

        public TrustComparisonController(IFinancialDataService financialDataService, IBenchmarkBasketService benchmarkBasketService)
        {
            _financialDataService = financialDataService;
            _benchmarkBasketService = benchmarkBasketService;
        }

        public async Task<ActionResult> Index(int companyNo)
        {            
            var benchmarkTrust = new TrustViewModel(companyNo);

            await LoadFinancialDataOfLatestYearAsync(benchmarkTrust);

            var trustComparisonList = _benchmarkBasketService.UpdateTrustComparisonListCookie(CookieActions.SetDefault, companyNo, benchmarkTrust.Name);

            var vm = new TrustCharacteristicsViewModel(benchmarkTrust, trustComparisonList);

            return View(vm);
        }

        public async Task<int> GenerateCountFromAdvancedCriteria(BenchmarkCriteriaVM criteria)
        {
            if (!ModelState.IsValid)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException("Invalid criteria entered for advanced search! : " + criteria.ToString()));
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
                _benchmarkBasketService.UpdateTrustComparisonListCookie(CookieActions.RemoveAll);
                _benchmarkBasketService.UpdateTrustComparisonListCookie(CookieActions.AddDefaultToList);
                var trustDocs = await _financialDataService.SearchTrustsByCriteriaAsync(criteria.AdvancedCriteria);
                foreach (var doc in trustDocs)
                {
                    try
                    {
                        _benchmarkBasketService.UpdateTrustComparisonListCookie(CookieActions.Add, doc.CompanyNumber, doc.TrustOrCompanyName);
                    }catch (ApplicationException)
                    {
                        //Default trust cannot be added twice. Do nothing.
                    }
                }
            }
            return Redirect("/BenchmarkCharts/Mats");
        }

        public PartialViewResult AddTrust(int companyNo, string matName)
        {
            TrustComparisonListModel vm;
            try
            {
                vm = _benchmarkBasketService.UpdateTrustComparisonListCookie(CookieActions.Add, companyNo, matName);
            }catch(ApplicationException ex)
            {
                vm = _benchmarkBasketService.ExtractTrustComparisonListFromCookie();
                ViewBag.Error = ex.Message;
            }

            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.CompanyNo != vm.DefaultTrustCompanyNo).ToList());
        }

        public PartialViewResult RemoveTrust(int companyNo)
        {
            var vm = _benchmarkBasketService.UpdateTrustComparisonListCookie(CookieActions.Remove, companyNo);

            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.CompanyNo != vm.DefaultTrustCompanyNo).ToList());
        }

        public PartialViewResult RemoveAllTrusts()
        {
            var vm = _benchmarkBasketService.UpdateTrustComparisonListCookie(CookieActions.RemoveAll);

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