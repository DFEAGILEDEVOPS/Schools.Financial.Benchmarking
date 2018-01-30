
using SFB.Web.UI.Models;
using System.Web.Mvc;
using SFB.Web.Domain.Services;
using SFB.Web.UI.Helpers.Filters;

namespace SFB.Web.UI.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILocalAuthoritiesService _laService;
        public HomeController(ILocalAuthoritiesService laService)
        {
            _laService = laService;
        }

        public ActionResult Index()
        {
            var vm = new SchoolSearchViewModel(base.ExtractSchoolComparisonListFromCookie(), null);
            vm.Authorities = _laService.GetLocalAuthorities();
            return View(vm);
        }
    }
}