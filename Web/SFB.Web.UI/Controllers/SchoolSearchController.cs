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
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Domain.Services.Search;

namespace SFB.Web.UI.Controllers
{
    public class SchoolSearchController : BaseController
    {
        private readonly ILocalAuthoritiesService _laService;
        private readonly IFilterBuilder _filterBuilder;
        private readonly IValidationService _valService;
        private readonly IEdubaseDataService _edubaseDataService;
        private readonly ISchoolSearchService _schoolSearchService;
        private readonly ITrustSearchService _trustSearchService;

        public SchoolSearchController(ILocalAuthoritiesService laService, IFilterBuilder filterBuilder,
            IValidationService valService, IEdubaseDataService edubaseDataService,
            ISchoolSearchService schoolSearchService, ITrustSearchService trustSearchService)
        {
            _laService = laService;
            _filterBuilder = filterBuilder;
            _valService = valService;
            _edubaseDataService = edubaseDataService;
            _schoolSearchService = schoolSearchService;
            _trustSearchService = trustSearchService;
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
            string orderby = "", 
            int page = 1,
            string tab = "list")
        {
            dynamic searchResp = null;
            string errorMessage;
            ViewBag.tab = tab;

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_NAME_ID:
                    if (IsNumeric(nameId))
                    {
                        errorMessage = _valService.ValidateSchoolIdParameter(nameId);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            var isLaEstab = nameId.Length == SearchParameterValidLengths.LAESTAB_LENGTH;
                            searchResp = isLaEstab
                                ? _edubaseDataService.GetSchoolByLaEstab(nameId)
                                : _edubaseDataService.GetSchoolByUrn(nameId);

                            if (searchResp == null)
                            {
                                return View("EmptyResult",
                                    new SchoolSearchViewModel(base.ExtractSchoolComparisonListFromCookie(),
                                        SearchTypes.SEARCH_BY_NAME_ID));
                            }

                            nameId = ((Microsoft.Azure.Documents.Document) searchResp).GetPropertyValue<string>("URN");

                            return RedirectToAction("Detail", "School", new {urn = nameId});
                        }
                        else
                        {
                            var searchVM = new SchoolSearchViewModel(base.ExtractSchoolComparisonListFromCookie(), searchType)
                            {
                                SearchType = searchType,
                                ErrorMessage = errorMessage,
                                Authorities = _laService.GetLocalAuthorities()
                            };

                            return View("../Home/Index", searchVM);
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
                            searchResp = await GetSearchResults(nameId, searchType, null, null, null, null, radius,
                                orderby, page);
                            if (searchResp.NumberOfResults == 0)
                            {
                                return RedirectToActionPermanent("SuggestSchool", "SchoolSearch",
                                    new RouteValueDictionary {{"nameId", nameId}});
                            }
                        }
                        else
                        {
                            var searchVM = new SchoolSearchViewModel(base.ExtractSchoolComparisonListFromCookie(), searchType)
                            {
                                SearchType = searchType,
                                ErrorMessage = errorMessage,
                                Authorities = _laService.GetLocalAuthorities()
                            };

                            return View("../Home/Index", searchVM);
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
                        var searchVM = new SchoolSearchViewModel(base.ExtractSchoolComparisonListFromCookie(), searchType)
                        {
                            SearchType = searchType,
                            ErrorMessage = errorMessage,
                            Authorities = _laService.GetLocalAuthorities()
                        };

                        return View("../Home/Index", searchVM);
                    }

                case SearchTypes.SEARCH_BY_LA_CODE_NAME:
                    if (!IsNumeric(laCodeName))
                    {
                        errorMessage = _valService.ValidateLaNameParameter(laCodeName);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            return RedirectToAction("Search", "La", new {name = laCodeName});
                        }
                        else
                        {
                            var searchVM = new SchoolSearchViewModel(base.ExtractSchoolComparisonListFromCookie(), searchType)
                            {
                                SearchType = searchType,
                                ErrorMessage = errorMessage,
                                Authorities = _laService.GetLocalAuthorities()
                            };

                            return View("../Home/Index", searchVM);
                        }
                    }
                    else
                    {
                        errorMessage = _valService.ValidateLaCodeParameter(laCodeName);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            searchResp = await GetSearchResults(nameId, searchType, null, locationorpostcode,
                                locationCoordinates, laCodeName, radius, orderby, page);

                            int resultCount = searchResp.NumberOfResults;
                            switch (resultCount)
                            {
                                case 0:
                                    return View("EmptyResult",
                                        new SchoolSearchViewModel(base.ExtractSchoolComparisonListFromCookie(), searchType));
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
                            var searchVM = new SchoolSearchViewModel(base.ExtractSchoolComparisonListFromCookie(), searchType)
                            {
                                SearchType = searchType,
                                ErrorMessage = errorMessage,
                                Authorities = _laService.GetLocalAuthorities()
                            };

                            return View("../Home/Index", searchVM);
                        }
                    }

                    break;

                case SearchTypes.SEARCH_BY_LOCATION:
                    errorMessage = _valService.ValidateLocationParameter(locationorpostcode);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        searchResp = await GetSearchResults(nameId, searchType, null, locationorpostcode,
                            locationCoordinates, laCodeName, radius, orderby, page);

                        int resultCnt = searchResp.NumberOfResults;
                        switch (resultCnt)
                        {
                            case 0:
                                return View("EmptyResult",
                                    new SchoolSearchViewModel(base.ExtractSchoolComparisonListFromCookie(), searchType));
                            case 1:
                                return RedirectToAction("Detail", "School",
                                    new {urn = ((Domain.Models.QueryResultsModel) searchResp).Results.First()["URN"]});
                        }
                    }
                    else
                    {
                        var searchVM = new SchoolSearchViewModel(base.ExtractSchoolComparisonListFromCookie(), searchType)
                        {
                            SearchType = searchType,
                            ErrorMessage = errorMessage,
                            Authorities = _laService.GetLocalAuthorities()
                        };

                        return View("../Home/Index", searchVM);
                    }
                    break;
            }

            var laName = _laService.GetLaName(laCodeName);
            return View("SearchResults", GetSchoolViewModelList(searchResp, orderby, page, searchType, nameId, locationorpostcode, laName));
        }

        public PartialViewResult UpdateBenchmarkBasket(int urn, string withAction)
        {
            var benchmarkSchool = new SchoolViewModel(_edubaseDataService.GetSchoolByUrn(urn.ToString()), null);

            var cookie = base.UpdateSchoolComparisonListCookie(withAction,
                new BenchmarkSchoolViewModel()
                {
                    Name = benchmarkSchool.Name,
                    Urn = benchmarkSchool.Id,
                    Type = benchmarkSchool.Type,
                    FinancialType = benchmarkSchool.FinancialType.ToString()
                });

            if (cookie != null)
            {
                Response.Cookies.Add(cookie);
            }

            return PartialView("Partials/BenchmarkListBanner",
                new SchoolViewModel(null, base.ExtractSchoolComparisonListFromCookie()));
        }

        public async Task<ActionResult> SuggestSchool(string nameId)
        {
            var vm = new SchoolNotFoundViewModel
            {
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
            string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius,
            string orderby = "", int page = 1)

        {
            var searchResponse = await GetSearchResults(nameId, searchType, null, locationorpostcode,
                locationCoordinates, laCodeName, radius, orderby, page);
            var vm = GetSchoolViewModelList(searchResponse, orderby,page, searchType, nameId, locationorpostcode, laCodeName);

            return PartialView("Partials/SchoolResults", vm);
        }

        [Route("SchoolSearch/Search-json")]
        public async Task<JsonResult> SearchJson(string nameId, string searchType, string suggestionurn,
            string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius,
            string matNo, string orderby = "", int page = 1)

        {
            dynamic searchResponse;
            if (string.IsNullOrEmpty(matNo))
            {
                searchResponse = await GetSearchResults(nameId, searchType, null, locationorpostcode,
                    locationCoordinates, laCodeName, radius, orderby, page, 1000);
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
            List<string> urnList,
            string locationorpostcode,
            string locationCoordinates,
            string laCode,
            decimal? radius, 
            string orderby, 
            int page, 
            int take = SearchDefaults.RESULTS_PER_PAGE)
        {
            dynamic response;

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
                default:
                    response = _edubaseDataService.GetMultipleSchoolsByUrns(urnList);
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
            var schoolListVm = new List<SchoolViewModel>();
            var vm = new SearchedSchoolListViewModel(schoolListVm, null, searchType, nameKeyword, locationKeyword, laKeyword, orderBy);
            if (response != null)
            {
                foreach (var result in response.Results)
                {
                    var schoolVm = new SchoolViewModel(result);
                    schoolListVm.Add(schoolVm);
                }

                vm.SchoolComparisonList = base.ExtractSchoolComparisonListFromCookie();

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
    }
}