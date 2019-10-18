
using SFB.Web.UI.Models;
using System.Web.Mvc;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Attributes;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class HomeController : Controller
    {
        private readonly ILocalAuthoritiesService _laService;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;

        public HomeController(ILocalAuthoritiesService laService, IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
        {
            _laService = laService;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
        }

        public ActionResult Index()
        {
            var vm = new SearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), null);
            vm.Authorities = _laService.GetLocalAuthorities();
            return View(vm);
        }
    }
}