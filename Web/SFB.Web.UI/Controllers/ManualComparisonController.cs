using RedDog.Search.Model;
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.Domain.Services;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Domain.Services.Search;
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
    public class ManualComparisonController : SchoolSearchBaseController
    {
        private readonly ILocalAuthoritiesService _laService;
        private readonly IContextDataService _contextDataService;
        private readonly IValidationService _valService;
        private readonly ILocationSearchService _locationSearchService;
        private readonly ILaSearchService _laSearchService;

        public ManualComparisonController(IBenchmarkBasketCookieManager benchmarkBasketCookieManager, ILocalAuthoritiesService laService, 
            IContextDataService contextDataService, IValidationService valService, ILocationSearchService locationSearchService, 
            ISchoolSearchService schoolSearchService, IFilterBuilder filterBuilder, ILaSearchService laSearchService)
            : base(schoolSearchService, benchmarkBasketCookieManager, filterBuilder)
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
            var vm = new SchoolSearchViewModel(schoolComparisonListModel, "");
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

        public async Task<ActionResult> ManualSearch(
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
            var contextDataObject = _contextDataService.GetSchoolDataObjectByUrn(int.Parse(comparisonList.HomeSchoolUrn));
            dynamic searchResp = null;
            string errorMessage;

            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_NAME_ID:
                    var vm = new SchoolViewModel(contextDataObject, comparisonList, manualComparisonList);
                    return View("AddSchoolsManually", vm);
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
                                return await ManualSearch(nameId, searchType, suggestionUrn, locationorpostcode,
                                    locationCoordinates, laCodeName, radius, openOnly, orderby, page, tab);
                            }
                            TempData["SearchMethod"] = "Manual";
                            return RedirectToAction("Search", "La", new { name = laCodeName, openOnly = openOnly });
                        }
                        else
                        {
                            var svm = new SchoolSearchViewModel(comparisonList, searchType)
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
                            searchResp = await GetSearchResults(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);

                            int resultCount = searchResp.NumberOfResults;
                            if (resultCount == 0)
                            {
                                return View("EmptyResult", new SchoolSearchViewModel(comparisonList, searchType));
                            }
                            else
                            {
                                ViewBag.manualCount = manualComparisonList?.BenchmarkSchools?.Count();
                                return View("ManualSearchResults", GetSchoolViewModelList(searchResp, manualComparisonList, orderby, page, searchType, nameId, locationorpostcode, _laService.GetLaName(laCodeName)));
                            }
                        }
                        else
                        {
                            var svm = new SchoolSearchViewModel(comparisonList, searchType)
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
                                        new SchoolSearchViewModel(_benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), searchType));
                                default:
                                    TempData["LocationResults"] = result;
                                    TempData["SearchMethod"] = "Manual";
                                    return RedirectToAction("Suggest", "Location", new { locationOrPostcode = locationorpostcode, openOnly = openOnly });
                            }
                        }
                        else
                        {
                            searchResp = await GetSearchResults(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);

                            if (searchResp.NumberOfResults == 0)
                            {
                                return View("EmptyManualLocationResult", new SchoolSearchViewModel(comparisonList, searchType));
                            }
                            ViewBag.manualCount = manualComparisonList?.BenchmarkSchools?.Count();
                            return View("ManualSearchResults", GetSchoolViewModelList(searchResp, manualComparisonList, orderby, page, searchType, nameId, locationorpostcode, _laService.GetLaName(laCodeName)));
                        }
                    }
                    else
                    {
                        var svm = new SchoolSearchViewModel(comparisonList, searchType)
                        {
                            SearchType = searchType,
                            Authorities = _laService.GetLocalAuthorities(),
                            ErrorMessage = errorMessage
                        };
                        return View("Index" , svm);
                    }
            }
        }

        [Route("ManualSearch/Search-js")]
        public async Task<PartialViewResult> ManualSearchJS(string nameId, string searchType, string suggestionurn,
        string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, 
        decimal? radius, bool openOnly = false, string orderby = "", int page = 1)
        {
            dynamic searchResponse;

            var schoolComparisonList = _benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie();

            if (IsLaEstab(nameId))
            {
                searchResponse = await GetSearchResults(nameId, SearchTypes.SEARCH_BY_LA_ESTAB, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);
            }
            else
            {
                searchResponse = await GetSearchResults(nameId, searchType, locationorpostcode, locationCoordinates, laCodeName, radius, openOnly, orderby, page);
            }
            var vm = GetSchoolViewModelList(searchResponse, schoolComparisonList, orderby, page, searchType, nameId, locationorpostcode, laCodeName);

            ViewBag.SearchMethod = "Manual";
            return PartialView("Partials/Search/SchoolResults", vm);
        }

        [Route("ManualSearch/Search-json")]
        public async Task<JsonResult> ManualSearchJson(string nameId, string searchType, string suggestionurn,
    string locationorpostcode, string locationCoordinates, string laCodeName, string schoolId, decimal? radius,
    int? companyNo, bool openOnly = false, string orderby = "", int page = 1)

        {
            dynamic searchResponse;
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

            var results = new List<SchoolSummaryViewModel>();
            foreach (var result in searchResponse.Results)
            {
                var schoolVm = new SchoolSummaryViewModel(result);
                results.Add(schoolVm);
            }

            return Json(new { count = results.Count, results = results }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OverwriteStrategy()
        {
            var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            var manualComparisonList = _benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie();
            if (comparisonList?.BenchmarkSchools?.Count > 0 && !comparisonList.BenchmarkSchools.All(s => s.Urn == comparisonList.HomeSchoolUrn))
            {
                var contextDataObject = _contextDataService.GetSchoolDataObjectByUrn(int.Parse(comparisonList.HomeSchoolUrn));
                var vm = new SchoolViewModel(contextDataObject, comparisonList, manualComparisonList);
                ViewBag.referrer = Request.UrlReferrer;
                return View(vm);
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