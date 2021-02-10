using SFB.Web.UI.Models;
using System.Web.Mvc;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Attributes;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Services;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class HomeController : Controller
    {
        private readonly ILocalAuthoritiesService _laService;
        private readonly ISchoolBenchmarkListService _benchmarkBasketService;

        public HomeController(ILocalAuthoritiesService laService, ISchoolBenchmarkListService benchmarkBasketService)
        {
            _laService = laService;
            _benchmarkBasketService = benchmarkBasketService;
        }

        public ActionResult Index()
        {
            var vm = new SearchViewModel(_benchmarkBasketService.GetSchoolBenchmarkList(), null);
            vm.Authorities = _laService.GetLocalAuthorities();
            return View(vm);
        }
    }
}