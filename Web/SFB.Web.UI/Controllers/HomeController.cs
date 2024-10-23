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
            _showDeprecationInformation = bool.TryParse(ConfigurationManager.AppSettings["DeprecationInformation:Enabled"], out var show) && show;
        }

        public ActionResult Index()
        {
            return _showDeprecationInformation
                ? View(nameof(Index), GetDeprecationViewModel())
                : View(nameof(Search), GetSearchViewModel());
        }

        [Route("Search")]
        public ActionResult Search()
        {
            if (_showDeprecationInformation)
            {
                return View(GetSearchViewModel());
            }
            
            return RedirectToAction("Index");
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
                return PartialView("Partials/Headers/Deprecation", GetDeprecationViewModel());
            }

            return new EmptyResult();
        }

        private SearchViewModel GetSearchViewModel()
        {
            return new SearchViewModel(_benchmarkBasketService.GetSchoolBenchmarkList(), null)
            {
                Authorities = _laService.GetLocalAuthorities()
            };
        }
        
        private DeprecationViewModel GetDeprecationViewModel()
        {
            return new DeprecationViewModel
            {
                Title = ConfigurationManager.AppSettings["DeprecationInformation:Title"] ?? string.Empty,
                Body = (ConfigurationManager.AppSettings["DeprecationInformation:Body"] ?? string.Empty).Replace(@"\n", Environment.NewLine),
                NewServiceUrl = ConfigurationManager.AppSettings["DeprecationInformation:NewServiceUrl"] ?? string.Empty,
                OldServiceLinkText = ConfigurationManager.AppSettings["DeprecationInformation:OldServiceLinkText"] ?? string.Empty
            };
        }
    }
}