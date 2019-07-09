using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;
using SFB.Web.Domain.Services.DataAccess;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using SFB.Web.Domain.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.Common;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Attributes;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class TrustComparisonController : Controller
    {
        private readonly IFinancialDataService _financialDataService;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;

        public TrustComparisonController(IFinancialDataService financialDataService, IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
        {
            _financialDataService = financialDataService;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
        }

        public ActionResult Index(int companyNo, string matName)
        {            
            var benchmarkTrust = new TrustViewModel(companyNo, matName);

            LoadFinancialDataOfLatestYear(benchmarkTrust);

            var trustComparisonList = _benchmarkBasketCookieManager.UpdateTrustComparisonListCookie(CookieActions.SetDefault, companyNo, matName);

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
        public async Task<ActionResult> GenerateListFromManualCriteria(BenchmarkCriteriaVM criteria)
        {
            if (!ModelState.IsValid)
            {
                new TelemetryClient().TrackException(new ApplicationException("Invalid criteria entered for advanced search!" + criteria));
                return null;
            }

            if (criteria.AdvancedCriteria != null && !criteria.AdvancedCriteria.IsAllPropertiesNull())
            {
                _benchmarkBasketCookieManager.UpdateTrustComparisonListCookie(CookieActions.RemoveAll);
                _benchmarkBasketCookieManager.UpdateTrustComparisonListCookie(CookieActions.AddDefaultToList);
                var trustDocs = await _financialDataService.SearchTrustsByCriteriaAsync(criteria.AdvancedCriteria);
                foreach (var doc in trustDocs)
                {
                    try
                    {
                        _benchmarkBasketCookieManager.UpdateTrustComparisonListCookie(CookieActions.Add, doc.CompanyNumber, doc.TrustOrCompanyName);
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
                vm = _benchmarkBasketCookieManager.UpdateTrustComparisonListCookie(CookieActions.Add, companyNo, matName);
            }catch(ApplicationException ex)
            {
                vm = _benchmarkBasketCookieManager.ExtractTrustComparisonListFromCookie();
                ViewBag.Error = ex.Message;
            }

            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.CompanyNo != vm.DefaultTrustCompanyNo).ToList());
        }

        public PartialViewResult RemoveTrust(int companyNo)
        {
            var vm = _benchmarkBasketCookieManager.UpdateTrustComparisonListCookie(CookieActions.Remove, companyNo);

            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.CompanyNo != vm.DefaultTrustCompanyNo).ToList());
        }

        public PartialViewResult RemoveAllTrusts()
        {
            var vm = _benchmarkBasketCookieManager.UpdateTrustComparisonListCookie(CookieActions.RemoveAll);

            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.CompanyNo != vm.DefaultTrustCompanyNo).ToList());
        }

        private void LoadFinancialDataOfLatestYear(TrustViewModel benchmarkTrust)
        {
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);
            var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
            var financialDataObject = _financialDataService.GetTrustFinancialDataObject(benchmarkTrust.CompanyNo, term, MatFinancingType.TrustAndAcademies);

            benchmarkTrust.HistoricalFinancialDataModels = new List<Domain.Models.FinancialDataModel>
            {
                new Domain.Models.FinancialDataModel(benchmarkTrust.CompanyNo.ToString(), term, financialDataObject, EstablishmentType.MAT)
            };
        }
    }
}