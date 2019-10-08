using Newtonsoft.Json;
using SFB.Web.Common;
using SFB.Web.Common.DataObjects;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Domain.Services.Search;
using SFB.Web.UI.Attributes;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using System.Collections.Generic;
using System.Diagnostics;
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
            dynamic searchResults = null;
            string errorMessage = string.Empty;
            ViewBag.tab = tab;
            ViewBag.SearchMethod = "MAT";
            ViewBag.SearchType = searchType;

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
                            searchResults = await GetSearchResultsAsync(trustNameId, SearchTypes.SEARCH_BY_TRUST_NAME_ID, searchType, locationCoordinates, laCodeName, radius, openOnly, orderby, 1);
                            if (searchResults.NumberOfResults == 0)
                            {
                                return RedirectToActionPermanent("SuggestTrust", "TrustSearch", new RouteValueDictionary { { "trustNameId", trustNameId } });
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        return ErrorView(searchType, referrer, errorMessage);
                    }

                    var trustVm = await GetTrustListViewModelAsync(searchResults, orderby, page, searchType, trustNameId, locationorpostcode, _laService.GetLaName(laCodeName));

                    return View("SearchResults", trustVm);

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
                                    return View("EmptyLocationResult", new SearchViewModel(null, searchType));
                                default:
                                    TempData["LocationResults"] = result;
                                    TempData["SearchMethod"] = "MAT";
                                    return RedirectToAction("Suggest", "Location", new { locationOrPostcode = locationorpostcode });
                            }
                        }
                        else
                        {
                            var schoolLevelOrdering = OverwriteSchoolLevelOrdering(orderby);

                            searchResults = await GetSearchResultsAsync(trustNameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, schoolLevelOrdering, page);

                            if (searchResults.NumberOfResults == 0)
                            {
                                return View("EmptyLocationResult", new SearchViewModel(null, searchType));
                            }

                            var trustsVm = await BuildTrustViewModelListFromFoundAcademiesAsync(searchResults, orderby, page, searchType, trustNameId, locationorpostcode, null);

                            ApplyTrustLevelOrdering(orderby, trustsVm);

                            return View("SearchResults", trustsVm);
                        }
                    }
                    else
                    {
                        return ErrorView(searchType, referrer, errorMessage);
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
                                return await Search(trustNameId, searchType, suggestionUrn, locationorpostcode,
                                    locationCoordinates, laCodeName, radius, openOnly, orderby, page, tab);
                            }
                            TempData["SearchMethod"] = "MAT";
                            return RedirectToAction("Search", "La", new { name = laCodeName, openOnly = openOnly });
                        }
                        else
                        {
                            return ErrorView(searchType, referrer, errorMessage);
                        }
                    }
                    else
                    {
                        errorMessage = _valService.ValidateLaCodeParameter(laCodeName);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            var schoolLevelOrdering = OverwriteSchoolLevelOrdering(orderby);

                            searchResults = await GetSearchResultsAsync(trustNameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, schoolLevelOrdering, page);

                            switch (searchResults.NumberOfResults)
                            {
                                case 0:
                                    return View("EmptyResult",
                                        new SearchViewModel(null, searchType));
                                case 1:
                                    return RedirectToAction("Detail", "School",
                                        new
                                        {
                                            urn = ((QueryResultsModel)searchResults).Results.First()["URN"]
                                        });
                            }

                            var trustsVm = await BuildTrustViewModelListFromFoundAcademiesAsync(searchResults, orderby, page, searchType, trustNameId, locationorpostcode, _laService.GetLaName(laCodeName));

                            ApplyTrustLevelOrdering(orderby, trustsVm);

                            return View("SearchResults", trustsVm);
                        }
                        else
                        {
                            return ErrorView(searchType, referrer, errorMessage);
                        }
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

            string schoolLevelOrdering = OverwriteSchoolLevelOrdering(orderby);

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

        public override async Task<dynamic> GetSearchResultsAsync(string nameId, string searchType, string locationorpostcode, string locationCoordinates, string laCode, decimal? radius, bool openOnly, string orderby, int page, int take = 50)
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
                        Request.QueryString) as QueryResultsModel;
                    break;
                case SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME:
                    response = await _schoolSearchService.SearchSchoolByLaCode(laCode,
                        0, SearchDefaults.SEARCHED_SCHOOLS_MAX, orderby,
                        Request.QueryString) as QueryResultsModel;
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
                var companyNo = int.Parse(result[EdubaseDBFieldNames.COMPANY_NUMBER]);
                var companyName = result[SchoolTrustFinanceDBFieldNames.TRUST_COMPANY_NAME];
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
                if (int.TryParse(academySearchResult[EdubaseDBFieldNames.COMPANY_NUMBER], out int companyNo))
                {
                    if (!academyTrustList.Any(t => t.CompanyNo == companyNo))
                    {
                        var academyTrust = new AcademyTrustViewModel(companyNo, academySearchResult[EdubaseDBFieldNames.TRUSTS], _contextDataService.GetSchoolsByCompanyNumberAsync(companyNo));
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
                var schoolMatch = trustList.SelectMany(t => t.AcademiesList).Where(a => a.Id.ToString() == academySearchResult[EdubaseDBFieldNames.URN]).FirstOrDefault();
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

        private static string OverwriteSchoolLevelOrdering(string orderby)
        {
            var schoolLevelOrdering = orderby;
            if (orderby == "AreaSchoolNumber" || orderby == "MatSchoolNumber")
            {
                schoolLevelOrdering = $"{EdubaseDBFieldNames.TRUSTS} asc";
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
    }
}