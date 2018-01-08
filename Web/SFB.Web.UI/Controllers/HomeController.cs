
using SFB.Web.UI.Models;
using System.Web.Mvc;
using SFB.Web.Domain.Services;

namespace SFB.Web.UI.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ISchoolApiService _schoolApiService;
        public HomeController(ISchoolApiService schoolApiService)
        {
            _schoolApiService = schoolApiService;
        }

        public ActionResult Index()
        {
            var vm = new SchoolSearchViewModel(base.ExtractSchoolComparisonListFromCookie(), null);
            vm.Authorities = _schoolApiService.GetLocalAuthorities();
            return View(vm);
        }
    }
}