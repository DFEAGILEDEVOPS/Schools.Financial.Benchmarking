using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Services.Search;
using SFB.Web.UI.Attributes;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class TrustSchoolsController : Controller
    {
        private readonly ISchoolSearchService _schoolSearchService;
        private readonly IFilterBuilder _filterBuilder;
        private readonly ISchoolBenchmarkListService _benchmarkBasketService;

        public TrustSchoolsController(IFilterBuilder filterBuilder, ISchoolSearchService schoolSearchService, ISchoolBenchmarkListService benchmarkBasketService)
        {
            _filterBuilder = filterBuilder;
            _schoolSearchService = schoolSearchService;
            _benchmarkBasketService = benchmarkBasketService;
        }
        
        public async Task<ActionResult> Index(int uid, int companyNo, string matName, string orderBy = "", int page = 1)
        {
            var searchResults = await _schoolSearchService.SearchAcademiesByUIDAsync(uid,
                (page - 1) * SearchDefaults.TRUST_SCHOOLS_PER_PAGE, SearchDefaults.TRUST_SCHOOLS_PER_PAGE, orderBy,
                Request.QueryString);

            ViewBag.CompanyNo = companyNo;
            ViewBag.MatName = matName;
            ViewBag.SearchMethod = "School";

            return View("SearchResults", GetSchoolViewModelList(searchResults, orderBy, page));
        }

        [Route("TrustSchoolSearch/Search-js")]
        public async Task<PartialViewResult> SearchJS(int uid, int companyNo, string matName, string orderBy = "", int page = 1)
        {
            var searchResults = await _schoolSearchService.SearchAcademiesByUIDAsync(uid,
                (page - 1) * SearchDefaults.TRUST_SCHOOLS_PER_PAGE, SearchDefaults.TRUST_SCHOOLS_PER_PAGE, orderBy,
                Request.QueryString);

            ViewBag.CompanyNo = companyNo;
            ViewBag.MatName = matName;

            return PartialView("Partials/Search/SchoolResults", GetSchoolViewModelList(searchResults, orderBy, page));
        }

        private SearchedSchoolListViewModel GetSchoolViewModelList(dynamic response, string orderBy, int page)
        {
            var schoolListVm = new List<SchoolSearchResultViewModel>();
            var vm = new SearchedSchoolListViewModel(schoolListVm, null, SearchTypes.SEARCH_BY_MAT, null, null,null, orderBy);
            if (response != null)
            {
                foreach (var result in response.Results)
                {
                    var schoolVm = new SchoolSearchResultViewModel(result);
                    schoolListVm.Add(schoolVm);
                }

                vm.SchoolComparisonList = _benchmarkBasketService.GetSchoolBenchmarkList();

                var filters = _filterBuilder.ConstructTrustSchoolSearchFilters(Request.QueryString, response.Facets);
                vm.Filters = filters;
                vm.FilterSelectionState = DetermineSelectionState(filters);

                vm.Pagination = new Pagination
                {
                    Start = (SearchDefaults.TRUST_SCHOOLS_PER_PAGE * (page - 1)) + 1,
                    Total = response.NumberOfResults,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.TRUST_SCHOOLS_PER_PAGE,
                    PagedEntityType = PagedEntityType.School
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