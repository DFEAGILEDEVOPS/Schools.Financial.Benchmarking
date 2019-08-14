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
using SFB.Web.Domain.Models;
using RedDog.Search.Model;
using SFB.Web.UI.Attributes;
using SFB.Web.Common.DataObjects;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class SchoolSearchController : Controller
    {
        private readonly ILocalAuthoritiesService _laService;
        private readonly ILaSearchService _laSearchService;
        private readonly ILocationSearchService _locationSearchService;
        private readonly IFilterBuilder _filterBuilder;
        private readonly IValidationService _valService;
        private readonly IContextDataService _contextDataService;
        private readonly ISchoolSearchService _schoolSearchService;
        private readonly ITrustSearchService _trustSearchService;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;

        public SchoolSearchController(ILocalAuthoritiesService laService, 
            ILaSearchService laSearchService, ILocationSearchService locationSearchService, IFilterBuilder filterBuilder,
            IValidationService valService, IContextDataService contextDataService,
            ISchoolSearchService schoolSearchService, ITrustSearchService trustSearchService,
            IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
        {
            _laService = laService;
            _laSearchService = laSearchService;
            _locationSearchService = locationSearchService;
            _filterBuilder = filterBuilder;
            _valService = valService;
            _contextDataService = contextDataService;
            _schoolSearchService = schoolSearchService;
            _trustSearchService = trustSearchService;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
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
            ViewBag.referrer = referrer;

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_NAME_ID:
                    var nameIdSanitized = Regex.Replace(nameId, @"(-|/)", "");
                    if (IsNumeric(nameIdSanitized))
                    {
                        errorMessage = _valService.ValidateSchoolIdParameter(nameIdSanitized);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            try
                            {
                                if(IsLaEstab(nameId))
                                {
                                    searchResp = _contextDataService.GetSchoolDataObjectByLaEstab(nameIdSanitized, openOnly);
                                    if (searchResp.Count == 0)
                                    {
                                        return View("EmptyResult", new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), SearchTypes.SEARCH_BY_NAME_ID));
                                    }
                                    else if(searchResp.Count == 1)
                                    {                                         
                                        return RedirectToAction("Detail", "School", new { urn = (searchResp as List<EdubaseDataObject>).First().URN });
                                    }
                                    else
                                    {
                                        searchResp = await GetSearchResults(nameId, SearchTypes.SEARCH_BY_LA_ESTAB, null, null, null, radius, openOnly, orderby, page);
                                    }
                                }
                                else
                                {
                                    searchResp = _contextDataService.GetSchoolDataObjectByUrn(Int32.Parse(nameIdSanitized));
                                    return RedirectToAction("Detail", "School", new { urn = searchResp.URN });
                                }
                            }
                            catch(Exception)
                            {
                                return View("EmptyResult",
                                    new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(),
                                        SearchTypes.SEARCH_BY_NAME_ID));
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
                                    new RouteValueDictionary {{"nameId", nameId}, { "openOnly", openOnly} });
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

                case SearchTypes.SEARCH_BY_TRUST_NAME_ID:
                    if(IsNumeric(trustNameId))
                    {
                        errorMessage = _valService.ValidateCompanyNoParameter(trustNameId);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            return RedirectToAction("Index", "Trust", new { companyNo = trustNameId });
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
                        errorMessage = _valService.ValidateTrustNameParameter(trustNameId);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            searchResp = await _trustSearchService.SearchTrustByName(trustNameId, 0, SearchDefaults.RESULTS_PER_PAGE, "", Request?.QueryString);
                            if (searchResp.NumberOfResults == 0)
                            {
                                return RedirectToActionPermanent("SuggestTrust", "Trust",
                                    new RouteValueDictionary { { "trustNameId", trustNameId } });
                            }
                            return RedirectToAction("Search", "Trust", new { name = trustNameId });
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
                                return await Search(nameId, trustNameId, searchType, suggestionUrn, locationorpostcode,
                                    locationCoordinates, laCodeName, radius, openOnly, orderby, page, tab, referrer);
                            }
                            return RedirectToAction("Search", "La", new {name = laCodeName, openOnly = openOnly, referrer = referrer});
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
                                    if (referrer != "schoolsearch/addschools")
                                    {
                                        return RedirectToAction("Detail", "School",
                                            new
                                            {
                                                urn = ((Domain.Models.QueryResultsModel)searchResp).Results.First()["URN"]
                                            });
                                    }
                                    break;
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
                        if (string.IsNullOrEmpty(locationCoordinates))
                        {
                            var result = _locationSearchService.SuggestLocationName(locationorpostcode);
                            switch (result.Matches.Count)
                            {
                                case 0:
                                    return View("EmptyLocationResult",
                                        new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType));
                                default:
                                    TempData["LocationResults"] = result;
                                    return RedirectToAction("Suggest", "Location", new { locationOrPostcode = locationorpostcode, openOnly = openOnly, referrer = referrer });
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
                                        new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType));
                                case 1:
                                    if (referrer != "schoolsearch/addschools")
                                    {
                                        return RedirectToAction("Detail", "School",
                                        new { urn = ((Domain.Models.QueryResultsModel)searchResp).Results.First()["URN"] });
                                    }
                                    break;
                            }
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

        public async Task<ActionResult> SuggestSchool(string nameId, bool openOnly = false)
        {
            var vm = new SchoolNotFoundViewModel
            {
                SearchKey = nameId,
                Suggestions = await _schoolSearchService.SuggestSchoolByName(nameId, openOnly)
            };
            return View("NotFound", vm);
        }

        public async Task<ActionResult> Suggest(string nameId, bool openOnly = false)
        {
            string json = null;

            if (!IsNumeric(nameId))
            {
                dynamic response = await _schoolSearchService.SuggestSchoolByName(nameId, openOnly);
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
            dynamic searchResponse;

            if (IsLaEstab(nameId))
            {
                searchResponse = await GetSearchResults(nameId, SearchTypes.SEARCH_BY_LA_ESTAB, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);
            }
            else
            {
                searchResponse = await GetSearchResults(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);
            }
            var vm = GetSchoolViewModelList(searchResponse, orderby,page, searchType, nameId, locationorpostcode, laCodeName);

            return PartialView("Partials/SchoolResults", vm);
        }

        [Route("SchoolSearch/Search-json")]
        public async Task<JsonResult> SearchJson(string nameId, string searchType, string suggestionurn,
            string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius, 
            int? companyNo, bool openOnly = false, string orderby = "", int page = 1)

        {
            dynamic searchResponse;
            if (!companyNo.HasValue)
            {
                if (IsLaEstab(nameId))
                {
                    searchResponse = await GetSearchResults(nameId, SearchTypes.SEARCH_BY_LA_ESTAB, locationorpostcode,
                        locationCoordinates, laCodeName, radius, openOnly, orderby, page, 1000);
                }
                else
                {
                    searchResponse = await GetSearchResults(nameId, searchType, locationorpostcode,
                        locationCoordinates, laCodeName, radius, openOnly, orderby, page, 1000);
                }
            }
            else
            {
                searchResponse = await _schoolSearchService.SearchSchoolByCompanyNo(companyNo.GetValueOrDefault(),
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
            QueryResultsModel response = null;

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_NAME_ID:
                    response = await _schoolSearchService.SearchSchoolByName(nameId,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take, orderby, 
                        Request.QueryString) as QueryResultsModel;
                    break;
                case SearchTypes.SEARCH_BY_LA_ESTAB:
                    response = await _schoolSearchService.SearchSchoolByLaEstab(nameId,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take, orderby,
                        Request.QueryString) as QueryResultsModel;
                    break;
                case SearchTypes.SEARCH_BY_LOCATION:
                        var latLng = locationCoordinates.Split(',');
                        response = await _schoolSearchService.SearchSchoolByLatLon(latLng[0], latLng[1],
                            (radius ?? SearchDefaults.LOCATION_SEARCH_DISTANCE) * 1.6m,
                            (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take, orderby,
                            Request.QueryString) as QueryResultsModel;
                    break;
                case SearchTypes.SEARCH_BY_LA_CODE_NAME:
                    response = await _schoolSearchService.SearchSchoolByLaCode(laCode,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take,
                        string.IsNullOrEmpty(orderby) ? "EstablishmentName" : orderby,
                        Request.QueryString) as QueryResultsModel;
                    break;
            }

            OrderFacetFilters(response);

            return response;
        }

        private void OrderFacetFilters(QueryResultsModel results)
        {
            if (results.Facets != null)
            {
                var orderedFacetFilters = new Dictionary<string, FacetResult[]>();
                foreach (var facet in results.Facets)
                {
                    if (facet.Key == "OverallPhase")
                    {
                        orderedFacetFilters.Add(facet.Key, facet.Value.OrderBy(fr => {
                            switch (fr.Value)
                            {
                                case "Nursery":
                                    return 1;
                                case "Primary":
                                    return 2;
                                case "Secondary":
                                    return 3;
                                case "All-through":
                                case "All through":
                                    return 4;
                                case "Pupil referral unit":
                                    return 5;
                                case "Special":
                                    return 6;
                                default:
                                    return 0; ;
                            }
                        }).ToArray());                        
                    }
                    else {
                        orderedFacetFilters.Add(facet.Key, facet.Value.OrderBy(fr => fr.Value).ToArray());
                    }
                }

                results.Facets = orderedFacetFilters;
            }
        }

        private string DetermineSelectionState(Models.Filter[] filters)
        {
            bool ofstedExpanded = false, schoolTypeExpanded = false, religiousCharacterExpanded = false, statusExpanded = false;

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

            if (filters.Where(x => x.Group == "establishmentStatus").Any(filter => filter.Metadata.Any(x => x.Checked)))
            {
                statusExpanded = true;
            }

            return
                $"{(ofstedExpanded ? "1" : "0")},{(schoolTypeExpanded ? "1" : "0")},{(religiousCharacterExpanded ? "1" : "0")},{(statusExpanded ? "1" : "0")}";
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

        private bool IsNumeric(string field) => field != null ? Regex.IsMatch(field, @"^\d+$") : false;
        private bool IsLaEstab(string field) => field != null ? Regex.IsMatch(field, "^[0-9]{3}(-|/)?[0-9]{4}$") : false;
        private bool IsURN(string field) => field != null ? Regex.IsMatch(field, "^[0-9]{6}$") : false;
    }
}