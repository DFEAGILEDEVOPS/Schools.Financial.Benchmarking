using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFB.Web.Common;
using SFB.Web.Domain.Services.Search;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Controllers
{
    public class TrustSchoolsController : Controller
    {
        private readonly ISchoolSearchService _schoolSearchService;
        private readonly IFilterBuilder _filterBuilder;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;

        public TrustSchoolsController(IFilterBuilder filterBuilder, ISchoolSearchService schoolSearchService, IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
        {
            _filterBuilder = filterBuilder;
            _schoolSearchService = schoolSearchService;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
        }
        
        public async Task<ActionResult> Index(string matNo, string matName, string orderBy = "", int page = 1)
        {
            var searchResults = await _schoolSearchService.SearchSchoolByMatNo(matNo,
                (page - 1) * SearchDefaults.TRUST_SCHOOLS_PER_PAGE, SearchDefaults.TRUST_SCHOOLS_PER_PAGE, orderBy,
                Request.QueryString);

            ViewBag.MatNo = matNo;
            ViewBag.MatName = matName;

            return View("SearchResults", GetSchoolViewModelList(searchResults, orderBy, page));
        }

        [Route("TrustSchoolSearch/Search-js")]
        public async Task<PartialViewResult> SearchJS(string matNo, string matName, string orderBy = "", int page = 1)

        {
            var searchResults = await _schoolSearchService.SearchSchoolByMatNo(matNo,
                (page - 1) * SearchDefaults.TRUST_SCHOOLS_PER_PAGE, SearchDefaults.TRUST_SCHOOLS_PER_PAGE, orderBy,
                Request.QueryString);

            ViewBag.MatNo = matNo;
            ViewBag.MatName = matName;

            return PartialView("Partials/SchoolResults", GetSchoolViewModelList(searchResults, orderBy, page));
        }

        private SchoolListViewModel GetSchoolViewModelList(dynamic response, string orderBy, int page)
        {
            var schoolListVm = new List<SchoolViewModel>();
            var vm = new SchoolListViewModel(schoolListVm, null, orderBy);
            if (response != null)
            {
                foreach (var result in response.Results)
                {
                    var schoolVm = new SchoolViewModel(result);
                    schoolListVm.Add(schoolVm);
                }

                vm.SchoolComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

                var filters = _filterBuilder.ConstructTrustSchoolSearchFilters(Request.QueryString, response.Facets);
                vm.Filters = filters;
                vm.FilterSelectionState = DetermineSelectionState(filters);

                vm.Pagination = new Pagination
                {
                    Start = (SearchDefaults.TRUST_SCHOOLS_PER_PAGE * (page - 1)) + 1,
                    Total = response.NumberOfResults,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.TRUST_SCHOOLS_PER_PAGE
                };
            }

            return vm;
        }

        private string DetermineSelectionState(Models.Filter[] filters)
        {
            bool ofstedExpanded = false, schoolTypeExpanded = false, religiousCharacterExpanded = false;

            if (filters.Where(x => x.Group == "ofstedrating").Any(filter => filter.Metadata.Any(x => x.Checked)))
            {
                ofstedExpanded = true;
            }

            if (filters.Where(x => x.Group == "schooltype").Any(filter => filter.Metadata.Any(x => x.Checked)))
            {
                schoolTypeExpanded = true;
            }

            if (filters.Where(x => x.Group == "faith").Any(filter => filter.Metadata.Any(x => x.Checked)))
            {
                religiousCharacterExpanded = true;
            }

            return
                $"{(ofstedExpanded ? "1" : "0")},{(schoolTypeExpanded ? "1" : "0")},{(religiousCharacterExpanded ? "1" : "0")}";
        }
    }
}