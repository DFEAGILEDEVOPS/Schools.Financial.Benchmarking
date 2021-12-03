using Moq;
using NUnit.Framework;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Services.Search;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Services.LocalAuthorities;

namespace SFB.Web.UI.UnitTests
{
    public class TrustSearchControllerUnitTests
    {
        private Mock<HttpRequestBase> _request;
        private Mock<HttpContextBase> _context;
        private RequestContext _rc;
        private IValidationService _valService;
        private Mock<IFilterBuilder> _mockFilterBuilder;
        private Mock<ILocalAuthoritiesService> _mockLaService;
        private Mock<ILocationSearchService> _mockLocationSearchService;
        private Mock<ILaSearchService> _mockLaSearchService;
        private Mock<IContextDataService> _mockContextDataService;
        private Mock<ISchoolSearchService> _mockSchoolSearchService;
        private Mock<ITrustSearchService> _mockTrustSearchService;
        private Mock<ISchoolBenchmarkListService> _mockCookieManager;

        [SetUp]
        public void Setup()
        {
            _request = new Mock<HttpRequestBase>(MockBehavior.Default);
            _context = new Mock<HttpContextBase>(MockBehavior.Default);
            _context.SetupGet(x => x.Request).Returns(_request.Object);
            var requestCookies = new HttpCookieCollection();
            _context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            _rc = new RequestContext(_context.Object, new RouteData());

            _valService = new ValidationService();//Real validation service is tested in this scope
            _mockFilterBuilder = new Mock<IFilterBuilder>();
            _mockLaService = new Mock<ILocalAuthoritiesService>();
            _mockLocationSearchService = new Mock<ILocationSearchService>();
            _mockLaSearchService = new Mock<ILaSearchService>();
            _mockContextDataService = new Mock<IContextDataService>();
            _mockSchoolSearchService = new Mock<ISchoolSearchService>();
            _mockTrustSearchService = new Mock<ITrustSearchService>();
            _mockCookieManager = new Mock<ISchoolBenchmarkListService>();
        }

        //[Test]
        //public async Task SearchByNameActionRedirectsToSuggestionViewIfNoTrustFound()
        //{
        //    Task<dynamic> task = Task.Run(() =>
        //    {
        //        var facets = new Dictionary<string, FacetResultModel[]>();
        //        var matches = new List<Dictionary<string, object>>();
        //        dynamic results = new QueryResultsModel(0, facets, matches, 0, 0);
        //        return results;
        //    });

        //    _mockTrustSearchService.Setup(m => m.SearchTrustByName("TestTrust", 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, null, null))
        //        .Returns((string name, int skip, int take, string @orderby, NameValueCollection queryParams) => task);

        //    var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
        //        _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

        //    var result = await controller.Search("TestTrust", SearchTypes.SEARCH_BY_TRUST_NAME_ID, null, null, null, null, false, null, 0);

        //    Assert.AreEqual("TrustSearch", (result as RedirectToRouteResult).RouteValues["controller"]);
        //    Assert.AreEqual("SuggestTrust", (result as RedirectToRouteResult).RouteValues["action"]);
        //    Assert.AreEqual("TestTrust", (result as RedirectToRouteResult).RouteValues["trustNameId"]);
        //}

        [Test]
        public async Task SearchByNameActionReturnsTrustSearchResultsViewIfValidTrustNameProvided()
        {
            Task<SearchResultsModel<TrustSearchResult>> task = Task.Run(() =>
            {
                var facets = new Dictionary<string, FacetResultModel[]>();
                var matches = new List<TrustSearchResult>();
                matches.Add(new TrustSearchResult());                
                var results = new SearchResultsModel<TrustSearchResult>(5, facets, matches, 5, 0);
                return results;
            });

            _mockTrustSearchService.Setup(m => m.SearchTrustByNameAsync("TestTrust", 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, null, null))
                .Returns((string name, int skip, int take, string @orderby, NameValueCollection queryParams) => task);

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search("TestTrust", null, SearchTypes.SEARCH_BY_TRUST_NAME_ID, null, null, null, null, false, null, 1);

            Assert.IsTrue(result is ViewResult);
            Assert.AreEqual("SearchResults", (result as ViewResult).ViewName);
        }

        [Test]
        public async Task SearchActionRedirectsToTrustViewIfCompanyNumberIsUsedAsId()
        {
            Task<SearchResultsModel<TrustSearchResult>> task = Task.Run(() =>
            {
                var facets = new Dictionary<string, FacetResultModel[]>();
                var matches = new List<TrustSearchResult>();
                matches.Add(new TrustSearchResult());
                var results = new SearchResultsModel<TrustSearchResult>(5, facets, matches, 5, 0);
                return results;
            });

            _mockTrustSearchService.Setup(m => m.SearchTrustByCompanyNoAsync("6182612", 0, 1, "", null)).Returns(task);

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("6182612", null, SearchTypes.SEARCH_BY_TRUST_NAME_ID, null, null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("Trust", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("6182612", (result as RedirectToRouteResult).RouteValues["companyNo"].ToString());
        }

        [Test]
        public async Task SearchActionRedirectsToTrustViewIfSuggestedCompanyNumberIsUsed()
        {
            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("test_name", "5112090", SearchTypes.SEARCH_BY_TRUST_NAME_ID, null, null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("Trust", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("5112090", (result as RedirectToRouteResult).RouteValues["companyNo"].ToString());
        }

        [Test]
        public async Task SearchByNameReturnsErrorPageForInvalidCompanyNo()
        {
            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search("12345", null, SearchTypes.SEARCH_BY_TRUST_NAME_ID, null, null, null, null, false, null, 0);

            Assert.IsTrue(result is ViewResult);
            Assert.IsNotEmpty(((result as ViewResult).Model as SearchViewModel).ErrorMessage);
        }

        [Test]
        public async Task SearchByNameReturnsErrorPageForInvalidTrustName()
        {
            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search("te", null, SearchTypes.SEARCH_BY_TRUST_NAME_ID, null, null, null, null, false, null, 0);

            Assert.IsTrue(result is ViewResult);
            Assert.IsNotEmpty(((result as ViewResult).Model as SearchViewModel).ErrorMessage);
        }

        [Test]
        public async Task SearchByNameReturnsErrorPageForEmptyTrustName()
        {
            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_NAME_ID, null, null, null, null, false, null, 0);

            Assert.IsTrue(result is ViewResult);
            Assert.IsNotEmpty(((result as ViewResult).Model as SearchViewModel).ErrorMessage);
        }

        [Test]
        public async Task SearchByLocationReturnsErrorPageForEmptyLocation()
        {
            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_LOCATION, null, null, null, null, false, null, 0);

            Assert.IsTrue(result is ViewResult);
            Assert.IsNotEmpty(((result as ViewResult).Model as SearchViewModel).ErrorMessage);
        }

        [Test]
        public async Task SearchByLocationReturnsErrorPageForInvalidLocation()
        {
            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_LOCATION, "x", null, null, null, false, null, 0);

            Assert.IsTrue(result is ViewResult);
            Assert.IsNotEmpty(((result as ViewResult).Model as SearchViewModel).ErrorMessage);
        }

        [Test]
        public async Task SearchByLocationReturnsToViewWithErrorForNotFoundLocation()
        {
            _mockLocationSearchService.Setup(m => m.SuggestLocationName("sw12")).Returns(new SuggestionQueryResult(new List<Disambiguation>()));

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_LOCATION, "sw12", null, null, null, false, null, 0);

            Assert.IsTrue(result is ViewResult);
            Assert.IsNotEmpty(((result as ViewResult).Model as SearchViewModel).ErrorMessage);
        }

        [Test]
        public async Task SearchByLocationReturnsLocationSuggestViewForPossibleLocationsFound()
        {
            _mockLocationSearchService.Setup(m => m.SuggestLocationName("sw12")).Returns(new SuggestionQueryResult(new List<Disambiguation>() { new Disambiguation() { LatLon ="1,2", Text="test" } }));

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_LOCATION, "sw12", null, null, null, false, null, 0);

            Assert.AreEqual("Location", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Suggest", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("sw12", (result as RedirectToRouteResult).RouteValues["locationOrPostcode"]);
        }

        [Test]
        public async Task SearchByLocationReturnsViewWithErrorForNotFoundLocationCoordinates()
        {
            var edubaseSearchResponse = new SearchResultsModel<SchoolSearchResult>(0, null, null, 50, 0);

            var task = Task.Run(() =>
            {
                return edubaseSearchResponse;
            });

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByLatLonAsync("1", "2", SearchDefaults.TRUST_LOCATION_SEARCH_DISTANCE * 1.6m, 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, null, null))
                .Returns(task);

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_LOCATION, "sw12", "1,2", null, null, false, null, 0);

            Assert.IsTrue(result is ViewResult);
            Assert.IsNotEmpty(((result as ViewResult).Model as SearchViewModel).ErrorMessage);
        }

        [Test]
        public async Task SchoolsAreOrderedAlphabeticallyWhenTrustsAreOrderedByTotalCountInLocationSearch()
        {
            var edubaseSearchResponse = new SearchResultsModel<SchoolSearchResult>(0, null, null, 50, 0);

            var task = Task.Run(() =>
            {
                return edubaseSearchResponse;
            });

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByLatLonAsync("1", "2", SearchDefaults.TRUST_LOCATION_SEARCH_DISTANCE * 1.6m, 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, $"{EdubaseDataFieldNames.TRUSTS} asc", null))
                .Returns(task);

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_LOCATION, "sw12", "1,2", null, null, false, "AreaSchoolNumber", 0);

            _mockSchoolSearchService.Verify(m => m.SearchSchoolByLatLonAsync("1", "2", SearchDefaults.TRUST_LOCATION_SEARCH_DISTANCE * 1.6m, 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, $"{EdubaseDataFieldNames.TRUSTS} asc", null));    
        }

        [Test]
        public async Task SearchByLaReturnsErrorPageForInvalidLaName()
        {
            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, null, null, "x", null, false, null, 0);

            Assert.IsTrue(result is ViewResult);
            Assert.IsNotEmpty(((result as ViewResult).Model as SearchViewModel).ErrorMessage);
        }

        [Test]
        public async Task SearchByLaReturnsErrorPageForInvalidLaCode()
        {
            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, null, null, "1234", null, false, null, 0);

            Assert.IsTrue(result is ViewResult);
            Assert.IsNotEmpty(((result as ViewResult).Model as SearchViewModel).ErrorMessage);
        }

        [Test]
        public async Task SearchesByLaCodeIfAValidLaNameIsProvided()
        {
            var edubaseSearchResponse = new SearchResultsModel<SchoolSearchResult>(0, null, null, 50, 0);

            var task = Task.Run(() =>
            {
                return edubaseSearchResponse;
            });

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByLaCodeAsync("319", 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, $"{EdubaseDataFieldNames.TRUSTS} asc", null))
                .Returns(task);

            _mockLaSearchService.Setup(m => m.SearchExactMatch("Croydon")).Returns(new LaModel(){Id= "319", LaName= "Croydon" });

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, null, null, "Croydon", null, false, null, 0);

            _mockSchoolSearchService.Verify(m => m.SearchSchoolByLaCodeAsync("319", 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, $"{EdubaseDataFieldNames.TRUSTS} asc", null));
        }

        [Test]
        public async Task TReturnsViewWithErrorIfAValidLaNameIsNotProvided()
        {
            var edubaseSearchResponse = new SearchResultsModel<SchoolSearchResult>(0, null, null, 50, 0);

            var task = Task.Run(() =>
            {
                return edubaseSearchResponse;
            });

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByLaCodeAsync("319", 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, null, null))
                .Returns(task);

            _mockLaSearchService.Setup(m => m.SearchExactMatch("test")).Returns<LaModel>(null);
            _mockLaSearchService.Setup(m => m.SearchContains("test")).Returns(new List<LaModel>());

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, null, null, "test", null, false, null, 0);

            Assert.IsTrue(result is ViewResult);
            Assert.IsNotEmpty(((result as ViewResult).Model as SearchViewModel).ErrorMessage);
        }

        [Test]
        public async Task SearchLaEmptyLocationResultPageForNotFoundLaCodes()
        {
            var edubaseSearchResponse = new SearchResultsModel<SchoolSearchResult>(0, null, null, 50, 0);

            var task = Task.Run(() =>
            {
                return edubaseSearchResponse;
            });

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByLaCodeAsync("000", 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, $"{EdubaseDataFieldNames.TRUSTS} asc", null))
                .Returns(task);

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var response = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, null, null, "000", null, false, null, 0);

            Assert.IsNotNull(response);
            Assert.IsNotNull((response as ViewResult).Model);
            Assert.IsTrue(((response as ViewResult).Model as SearchViewModel).HasError());
            Assert.AreEqual(SearchErrorMessages.NO_LA_RESULTS, ((response as ViewResult).Model as SearchViewModel).ErrorMessage);
        }

        [Test]
        public async Task SchoolsAreOrderedAlphabeticallyWhenTrustsAreOrderedByTotalCountInLaSearch()
        {
            var edubaseSearchResponse = new SearchResultsModel<SchoolSearchResult>(0, null, null, 50, 0);

            var task = Task.Run(() =>
            {
                return edubaseSearchResponse;
            });

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByLaCodeAsync("123", 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, $"{EdubaseDataFieldNames.TRUSTS} asc", null))
                .Returns(task);

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockSchoolSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search(null, null, SearchTypes.SEARCH_BY_TRUST_LA_CODE_NAME, null, null, "123", null, false, "AreaSchoolNumber", 0);

            _mockSchoolSearchService.Verify(m => m.SearchSchoolByLaCodeAsync("123", 0, SearchDefaults.SEARCHED_SCHOOLS_MAX, $"{EdubaseDataFieldNames.TRUSTS} asc", null));
        }
    }
}
