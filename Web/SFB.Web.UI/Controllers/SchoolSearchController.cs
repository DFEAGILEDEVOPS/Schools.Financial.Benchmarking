using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Services.Search;
using SFB.Web.UI.Helpers.Constants;
using System;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Attributes;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Services.LocalAuthorities;
using SFB.Web.UI.Services;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class SchoolSearchController : SearchBaseController
    {
        private readonly ILocalAuthoritiesService _laService;
        private readonly ILaSearchService _laSearchService;
        private readonly ILocationSearchService _locationSearchService;
        private readonly IValidationService _valService;
        private readonly IContextDataService _contextDataService;

        public SchoolSearchController(ILocalAuthoritiesService laService,
            ILaSearchService laSearchService, 
            ILocationSearchService locationSearchService, 
            IFilterBuilder filterBuilder,
            IValidationService valService, 
            IContextDataService contextDataService,
            ISchoolSearchService schoolSearchService,
            ISchoolBenchmarkListService benchmarkBasketService)
            : base(schoolSearchService, null, benchmarkBasketService, filterBuilder)
        {
            _laService = laService;
            _laSearchService = laSearchService;
            _locationSearchService = locationSearchService;
            _valService = valService;
            _contextDataService = contextDataService;
        }

        public async Task<ActionResult> Search(
            string nameId,
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
            string errorMessage = string.Empty;
            ViewBag.tab = tab;
            ViewBag.SearchMethod = "School";

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_NAME_ID:
                    var nameIdSanitized = Regex.Replace(nameId, @"(-|/)", "");
                    if (IsNumeric(nameIdSanitized))
                    {
                        return await SearchSchoolByUrnOrLaEstab(nameIdSanitized, openOnly, orderby, page, referrer);
                    }
                    else
                    {
                        return await SearchSchoolByName(nameIdSanitized, suggestionUrn, openOnly, orderby, page, referrer);
                    }
                case SearchTypes.SEARCH_BY_LOCATION:
                    if (string.IsNullOrEmpty(locationCoordinates))
                    {
                        return SearchSchoolByLocationName(locationorpostcode, openOnly, orderby, page, referrer);
                    }
                    else
                    {
                        return await SearchSchoolByLocationCoordinates(locationorpostcode, locationCoordinates, radius, openOnly, orderby, page, referrer);
                    }
                case SearchTypes.SEARCH_BY_LA_CODE_NAME:
                    if (IsNumeric(laCodeName))
                    {
                        return await SearchSchoolByLaCode(laCodeName, openOnly, orderby, page, referrer);
                    }
                    else
                    {
                        return await SearchSchoolByLaName(laCodeName, tab, openOnly, orderby, page, referrer);
                    }
                default:
                    return ErrorView(searchType, referrer, errorMessage, _schoolBenchmarkListService.GetSchoolBenchmarkList());
            }
        }

        /// <summary>
        /// Used by filtering and paging
        /// </summary>
        [Route("SchoolSearch/Search-js")]
        public async Task<PartialViewResult> SearchJS(
            string nameId, 
            string searchType, 
            string locationorpostcode, 
            string locationCoordinates, 
            string laCodeName, 
            decimal? radius, 
            bool openOnly = false,
            string orderby = "", 
            int page = 1)
        {
            ViewBag.SearchMethod = "School";
            dynamic searchResponse;
            var schoolComparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();

            if (IsLaEstab(nameId))
            {
                searchResponse = await GetSearchResultsAsync(nameId, SearchTypes.SEARCH_BY_LA_ESTAB, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);
            }
            else
            {
                searchResponse = await GetSearchResultsAsync(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);
            }
            var vm = GetSearchedSchoolViewModelList(searchResponse, schoolComparisonList, orderby, page, searchType, nameId, locationorpostcode, laCodeName);

            return PartialView("Partials/Search/SchoolResults", vm);
        }

        /// <summary>
        /// Used by the map widget
        /// </summary>
        [Route("SchoolSearch/Search-json")]
        public async Task<JsonResult> SearchJson(
            string nameId, 
            string searchType,
            string locationorpostcode, 
            string locationCoordinates, 
            string laCodeName, 
            decimal? radius, 
            int? companyNo,
            int? uid,
            bool openOnly = false, 
            string orderby = "", 
            int page = 1)
        {
            dynamic searchResponse;
            if (!companyNo.HasValue)
            {
                if (IsLaEstab(nameId))
                {
                    searchResponse = await GetSearchResultsAsync(nameId, SearchTypes.SEARCH_BY_LA_ESTAB, locationorpostcode,
                        locationCoordinates, laCodeName, radius, openOnly, orderby, page, SearchDefaults.SEARCHED_SCHOOLS_MAX);
                }
                else
                {
                    searchResponse = await GetSearchResultsAsync(nameId, searchType, locationorpostcode,
                        locationCoordinates, laCodeName, radius, openOnly, orderby, page, SearchDefaults.SEARCHED_SCHOOLS_MAX);
                }
            }
            else
            {               
                searchResponse = await _schoolSearchService.SearchAcademiesByUIDAsync(uid.GetValueOrDefault(),
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

        protected override async Task<dynamic> GetSearchResultsAsync(string nameId, string searchType, string locationorpostcode, string locationCoordinates, string laCode, decimal? radius, bool openOnly, string orderby, int page, int take = 50)
        {
            SearchResultsModel<SchoolSearchResult> response = null;

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_NAME_ID:
                    response = await _schoolSearchService.SearchSchoolByNameAsync(nameId,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take, orderby,
                        Request.QueryString);
                    break;
                case SearchTypes.SEARCH_BY_LA_ESTAB:
                    response = await _schoolSearchService.SearchSchoolByLaEstabAsync(nameId,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take, orderby,
                        Request.QueryString);
                    break;
                case SearchTypes.SEARCH_BY_LOCATION:
                    var latLng = locationCoordinates.Split(',');
                    response = await _schoolSearchService.SearchSchoolByLatLonAsync(latLng[0], latLng[1],
                        (radius ?? SearchDefaults.TRUST_LOCATION_SEARCH_DISTANCE) * 1.6m,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take, orderby,
                        Request.QueryString);
                    break;
                case SearchTypes.SEARCH_BY_LA_CODE_NAME:
                    response = await _schoolSearchService.SearchSchoolByLaCodeAsync(laCode,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take,
                        string.IsNullOrEmpty(orderby) ? EdubaseDataFieldNames.ESTAB_NAME : orderby,
                        Request.QueryString);
                    break;
            }

            OrderFacetFilters(response);

            return response;
        }

        public ActionResult AddSchools()
        {
            var schoolComparisonListModel = _schoolBenchmarkListService.GetSchoolBenchmarkList();
            var vm = new SearchViewModel(schoolComparisonListModel, "");
            vm.Authorities = _laService.GetLocalAuthorities();

            return View(vm);
        }

        public async Task<PartialViewResult> UpdateBenchmarkBasket(int urn, CookieActions withAction)
        {
            switch (withAction)
            {
                case CookieActions.SetDefault:
                    await _schoolBenchmarkListService.SetSchoolAsDefaultAsync(urn);
                    break;
                case CookieActions.Add:
                    await _schoolBenchmarkListService.AddSchoolToBenchmarkListAsync(urn);
                    break;
                case CookieActions.Remove:
                    await _schoolBenchmarkListService.RemoveSchoolFromBenchmarkListAsync(urn);
                    break;
                case CookieActions.RemoveAll:
                    _schoolBenchmarkListService.ClearSchoolBenchmarkList();
                    break;
                case CookieActions.UnsetDefault:
                    _schoolBenchmarkListService.UnsetDefaultSchool();
                    break;
            }

            return PartialView("Partials/BenchmarkListBanner",
                new SchoolViewModel(null, _schoolBenchmarkListService.GetSchoolBenchmarkList()));
        }

        public async Task<ActionResult> Suggest(string nameId, bool openOnly = false)
        {
            string json = null;

            if (!IsNumeric(nameId))
            {
                dynamic response = await _schoolSearchService.SuggestSchoolByNameAsync(nameId, openOnly);
                json = JsonConvert.SerializeObject(response);
            }

            return Content(json, "application/json");
        }

        private ActionResult ErrorView(string searchType, string referrer, string errorMessage, SchoolComparisonListModel schoolComparisonList)
        {
            var searchVM = new SearchViewModel(schoolComparisonList, searchType)
            {
                SearchType = searchType,
                ErrorMessage = errorMessage,
                Authorities = _laService.GetLocalAuthorities()
            };

            return View("../" + referrer, searchVM);
        }

        private async Task<ActionResult> SearchSchoolByUrnOrLaEstab(string nameId, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var schoolComparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
            dynamic searchResp = null;
            var errorMessage = _valService.ValidateSchoolIdParameter(nameId);
            if (string.IsNullOrEmpty(errorMessage))
            {
                try
                {
                    if (IsLaEstab(nameId))
                    {
                        searchResp = await _contextDataService.GetSchoolDataObjectByLaEstabAsync(nameId, openOnly);
                        if (searchResp.Count == 0)
                        {
                            return ErrorView(SearchTypes.SEARCH_BY_LA_ESTAB, referrer, SearchErrorMessages.NO_SCHOOL_NAME_RESULTS, schoolComparisonList);
                        }
                        else if (searchResp.Count == 1)
                        {
                            return RedirectToAction("Detail", "School", new { urn = (searchResp as List<EdubaseDataObject>).First().URN });
                        }
                        else
                        {
                            searchResp = await GetSearchResultsAsync(nameId, SearchTypes.SEARCH_BY_LA_ESTAB, null, null, null, null, openOnly, orderby, page);
                            return View("SearchResults", GetSearchedSchoolViewModelList(searchResp, schoolComparisonList, orderby, page, SearchTypes.SEARCH_BY_NAME_ID, nameId, null, null));
                        }
                    }
                    else
                    {
                        searchResp = await _contextDataService.GetSchoolDataObjectByUrnAsync(Int32.Parse(nameId));
                        return RedirectToAction("Detail", "School", new { urn = searchResp.URN });
                    }
                }
                catch (Exception)
                {
                    return ErrorView(SearchTypes.SEARCH_BY_LA_ESTAB, referrer, SearchErrorMessages.NO_SCHOOL_NAME_RESULTS, schoolComparisonList);
                }
            }
            else
            {
                return ErrorView(SearchTypes.SEARCH_BY_NAME_ID, referrer, errorMessage, schoolComparisonList);
            }
        }

        private async Task<ActionResult> SearchSchoolByName(string nameId, string suggestionUrn, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var schoolComparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
            dynamic searchResp = null;

            if (string.IsNullOrEmpty(_valService.ValidateSchoolIdParameter(suggestionUrn)))
            {
                return RedirectToAction("Detail", "School", new { urn = suggestionUrn });
            }

            var errorMessage = _valService.ValidateNameParameter(nameId);
            if (string.IsNullOrEmpty(errorMessage))
            {
                searchResp = await GetSearchResultsAsync(nameId, SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, openOnly, orderby, page);
                if(searchResp.NumberOfResults == 0)
                {
                    return ErrorView(SearchTypes.SEARCH_BY_NAME_ID, referrer, SearchErrorMessages.NO_SCHOOL_NAME_RESULTS, schoolComparisonList);
                }
                return View("SearchResults", GetSearchedSchoolViewModelList(searchResp, schoolComparisonList, orderby, page, SearchTypes.SEARCH_BY_NAME_ID, nameId, null, null));
            }
            else
            {
                return ErrorView(SearchTypes.SEARCH_BY_NAME_ID, referrer, errorMessage, schoolComparisonList);
            }
        }

        private async Task<ActionResult> SearchSchoolByLaName(string laName, string tab, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var schoolComparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
            var errorMessage = _valService.ValidateLaNameParameter(laName);
            if (string.IsNullOrEmpty(errorMessage))
            {
                var exactMatch = _laSearchService.SearchExactMatch(laName);
                if (exactMatch != null)
                {
                    laName = exactMatch.Id;
                    return await Search(null, SearchTypes.SEARCH_BY_LA_CODE_NAME, null, null, null, laName, null, openOnly, orderby, page, tab);
                }
                var similarMatch = _laSearchService.SearchContains(laName);
                if (similarMatch.Count == 0)
                {
                    return ErrorView(SearchTypes.SEARCH_BY_LA_CODE_NAME, referrer, SearchErrorMessages.NO_LA_RESULTS, schoolComparisonList);
                }
                TempData["SearchMethod"] = "Random";
                return RedirectToAction("Search", "La", new { name = laName, openOnly = openOnly });
            }
            else
            {
                return ErrorView(SearchTypes.SEARCH_BY_LA_CODE_NAME, referrer, errorMessage, schoolComparisonList);
            }
        }

        private async Task<ActionResult> SearchSchoolByLaCode(string laCode, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var schoolComparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
            var errorMessage = _valService.ValidateLaCodeParameter(laCode);
            if (string.IsNullOrEmpty(errorMessage))
            {
                var searchResp = await GetSearchResultsAsync(null, SearchTypes.SEARCH_BY_LA_CODE_NAME, null, null, laCode, null, openOnly, orderby, page);

                if (searchResp.NumberOfResults == 0)
                {
                    return ErrorView(SearchTypes.SEARCH_BY_LA_CODE_NAME, referrer, SearchErrorMessages.NO_LA_RESULTS, schoolComparisonList);
                }

                ViewBag.LaCodeName = laCode;
                return View("SearchResults", GetSearchedSchoolViewModelList(searchResp, schoolComparisonList, orderby, page, SearchTypes.SEARCH_BY_LA_CODE_NAME, null, null, _laService.GetLaName(laCode)));
            }
            else
            {
                return ErrorView(SearchTypes.SEARCH_BY_LA_CODE_NAME, referrer, errorMessage, schoolComparisonList);
            }
        }

        private ActionResult SearchSchoolByLocationName(string locationName, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var schoolComparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
            var errorMessage = _valService.ValidateLocationParameter(locationName);
            if (string.IsNullOrEmpty(errorMessage))
            {
                var result = _locationSearchService.SuggestLocationName(locationName);
                switch (result.Matches.Count)
                {
                    case 0:
                        return ErrorView(SearchTypes.SEARCH_BY_LOCATION, referrer, SearchErrorMessages.NO_LOCATION_RESULTS, schoolComparisonList);
                    default:
                        TempData["LocationResults"] = result;
                        TempData["SearchMethod"] = "Random";
                        return RedirectToAction("Suggest", "Location", new { locationOrPostcode = locationName, openOnly = openOnly });
                }
            }
            else
            {
                return ErrorView(SearchTypes.SEARCH_BY_LOCATION, referrer, errorMessage, schoolComparisonList);
            }
        }

        private async Task<ActionResult> SearchSchoolByLocationCoordinates(string locationOrPostCode, string locationCoordinates, decimal? radius, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var schoolComparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();

            var searchResp = await GetSearchResultsAsync(null, SearchTypes.SEARCH_BY_LOCATION, null, locationCoordinates, null, radius, openOnly, orderby, page);

            if (searchResp.NumberOfResults == 0)
            {
                return View("EmptyLocationResult", new SearchViewModel(schoolComparisonList, SearchTypes.SEARCH_BY_LOCATION));
            }

            return View("SearchResults", GetSearchedSchoolViewModelList(searchResp, schoolComparisonList, orderby, page, SearchTypes.SEARCH_BY_LOCATION, null, locationOrPostCode, null));
        }
    }
}