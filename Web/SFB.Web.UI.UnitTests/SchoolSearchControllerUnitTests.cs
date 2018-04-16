using Moq;
using Newtonsoft.Json;
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
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Domain.Services.Search;
using SFB.Web.UI.Helpers.Constants;

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
        private Mock<IContextDataService> _mockEdubaseDataService;
        private Mock<ISchoolSearchService> _mockEdubaseSearchService;
        private Mock<ITrustSearchService> _mockTrustSearchService;

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
            _mockEdubaseDataService = new Mock<IContextDataService>();
            _mockEdubaseSearchService = new Mock<ISchoolSearchService>();
            _mockTrustSearchService = new Mock<ITrustSearchService>();
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

            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);
            controller.ControllerContext = new ControllerContext(_rc, controller);
 
            var result = await controller.Search("Test", "", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, null);

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

            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);
            controller.ControllerContext = new ControllerContext(_rc, controller);
 
            var result = await controller.Search(null, "", SearchTypes.SEARCH_BY_LA_CODE_NAME, null, null, null, "Test", null, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("Search", (result as RedirectToRouteResult).RouteValues["Action"]);
            Assert.AreEqual("Test", (result as RedirectToRouteResult).RouteValues["Name"]);
        }


        [Test]
        public async Task SearchActionReturnsHomeViewIfNotValid()
        {
            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);
            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("" , "", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("../Home/Index", (result as ViewResult).ViewName);            
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfValidUrnProvided()
        {
            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);

            var result = await controller.Search("", "", SearchTypes.SEARCH_BY_NAME_ID, "123456", null, null, null, null, null, 0);

            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("123456", (result as RedirectToRouteResult).RouteValues["urn"]);
        }

        [Test]
        public async Task SearchActionRedirectsToTrustSearchViewIfValidTrustNameProvided()
        {
            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);

            var result = await controller.Search("", "TestTrust", SearchTypes.SEARCH_BY_TRUST_NAME, null, null, null, null, null, null, 0);

            Assert.AreEqual("Trust", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Search", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("TestTrust", (result as RedirectToRouteResult).RouteValues["name"]);
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfUrnIsUsedAsId()
        {            
            dynamic testResult = new Microsoft.Azure.Documents.Document();
            testResult.URN = "123456";

            _mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123456")).Returns((string urn) => testResult);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);

            var result = await controller.Search("123456", "", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("123456", (result as RedirectToRouteResult).RouteValues["urn"]);
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfLaEstabIsUsedAsId()
        {
            dynamic testResult = new Microsoft.Azure.Documents.Document();
            testResult.URN = "1234567";

            _mockEdubaseDataService.Setup(m => m.GetSchoolByLaEstab("1234567")).Returns((string urn) => testResult);
                       
            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("1234567", null, SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("1234567", (result as RedirectToRouteResult).RouteValues["urn"].ToString());
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfLaEstabWithDashIsUsedAsId()
        {
            dynamic testResult = new Microsoft.Azure.Documents.Document();
            testResult.URN = "1234567";

            _mockEdubaseDataService.Setup(m => m.GetSchoolByLaEstab("1234567")).Returns((string urn) => testResult);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("123-4567", null, SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("1234567", (result as RedirectToRouteResult).RouteValues["urn"].ToString());
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfLaEstabWithSlashIsUsedAsId()
        {
            dynamic testResult = new Microsoft.Azure.Documents.Document();
            testResult.URN = "1234567";

            _mockEdubaseDataService.Setup(m => m.GetSchoolByLaEstab("1234567")).Returns((string urn) => testResult);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("123/4567", null, SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("1234567", (result as RedirectToRouteResult).RouteValues["urn"].ToString());
        }

        [Test]
        public void SearchActionCallsServiceWithPagingParams()
        {
            var testDictionary = new Dictionary<string, object>();
            testDictionary.Add("URN", "654321");
            dynamic edubaseSearchResponse = new QueryResultsModel(1, null, new List<IDictionary<string, object>>() { testDictionary }, 50, 0);
            Task<dynamic> task = Task.Run(() =>
            {
                return edubaseSearchResponse;
            });

            _mockEdubaseSearchService.Setup(m => m.SearchSchoolByName("Test", 0, 50, string.Empty, null))
                .Returns((string name, int skip, int take, string orderby, NameValueCollection queryParams) => task);            
            
            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);
            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.Search("Test", "", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, "", 2);

            _mockEdubaseSearchService.Verify(req => req.SearchSchoolByName("Test", 50, 50, "", null), Times.Once());
        }

        [Test]
        public void UpdateActionReturnsCookieWhenAddToComparisonList()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var requestCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var responseCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Response.Cookies).Returns(responseCookies);
            var rc = new RequestContext(context.Object, new RouteData());
            
            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);

            controller.ControllerContext = new ControllerContext(rc, controller);

            dynamic testResult = new Microsoft.Azure.Documents.Document();
            testResult.URN = "123456";

            _mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123456")).Returns((string urn) => testResult);

            var result = controller.UpdateBenchmarkBasket(123456, CompareActions.ADD_TO_COMPARISON_LIST);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, controller.Response.Cookies.Count);
            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);
            Assert.AreEqual(1, cookie.BenchmarkSchools.Count);
            Assert.AreEqual("123456", cookie.BenchmarkSchools[0].Urn);
        }

        [Test]
        public void UpdateActionReturnsCookieWhenMultipleAddToComparisonList()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var requestCookies = new HttpCookieCollection();
            var listCookie = new ComparisonListModel();
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>() { new BenchmarkSchoolViewModel() { Urn = "123", Name = "test" } };
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var responseCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Response.Cookies).Returns(responseCookies);
            var rc = new RequestContext(context.Object, new RouteData());

            dynamic testResult = new Microsoft.Azure.Documents.Document();
            testResult.URN = "123456";

            _mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123456")).Returns((string urn) => testResult);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);

            controller.ControllerContext = new ControllerContext(rc, controller);

            var result = controller.UpdateBenchmarkBasket(123456, CompareActions.ADD_TO_COMPARISON_LIST);

            Assert.AreEqual(1, controller.Response.Cookies.Count);
            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);
            Assert.AreEqual(2, cookie.BenchmarkSchools.Count);
            Assert.AreEqual("123", cookie.BenchmarkSchools[0].Urn);
            Assert.AreEqual("123456", cookie.BenchmarkSchools[1].Urn);
        }

        [Test]
        public void UpdateActionCannotAddMoreThanLimitToComparisonList()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var requestCookies = new HttpCookieCollection();
            var listCookie = new ComparisonListModel();
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>();
            for (int i = 0; i < ComparisonListLimit.LIMIT; i++)
            {
                listCookie.BenchmarkSchools.Add(new BenchmarkSchoolViewModel() { Urn = i.ToString(), Name = "test"});
            }
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var responseCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Response.Cookies).Returns(responseCookies);
            var rc = new RequestContext(context.Object, new RouteData());

            dynamic testResult = new Microsoft.Azure.Documents.Document();
            testResult.URN = "123456";

            _mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123456")).Returns((string urn) => testResult);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);

            controller.ControllerContext = new ControllerContext(rc, controller);

            var result = controller.UpdateBenchmarkBasket(123456, CompareActions.ADD_TO_COMPARISON_LIST);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, controller.Response.Cookies.Count);
            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);
            Assert.AreEqual(ComparisonListLimit.LIMIT, cookie.BenchmarkSchools.Count);
        }

        [Test]
        public void UpdateActionReturnsCookieWhenDoubleAddToComparisonList()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var requestCookies = new HttpCookieCollection();
            var listCookie = new ComparisonListModel();
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>() { new BenchmarkSchoolViewModel() { Urn = "123", Name = "test"} };
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var responseCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Response.Cookies).Returns(responseCookies);
            var rc = new RequestContext(context.Object, new RouteData());
            
            dynamic testResult = new Microsoft.Azure.Documents.Document();
            testResult.URN = "123456";

            _mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123456")).Returns((string urn) => testResult);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);

            controller.ControllerContext = new ControllerContext(rc, controller);

            controller.UpdateBenchmarkBasket(123456, CompareActions.ADD_TO_COMPARISON_LIST);
            var result = controller.UpdateBenchmarkBasket(123456, CompareActions.ADD_TO_COMPARISON_LIST);

            Assert.IsNotNull(result);
            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);
            Assert.AreEqual(2, cookie.BenchmarkSchools.Count);
            Assert.AreEqual("123456", cookie.BenchmarkSchools[1].Urn);
        }

        [Test]
        public void UpdateActionReturnsCookieWhenRemovedFromComparisonList()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var requestCookies = new HttpCookieCollection();
            var listCookie = new ComparisonListModel();
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>()
            {
                new BenchmarkSchoolViewModel() {Urn = "123", Name = "test"}
            };
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var responseCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Response.Cookies).Returns(responseCookies);

            dynamic testResult = new Microsoft.Azure.Documents.Document();
            testResult.URN = "123";

            _mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123")).Returns((string urn) => testResult);

            var rc = new RequestContext(context.Object, new RouteData());

            var controller = new SchoolSearchController(_mockLaService.Object, _mockFilterBuilder.Object, _valService, _mockEdubaseDataService.Object, _mockEdubaseSearchService.Object, _mockTrustSearchService.Object);

            controller.ControllerContext = new ControllerContext(rc, controller);

            var result = controller.UpdateBenchmarkBasket(123, CompareActions.REMOVE_FROM_COMPARISON_LIST);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, controller.Response.Cookies.Count);
            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);
            Assert.AreEqual(0, cookie.BenchmarkSchools.Count);
        }

        #region pd-8746
        [Test]
        public void SearchGivenSuggestionUrnOrSchoolIdActionRedirectsToSchoolDetailPage() { }

        public void SearchGivenPartialNameIfOnlyOneMatchActionRedirectsToSchoolDetailPage()
        {
        }

        public void SearchGivenNameAndUrnOrSchoolIdNameIsIgnoredAndActionRedirectsToSchoolDetailPage()
        {
        }

        public void SearchGivenPartialNameIfTwoOrMoreMatchesActionRedirectsToSuggestionPage()
        {
        }

        #endregion
    }
}
