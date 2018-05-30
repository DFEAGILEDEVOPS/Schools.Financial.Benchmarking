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
using SFB.Web.Domain.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.Common;
using SFB.Web.UI.Helpers;

namespace SFB.Web.UI.Controllers
{
    public class TrustComparisonController : Controller
    {
        private readonly IFinancialDataService _financialDataService;
        public TrustComparisonController(IFinancialDataService financialDataService)
        {
            _financialDataService = financialDataService;
        }

        public ActionResult Index(string matNo, string matName)
        {            
            var benchmarkTrust = new TrustViewModel(matNo, matName);

            LoadFinancialDataOfLatestYear(benchmarkTrust);

            var trustComparisonList = UpdateTrustCookie(TrustCookieActions.SetDefault, matNo, matName);

            var vm = new TrustCharacteristicsViewModel(benchmarkTrust, trustComparisonList);

            return View(vm);
        }

        public async Task<int> GenerateCountFromManualCriteria(BenchmarkCriteriaVM criteria)
        {
            if (!ModelState.IsValid)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException("Invalid criteria entered for advanced search! : " + criteria.ToString()));
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException("Invalid criteria entered for advanced search! : " + criteria.ToString()));
                return null;
            }

            if (criteria.AdvancedCriteria != null && !criteria.AdvancedCriteria.IsAllPropertiesNull())
            {
                UpdateTrustCookie(TrustCookieActions.RemoveAll);
                UpdateTrustCookie(TrustCookieActions.AddDefaultToList);
                var trustDocs = await _financialDataService.SearchTrustsByCriteriaAsync(criteria.AdvancedCriteria);
                foreach (var doc in trustDocs)
                {
                    UpdateTrustCookie(TrustCookieActions.Add, doc.GetPropertyValue<string>("MATNumber"), doc.GetPropertyValue<string>("TrustOrCompanyName"));
                }
            }
            return Redirect("/BenchmarkCharts/Mats");
        }

        public PartialViewResult AddTrust(string matNo, string matName)
        {
            var vm = UpdateTrustCookie(TrustCookieActions.Add, matNo, matName);
            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.MatNo != vm.DefaultTrustMatNo).ToList());
        }

        public PartialViewResult RemoveTrust(string matNo)
        {
            var vm = UpdateTrustCookie(TrustCookieActions.Remove, matNo);

            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.MatNo != vm.DefaultTrustMatNo).ToList());
        }

        public PartialViewResult RemoveAllTrusts()
        {
            var vm = UpdateTrustCookie(TrustCookieActions.RemoveAll);

            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.MatNo != vm.DefaultTrustMatNo).ToList());
        }

        private TrustComparisonListModel UpdateTrustCookie(TrustCookieActions withAction, string matNo = null, string matName = null)
        {
            TrustComparisonListModel comparisonList = null;
            HttpCookie cookie = Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
            switch (withAction)
            {
                case TrustCookieActions.SetDefault:
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST_MAT);
                        comparisonList = new TrustComparisonListModel(matNo, matName)
                        {
                            Trusts = new List<BenchmarkTrustModel> {new BenchmarkTrustModel(matNo, matName)}
                        };
                    }
                    else
                    {
                        comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                        comparisonList.DefaultTrustMatNo = matNo;
                        comparisonList.DefaultTrustName = matName;
                        if (comparisonList.Trusts.All(s => s.MatNo != matNo))
                        {
                            comparisonList.Trusts.Add(new BenchmarkTrustModel(matNo, matName));
                        }
                    }
                    break;

                case TrustCookieActions.Add:
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST_MAT);
                        comparisonList = new TrustComparisonListModel(matNo, matName)
                        {
                            Trusts = new List<BenchmarkTrustModel> { new BenchmarkTrustModel(matNo, matName) }
                        };
                    }
                    else
                    {
                        comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                        if (comparisonList.DefaultTrustMatNo == matNo || comparisonList.Trusts.Any(s => s.MatNo == matNo))
                        {
                            ViewBag.Error = ErrorMessages.DuplicateTrust;
                        }
                        else
                        {
                            comparisonList.Trusts.Add(new BenchmarkTrustModel(matNo, matName));
                        }
                    }
                    break;
                case TrustCookieActions.Remove:
                    comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                    comparisonList.Trusts.Remove(new BenchmarkTrustModel(matNo));
                    break;
                case TrustCookieActions.RemoveAll:
                    comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                    comparisonList.Trusts.Clear();
                    break;
                case TrustCookieActions.AddDefaultToList:
                    comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                    if (comparisonList.Trusts.All(s => comparisonList.DefaultTrustMatNo != matNo))
                    {
                        comparisonList.Trusts.Add(new BenchmarkTrustModel(comparisonList.DefaultTrustMatNo, comparisonList.DefaultTrustName));
                    }
                    break;
            }

            cookie.Value = JsonConvert.SerializeObject(comparisonList);
            cookie.Expires = DateTime.MaxValue;
            Response.Cookies.Add(cookie);

            return comparisonList;
        }

        private void LoadFinancialDataOfLatestYear(TrustViewModel benchmarkTrust)
        {
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);
            var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
            var dataDocument = _financialDataService.GetMATDataDocument(benchmarkTrust.MatNo, term, MatFinancingType.TrustAndAcademies);

            benchmarkTrust.HistoricalFinancialDataModels = new List<Domain.Models.FinancialDataModel>
            {
                new Domain.Models.FinancialDataModel(benchmarkTrust.MatNo, term, dataDocument, EstablishmentType.MAT)
            };
        }
    }
}