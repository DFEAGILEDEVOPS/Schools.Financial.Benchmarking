﻿using System.Web.Mvc;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using SFB.Web.UI.Helpers;
using SFB.Web.ApplicationCore.Helpers.Enums;

namespace SFB.Web.UI.Controllers
{
    public class LaController : Controller
    {
        private readonly ILaSearchService _laService;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;

        public LaController(ILaSearchService laService, IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
        {
            _laService = laService;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
        }

        public ActionResult Search(string name, string orderby = "", int page = 1, bool openOnly = false)
        {
            var searchMethod = TempData["SearchMethod"] as string;

            var filteredResults = _laService.SearchContains(name);

            var vm = new LaListViewModel(filteredResults, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), orderby, openOnly, searchMethod);
            
            vm.Pagination = new Pagination
                {
                    Start = (SearchDefaults.RESULTS_PER_PAGE * (page - 1)) + 1,
                    Total = filteredResults.Count,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.RESULTS_PER_PAGE,
                    PagedEntityType = PagedEntityType.LA
                };

            return View(vm);
        }

    }
}