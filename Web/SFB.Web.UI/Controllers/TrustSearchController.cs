using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Services.Search;
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
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Services.LocalAuthorities;

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
            ITrustSearchService trustSearchService, ISchoolSearchService schoolSearchService,
            IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
            : base(schoolSearchService, trustSearchService, benchmarkBasketCookieManager, filterBuilder)
        {
            _laService = laService;
            _laSearchService = laSearchService;
            _locationSearchService = locationSearchService;
            _valService = valService;
            _contextDataService = contextDataService;
        }

        public async Task<ActionResult> Search(
        string trustNameId,
        string searchType,
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
            ViewBag.SearchMethod = "MAT";
            ViewBag.SearchType = searchType;

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_TRUST_NAME_ID:
                    if (IsNumeric(trustNameId))
                    {
                        return SearchByTrustId(trustNameId, openOnly, orderby, page, referrer);
                    }
                    else
                    {
                        return await SearchByTrustName(trustNameId, openOnly, orderby, page, referrer);
                    }
                case SearchTypes.SEARCH_BY_TRUST_LOCATION:
                    if (string.IsNullOrEmpty(locationCoordinates))
                    {
                        return SearchByTrustLocationOrPostcode(locationorpostcode, openOnly, orderby, page, referrer);
                    }
                    else
                    {
                        return await SearchByTrustLocationCoordinates(locationorpostcode, locationCoordinates, radius, openOnly, orderby, page, referrer);
                    }
                case SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME:
                    if (IsNumeric(laCodeName))
                    {
                        return await SearchByTrustLaCode(laCodeName, openOnly, orderby, page, referrer);                        
                    }
                    else
                    {
                        return await SearchByTrustLaName(laCodeName, tab, openOnly, orderby, page, referrer);
                    }
                default:
                    return ErrorView(searchType, referrer, errorMessage);
            }
        }

        [Route("TrustSearch/Search-js")]
        public async Task<PartialViewResult> SearchJS(string trustNameId, string searchType, string trustSuggestionUrn,
            string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius,
            bool openOnly = false, string orderby = "", int page = 1)
        {
            ViewBag.SearchMethod = "MAT";
            ViewBag.SearchType = searchType;

            string schoolLevelOrdering = DetermineSchoolLevelOrdering(orderby);

            dynamic searchResponse = await GetSearchResultsAsync(trustNameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, schoolLevelOrdering, page);

            TrustListViewModel trustsVm;
            if (searchType == SearchTypes.SEARCH_BY_TRUST_NAME_ID)
            {
                trustsVm = await GetTrustListViewModelAsync(searchResponse, orderby, page, searchType, trustNameId, locationorpostcode, null);
            }
            else
            {
                trustsVm = await BuildTrustViewModelListFromFoundAcademiesAsync(searchResponse, schoolLevelOrdering, page, searchType, trustNameId, locationorpostcode, null);
            }

            ApplyTrustLevelOrdering(orderby, trustsVm);

            return PartialView("Partials/TrustResults", trustsVm);
        }

        [Route("TrustSearch/Search-json")]
        public async Task<JsonResult> SearchJson(string trustNameId, string searchType, string trustSuggestionUrn,
            string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius,
            bool openOnly = false, string orderby = "", int page = 1)
        {
            dynamic searchResponse = await GetSearchResultsAsync(trustNameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, "", page, SearchDefaults.SEARCHED_SCHOOLS_MAX);

            TrustListViewModel trusts;
            List<SchoolSummaryViewModel> results = new List<SchoolSummaryViewModel>();
            if (searchType == SearchTypes.SEARCH_BY_TRUST_NAME_ID)
            {
                trusts = await GetTrustListViewModelAsync(searchResponse, orderby, page, searchType, trustNameId, locationorpostcode, null);
                foreach (var trust in trusts.ModelList)
                {
                    var schoolSearchResponse = await _schoolSearchService.SearchSchoolByCompanyNoAsync(trust.CompanyNo, 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, null, null);
                    foreach (var school in schoolSearchResponse.Results)
                    {
                        var schoolVm = new SchoolSummaryViewModel(school);
                        results.Add(schoolVm);
                    }
                }
            }
            else
            {
                foreach (var result in searchResponse.Results)
                {
                    var schoolVm = new SchoolSummaryViewModel(result);
                    results.Add(schoolVm);
                }
            }

            return Json(new { count = results.Count, results = results }, JsonRequestBehavior.AllowGet);
        }

        protected override async Task<dynamic> GetSearchResultsAsync(string nameId, string searchType, string locationorpostcode, string locationCoordinates, string laCode, decimal? radius, bool openOnly, string orderby, int page, int take = 50)
        {
            QueryResultsModel response = null;

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_TRUST_NAME_ID:
                    response = await _trustSearchService.SearchTrustByName(nameId,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE,
                        SearchDefaults.SEARCHED_SCHOOLS_MAX, orderby, Request?.QueryString);
                    break;
                case SearchTypes.SEARCH_BY_TRUST_LOCATION:
                    var latLng = locationCoordinates.Split(',');
                    response = await _schoolSearchService.SearchSchoolByLatLon(latLng[0], latLng[1],
                        (radius ?? SearchDefaults.TRUST_LOCATION_SEARCH_DISTANCE) * 1.6m,
                        0, SearchDefaults.SEARCHED_SCHOOLS_MAX, orderby,
                        Request?.QueryString) as QueryResultsModel;
                    break;
                case SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME:
                    response = await _schoolSearchService.SearchSchoolByLaCode(laCode,
                        0, SearchDefaults.SEARCHED_SCHOOLS_MAX, orderby,
                        Request?.QueryString) as QueryResultsModel;
                    break;
            }

            return response;
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

        private async Task<TrustListViewModel> GetTrustListViewModelAsync(dynamic trustSearchResults, string orderBy, int page, string searchType, string nameKeyword, string locationKeyword, string laKeyword)
        {
            var academyTrustList = new List<AcademyTrustViewModel>();

            foreach (var result in trustSearchResults.Results)
            {
                var companyNo = int.Parse(result[EdubaseDataFieldNames.COMPANY_NUMBER]);
                var companyName = result[SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME];
                IEnumerable<EdubaseDataObject> academiesOfTrust = await _contextDataService.GetSchoolsByCompanyNumberAsync(companyNo);

                var academiesList = academiesOfTrust.Select(a => new SchoolViewModel(a)).OrderBy(a => a.Name).ToList();

                if (academiesList.Count > 0)
                {
                    academyTrustList.Add(new AcademyTrustViewModel(companyNo, companyName, academiesList));
                }
            }

            var trustListViewModel = new TrustListViewModel(academyTrustList, null, searchType, nameKeyword, locationKeyword, laKeyword, orderBy)
            {
                SchoolComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(),

                Pagination = new Pagination
                {
                    Start = (SearchDefaults.RESULTS_PER_PAGE * (page - 1)) + 1,
                    Total = academyTrustList.Count,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.RESULTS_PER_PAGE,
                    PagedEntityType = PagedEntityType.MAT
                }
            };

            return trustListViewModel;
        }

        private async Task<TrustListViewModel> BuildTrustViewModelListFromFoundAcademiesAsync(dynamic academySearchResults, string orderBy, int page, string searchType, string nameKeyword, string locationKeyword, string laKeyword)
        {
            var academyTrustList = new List<AcademyTrustViewModel>();

            foreach (var academySearchResult in academySearchResults.Results)
            {
                if (int.TryParse(academySearchResult[EdubaseDataFieldNames.COMPANY_NUMBER], out int companyNo))
                {
                    if (!academyTrustList.Any(t => t.CompanyNo == companyNo))
                    {
                        var academyTrust = new AcademyTrustViewModel(companyNo, academySearchResult[EdubaseDataFieldNames.TRUSTS], _contextDataService.GetSchoolsByCompanyNumberAsync(companyNo));
                        academyTrustList.Add(academyTrust);
                    }
                }
            }

            foreach (var academyTrust in academyTrustList)
            {
                var result = await academyTrust.AcademiesListBuilderTask;
                academyTrust.AcademiesList = result.Select(a => new SchoolViewModel(a)).OrderBy(a => a.Name).ToList();
            }

            MarkAcademiesInsideSearchArea(academySearchResults, academyTrustList);

            var trustListViewModel = new TrustListViewModel(academyTrustList, null, searchType, nameKeyword, locationKeyword, laKeyword, orderBy)
            {
                Pagination = new Pagination
                {
                    Start = (SearchDefaults.RESULTS_PER_PAGE * (page - 1)) + 1,
                    Total = academyTrustList.Count,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.SEARCHED_SCHOOLS_MAX,
                    PagedEntityType = PagedEntityType.MAT
                }
            };

            return trustListViewModel;
        }

        private static void MarkAcademiesInsideSearchArea(dynamic academySearchResults, List<AcademyTrustViewModel> trustList)
        {
            foreach (var academySearchResult in academySearchResults.Results)
            {
                var schoolMatch = trustList.SelectMany(t => t.AcademiesList).Where(a => a.Id.ToString() == academySearchResult[EdubaseDataFieldNames.URN]).FirstOrDefault();
                if (schoolMatch != null)
                {
                    schoolMatch.InsideSearchArea = true;
                }
            }
        }

        private void ApplyTrustLevelOrdering(string orderby, TrustListViewModel trustsVm)
        {
            if (orderby == "MatSchoolNumber")
            {
                trustsVm.ModelList = trustsVm.ModelList.OrderByDescending(t => t.AcademiesList.Count).ToList();
                trustsVm.OrderBy = orderby;
            }

            if (orderby == "AreaSchoolNumber")
            {
                trustsVm.ModelList = trustsVm.ModelList.OrderByDescending(t => t.AcademiesList.Where(a => a.InsideSearchArea).ToList().Count).ToList();
                trustsVm.OrderBy = orderby;
            }
        }

        private string DetermineSchoolLevelOrdering(string orderby)
        {
            var schoolLevelOrdering = orderby;
            if (orderby == "AreaSchoolNumber" || orderby == "MatSchoolNumber")
            {
                schoolLevelOrdering = $"{EdubaseDataFieldNames.TRUSTS} asc";
            }

            return schoolLevelOrdering;
        }

        private ActionResult ErrorView(string searchType, string referrer, string errorMessage)
        {
            var searchVM = new SearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType)
            {
                SearchType = searchType,
                ErrorMessage = errorMessage,
                Authorities = _laService.GetLocalAuthorities()
            };

            return View("../" + referrer, searchVM);
        }

        private ActionResult SearchByTrustId(string trustNameId, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var errorMessage = _valService.ValidateCompanyNoParameter(trustNameId);
            if (string.IsNullOrEmpty(errorMessage))
            {
                return RedirectToAction("Index", "Trust", new { companyNo = trustNameId });
            }
            else
            {
                return ErrorView(SearchTypes.SEARCH_BY_TRUST_NAME_ID, referrer, errorMessage);
            }
        }

        private async Task<ActionResult> SearchByTrustName(string trustName, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var errorMessage = _valService.ValidateTrustNameParameter(trustName);
            if (string.IsNullOrEmpty(errorMessage))
            {
                var searchResults = await GetSearchResultsAsync(trustName, SearchTypes.SEARCH_BY_TRUST_NAME_ID, null, null, null, null, openOnly, orderby, 1);
                if (searchResults.NumberOfResults == 0)
                {
                    return RedirectToActionPermanent("SuggestTrust", "TrustSearch", new RouteValueDictionary { { "trustNameId", trustName } });
                }

                var trustVm = await GetTrustListViewModelAsync(searchResults, orderby, page, SearchTypes.SEARCH_BY_TRUST_NAME_ID, trustName, null, null);

                return View("SearchResults", trustVm);
            }
            else
            {
                return ErrorView(SearchTypes.SEARCH_BY_TRUST_NAME_ID, referrer, errorMessage);
            }
        }

        private ActionResult SearchByTrustLocationOrPostcode(string locationOrPostCode, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var errorMessage = _valService.ValidateLocationParameter(locationOrPostCode);
            if (string.IsNullOrEmpty(errorMessage))
            {
                var result = _locationSearchService.SuggestLocationName(locationOrPostCode);
                switch (result.Matches.Count)
                {
                    case 0:
                        return View("EmptyLocationResult", new SearchViewModel(null, SearchTypes.SEARCH_BY_TRUST_LOCATION));
                    default:
                        TempData["LocationResults"] = result;
                        TempData["SearchMethod"] = "MAT";
                        return RedirectToAction("Suggest", "Location", new { locationOrPostcode = locationOrPostCode });
                }
            }
            else
            {
                return ErrorView(SearchTypes.SEARCH_BY_TRUST_LOCATION, referrer, errorMessage);
            }
        }

        private async Task<ActionResult> SearchByTrustLocationCoordinates(string locationOrPostcode, string locationCoordinates, decimal? radius, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var schoolLevelOrdering = DetermineSchoolLevelOrdering(orderby);

            var searchResults = await GetSearchResultsAsync(null, SearchTypes.SEARCH_BY_TRUST_LOCATION, null, locationCoordinates, null, radius, openOnly, schoolLevelOrdering, page);

            if (searchResults.NumberOfResults == 0)
            {
                return View("EmptyLocationResult", new SearchViewModel(null, SearchTypes.SEARCH_BY_TRUST_LOCATION));
            }

            var trustsVm = await BuildTrustViewModelListFromFoundAcademiesAsync(searchResults, orderby, page, SearchTypes.SEARCH_BY_TRUST_LOCATION, null, locationOrPostcode, null);

            ApplyTrustLevelOrdering(orderby, trustsVm);

            return View("SearchResults", trustsVm);
        }

        private async Task<ActionResult> SearchByTrustLaName(string laName, string tab, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var errorMessage = _valService.ValidateLaNameParameter(laName);
            if (string.IsNullOrEmpty(errorMessage))
            {
                var exactMatch = _laSearchService.SearchExactMatch(laName);
                if (exactMatch != null)
                {
                    laName = exactMatch.Id;
                    return await Search(null, SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, null, null, laName, null, openOnly, orderby, page, tab);
                }
                TempData["SearchMethod"] = "MAT";
                return RedirectToAction("Search", "La", new { name = laName, openOnly = openOnly });
            }
            else
            {
                return ErrorView(SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, referrer, errorMessage);
            }
        }

        private async Task<ActionResult> SearchByTrustLaCode(string laCode, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var errorMessage = _valService.ValidateLaCodeParameter(laCode);
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (string.IsNullOrEmpty(orderby))
                {
                    orderby = $"{EdubaseDataFieldNames.TRUSTS} asc";
                }

                var schoolLevelOrdering = DetermineSchoolLevelOrdering(orderby);

                var searchResults = await GetSearchResultsAsync(null, SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, null, null, laCode, null, openOnly, schoolLevelOrdering, page);

                if (searchResults.NumberOfResults == 0)
                {
                    return View("EmptyResult", new SearchViewModel(null, SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME));
                }

                var trustsVm = await BuildTrustViewModelListFromFoundAcademiesAsync(searchResults, orderby, page, SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, null, null, _laService.GetLaName(laCode));

                ApplyTrustLevelOrdering(orderby, trustsVm);

                return View("SearchResults", trustsVm);
            }
            else
            {
                return ErrorView(SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, referrer, errorMessage);
            }
        }
    }
}