using Moq;
using NUnit.Framework;
using RedDog.Search.Model;
using SFB.Web.Common.DataObjects;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Domain.Services.Search;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Services;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

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
        private Mock<IBenchmarkBasketCookieManager> _mockCookieManager;

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
            _mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
        }

        [Test]
        public async Task SearchActionRedirectsToSuggestionViewIfNoTrustFound()
        {
            Task<dynamic> task = Task.Run(() =>
            {
                var facets = new Dictionary<string, FacetResult[]>();
                var matches = new List<Dictionary<string, object>>();
                dynamic results = new QueryResultsModel(0, facets, matches, 0, 0);
                return results;
            });

            _mockTrustSearchService.Setup(m => m.SearchTrustByName("TestTrust", 0, SearchDefaults.RESULTS_PER_PAGE, null, null))
                .Returns((string name, int skip, int take, string @orderby, NameValueCollection queryParams) => task);

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search("TestTrust", SearchTypes.SEARCH_BY_TRUST_NAME_ID, null, null, null, null, null, false, null, 0);

            Assert.AreEqual("TrustSearch", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("SuggestTrust", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("TestTrust", (result as RedirectToRouteResult).RouteValues["trustNameId"]);
        }

        [Test]
        public async Task SearchActionReturnsTrustSearchResultsViewIfValidTrustNameProvided()
        {
            Task<dynamic> task = Task.Run(() =>
            {
                var facets = new Dictionary<string, FacetResult[]>();
                facets.Add("OverallPhase", new FacetResult[] { new FacetResult() { Value = "Primary", Count = 2 }, new FacetResult() { Value = "Secondary", Count = 1 }, new FacetResult() { Value = "All through", Count = 1 } });
                facets.Add("TypeOfEstablishment", new FacetResult[] { new FacetResult() { Value = "Pupil Referral Unit", Count = 2 }, new FacetResult() { Value = "Nursery", Count = 1 }, new FacetResult() { Value = "Primary", Count = 1 } });
                facets.Add("OfstedRating", new FacetResult[] { new FacetResult() { Value = "Outstanding", Count = 2 }, new FacetResult() { Value = "Good", Count = 1 }, new FacetResult() { Value = "Requires Improvement", Count = 1 } });
                facets.Add("ReligiousCharacter", new FacetResult[] { new FacetResult() { Value = "Hindu", Count = 2 }, new FacetResult() { Value = "Church of England", Count = 1 }, new FacetResult() { Value = "Roman Catholic", Count = 1 } });
                facets.Add("EstablishmentStatus", new FacetResult[] { new FacetResult() { Value = "Open", Count = 2 }, new FacetResult() { Value = "Closed", Count = 1 } });

                var matchedResults = new Dictionary<string, object>();
                matchedResults.Add("CompanyNumber", "132");
                matchedResults.Add("TrustOrCompanyName", "test");
                var matches = new List<Dictionary<string, object>>();
                matches.Add(matchedResults);
                dynamic results = new QueryResultsModel(5, facets, matches, 5, 0);
                return results;
            });

            _mockTrustSearchService.Setup(m => m.SearchTrustByName("TestTrust", 0, SearchDefaults.RESULTS_PER_PAGE, null, null))
                .Returns((string name, int skip, int take, string @orderby, NameValueCollection queryParams) => task);

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);

            var result = await controller.Search("TestTrust", SearchTypes.SEARCH_BY_TRUST_NAME_ID, null, null, null, null, null, false, null, 1);

            Assert.IsTrue(result is ViewResult);
            Assert.AreEqual("SearchResults", (result as ViewResult).ViewName);
        }

        [Test]
        public async Task SearchActionRedirectsToTrustViewIfCompanyNumberIsUsedAsId()
        {
            var testResult = new List<EdubaseDataObject>() { new EdubaseDataObject() { URN = 1234567 } };

            _mockContextDataService.Setup(m => m.GetSchoolDataObjectByLaEstab("1234567", false)).Returns((string urn, bool openOnly) => testResult);

            var controller = new TrustSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockTrustSearchService.Object, _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("6182612", SearchTypes.SEARCH_BY_TRUST_NAME_ID, null, null, null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("Trust", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Index", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("6182612", (result as RedirectToRouteResult).RouteValues["companyNo"].ToString());
        }
    }
}
