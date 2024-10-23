using System;
using System.Configuration;
using SFB.Web.UI.Models;
using System.Web.Mvc;
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
        private readonly bool _showDeprecationInformation;

        public HomeController(ILocalAuthoritiesService laService, ISchoolBenchmarkListService benchmarkBasketService)
        {
            _laService = laService;
            _benchmarkBasketService = benchmarkBasketService;
            _showDeprecationInformation = bool.TryParse(ConfigurationManager.AppSettings["ShowDeprecationInformation"], out var showWarning) && showWarning;
        }

        public ActionResult Index()
        {
            var vm = new SearchViewModel(_benchmarkBasketService.GetSchoolBenchmarkList(), null)
            {
                Authorities = _laService.GetLocalAuthorities()
            };
            return View(vm);
        }

        public ActionResult News()
        {
            return View();
        }

        public ActionResult Header()
        {
            if (!_showDeprecationInformation)
            {
                return PartialView("Partials/Headers/Help");
            }

            if (Request.Url?.PathAndQuery != "/")
            {
                return PartialView("Partials/Headers/Deprecation", new DeprecationViewModel
                {
                    Title = ConfigurationManager.AppSettings["DeprecationInformation:Title"] ?? string.Empty,
                    Body = (ConfigurationManager.AppSettings["DeprecationInformation:Body"] ?? string.Empty).Replace(@"\n", Environment.NewLine)
                });
            }

            return new EmptyResult();
        }
    }
}