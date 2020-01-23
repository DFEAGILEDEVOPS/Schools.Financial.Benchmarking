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

        public ManualComparisonController(IBenchmarkBasketCookieManager benchmarkBasketCookieManager, ILocalAuthoritiesService laService, 
            IContextDataService contextDataService, IValidationService valService, ILocationSearchService locationSearchService, 
            ISchoolSearchService schoolSearchService, IFilterBuilder filterBuilder, ILaSearchService laSearchService)
            : base(schoolSearchService, null, benchmarkBasketCookieManager, filterBuilder)
        {
            _laService = laService;
            _laSearchService = laSearchService;
            _contextDataService = contextDataService;
            _valService = valService;
            _locationSearchService = locationSearchService;
        }

        public ActionResult Index()
        {
            var schoolComparisonListModel = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            var vm = new SearchViewModel(schoolComparisonListModel, "");
            vm.Authorities = _laService.GetLocalAuthorities();
            _benchmarkBasketCookieManager.UpdateManualComparisonListCookie(CookieActions.RemoveAll, null);
            _benchmarkBasketCookieManager.UpdateManualComparisonListCookie(CookieActions.SetDefault, new BenchmarkSchoolModel()
            {
                Name = schoolComparisonListModel.HomeSchoolName,
                Urn = schoolComparisonListModel.HomeSchoolUrn,
                Type = schoolComparisonListModel.HomeSchoolType,
                EstabType = schoolComparisonListModel.HomeSchoolFinancialType
            });
            return View("Index",vm);
        }

        public ActionResult WithoutBaseSchool()
        {
            var vm = new SearchViewModel(null, "")
            {
                Authorities = _laService.GetLocalAuthorities()
            };
            _benchmarkBasketCookieManager.UpdateManualComparisonListCookie(CookieActions.RemoveAll, null);
            _benchmarkBasketCookieManager.UpdateManualComparisonListCookie(CookieActions.UnsetDefault, null);
            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.UnsetDefault, null);
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

            var comparisonList = base._benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();            
            var manualComparisonList = _benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie();

            EdubaseDataObject contextDataObject = null;
            if (!string.IsNullOrEmpty(nameId))
            {
                _contextDataService.GetSchoolDataObjectByUrn(int.Parse(comparisonList.HomeSchoolUrn));
            }

            dynamic searchResp = null;
            string errorMessage;

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_NAME_ID:
                    if (string.IsNullOrEmpty(nameId))
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
                        errorMessage = _valService.ValidateLaNameParameter(laCodeName);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            var exactMatch = _laSearchService.SearchExactMatch(laCodeName);
                            if (exactMatch != null)
                            {
                                laCodeName = exactMatch.Id;
                                return await Search(nameId, searchType, suggestionUrn, locationorpostcode,
                                    locationCoordinates, laCodeName, radius, openOnly, orderby, page, tab);
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
                        errorMessage = _valService.ValidateLaCodeParameter(laCodeName);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            searchResp = await GetSearchResultsAsync(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);

                            int resultCount = searchResp.NumberOfResults;
                            if (resultCount == 0)
                            {
                                return View("EmptyResult", new SearchViewModel(comparisonList, searchType));
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
                    errorMessage = _valService.ValidateLocationParameter(locationorpostcode);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        if (string.IsNullOrEmpty(locationCoordinates))
                        {
                            var result = _locationSearchService.SuggestLocationName(locationorpostcode);
                            switch (result.Matches.Count)
                            {
                                case 0:
                                    return View("EmptyManualLocationResult",
                                        new SearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType));
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
                                return View("EmptyManualLocationResult", new SearchViewModel(comparisonList, searchType));
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

            var schoolComparisonList = _benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie();
            var manualComparisonList = _benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie();

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
            QueryResultsModel response = null;

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_LA_ESTAB:
                    response = await _schoolSearchService.SearchSchoolByLaEstab(nameId,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take, orderby,
                        Request.QueryString) as QueryResultsModel;
                    break;
                case SearchTypes.SEARCH_BY_LOCATION:
                    var latLng = locationCoordinates.Split(',');
                    response = await _schoolSearchService.SearchSchoolByLatLon(latLng[0], latLng[1],
                        (radius ?? SearchDefaults.TRUST_LOCATION_SEARCH_DISTANCE) * 1.6m,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take, orderby,
                        Request.QueryString) as QueryResultsModel;
                    break;
                case SearchTypes.SEARCH_BY_LA_CODE_NAME:
                    response = await _schoolSearchService.SearchSchoolByLaCode(laCode,
                        (page - 1) * SearchDefaults.RESULTS_PER_PAGE, take,
                        string.IsNullOrEmpty(orderby) ? EdubaseDataFieldNames.ESTAB_NAME : orderby,
                        Request.QueryString) as QueryResultsModel;
                    break;
            }

            OrderFacetFilters(response);

            return response;
        }

        public ActionResult OverwriteStrategy()
        {
            var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            var manualComparisonList = _benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie();
            if (comparisonList?.BenchmarkSchools?.Count > 0 && !comparisonList.BenchmarkSchools.All(s => s.Urn == comparisonList.HomeSchoolUrn))
            {
                if (comparisonList.HomeSchoolUrn == null)
                {
                    var vm = new SchoolViewModelWithNoDefaultSchool(comparisonList, manualComparisonList);
                    ViewBag.referrer = Request.UrlReferrer;
                    return View(vm);
                }
                else
                {
                    var contextDataObject = _contextDataService.GetSchoolDataObjectByUrn(int.Parse(comparisonList.HomeSchoolUrn));
                    var vm = new SchoolViewModel(contextDataObject, comparisonList, manualComparisonList);
                    ViewBag.referrer = Request.UrlReferrer;
                    return View(vm);
                }
            }
            else
            {
                foreach (var school in manualComparisonList.BenchmarkSchools)
                {
                    try
                    {
                        _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, school);
                    }
                    catch (ApplicationException) { }//ignoring duplicate add exceptions
                }
                return Redirect("/BenchmarkCharts");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReplaceAdd(BenchmarkListOverwriteStrategy overwriteStrategy, string referrer)
        {
            var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            var manualComparisonList = _benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie();

            switch (overwriteStrategy)
            {                              
                case BenchmarkListOverwriteStrategy.Add:
                    if (comparisonList.BenchmarkSchools.Count + manualComparisonList.BenchmarkSchools.Where(s => s.Urn != manualComparisonList.HomeSchoolUrn).Count()  > ComparisonListLimit.LIMIT)
                    {
                        var contextDataObject = _contextDataService.GetSchoolDataObjectByUrn(int.Parse(comparisonList.HomeSchoolUrn));
                        var vm = new SchoolViewModel(contextDataObject, comparisonList, manualComparisonList);
                        vm.ErrorMessage = ErrorMessages.BMBasketLimitExceed;
                        ViewBag.referrer = referrer;
                        return View("OverwriteStrategy", vm);
                    }
                    else
                    {
                        foreach (var school in manualComparisonList.BenchmarkSchools.Where(s => s.Urn != manualComparisonList.HomeSchoolUrn))
                        {
                            try
                            {
                                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, school);
                            }catch(ApplicationException) { } //duplicate school adds will be ignored
                        }
                        return Redirect("/BenchmarkCharts");
                    }
                case BenchmarkListOverwriteStrategy.Overwrite:                    
                default:
                    _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.RemoveAll, null);
                    foreach (var school in manualComparisonList.BenchmarkSchools.Where(s => s.Urn != manualComparisonList.HomeSchoolUrn))
                    {
                        _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, school);
                    }
                    _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.SetDefault, new BenchmarkSchoolModel() {
                        Urn = manualComparisonList.HomeSchoolUrn,
                        Name = manualComparisonList.HomeSchoolName,
                        Type = manualComparisonList.HomeSchoolType,
                        EstabType = manualComparisonList.HomeSchoolFinancialType
                    });
                    return Redirect("/BenchmarkCharts");
            }
        }

        public PartialViewResult AddSchool(int urn)
        {
            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), null);

            try
            {
                _benchmarkBasketCookieManager.UpdateManualComparisonListCookie(CookieActions.Add,
                    new BenchmarkSchoolModel()
                    {
                        Name = benchmarkSchool.Name,
                        Urn = benchmarkSchool.Id.ToString(),
                        Type = benchmarkSchool.Type,
                        EstabType = benchmarkSchool.EstablishmentType.ToString()
                    });
            }
            catch (ApplicationException ex)
            {
                ViewBag.Error = ex.Message;
            }

            var vm = _benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie();

            return PartialView("Partials/SchoolsToAdd", vm.BenchmarkSchools.Where(s => s.Id != vm.HomeSchoolUrn).ToList());
        }

        public PartialViewResult RemoveSchool(int urn)
        {
            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), null);

            _benchmarkBasketCookieManager.UpdateManualComparisonListCookie(CookieActions.Remove, new BenchmarkSchoolModel()
            {
                Name = benchmarkSchool.Name,
                Urn = benchmarkSchool.Id.ToString(),
                Type = benchmarkSchool.Type,
                EstabType = benchmarkSchool.EstablishmentType.ToString()
            });

            var vm = _benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie();

            return PartialView("Partials/SchoolsToAdd", vm.BenchmarkSchools.Where(s => s.Id != vm.HomeSchoolUrn).ToList());
        }

        public JsonResult UpdateManualBasket(int? urn, CookieActions withAction)
        {
            if (urn.HasValue)
            {
                var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn.GetValueOrDefault()), null);

                _benchmarkBasketCookieManager.UpdateManualComparisonListCookie(withAction,
                    new BenchmarkSchoolModel()
                    {
                        Name = benchmarkSchool.Name,
                        Urn = benchmarkSchool.Id.ToString(),
                        Type = benchmarkSchool.Type,
                        EstabType = benchmarkSchool.EstablishmentType.ToString()
                    });
            }
            else
            {
                _benchmarkBasketCookieManager.UpdateManualComparisonListCookie(withAction, null);
            }

            return Json(_benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie().BenchmarkSchools.Count, JsonRequestBehavior.AllowGet);
        }
    }
}