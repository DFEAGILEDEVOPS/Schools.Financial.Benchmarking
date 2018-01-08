using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Controllers
{
    public class TrustComparisonController : Controller
    {
        public ActionResult Index(string matNo, string matName)
        {
            var vm = UpdateTrustCookie("SetDefault", matNo, matName);
            return View(vm);
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

        private TrustComparisonViewModel UpdateTrustCookie(string withAction, string matNo = null, string matName = null)
        {
            TrustComparisonViewModel vm = null;
            HttpCookie cookie = Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
            switch (withAction)
            {
                case "SetDefault":
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST_MAT);
                        vm = new TrustComparisonViewModel(matNo, matName)
                        {
                            Trusts = new List<TrustToCompareViewModel> {new TrustToCompareViewModel(matNo, matName)}
                        };
                    }
                    else
                    {
                        vm = JsonConvert.DeserializeObject<TrustComparisonViewModel>(cookie.Value);
                        vm.DefaultTrustMatNo = matNo;
                        vm.DefaultTrustName = matName;
                        if (vm.Trusts.All(s => s.MatNo != matNo))
                        {
                            vm.Trusts.Add(new TrustToCompareViewModel(matNo, matName));
                        }
                    }
                    break;

                case "Add":
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST_MAT);
                        vm = new TrustComparisonViewModel(matNo, matName)
                        {
                            Trusts = new List<TrustToCompareViewModel> { new TrustToCompareViewModel(matNo, matName) }
                        };
                    }
                    else
                    {
                        vm = JsonConvert.DeserializeObject<TrustComparisonViewModel>(cookie.Value);
                        if (vm.DefaultTrustMatNo == matNo || vm.Trusts.Any(s => s.MatNo == matNo))
                        {
                            ViewBag.Error = ErrorMessages.DuplicateTrust;
                        }
                        else
                        {
                            vm.Trusts.Add(new TrustToCompareViewModel(matNo, matName));
                        }
                    }
                    break;
                case "Remove":
                    vm = JsonConvert.DeserializeObject<TrustComparisonViewModel>(cookie.Value);
                    vm.Trusts.Remove(new TrustToCompareViewModel(matNo));
                    break;
                case "RemoveAll":
                    vm = JsonConvert.DeserializeObject<TrustComparisonViewModel>(cookie.Value);
                    vm.Trusts.Clear();
                    break;
            }

            cookie.Value = JsonConvert.SerializeObject(vm);
            cookie.Expires = DateTime.MaxValue;
            Response.Cookies.Add(cookie);

            return vm;
        }
    }
}