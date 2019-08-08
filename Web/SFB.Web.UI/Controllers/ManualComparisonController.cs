using SFB.Web.Domain.Services;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFB.Web.UI.Controllers
{
    public class ManualComparisonController : Controller
    {
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;
        private readonly ILocalAuthoritiesService _laService;
        private readonly IContextDataService _contextDataService;

        public ManualComparisonController(IBenchmarkBasketCookieManager benchmarkBasketCookieManager, ILocalAuthoritiesService laService, IContextDataService contextDataService)
        {
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
            _laService = laService;
            _contextDataService = contextDataService;
        }

        // GET: ManualComparison
        public ActionResult Index()
        {
            var schoolComparisonListModel = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            var vm = new SchoolSearchViewModel(schoolComparisonListModel, "");
            vm.Authorities = _laService.GetLocalAuthorities();
            _benchmarkBasketCookieManager.UpdateManualComparisonListCookie(CookieActions.SetDefault, new BenchmarkSchoolModel()
            {
                Name = schoolComparisonListModel.HomeSchoolName,
                Urn = schoolComparisonListModel.HomeSchoolUrn,
                Type = schoolComparisonListModel.HomeSchoolType,
                EstabType = schoolComparisonListModel.HomeSchoolFinancialType
            });
            return View(vm);
        }

        public ActionResult ManualSearch(
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
        string referrer = "home/index")//TODO: Do we need this many parameters here?
        {
            switch (searchType)
            {
                case SearchTypes.SEARCH_BY_NAME_ID:
                    ViewBag.OpenOnly = openOnly;
                    var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
                    var manualComparisonList = _benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie();
                    var contextDataObject = _contextDataService.GetSchoolDataObjectByUrn(int.Parse(comparisonList.HomeSchoolUrn));
                    var vm = new SchoolViewModel(contextDataObject, comparisonList, manualComparisonList);
                    return View("AddSchoolsManually", vm);
                case SearchTypes.SEARCH_BY_LA_CODE_NAME:
                    //TODO: Call search with an extra parameter and return a new view
                    break;
                case SearchTypes.SEARCH_BY_LOCATION:
                    //TODO: Call search with an extra parameter and return a new view
                    break;
            }
            return null;
        }

        public PartialViewResult AddSchool(int urn)
        {
            var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn), null);

            _benchmarkBasketCookieManager.UpdateManualComparisonListCookie(CookieActions.Add,
                new BenchmarkSchoolModel()
                {
                    Name = benchmarkSchool.Name,
                    Urn = benchmarkSchool.Id.ToString(),
                    Type = benchmarkSchool.Type,
                    EstabType = benchmarkSchool.EstablishmentType.ToString()
                });

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

        public PartialViewResult RemoveAllTrusts()
        {
            _benchmarkBasketCookieManager.UpdateManualComparisonListCookie(CookieActions.RemoveAll, null);

            var vm = _benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie();

            return PartialView("Partials/SchoolsToAdd", vm.BenchmarkSchools.Where(s => s.Id != vm.HomeSchoolUrn).ToList());
        }
    }
}