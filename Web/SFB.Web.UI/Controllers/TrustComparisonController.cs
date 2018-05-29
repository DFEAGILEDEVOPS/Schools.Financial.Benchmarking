using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;
using SFB.Web.UI.Helpers;
using SFB.Web.Common;
using SFB.Web.Domain.Services.DataAccess;
using System.Threading.Tasks;
using SFB.Web.Domain.Helpers;

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
            var latestYear = _financialDataService.GetLatestDataYearForTrusts();
            var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
            var dataDocument = _financialDataService.GetMATDataDocument(matNo, term, MatFinancingType.TrustAndAcademies);

            benchmarkTrust.HistoricalFinancialDataModels = new List<Domain.Models.FinancialDataModel>
            {
                new Domain.Models.FinancialDataModel(matNo, term, dataDocument, EstabType.MAT)
            };

            var trustComparisonList = UpdateTrustCookie("SetDefault", matNo, matName);
            var vm = new TrustCharacteristicsViewModel(benchmarkTrust, trustComparisonList);
            return View(vm);
        }

        public async Task<int> GenerateCountFromManualCriteria(BenchmarkCriteriaVM criteria)
        {
            if (!ModelState.IsValid)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new System.ApplicationException("Invalid criteria entered for advanced search!"));
                return 0;
            }

            if (criteria.AdvancedCriteria != null && !criteria.AdvancedCriteria.IsAllPropertiesNull())
            {                
                var result = await _financialDataService.SearchTrustCountByCriteriaAsync(criteria.AdvancedCriteria);
                return result;
            }
            return 0;
        }

        [HttpPost]
        public async Task<ActionResult> GenerateListFromManualCriteria(BenchmarkCriteriaVM criteria)
        {
            if (!ModelState.IsValid)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new System.ApplicationException("Invalid criteria entered for advanced search!"));
                return null;
            }

            if (criteria.AdvancedCriteria != null && !criteria.AdvancedCriteria.IsAllPropertiesNull())
            {
                UpdateTrustCookie("RemoveAll");
                UpdateTrustCookie("AddDefaultToList");
                var trustDocs = await _financialDataService.SearchTrustsByCriteriaAsync(criteria.AdvancedCriteria);
                foreach (var doc in trustDocs)
                {
                    UpdateTrustCookie("Add", doc.GetPropertyValue<string>("MATNumber"), doc.GetPropertyValue<string>("TrustOrCompanyName"));
                }
            }
            return Redirect("/BenchmarkCharts/Mats");
        }

        public PartialViewResult AddTrust(string matNo, string matName)
        {
            var vm = UpdateTrustCookie("Add", matNo, matName);
            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.MatNo != vm.DefaultTrustMatNo).ToList());
        }

        public PartialViewResult RemoveTrust(string matNo)
        {
            var vm = UpdateTrustCookie("Remove", matNo);

            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.MatNo != vm.DefaultTrustMatNo).ToList());
        }

        public PartialViewResult RemoveAllTrusts()
        {
            var vm = UpdateTrustCookie("RemoveAll");

            return PartialView("Partials/TrustsToCompare", vm.Trusts.Where(t => t.MatNo != vm.DefaultTrustMatNo).ToList());
        }

        private TrustComparisonListModel UpdateTrustCookie(string withAction, string matNo = null, string matName = null)
        {
            TrustComparisonListModel vm = null;
            HttpCookie cookie = Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
            switch (withAction)
            {
                case "SetDefault":
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST_MAT);
                        vm = new TrustComparisonListModel(matNo, matName)
                        {
                            Trusts = new List<BenchmarkTrustModel> {new BenchmarkTrustModel(matNo, matName)}
                        };
                    }
                    else
                    {
                        vm = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                        vm.DefaultTrustMatNo = matNo;
                        vm.DefaultTrustName = matName;
                        if (vm.Trusts.All(s => s.MatNo != matNo))
                        {
                            vm.Trusts.Add(new BenchmarkTrustModel(matNo, matName));
                        }
                    }
                    break;

                case "Add":
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST_MAT);
                        vm = new TrustComparisonListModel(matNo, matName)
                        {
                            Trusts = new List<BenchmarkTrustModel> { new BenchmarkTrustModel(matNo, matName) }
                        };
                    }
                    else
                    {
                        vm = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                        if (vm.DefaultTrustMatNo == matNo || vm.Trusts.Any(s => s.MatNo == matNo))
                        {
                            ViewBag.Error = ErrorMessages.DuplicateTrust;
                        }
                        else
                        {
                            vm.Trusts.Add(new BenchmarkTrustModel(matNo, matName));
                        }
                    }
                    break;
                case "Remove":
                    vm = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                    vm.Trusts.Remove(new BenchmarkTrustModel(matNo));
                    break;
                case "RemoveAll":
                    vm = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                    vm.Trusts.Clear();
                    break;
                case "AddDefaultToList":
                    vm = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                    if (vm.Trusts.All(s => vm.DefaultTrustMatNo != matNo))
                    {
                        vm.Trusts.Add(new BenchmarkTrustModel(vm.DefaultTrustMatNo, vm.DefaultTrustName));
                    }
                    break;
            }

            cookie.Value = JsonConvert.SerializeObject(vm);
            cookie.Expires = DateTime.MaxValue;
            Response.Cookies.Add(cookie);

            return vm;
        }
    }
}