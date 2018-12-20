using Moq;
using NUnit.Framework;
using SFB.Web.Domain.Services;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Domain.Services.Search;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Services;
using SFB.Web.Common.DataObjects;

namespace SFB.Web.UI.UnitTests
{
    public class SchoolSearchControllerUnitTests
    {        
        private Mock<HttpRequestBase> _request;
        private Mock<HttpContextBase> _context;
        private RequestContext _rc;
        private IValidationService _valService;
        private Mock<IFilterBuilder> _mockFilterBuilder;
        private Mock<ILocalAuthoritiesService> _mockLaService;
        private Mock<ILocationSearchService> _mockLocationSearchService;
        private Mock<ILaSearchService> _mockLaSearchService;
        private Mock<IContextDataService> _mockEdubaseDataService;
        private Mock<ISchoolSearchService> _mockEdubaseSearchService;
        private Mock<ITrustSearchService> _mockTrustSearchService;
        private Mock<IBenchmarkBasketCookieManager> _mockCookieManager;

        [SetUp]
        public void Setup()
        {
            _request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            _context = new Mock<HttpContextBase>(MockBehavior.Strict);
            _context.SetupGet(x => x.Request).Returns(_request.Object);
            var requestCookies = new HttpCookieCollection();
            _context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            _rc = new RequestContext(_context.Object, new RouteData());

            _valService = new ValidationService();//Real validation service is tested in this scope
            _mockFilterBuilder = new Mock<IFilterBuilder>();
            _mockLaService = new Mock<ILocalAuthoritiesService>();
            _mockLocationSearchService = new Mock<ILocationSearchService>();
            _mockLaSearchService = new Mock<ILaSearchService>();            
            _mockEdubaseDataService = new Mock<IContextDataService>();
            _mockEdubaseSearchService = new Mock<ISchoolSearchService>();
            _mockTrustSearchService = new Mock<ITrustSearchService>();
            _mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
        }

        [Test]
        public async Task SearchActionReturnsSuggestionsViewIfResultEmpty()
        {
            dynamic edubaseSearchResponse = new QueryResultsModel(0, null, null, 50, 0);            

            Task<dynamic> task = Task.Run(() =>
            {
                return edubaseSearchResponse;
            });

            _mockEdubaseSearchService.Setup(m => m.SearchSchoolByName("Test", 0, 50, null, null)).Returns((string name, int skip, int take, string orderby, NameValueCollection queryParams) => task);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, _valService, 
                _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);
            controller.ControllerContext = new ControllerContext(_rc, controller);
 
            var result = await controller.Search("Test", "", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null);

            Assert.IsNotNull(result);
            Assert.AreEqual("SchoolSearch", (result as RedirectToRouteResult).RouteValues["Controller"]);
            Assert.AreEqual("SuggestSchool", (result as RedirectToRouteResult).RouteValues["Action"]);
            Assert.AreEqual("Test", (result as RedirectToRouteResult).RouteValues["NameId"]);
        }

        [Test]
        public async Task SearchActionReturnsSuggestionsViewIfSearchByLAName()
        {
            dynamic laSearchResponse = new List<dynamic>()
            {
                new{
                    id = "840",
                    LANAME = "County Durham",
                    REGION = "1",
                    REGIONNAME = "North East A"
                },
                new{
                    id = "841",
                    LANAME = "Darlington",
                    REGION = "1",
                    REGIONNAME = "North East A"
                }
            };            

            _mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => laSearchResponse);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);
 
            var result = await controller.Search(null, "", SearchTypes.SEARCH_BY_LA_CODE_NAME, null, null, null, "Test", null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("La", (result as RedirectToRouteResult).RouteValues["Controller"]);
            Assert.AreEqual("Search", (result as RedirectToRouteResult).RouteValues["Action"]);
            Assert.AreEqual("Test", (result as RedirectToRouteResult).RouteValues["Name"]);
        }

        [Test]
        public async Task SearchActionReturnsSuggestionsViewIfSearchByLocationKeyword()
        {
            _mockLocationSearchService.Setup(m => m.SuggestLocationName("Test")).Returns(() => new SuggestionQueryResult(new List<Disambiguation>() { new Disambiguation { Text= "Test" }  }));

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search(null, "", SearchTypes.SEARCH_BY_LOCATION, null, "Test", null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("Location", (result as RedirectToRouteResult).RouteValues["Controller"]);
            Assert.AreEqual("Suggest", (result as RedirectToRouteResult).RouteValues["Action"]);
            Assert.AreEqual("Test", (result as RedirectToRouteResult).RouteValues["locationorpostcode"]);
        }

        [Test]
        public async Task SearchActionRunsAndReturnsTheOnlyLaSuggestionResultIfSearchByLAName()
        {
            dynamic laSearchResponse = new List<dynamic>()
            {
                new{
                    id = "840",
                    LANAME = "County Durham",
                    REGION = "1",
                    REGIONNAME = "North East A"
                },
                new{
                    id = "841",
                    LANAME = "Darlington",
                    REGION = "1",
                    REGIONNAME = "North East A"
                }
            };

            _mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => laSearchResponse);

            _mockLaSearchService.Setup(m => m.SearchExactMatch("Test")).Returns(() => new LaViewModel(){ id="123", LaName = "Test"});

            var testDictionary = new Dictionary<string, object>();
            testDictionary.Add("URN", "654321");
            dynamic edubaseSearchResponse = new QueryResultsModel(2, null, new List<IDictionary<string, object>>() { testDictionary }, 50, 0);
            Task<dynamic> task = Task.Run(() =>
            {
                return edubaseSearchResponse;
            });

            _mockEdubaseSearchService.Setup(m => m.SearchSchoolByLaCode("123", 0, 50, "EstablishmentName", null)).Returns((string name, int skip, int take, string orderby, NameValueCollection queryParams) => task);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, 
                _mockEdubaseSearchService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search(null, "", SearchTypes.SEARCH_BY_LA_CODE_NAME, null, null, null, "Test", null, false, null);

            Assert.IsTrue(result is ViewResult);
            Assert.AreEqual("SearchResults", (result as ViewResult).ViewName);
        }


        [Test]
        public async Task SearchActionReturnsHomeViewIfNotValid()
        {
            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object,
                _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object,
                _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("" , "", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("../home/index", (result as ViewResult).ViewName);            
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfValidUrnProvided()
        {
            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, _valService, 
                _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search("", "", SearchTypes.SEARCH_BY_NAME_ID, "123456", null, null, null, null, false, null, 0);

            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("123456", (result as RedirectToRouteResult).RouteValues["urn"]);
        }

        [Test]
        public async Task SearchActionRedirectsToTrustSearchViewIfValidTrustNameProvided()
        {
            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, 
                _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search("", "TestTrust", SearchTypes.SEARCH_BY_TRUST_NAME, null, null, null, null, null, false, null, 0);

            Assert.AreEqual("Trust", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Search", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("TestTrust", (result as RedirectToRouteResult).RouteValues["name"]);
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfUrnIsUsedAsId()
        {            
            var testResult = new EdubaseDataObject();
            testResult.URN = 123456;

            _mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(123456)).Returns((int urn) => testResult);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, _valService, 
                _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search("123456", "", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual(123456, (result as RedirectToRouteResult).RouteValues["urn"]);
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfLaEstabIsUsedAsId()
        {
            var testResult = new EdubaseDataObject();
            testResult.URN = 1234567;

            _mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByLaEstab("1234567")).Returns((string urn) => testResult);
                       
            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, 
                _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("1234567", null, SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("1234567", (result as RedirectToRouteResult).RouteValues["urn"].ToString());
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfLaEstabWithDashIsUsedAsId()
        {
            var testResult = new EdubaseDataObject();
            testResult.URN = 1234567;

            _mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByLaEstab("1234567")).Returns((string urn) => testResult);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, 
                _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("123-4567", null, SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("1234567", (result as RedirectToRouteResult).RouteValues["urn"].ToString());
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfLaEstabWithSlashIsUsedAsId()
        {
            var testResult = new EdubaseDataObject();
            testResult.URN = 1234567;

            _mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByLaEstab("1234567")).Returns((string urn) => testResult);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, _valService, 
                _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("123/4567", null, SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("1234567", (result as RedirectToRouteResult).RouteValues["urn"].ToString());
        }

        [Test]
        public async Task SearchActionCallsServiceWithPagingParams()
        {
            var testDictionary = new Dictionary<string, object>();
            testDictionary.Add("URN", "654321");
            dynamic edubaseSearchResponse = new QueryResultsModel(1, null, new List<IDictionary<string, object>>() { testDictionary }, 50, 0);
            Task<dynamic> task = Task.Run(() =>
            {
                return edubaseSearchResponse;
            });

            _mockEdubaseSearchService.Setup(m => m.SearchSchoolByName("Test", 50, 50, string.Empty, null))
                .Returns((string name, int skip, int take, string orderby, NameValueCollection queryParams) => task);            
            
            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, _valService, 
                _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);
            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("Test", "", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, "", 2);

            _mockEdubaseSearchService.Verify(req => req.SearchSchoolByName("Test", 50, 50, "", null), Times.Once());
        }
      
    }
}
