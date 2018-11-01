using Newtonsoft.Json;
using SFB.Web.Domain.Services;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Domain.Services.Search;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Services;
using System;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Controllers
{
    public class SchoolSearchController : Controller
    {
        private readonly ILocalAuthoritiesService _laService;
        private readonly ILaSearchService _laSearchService;
        private readonly IFilterBuilder _filterBuilder;
        private readonly IValidationService _valService;
        private readonly IContextDataService _contextDataService;
        private readonly ISchoolSearchService _schoolSearchService;
        private readonly ITrustSearchService _trustSearchService;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;

        public SchoolSearchController(ILocalAuthoritiesService laService, 
            ILaSearchService laSearchService, IFilterBuilder filterBuilder,
            IValidationService valService, IContextDataService contextDataService,
            ISchoolSearchService schoolSearchService, ITrustSearchService trustSearchService,
            IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
        {
            _laService = laService;
            _laSearchService = laSearchService;
            _filterBuilder = filterBuilder;
            _valService = valService;
            _contextDataService = contextDataService;
            _schoolSearchService = schoolSearchService;
            _trustSearchService = trustSearchService;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
        }

        public async Task<ActionResult> Search(
            string nameId,
            string trustName,
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

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_NAME_ID:
                    var nameIdSanitized = Regex.Replace(nameId, @"(-|/)", "");
                    if (IsNumeric(nameIdSanitized))
                    {
                        errorMessage = _valService.ValidateSchoolIdParameter(nameIdSanitized);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            searchResp = IsLaEstab(nameId)
                                ? _contextDataService.GetSchoolDataObjectByLaEstab(nameIdSanitized)
                                : _contextDataService.GetSchoolDataObjectByUrn(Int32.Parse(nameIdSanitized));

                            if (searchResp == null)
                            {
                                return View("EmptyResult",
                                    new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(),
                                        SearchTypes.SEARCH_BY_NAME_ID));
                            }                                                       

                            return RedirectToAction("Detail", "School", new {urn = searchResp.URN});
                        }
                        else
                        {
                            var searchVM = new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType)
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
                        if (string.IsNullOrEmpty(_valService.ValidateSchoolIdParameter(suggestionUrn)))
                        {
                            return RedirectToAction("Detail", "School", new {urn = suggestionUrn});
                        }

                        errorMessage = _valService.ValidateNameParameter(nameId);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            // first see if we get a match on the word
                            searchResp = await GetSearchResults(nameId, searchType, null, null, null, radius, openOnly, orderby, page);
                            if (searchResp.NumberOfResults == 0)
                            {
                                return RedirectToActionPermanent("SuggestSchool", "SchoolSearch",
                                    new RouteValueDictionary {{"nameId", nameId}});
                            }
                        }
                        else
                        {
                            var searchVM = new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType)
                            {
                                SearchType = searchType,
                                ErrorMessage = errorMessage,
                                Authorities = _laService.GetLocalAuthorities()
                            };

                            return View("../" + referrer, searchVM);
                        }
                    }
                    break;

                case SearchTypes.SEARCH_BY_TRUST_NAME:

                    errorMessage = _valService.ValidateTrustNameParameter(trustName);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        return RedirectToAction("Search", "Trust", new {name = trustName});
                    }
                    else
                    {
                        var searchVM = new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType)
                        {
                            SearchType = searchType,
                            ErrorMessage = errorMessage,
                            Authorities = _laService.GetLocalAuthorities()
                        };

                        return View("../" + referrer, searchVM);
                    }

                case SearchTypes.SEARCH_BY_LA_CODE_NAME:
                    if (!IsNumeric(laCodeName))
                    {
                        errorMessage = _valService.ValidateLaNameParameter(laCodeName);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            var exactMatch = _laSearchService.SearchExactMatch(laCodeName);
                            if (exactMatch != null)
                            {
                                laCodeName = exactMatch.id;
                                return await Search(nameId, trustName, searchType, suggestionUrn, locationorpostcode,
                                    locationCoordinates, laCodeName, radius, openOnly, orderby, page, tab);
                            }
                            return RedirectToAction("Search", "La", new {name = laCodeName, openOnly = openOnly});
                        }
                        else
                        {
                            var searchVM = new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType)
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
                                        new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType));
                                case 1:
                                    return RedirectToAction("Detail", "School",
                                        new
                                        {
                                            urn = ((Domain.Models.QueryResultsModel) searchResp).Results.First()["URN"]
                                        });
                            }
                        }
                        else
                        {
                            var searchVM = new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType)
                            {
                                SearchType = searchType,
                                ErrorMessage = errorMessage,
                                Authorities = _laService.GetLocalAuthorities()
                            };

                            return View("../" + referrer, searchVM);
                        }
                    }

                    break;

                case SearchTypes.SEARCH_BY_LOCATION:
                    errorMessage = _valService.ValidateLocationParameter(locationorpostcode);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        searchResp = await GetSearchResults(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);

                        int resultCnt = searchResp.NumberOfResults;
                        switch (resultCnt)
                        {
                            case 0:
                                return View("EmptyLocationResult",
                                    new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType));
                            case 1:
                                return RedirectToAction("Detail", "School",
                                    new {urn = ((Domain.Models.QueryResultsModel) searchResp).Results.First()["URN"]});
                        }
                    }
                    else
                    {
                        var searchVM = new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType)
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
            return View("SearchResults", GetSchoolViewModelList(searchResp, orderby, page, searchType, nameId, locationorpostcode, laName));
        }

        public ActionResult AddSchools()
        {
            var schoolComparisonListModel = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            var vm = new SchoolSearchViewModel(schoolComparisonListModel, "");
            vm.Authorities = _laService.GetLocalAuthorities();

            return View(vm);
        }

        public PartialViewResult UpdateBenchmarkBasket(int urn, CookieActions withAction)
        {
            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), null);

            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(withAction,
                new BenchmarkSchoolModel()
                {
                    Name = benchmarkSchool.Name,
                    Urn = benchmarkSchool.Id.ToString(),
                    Type = benchmarkSchool.Type,
                    EstabType = benchmarkSchool.EstablishmentType.ToString()
                });                      

            return PartialView("Partials/BenchmarkListBanner",
                new SchoolViewModel(null, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie()));
        }

        public async Task<ActionResult> SuggestSchool(string nameId)
        {
            var vm = new SchoolNotFoundViewModel
            {
                SearchKey = nameId,
                Suggestions = await _schoolSearchService.SuggestSchoolByName(nameId)
            };
            return View("NotFound", vm);
        }

        public async Task<ActionResult> Suggest(string nameId)
        {
            string json = null;

            if (!IsNumeric(nameId))
            {
                dynamic response = await _schoolSearchService.SuggestSchoolByName(nameId);
                json = JsonConvert.SerializeObject(response);
            }

            return Content(json, "application/json");
        }

        public async Task<ActionResult> SuggestTrust(string name)
        {
            dynamic response = await _trustSearchService.SuggestTrustByName(name);

            var json = JsonConvert.SerializeObject(response);
            return Content(json, "application/json");
        }

        [Route("SchoolSearch/Search-js")]
        public async Task<PartialViewResult> SearchJS(string nameId, string searchType, string suggestionurn,
            string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius, bool openOnly = false,
            string orderby = "", int page = 1)

        {
            var searchResponse = await GetSearchResults(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);
            var vm = GetSchoolViewModelList(searchResponse, orderby,page, searchType, nameId, locationorpostcode, laCodeName);

            return PartialView("Partials/SchoolResults", vm);
        }

        [Route("SchoolSearch/Search-json")]
        public async Task<JsonResult> SearchJson(string nameId, string searchType, string suggestionurn,
            string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius, 
            string matNo, bool openOnly = false, string orderby = "", int page = 1)

        {
            dynamic searchResponse;
            if (string.IsNullOrEmpty(matNo))
            {
                searchResponse = await GetSearchResults(nameId, searchType, locationorpostcode,
                    locationCoordinates, laCodeName, radius, openOnly, orderby, page, 1000);
            }
            else
            {
                searchResponse = await _schoolSearchService.SearchSchoolByMatNo(matNo,
                    0, SearchDefaults.TRUST_SCHOOLS_PER_PAGE, "", Request.QueryString);
            }

            var results = new List<SchoolSummaryViewModel>();
            foreach (var result in searchResponse.Results)
            {
                var schoolVm = new SchoolSummaryViewModel(result);
                results.Add(schoolVm);
            }

            return Json(new { count = results.Count, results = results }, JsonRequestBehavior.AllowGet);
        }

        private async Task<dynamic> GetSearchResults(
            string nameId,
            string searchType,
            string locationorpostcode,
            string locationCoordinates,
            string laCode,
            decimal? radius,
            bool openOnly,
            string orderby,
            int page,
            int take = SearchDefaults.RESULTS_PER_PAGE)
        {
            dynamic response = null;

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_NAME_ID:
                    response = await _schoolSearchService.SearchSchoolByName(nameId,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take, orderby, 
                        Request.QueryString);
                    break;
                case SearchTypes.SEARCH_BY_LOCATION:
                    if (string.IsNullOrEmpty(locationCoordinates))
                    {
                        response = await _schoolSearchService.SearchSchoolByLocation(locationorpostcode,
                            (radius ?? SearchDefaults.LOCATION_SEARCH_DISTANCE) * 1.6m,
                            (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take, orderby,
                            Request.QueryString);
                    }
                    else
                    {
                        var latLng = locationCoordinates.Split(',');
                        response = await _schoolSearchService.SearchSchoolByLocation(latLng[0], latLng[1],
                            (radius ?? SearchDefaults.LOCATION_SEARCH_DISTANCE) * 1.6m,
                            (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take, orderby,
                            Request.QueryString);
                    }
                    break;
                case SearchTypes.SEARCH_BY_LA_CODE_NAME:
                    response = await _schoolSearchService.SearchSchoolByLaCode(laCode,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take,
                        string.IsNullOrEmpty(orderby) ? "EstablishmentName" : orderby,
                        Request.QueryString);
                    break;
            }
            return response;
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

        private SearchedSchoolListViewModel GetSchoolViewModelList(dynamic response, string orderBy, int page, string searchType, string nameKeyword, string locationKeyword, string laKeyword)
        {
            var schoolListVm = new List<SchoolSearchResultViewModel>();
            var vm = new SearchedSchoolListViewModel(schoolListVm, null, searchType, nameKeyword, locationKeyword, laKeyword, orderBy);
            if (response != null)
            {
                foreach (var result in response.Results)
                {
                    var schoolVm = new SchoolSearchResultViewModel(result);
                    schoolListVm.Add(schoolVm);
                }

                vm.SchoolComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

                var filters = _filterBuilder.ConstructSchoolSearchFilters(Request.QueryString, response.Facets);
                vm.Filters = filters;
                vm.FilterSelectionState = DetermineSelectionState(filters);

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

        private bool IsNumeric(string field) => Regex.IsMatch(field, @"^\d+$");
        private bool IsLaEstab(string field) => Regex.IsMatch(field, "^[0-9]{3}(-|/)?[0-9]{4}$");
        private bool IsURN(string field) => Regex.IsMatch(field, "^[0-9]{5}$");
    }
}