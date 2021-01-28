using System.Web.Mvc;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using SFB.Web.UI.Helpers;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Services.LocalAuthorities;
using System.Linq;

namespace SFB.Web.UI.Controllers
{
    public class LaController : Controller
    {
        private readonly ILaSearchService _laService;
        private readonly IBenchmarkBasketService _benchmarkBasketService;

        public LaController(ILaSearchService laService, IBenchmarkBasketService benchmarkBasketService)
        {
            _laService = laService;
            _benchmarkBasketService = benchmarkBasketService;
        }

        public ActionResult Search(string name, string orderby = "", int page = 1, bool openOnly = false)
        {
            var searchMethod = TempData["SearchMethod"] as string;

            var laModels = _laService.SearchContains(name);
            var laViewModels = laModels.Select(la => new LaViewModel(la.Id, la.LaName)).ToList();

            var vm = new LaListViewModel(laViewModels, _benchmarkBasketService.GetSchoolBenchmarkList(), orderby, openOnly, searchMethod);
            
            vm.Pagination = new Pagination
                {
                    Start = (SearchDefaults.RESULTS_PER_PAGE * (page - 1)) + 1,
                    Total = laModels.Count,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.RESULTS_PER_PAGE,
                    PagedEntityType = PagedEntityType.LA
                };

            return View(vm);
        }

    }
}