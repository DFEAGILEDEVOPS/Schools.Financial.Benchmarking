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
            ISchoolBenchmarkListService benchmarkBasketService,
            IPlacesLookupService placesLookupService)
            : base(schoolSearchService, trustSearchService, benchmarkBasketService, filterBuilder, placesLookupService)
        {
            _laService = laService;
            _laSearchService = laSearchService;
            _locationSearchService = locationSearchService;
            _valService = valService;
            _contextDataService = contextDataService;
        }

        public async Task<ActionResult> Search(
        string trustNameId,
        string trustsuggestionUrn,
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
                        return await SearchByTrustIdAsync(trustNameId, openOnly, orderby, page, referrer);
                    }
                    else
                    {
                        return await SearchByTrustName(trustNameId, trustsuggestionUrn, openOnly, orderby, page, referrer);
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
                    var schoolSearchResponse = await _schoolSearchService.SearchAcademiesByUIDAsync(trust.Uid, 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, null, null);
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
            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_TRUST_NAME_ID:
                   return await _trustSearchService.SearchTrustByNameAsync(nameId,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE,
                        SearchDefaults.SEARCHED_SCHOOLS_MAX, orderby, Request?.QueryString);
                case SearchTypes.SEARCH_BY_TRUST_LOCATION:
                    var latLng = locationCoordinates.Split(',');
                    return await _schoolSearchService.SearchSchoolByLatLonAsync(latLng[0], latLng[1],
                        (radius ?? SearchDefaults.TRUST_LOCATION_SEARCH_DISTANCE) * 1.6m,
                        0, SearchDefaults.SEARCHED_SCHOOLS_MAX, orderby,
                        Request?.QueryString);
                case SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME:
                    return await _schoolSearchService.SearchSchoolByLaCodeAsync(laCode,
                        0, SearchDefaults.SEARCHED_SCHOOLS_MAX, orderby,
                        Request?.QueryString);
                default:
                    return null;
            }
        }

        public async Task<ActionResult> Suggest(string name)
        {
            dynamic response = await _trustSearchService.SuggestTrustByNameAsync(name);

            var json = JsonConvert.SerializeObject(response.Matches);
            return Content(json, "application/json");
        }

        public async Task<ActionResult> SuggestTrust(string trustNameId)
        {
            var vm = new SchoolNotFoundViewModel
            {
                SearchKey = trustNameId,
                Suggestions = await _trustSearchService.SuggestTrustByNameAsync(trustNameId)
            };
            return View("NotFound", vm);
        }
        
        private async Task<TrustListViewModel> GetTrustListViewModelAsync(dynamic trustSearchResults, string orderBy, int page, string searchType, string nameKeyword, string locationKeyword, string laKeyword)
        {
            var academyTrustList = new List<AcademyTrustViewModel>();

            foreach (var result in trustSearchResults.Results)
            {
                int.TryParse(result.CompanyNumber, out int companyNo);
                int.TryParse(result.Uid, out int uid);
                var companyName = result.TrustOrCompanyName;
                IEnumerable<EdubaseDataObject> academiesOfTrust = await _contextDataService.GetAcademiesByCompanyNumberAsync(companyNo);

                var academiesList = academiesOfTrust.Select(a => new SchoolViewModel(a)).OrderBy(a => a.Name).ToList();

                if (academiesList.Count > 0)
                {
                    academyTrustList.Add(new AcademyTrustViewModel(uid, companyNo, companyName, academiesList));
                }
            }

            var trustListViewModel = new TrustListViewModel(academyTrustList, null, searchType, nameKeyword, locationKeyword, laKeyword, orderBy)
            {
                SchoolComparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList(),

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

        private async Task<TrustListViewModel> BuildTrustViewModelListFromFoundAcademiesAsync(SearchResultsModel<SchoolSearchResult> academySearchResults, string orderBy, int page, string searchType, string nameKeyword, string locationKeyword, string laKeyword)
        {
            var academyTrustList = new List<AcademyTrustViewModel>();

            foreach (var academySearchResult in academySearchResults.Results)
            {
                if (int.TryParse(academySearchResult.UID, out int uid))
                {
                    if (!academyTrustList.Any(t => t.Uid == uid))
                    {
                        int.TryParse(academySearchResult.CompanyNumber, out int companyNo);
                        var academyTrust = new AcademyTrustViewModel(uid, companyNo, academySearchResult.Trusts, _contextDataService.GetAcademiesByUidAsync(uid));
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

        private void MarkAcademiesInsideSearchArea(SearchResultsModel<SchoolSearchResult> academySearchResults, List<AcademyTrustViewModel> trustList)
        {
            foreach (var academySearchResult in academySearchResults.Results)
            {
                var schoolMatch = trustList.SelectMany(t => t.AcademiesList).Where(a => a.Id.ToString() == academySearchResult.URN).FirstOrDefault();
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
            var searchVM = new SearchViewModel(_schoolBenchmarkListService.GetSchoolBenchmarkList(), searchType)
            {
                SearchType = searchType,
                ErrorMessage = errorMessage,
                Authorities = _laService.GetLocalAuthorities()
            };

            return View("../" + referrer, searchVM);
        }

        private async Task<ActionResult> SearchByTrustIdAsync(string trustNameId, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            var errorMessage = _valService.ValidateCompanyNoParameter(trustNameId);
            if (string.IsNullOrEmpty(errorMessage))
            {
                var result = await _trustSearchService.SearchTrustByCompanyNoAsync(trustNameId, 0, 1, "", null);
                if(result.NumberOfResults == 0)
                {
                    return ErrorView(SearchTypes.SEARCH_BY_TRUST_NAME_ID, referrer, SearchErrorMessages.NO_TRUST_NAME_RESULTS);
                }

                return RedirectToAction("Detail", "Trust", new { companyNo = trustNameId });
            }
            else
            {
                return ErrorView(SearchTypes.SEARCH_BY_TRUST_NAME_ID, referrer, errorMessage);
            }
        }

        private async Task<ActionResult> SearchByTrustName(string trustName, string suggestionId, bool openOnly = false, string orderby = "", int page = 1, string referrer = "home/index")
        {
            if (string.IsNullOrEmpty(_valService.ValidateCompanyNoParameter(suggestionId)))
            {
                return RedirectToAction("Detail", "Trust", new { companyNo = suggestionId });
            }

            var errorMessage = _valService.ValidateTrustNameParameter(trustName);
            if (string.IsNullOrEmpty(errorMessage))
            {
                var searchResults = await GetSearchResultsAsync(trustName, SearchTypes.SEARCH_BY_TRUST_NAME_ID, null, null, null, null, openOnly, orderby, 1);

                if (searchResults.NumberOfResults == 0)
                {
                    return ErrorView(SearchTypes.SEARCH_BY_TRUST_NAME_ID, referrer, SearchErrorMessages.NO_TRUST_NAME_RESULTS);
                }

                if (searchResults.NumberOfResults == 1)
                {
                    var res = (TrustSearchResult)System.Linq.Enumerable.First(searchResults.Results);
                    return RedirectToAction("Detail", "Trust", new { companyNo = res.CompanyNumber });
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
                        return ErrorView(SearchTypes.SEARCH_BY_TRUST_LOCATION, referrer, SearchErrorMessages.NO_LOCATION_RESULTS);
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

            SearchResultsModel<SchoolSearchResult> searchResults = await GetSearchResultsAsync(null, SearchTypes.SEARCH_BY_TRUST_LOCATION, null, locationCoordinates, null, radius, openOnly, schoolLevelOrdering, page);

            if (searchResults.NumberOfResults == 0)
            {
                return ErrorView(SearchTypes.SEARCH_BY_TRUST_LOCATION, referrer, SearchErrorMessages.NO_LOCATION_RESULTS);
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
                    return await Search(null, null, SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, null, null, laName, null, openOnly, orderby, page, tab);
                }
                var similarMatch = _laSearchService.SearchContains(laName);
                if (similarMatch.Count == 0)
                {
                    return ErrorView(SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, referrer, SearchErrorMessages.NO_LA_RESULTS);
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
                    return ErrorView(SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, referrer, SearchErrorMessages.NO_LA_RESULTS);
                }

                var trustsVm = await BuildTrustViewModelListFromFoundAcademiesAsync(searchResults, orderby, page, SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, null, null, _laService.GetLaName(laCode));

                ApplyTrustLevelOrdering(orderby, trustsVm);

                ViewBag.LaCodeName = laCode;
                return View("SearchResults", trustsVm);
            }
            else
            {
                return ErrorView(SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, referrer, errorMessage);
            }
        }
    }
}