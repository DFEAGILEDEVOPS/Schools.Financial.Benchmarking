using Newtonsoft.Json;
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

        public TrustSearchController(ILocalAuthoritiesService laService,
            ILaSearchService laSearchService, ILocationSearchService locationSearchService, IFilterBuilder filterBuilder,
            IValidationService valService, IContextDataService contextDataService,
            ITrustSearchService trustSearchService,
            IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
            : base(null, trustSearchService, benchmarkBasketCookieManager, filterBuilder)
        {
            _laService = laService;
            _laSearchService = laSearchService;
            _locationSearchService = locationSearchService;
            _valService = valService;
            _contextDataService = contextDataService;
        }

        public async Task<ActionResult> Search(
        string nameId,
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
            string errorMessage;
            ViewBag.tab = tab;

            var schoolComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

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
                        else
                        {
                            var searchVM = new SearchViewModel(schoolComparisonList, searchType)
                            {
                                SearchType = searchType,
                                ErrorMessage = errorMessage,
                                Authorities = _laService.GetLocalAuthorities()
                            };

                            return View("../" + referrer, searchVM);
                        }
                    }
                    else
                    {
                        errorMessage = _valService.ValidateTrustNameParameter(trustNameId);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            searchResp = await _trustSearchService.SearchTrustByName(trustNameId, 0, SearchDefaults.RESULTS_PER_PAGE, "", Request?.QueryString);
                            if (searchResp.NumberOfResults == 0)
                            {
                                return RedirectToActionPermanent("SuggestTrust", "TrustSearch",
                                    new RouteValueDictionary { { "trustNameId", trustNameId } });
                            }

                            var response = await GetSearchResults(trustNameId, SearchTypes.SEARCH_BY_TRUST_NAME_ID, searchType, locationCoordinates, laCodeName, radius, openOnly, orderby, 1);

                            TrustListViewModel vm = GetTrustViewModelList(response, orderby, page);

                            return View("Partials/Search/TrustResults", vm);
                        }
                        else
                        {
                            var searchVM = new SearchViewModel(schoolComparisonList, searchType)
                            {
                                SearchType = searchType,
                                ErrorMessage = errorMessage,
                                Authorities = _laService.GetLocalAuthorities()
                            };

                            return View("../" + referrer, searchVM);
                        }
                    }
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
                                return await Search(nameId, trustNameId, searchType, suggestionUrn, locationorpostcode,
                                    locationCoordinates, laCodeName, radius, openOnly, orderby, page, tab);
                            }
                            TempData["SearchMethod"] = "Random";
                            return RedirectToAction("Search", "La", new { name = laCodeName, openOnly = openOnly });
                        }
                        else
                        {
                            var searchVM = new SearchViewModel(schoolComparisonList, searchType)
                            {
                                SearchType = searchType,
                                ErrorMessage = errorMessage,
                                Authorities = _laService.GetLocalAuthorities()
                            };

                            return View("../" + referrer, searchVM);
                        }
                    }
                    else
                    {
                        errorMessage = _valService.ValidateLaCodeParameter(laCodeName);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            searchResp = await GetSearchResults(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);

                            int resultCount = searchResp.NumberOfResults;
                            switch (resultCount)
                            {
                                case 0:
                                    return View("EmptyResult",
                                        new SearchViewModel(schoolComparisonList, searchType));
                                case 1:
                                    return RedirectToAction("Detail", "School",
                                        new
                                        {
                                            urn = ((Domain.Models.QueryResultsModel)searchResp).Results.First()["URN"]
                                        });
                            }
                        }
                        else
                        {
                            var searchVM = new SearchViewModel(schoolComparisonList, searchType)
                            {
                                SearchType = searchType,
                                ErrorMessage = errorMessage,
                                Authorities = _laService.GetLocalAuthorities()
                            };

                            return View("../" + referrer, searchVM);
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
                                        new SearchViewModel(schoolComparisonList, searchType));
                                default:
                                    TempData["LocationResults"] = result;
                                    TempData["SearchMethod"] = "Random";
                                    return RedirectToAction("Suggest", "Location", new { locationOrPostcode = locationorpostcode, openOnly = openOnly });
                            }
                        }
                        else
                        {
                            searchResp = await GetSearchResults(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);

                            int resultCnt = searchResp.NumberOfResults;
                            switch (resultCnt)
                            {
                                case 0:
                                    return View("EmptyLocationResult",
                                        new SearchViewModel(schoolComparisonList, searchType));
                                case 1:
                                    return RedirectToAction("Detail", "School",
                                        new { urn = ((Domain.Models.QueryResultsModel)searchResp).Results.First()["URN"] });
                            }
                        }
                    }
                    else
                    {
                        var searchVM = new SearchViewModel(schoolComparisonList, searchType)
                        {
                            SearchType = searchType,
                            ErrorMessage = errorMessage,
                            Authorities = _laService.GetLocalAuthorities()
                        };

                        return View("../" + referrer, searchVM);
                    }
                    break;
            }

            var laName = _laService.GetLaName(laCodeName);
            return View("SearchResults", GetSchoolViewModelList(searchResp, schoolComparisonList, orderby, page, searchType, nameId, locationorpostcode, laName));
        }

        [Route("TrustSearch/Search-js")]
        public async Task<PartialViewResult> SearchJS(string trustNameId, string searchType, string trustSuggestionUrn,
    string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius, bool openOnly = false,
    string orderby = "", int page = 1)

        {
            dynamic searchResponse;
            var schoolComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

            if (IsLaEstab(trustNameId))
            {
                searchResponse = await GetSearchResults(trustNameId, SearchTypes.SEARCH_BY_LA_ESTAB, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);
            }
            else
            {
                searchResponse = await GetSearchResults(trustNameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);
            }
            var vm = GetTrustViewModelList(searchResponse, orderby, page);

            return PartialView("Partials/Search/TrustResults", vm);
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

        private TrustListViewModel GetTrustViewModelList(dynamic response, string orderBy, int page)
        {
            var trustListVm = new List<TrustViewModel>();
            var vm = new TrustListViewModel(trustListVm, orderBy);
            if (response != null)
            {
                foreach (var result in response.Results)
                {
                    var trustVm = new TrustViewModel(int.Parse(result["CompanyNumber"]), result["TrustOrCompanyName"], null, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());
                    trustListVm.Add(trustVm);
                }

                vm.SchoolComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

                vm.Pagination = new Pagination
                {
                    Start = (SearchDefaults.RESULTS_PER_PAGE * (page - 1)) + 1,
                    Total = response.NumberOfResults,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.RESULTS_PER_PAGE
                };
            }

            return vm;
        }
    }
}