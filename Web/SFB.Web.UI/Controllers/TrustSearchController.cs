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
                            searchResp = await GetSearchResults(trustNameId, SearchTypes.SEARCH_BY_TRUST_NAME_ID, searchType, locationCoordinates, laCodeName, radius, openOnly, orderby, 1);
                            if (searchResp.NumberOfResults == 0)
                            {
                                return RedirectToActionPermanent("SuggestTrust", "TrustSearch", new RouteValueDictionary { { "trustNameId", trustNameId } });
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        return ErrorView(searchType, referrer, errorMessage);
                    }

                    return View("SearchResults", GetTrustViewModelList(searchResp, orderby, page, searchType, trustNameId, locationorpostcode, _laService.GetLaName(laCodeName)));


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
                            var schoolLevelOrdering = orderby;
                            if (orderby == "AreaSchoolNumber" || orderby == "MatSchoolNumber")
                            {
                                schoolLevelOrdering = $"{EdubaseDBFieldNames.TRUSTS} asc";
                            }

                            searchResp = await GetSearchResults(trustNameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, schoolLevelOrdering, page);

                            if (searchResp.NumberOfResults == 0)
                            {
                                return View("EmptyLocationResult", new SearchViewModel(null, searchType));
                            }

                            TrustListViewModel trustsVm = BuildTrustViewModelListFromSchools(searchResp, orderby, page, searchType, trustNameId, locationorpostcode, null);

                            TrustLevelOrdering(orderby, trustsVm);

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
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        return ErrorView(searchType, referrer, errorMessage);
                    }

                    return View("SearchResults", GetTrustViewModelList(searchResp, orderby, page, searchType, trustNameId, locationorpostcode, _laService.GetLaName(laCodeName)));

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

            var schoolLevelOrdering = orderby;
            if (orderby == "AreaSchoolNumber" || orderby == "MatSchoolNumber")
            {
                schoolLevelOrdering = $"{EdubaseDBFieldNames.TRUSTS} asc";
            }

            dynamic searchResponse = await GetSearchResults(trustNameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, schoolLevelOrdering, page);

            TrustListViewModel trustsVm;
            if (searchType == SearchTypes.SEARCH_BY_TRUST_NAME_ID)
            {
                trustsVm = GetTrustViewModelList(searchResponse, orderby, page, searchType, trustNameId, locationorpostcode, null);
            }
            else
            {
                trustsVm = BuildTrustViewModelListFromSchools(searchResponse, schoolLevelOrdering, page, searchType, trustNameId, locationorpostcode, null);
            }

            TrustLevelOrdering(orderby, trustsVm);

            return PartialView("Partials/TrustResults", trustsVm);
        }

        [Route("TrustSearch/Search-json")]
        public async Task<JsonResult> SearchJson(string trustNameId, string searchType, string trustSuggestionUrn,
            string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius,
            bool openOnly = false, string orderby = "", int page = 1)
        {
            var schoolLevelOrdering = orderby;
            if (orderby == "AreaSchoolNumber" || orderby == "MatSchoolNumber")
            {
                schoolLevelOrdering = $"{EdubaseDBFieldNames.TRUSTS} asc";
            }

            dynamic searchResponse = await GetSearchResults(trustNameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, schoolLevelOrdering, page, 1000);

            TrustListViewModel trusts;
            List<SchoolSummaryViewModel> results = new List<SchoolSummaryViewModel>();
            if (searchType == SearchTypes.SEARCH_BY_TRUST_NAME_ID)
            {
                trusts = GetTrustViewModelList(searchResponse, orderby, page, searchType, trustNameId, locationorpostcode, null);
                foreach (var trust in trusts.ModelList)
                {
                    var schoolSearchResponse = await _schoolSearchService.SearchSchoolByCompanyNo(trust.CompanyNo, 0, 1000, null, null);
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
                    var companyNo = int.Parse(result[EdubaseDBFieldNames.COMPANY_NUMBER]);
                    var companyName = result[SchoolTrustFinanceDBFieldNames.TRUST_COMPANY_NAME];
                    List<AcademiesContextualDataObject> academiesList = _financialDataService.GetAcademiesByCompanyNumber(LatestMATTerm(), companyNo);
                    if (academiesList.Count > 0)
                    {
                        var trustVm = new TrustViewModel(companyNo, companyName, academiesList);
                        trustListVm.Add(trustVm);
                    }
                }

                vm.SchoolComparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

                //var filters = _filterBuilder.ConstructSchoolSearchFilters(Request.QueryString, response.Facets);
                //vm.Filters = filters;
                //vm.FilterSelectionState = DetermineSelectionState(filters);

                vm.Pagination = new Pagination
                {
                    Start = (SearchDefaults.RESULTS_PER_PAGE * (page - 1)) + 1,
                    Total = trustListVm.Count,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.RESULTS_PER_PAGE,
                    PagedEntityType = Common.PagedEntityType.MAT
                };
            }

            return vm;
        }

        private TrustListViewModel BuildTrustViewModelListFromSchools(dynamic response, string orderBy, int page, string searchType, string nameKeyword, string locationKeyword, string laKeyword)
        {
            TrustListViewModel tvm = null;

            if (response != null)
            {
                var trustList = new List<TrustViewModel>();
                foreach (var result in response.Results)
                {
                    int companyNo;
                    if (int.TryParse(result[EdubaseDBFieldNames.COMPANY_NUMBER], out companyNo))
                    {
                        var companyName = result[EdubaseDBFieldNames.TRUSTS];
                        if (!trustList.Any(t => t.CompanyNo == companyNo))
                        {
                            var academiesList = _financialDataService.GetAcademiesByCompanyNumber(LatestMATTerm(), companyNo);
                            if (academiesList.Count > 0)
                            {
                                var academy = academiesList.Find(a => a.URN == int.Parse(result["URN"]));
                                if (academy != null)
                                {
                                    academy.InsideSearchArea = true;
                                }

                                var trustVm = new TrustViewModel(companyNo, companyName, academiesList);
                                trustList.Add(trustVm);
                            }
                        }
                        else
                        {
                            var trust = trustList.Find(t => t.CompanyNo == companyNo);
                            var academy = trust.AcademiesList.Find(a => a.URN == int.Parse(result["URN"]));
                            if (academy != null)
                            {
                                academy.InsideSearchArea = true;
                            }
                        }
                    }
                }

                tvm = new TrustListViewModel(trustList, null, searchType, nameKeyword, locationKeyword, laKeyword, orderBy);

                //var filters = _filterBuilder.ConstructSchoolSearchFilters(Request.QueryString, response.Facets);
                //vm.Filters = filters;
                //vm.FilterSelectionState = DetermineSelectionState(filters);

                tvm.Pagination = new Pagination
                {
                    Start = (SearchDefaults.RESULTS_PER_PAGE * (page - 1)) + 1,
                    Total = tvm.ModelList.Count,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.TRUST_SCHOOLS_TOTAL,
                    PagedEntityType = Common.PagedEntityType.MAT
                };
            }

            return tvm;
        }

        private void TrustLevelOrdering(string orderby, TrustListViewModel trustsVm)
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

        private string LatestMATTerm()
        {
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);
            return FormatHelpers.FinancialTermFormatAcademies(latestYear);
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