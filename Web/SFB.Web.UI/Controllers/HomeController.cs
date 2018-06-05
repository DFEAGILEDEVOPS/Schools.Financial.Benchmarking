
using SFB.Web.UI.Models;
using System.Web.Mvc;
using SFB.Web.Domain.Services;
using SFB.Web.UI.Helpers;

namespace SFB.Web.UI.Controllers
{
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
            var vm = new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), null);
            vm.Authorities = _laService.GetLocalAuthorities();
            return View(vm);
        }
    }
}