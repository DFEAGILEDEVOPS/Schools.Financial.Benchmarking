using Newtonsoft.Json;
using SFB.Web.Common;
using SFB.Web.Common.DataObjects;
using SFB.Web.Domain.Services;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Domain.Services.Search;
using SFB.Web.UI.Attributes;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class TrustSearchController : SearchBaseController
    {
        private readonly ILocalAuthoritiesService _laService;
        private readonly ILaSearchService _laSearchService;
        private readonly ILocationSearchService _locationSearchService;
        private readonly IValidationService _valService;
        private readonly IContextDataService _contextDataService;
        private readonly IFinancialDataService _financialDataService;

        public TrustSearchController(ILocalAuthoritiesService laService,
            ILaSearchService laSearchService, ILocationSearchService locationSearchService, IFilterBuilder filterBuilder,
            IValidationService valService, IContextDataService contextDataService,
            ITrustSearchService trustSearchService, ISchoolSearchService schoolSearchService,
            IBenchmarkBasketCookieManager benchmarkBasketCookieManager, IFinancialDataService financialDataService)
            : base(schoolSearchService, trustSearchService, benchmarkBasketCookieManager, filterBuilder)
        {
            _laService = laService;
            _laSearchService = laSearchService;
            _locationSearchService = locationSearchService;
            _valService = valService;
            _contextDataService = contextDataService;
            _financialDataService = financialDataService;
        }

        public async Task<ActionResult> Search(
        string trustNameId,
        string searchType,
        string suggestionUrn,
        string locationorpostcode,
        string locationCoordinates,
        string laCodeName,
        decimal? radius,
        bool openOnly = false,
        string orderby = "",
        int page = 1,
        string tab = "list",
        string referrer = "home/index")
        {
            dynamic searchResp = null;
            string errorMessage = string.Empty;
            ViewBag.tab = tab;
            ViewBag.SearchMethod = "MAT";
            
            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_TRUST_NAME_ID:
                    if (IsNumeric(trustNameId))
                    {
                        errorMessage = _valService.ValidateCompanyNoParameter(trustNameId);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            return RedirectToAction("Index", "Trust", new { companyNo = trustNameId });
                        }
                    }
                    else
                    {
                        errorMessage = _valService.ValidateTrustNameParameter(trustNameId);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            searchResp = await GetSearchResults(trustNameId, SearchTypes.SEARCH_BY_TRUST_NAME_ID, searchType, locationCoordinates, laCodeName, radius, openOnly, orderby, 1);
                            if (searchResp.NumberOfResults == 0)
                            {
                                return RedirectToActionPermanent("SuggestTrust", "TrustSearch",
                                    new RouteValueDictionary { { "trustNameId", trustNameId } });
                            }
                        }
                    }
                    break;
                case SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME:
                    if (!IsNumeric(laCodeName))
                    {
                        errorMessage = _valService.ValidateLaNameParameter(laCodeName);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            var exactMatch = _laSearchService.SearchExactMatch(laCodeName);
                            if (exactMatch != null)
                            {
                                laCodeName = exactMatch.id;
                                return await Search(trustNameId, searchType, suggestionUrn, locationorpostcode,
                                    locationCoordinates, laCodeName, radius, openOnly, orderby, page, tab);
                            }
                            TempData["SearchMethod"] = "Random";
                            return RedirectToAction("Search", "La", new { name = laCodeName, openOnly = openOnly });
                        }
                    }
                    else
                    {
                        errorMessage = _valService.ValidateLaCodeParameter(laCodeName);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            searchResp = await GetSearchResults(trustNameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);

                            int resultCount = searchResp.NumberOfResults;
                            switch (resultCount)
                            {
                                case 0:
                                    return View("EmptyResult",
                                        new SearchViewModel(null, searchType));
                                case 1:
                                    return RedirectToAction("Detail", "School",
                                        new
                                        {
                                            urn = ((Domain.Models.QueryResultsModel)searchResp).Results.First()["URN"]
                                        });
                            }
                        }
                    }
                    break;

                case SearchTypes.SEARCH_BY_TRUST_LOCATION:
                    errorMessage = _valService.ValidateLocationParameter(locationorpostcode);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        if (string.IsNullOrEmpty(locationCoordinates))
                        {
                            var result = _locationSearchService.SuggestLocationName(locationorpostcode);
                            switch (result.Matches.Count)
                            {
                                case 0:
                                    return View("EmptyLocationResult",
                                        new SearchViewModel(null, searchType));
                                default:
                                    TempData["LocationResults"] = result;
                                    TempData["SearchMethod"] = "Random";
                                    return RedirectToAction("Suggest", "Location", new { locationOrPostcode = locationorpostcode, openOnly = openOnly });
                            }
                        }
                        else
                        {
                            searchResp = await GetSearchResults(trustNameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);

                            int resultCnt = searchResp.NumberOfResults;
                            switch (resultCnt)
                            {
                                case 0:
                                    return View("EmptyLocationResult",
                                        new SearchViewModel(null, searchType));
                                case 1:
                                    return RedirectToAction("Detail", "School",
                                        new { urn = ((Domain.Models.QueryResultsModel)searchResp).Results.First()["URN"] });
                            }
                        }
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                var searchVM = new SearchViewModel(null, searchType)
                {
                    SearchType = searchType,
                    ErrorMessage = errorMessage,
                    Authorities = _laService.GetLocalAuthorities()
                };

                return View("../" + referrer, searchVM);
            }

            return View("SearchResults", GetTrustViewModelList(searchResp, orderby, page, searchType, trustNameId, locationorpostcode, _laService.GetLaName(laCodeName)));
        }

        [Route("TrustSearch/Search-js")]
        public async Task<PartialViewResult> SearchJS(string trustNameId, string searchType, string trustSuggestionUrn,
            string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius, 
            bool openOnly = false, string orderby = "", int page = 1)
        {
            dynamic searchResponse = await GetSearchResults(trustNameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);

            var vm = GetTrustViewModelList(searchResponse, orderby, page, searchType, trustNameId, locationorpostcode, _laService.GetLaName(laCodeName));

            ViewBag.SearchMethod = "MAT";
            return PartialView("Partials/TrustResults", vm);
        }

        [Route("TrustSearch/Search-json")]
        public async Task<JsonResult> SearchJson(string trustNameId, string searchType, string trustSuggestionUrn,
            string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius,
            bool openOnly = false, string orderby = "", int page = 1)
        {
            dynamic trustSearchResponse = await GetSearchResults(trustNameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page, 1000);

            TrustListViewModel trusts = GetTrustViewModelList(trustSearchResponse, orderby, page, searchType, trustNameId, locationorpostcode, _laService.GetLaName(laCodeName));

            var results = new List<SchoolSummaryViewModel>();
            foreach (var trust in trusts.ModelList)
            {
                var schoolSearchResponse = await _schoolSearchService.SearchSchoolByCompanyNo(trust.CompanyNo, 0, 1000, null, null);
                foreach (var school in schoolSearchResponse.Results)
                {
                    var schoolVm = new SchoolSummaryViewModel(school);
                    results.Add(schoolVm);
                }
            }

            return Json(new { count = results.Count, results = results }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Suggest(string name)
        {
            dynamic response = await _trustSearchService.SuggestTrustByName(name);

            var json = JsonConvert.SerializeObject(response);
            return Content(json, "application/json");
        }

        public async Task<ActionResult> SuggestTrust(string trustNameId)
        {
            var vm = new SchoolNotFoundViewModel
            {
                SearchKey = trustNameId,
                Suggestions = await _trustSearchService.SuggestTrustByName(trustNameId)
            };
            return View("NotFound", vm);
        }

        private TrustListViewModel GetTrustViewModelList(dynamic response, string orderBy, int page, string searchType, string nameKeyword, string locationKeyword, string laKeyword)
        {
            var trustListVm = new List<TrustViewModel>();
            var vm = new TrustListViewModel(trustListVm, null, searchType, nameKeyword, locationKeyword, laKeyword, orderBy);
            if (response != null)
            {
                foreach (var result in response.Results)
                {
                    var companyNo = int.Parse(result["CompanyNumber"]);
                    var companyName = result["TrustOrCompanyName"];
                    var academiesList = _financialDataService.GetAcademiesByCompanyNumber(LatestMATTerm(), companyNo);
                    var trustVm = new TrustViewModel(companyNo, companyName, academiesList);
                    trustListVm.Add(trustVm);
                }

                vm.SchoolComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

                //var filters = _filterBuilder.ConstructSchoolSearchFilters(Request.QueryString, response.Facets);
                //vm.Filters = filters;
                //vm.FilterSelectionState = DetermineSelectionState(filters);

                vm.Pagination = new Pagination
                {
                    Start = (SearchDefaults.RESULTS_PER_PAGE * (page - 1)) + 1,
                    Total = response.NumberOfResults,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.RESULTS_PER_PAGE,
                    PagedEntityType = Common.PagedEntityType.MAT
                };
            }

            return vm;
        }

        private string LatestMATTerm()
        {
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);
            return FormatHelpers.FinancialTermFormatAcademies(latestYear);
        }
    }
}