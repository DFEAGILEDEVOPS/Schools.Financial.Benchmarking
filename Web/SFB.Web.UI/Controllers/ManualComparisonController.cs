using SFB.Web.ApplicationCore.Entities; 
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Services.LocalAuthorities;
using SFB.Web.ApplicationCore.Services.Search;
using SFB.Web.UI.Attributes;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class ManualComparisonController : SearchBaseController
    {
        private readonly ILocalAuthoritiesService _laService;
        private readonly IContextDataService _contextDataService;
        private readonly IValidationService _valService;
        private readonly ILocationSearchService _locationSearchService;
        private readonly ILaSearchService _laSearchService;
        private readonly IManualBenchmarkListService _manualBenchmarkListService;

        public ManualComparisonController(ISchoolBenchmarkListService benchmarkBasketService, ILocalAuthoritiesService laService, 
            IContextDataService contextDataService, IValidationService valService, ILocationSearchService locationSearchService, 
            ISchoolSearchService schoolSearchService, IFilterBuilder filterBuilder, ILaSearchService laSearchService, IManualBenchmarkListService manualBenchmarkListService)
            : base(schoolSearchService, null, benchmarkBasketService, filterBuilder)
        {
            _laService = laService;
            _laSearchService = laSearchService;
            _contextDataService = contextDataService;
            _valService = valService;
            _locationSearchService = locationSearchService;
            _manualBenchmarkListService = manualBenchmarkListService;
        }

        public ActionResult Index()
        {
            var schoolComparisonListModel = _schoolBenchmarkListService.GetSchoolBenchmarkList();
            var vm = new SearchViewModel(schoolComparisonListModel, "");
            vm.Authorities = _laService.GetLocalAuthorities();
            _manualBenchmarkListService.ClearManualBenchmarkList();
            _manualBenchmarkListService.SetSchoolAsDefaultInManualBenchmarkList(schoolComparisonListModel);
            return View("Index",vm);
        }

        public ActionResult WithoutBaseSchool()
        {
            var vm = new SearchViewModel(null, "")
            {
                Authorities = _laService.GetLocalAuthorities()
            };
            _manualBenchmarkListService.ClearManualBenchmarkList();
            _manualBenchmarkListService.UnsetDefaultSchoolInManualBenchmarkList();
            _schoolBenchmarkListService.UnsetDefaultSchool();
            return View("Index", vm);
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
            ViewBag.OpenOnly = openOnly;
            ViewBag.SearchMethod = "Manual";
            ViewBag.LaCodeName = laCodeName;

            var comparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();            
            var manualComparisonList = _manualBenchmarkListService.GetManualBenchmarkList();

            EdubaseDataObject contextDataObject = null;
            if (!string.IsNullOrEmpty(comparisonList.HomeSchoolUrn))
            {
                await _contextDataService.GetSchoolDataObjectByUrnAsync(int.Parse(comparisonList.HomeSchoolUrn));
            }

            dynamic searchResp = null;
            string errorMessage;

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_NAME_ID:
                    if (string.IsNullOrEmpty(comparisonList.HomeSchoolUrn))
                    {
                        var vm = new SchoolViewModelWithNoDefaultSchool(manualComparisonList);
                        return View("AddSchoolsManually", vm);
                    }
                    else
                    {
                        var vm = new SchoolViewModel(contextDataObject, comparisonList, manualComparisonList);
                        return View("AddSchoolsManually", vm);
                    }
                case SearchTypes.SEARCH_BY_LA_CODE_NAME:
                    if (!IsNumeric(laCodeName))
                    {
                        errorMessage = _valService.ValidateLaNameParameterForComparison(laCodeName);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            var exactMatch = _laSearchService.SearchExactMatch(laCodeName);
                            if (exactMatch != null)
                            {
                                laCodeName = exactMatch.Id;
                                return await Search(nameId, searchType, suggestionUrn, locationorpostcode,
                                    locationCoordinates, laCodeName, radius, openOnly, orderby, page, tab);
                            }
                            var similarMatch = _laSearchService.SearchContains(laCodeName);
                            if(similarMatch.Count == 0)
                            {
                                var svm = new SearchViewModel(comparisonList, searchType)
                                {
                                    SearchType = searchType,
                                    Authorities = _laService.GetLocalAuthorities(),
                                    ErrorMessage = SearchErrorMessages.NO_LA_RESULTS
                                };
                                return View("Index", svm);
                            }
                            TempData["SearchMethod"] = "Manual";
                            return RedirectToAction("Search", "La", new { name = laCodeName, openOnly = openOnly });
                        }
                        else
                        {
                            var svm = new SearchViewModel(comparisonList, searchType)
                            {
                                SearchType = searchType,
                                Authorities = _laService.GetLocalAuthorities(),
                                ErrorMessage = errorMessage
                            };
                            return View("Index", svm);
                        }
                    }
                    else
                    {
                        errorMessage = _valService.ValidateLaCodeParameterForComparison(laCodeName);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            searchResp = await GetSearchResultsAsync(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);

                            int resultCount = searchResp.NumberOfResults;
                            if (resultCount == 0)
                            {
                                var svm = new SearchViewModel(comparisonList, searchType)
                                {
                                    SearchType = searchType,
                                    Authorities = _laService.GetLocalAuthorities(),
                                    ErrorMessage = SearchErrorMessages.NO_LA_RESULTS
                                };
                                return View("Index", svm);
                            }
                            else
                            {
                                ViewBag.manualCount = manualComparisonList?.BenchmarkSchools?.Count();
                                return View("ManualSearchResults", GetSearchedSchoolViewModelList(searchResp, manualComparisonList, orderby, page, searchType, nameId, locationorpostcode, _laService.GetLaName(laCodeName)));
                            }
                        }
                        else
                        {
                            var svm = new SearchViewModel(comparisonList, searchType)
                            {
                                SearchType = searchType,
                                Authorities = _laService.GetLocalAuthorities(),
                                ErrorMessage = errorMessage
                            };
                            return View("Index", svm);
                        }
                    }
                case SearchTypes.SEARCH_BY_LOCATION:
                default:
                    errorMessage = _valService.ValidateLocationParameterForComparison(locationorpostcode);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        if (string.IsNullOrEmpty(locationCoordinates))
                        {
                            var result = _locationSearchService.SuggestLocationName(locationorpostcode);
                            switch (result.Matches.Count)
                            {
                                case 0:
                                    var svm = new SearchViewModel(comparisonList, searchType)
                                    {
                                        SearchType = searchType,
                                        Authorities = _laService.GetLocalAuthorities(),
                                        ErrorMessage = SearchErrorMessages.NO_LOCATION_RESULTS
                                    };
                                    return View("Index", svm);
                                default:
                                    TempData["LocationResults"] = result;
                                    TempData["SearchMethod"] = "Manual";
                                    return RedirectToAction("Suggest", "Location", new { locationOrPostcode = locationorpostcode, openOnly = openOnly });
                            }
                        }
                        else
                        {
                            searchResp = await GetSearchResultsAsync(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);

                            if (searchResp.NumberOfResults == 0)
                            {
                                var svm = new SearchViewModel(comparisonList, searchType)
                                {
                                    SearchType = searchType,
                                    Authorities = _laService.GetLocalAuthorities(),
                                    ErrorMessage = SearchErrorMessages.NO_LOCATION_RESULTS
                                };
                                return View("Index", svm);
                            }
                            ViewBag.manualCount = manualComparisonList?.BenchmarkSchools?.Count();
                            return View("ManualSearchResults", GetSearchedSchoolViewModelList(searchResp, manualComparisonList, orderby, page, searchType, nameId, locationorpostcode, _laService.GetLaName(laCodeName)));
                        }
                    }
                    else
                    {
                        var svm = new SearchViewModel(comparisonList, searchType)
                        {
                            SearchType = searchType,
                            Authorities = _laService.GetLocalAuthorities(),
                            ErrorMessage = errorMessage
                        };
                        return View("Index" , svm);
                    }
            }
        }

        [Route("ManualComparison/Search-js")]
        public async Task<PartialViewResult> ManualSearchJS(string nameId, string searchType, string suggestionurn,
        string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, 
        decimal? radius, bool openOnly = false, string orderby = "", int page = 1)
        {
            dynamic searchResponse;

            var schoolComparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
            var manualComparisonList = _manualBenchmarkListService.GetManualBenchmarkList();

            if (IsLaEstab(nameId))
            {
                searchResponse = await GetSearchResultsAsync(nameId, SearchTypes.SEARCH_BY_LA_ESTAB, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);
            }
            else
            {
                searchResponse = await GetSearchResultsAsync(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);
            }
            var vm = GetSearchedSchoolViewModelList(searchResponse, schoolComparisonList, orderby, page, searchType, nameId, locationorpostcode, laCodeName);

            ViewBag.SearchMethod = "Manual";
            ViewBag.manualCount = manualComparisonList?.BenchmarkSchools?.Count();
            return PartialView("Partials/Search/SchoolResults", vm);
        }

        [Route("ManualComparison/Search-json")]
        public async Task<JsonResult> ManualSearchJson(string nameId, string searchType, string suggestionurn,
            string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius,
            int? companyNo, bool openOnly = false, string orderby = "", int page = 1)

        {
            dynamic searchResponse;
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

        public async Task<ActionResult> OverwriteStrategy()
        {
            var comparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
            var manualComparisonList = _manualBenchmarkListService.GetManualBenchmarkList();
            if (comparisonList?.BenchmarkSchools?.Count > 0 && !comparisonList.BenchmarkSchools.All(s => s.Urn == comparisonList.HomeSchoolUrn))
            {
                SchoolViewModel vm;
                if (comparisonList?.BenchmarkSchools?.Count + manualComparisonList.BenchmarkSchools.Count > ComparisonListLimit.LIMIT)
                {
                    if (comparisonList.HomeSchoolUrn == null)
                    {
                        vm = new SchoolViewModelWithNoDefaultSchool(comparisonList, manualComparisonList);
                    }
                    else
                    {
                        var contextDataObject = await _contextDataService.GetSchoolDataObjectByUrnAsync(int.Parse(comparisonList.HomeSchoolUrn));
                        vm = new SchoolViewModel(contextDataObject, comparisonList, manualComparisonList);
                    }
                    ViewBag.referrer = Request?.UrlReferrer;
                    return View("OverwriteReplace", vm);
                }
                else
                {
                    if (comparisonList.HomeSchoolUrn == null)
                    {
                        vm = new SchoolViewModelWithNoDefaultSchool(comparisonList, manualComparisonList);
                    }
                    else
                    {
                        var contextDataObject = await _contextDataService.GetSchoolDataObjectByUrnAsync(int.Parse(comparisonList.HomeSchoolUrn));
                        vm = new SchoolViewModel(contextDataObject, comparisonList, manualComparisonList);
                    }
                    ViewBag.referrer = Request?.UrlReferrer;
                    return View(vm);
                }
            }
            else
            {
                foreach (var school in manualComparisonList.BenchmarkSchools)
                {
                    _schoolBenchmarkListService.TryAddSchoolToBenchmarkList(school);
                }
                return Redirect("/BenchmarkCharts");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReplaceAdd(BenchmarkListOverwriteStrategy? overwriteStrategy, string referrer)
        {
            var comparisonList = _schoolBenchmarkListService.GetSchoolBenchmarkList();
            var manualComparisonList = _manualBenchmarkListService.GetManualBenchmarkList();

            switch (overwriteStrategy)
            {
                case null:
                    SchoolViewModel vm = null;
                    if (comparisonList.HomeSchoolUrn == null)
                    {
                        vm = new SchoolViewModelWithNoDefaultSchool(comparisonList, manualComparisonList);
                    }
                    else
                    {
                        var contextDataObject = await _contextDataService.GetSchoolDataObjectByUrnAsync(int.Parse(comparisonList.HomeSchoolUrn));
                        vm = new SchoolViewModel(contextDataObject, comparisonList, manualComparisonList);
                    }

                    vm.ErrorMessage = ErrorMessages.SelectOverwriteStrategy;
                    ViewBag.referrer = referrer;
                    return View("OverwriteStrategy", vm);
                case BenchmarkListOverwriteStrategy.Add:
                    if (comparisonList.BenchmarkSchools.Count + manualComparisonList.BenchmarkSchools.Where(s => s.Urn != manualComparisonList.HomeSchoolUrn).Count()  > ComparisonListLimit.LIMIT)
                    {
                        if (comparisonList.HomeSchoolUrn == null)
                        {
                            vm = new SchoolViewModelWithNoDefaultSchool(comparisonList, manualComparisonList);
                        }
                        else
                        {
                            var contextDataObject = await _contextDataService.GetSchoolDataObjectByUrnAsync(int.Parse(comparisonList.HomeSchoolUrn));
                            vm = new SchoolViewModel(contextDataObject, comparisonList, manualComparisonList);
                        }

                        vm.ErrorMessage = ErrorMessages.BMBasketLimitExceed;
                        ViewBag.referrer = referrer;
                        return View("OverwriteStrategy", vm);
                    }
                    else
                    {
                        foreach (var school in manualComparisonList.BenchmarkSchools.Where(s => s.Urn != manualComparisonList.HomeSchoolUrn))
                        {
                            _schoolBenchmarkListService.TryAddSchoolToBenchmarkList(school);
                        }
                        return Redirect("/BenchmarkCharts");
                    }
                case BenchmarkListOverwriteStrategy.Overwrite:                    
                default:
                    _schoolBenchmarkListService.ClearSchoolBenchmarkList();
                    foreach (var school in manualComparisonList.BenchmarkSchools.Where(s => s.Urn != manualComparisonList.HomeSchoolUrn))
                    {
                        _schoolBenchmarkListService.AddSchoolToBenchmarkList(school);
                    }
                    var benchmarkSchool = new BenchmarkSchoolModel(manualComparisonList);

                    _schoolBenchmarkListService.SetSchoolAsDefault(benchmarkSchool);
                    return Redirect("/BenchmarkCharts");
            }
        }

        public async Task<PartialViewResult> AddSchool(int urn)
        {
            var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), null);

            try
            {
                _manualBenchmarkListService.AddSchoolToManualBenchmarkList(benchmarkSchool);
            }
            catch (ApplicationException ex)
            {
                ViewBag.Error = ex.Message;
            }

            var vm = _manualBenchmarkListService.GetManualBenchmarkList();

            return PartialView("Partials/SchoolsToAdd", vm.BenchmarkSchools.Where(s => s.Id != vm.HomeSchoolUrn).ToList());
        }

        public PartialViewResult RemoveSchool(int urn)
        {
            _manualBenchmarkListService.RemoveSchoolFromManualBenchmarkListAsync(urn);

            var vm = _manualBenchmarkListService.GetManualBenchmarkList();

            return PartialView("Partials/SchoolsToAdd", vm.BenchmarkSchools.Where(s => s.Id != vm.HomeSchoolUrn).ToList());
        }

        public async Task<JsonResult> UpdateManualBasket(int? urn, CookieActions withAction)
        {
            if (urn.HasValue)
            {
                switch (withAction)
                {
                    case CookieActions.SetDefault:
                        await _manualBenchmarkListService.SetSchoolAsDefaultInManualBenchmarkList(urn.GetValueOrDefault());
                        break;
                    case CookieActions.Add:
                        await _manualBenchmarkListService.AddSchoolToManualBenchmarkListAsync(urn.GetValueOrDefault());
                        break;
                    case CookieActions.Remove:
                        await _manualBenchmarkListService.RemoveSchoolFromManualBenchmarkListAsync(urn.GetValueOrDefault());
                        break;
                    case CookieActions.UnsetDefault:
                        _manualBenchmarkListService.UnsetDefaultSchoolInManualBenchmarkList();
                        break;
                }
            }
            else
            {
                _manualBenchmarkListService.ClearManualBenchmarkList();
            }

            return Json(_manualBenchmarkListService.GetManualBenchmarkList().BenchmarkSchools.Count, JsonRequestBehavior.AllowGet);
        }
    }
}