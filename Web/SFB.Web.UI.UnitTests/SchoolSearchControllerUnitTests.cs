using Moq;
using NUnit.Framework;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Services.Search;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.ApplicationCore.Entities;
using System.Linq;
using SFB.Web.ApplicationCore.Services.LocalAuthorities;

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

        //[Test]
        //public async Task SearchActionReturnsSchoolSuggestionsViewIfResultEmpty()
        //{
        //    dynamic edubaseSearchResponse = new QueryResultsModel(0, null, null, 50, 0);            

        //    Task<dynamic> task = Task.Run(() =>
        //    {
        //        return edubaseSearchResponse;
        //    });

        //    _mockSchoolSearchService.Setup(m => m.SearchSchoolByName("Test", 0, 50, null, null)).Returns((string name, int skip, int take, string orderby, NameValueCollection queryParams) => task);

        //    var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, _valService, 
        //        _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);
        //    controller.ControllerContext = new ControllerContext(_rc, controller);
 
        //    var result = await controller.Search("Test", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null);

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("SchoolSearch", (result as RedirectToRouteResult).RouteValues["Controller"]);
        //    Assert.AreEqual("SuggestSchool", (result as RedirectToRouteResult).RouteValues["Action"]);
        //    Assert.AreEqual("Test", (result as RedirectToRouteResult).RouteValues["NameId"]);
        //}

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
                _valService, _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);
 
            var result = await controller.Search(null, SearchTypes.SEARCH_BY_LA_CODE_NAME, null, null, null, "Test", null, false, null, 0);

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
                _valService, _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search(null, SearchTypes.SEARCH_BY_LOCATION, null, "Test", null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("Location", (result as RedirectToRouteResult).RouteValues["Controller"]);
            Assert.AreEqual("Suggest", (result as RedirectToRouteResult).RouteValues["Action"]);
            Assert.AreEqual("Test", (result as RedirectToRouteResult).RouteValues["locationorpostcode"]);
        }

        [Test]
        public async Task SearchActionReturnsAlphabeticalOrderedFacetFiltersForType ()
        {
            Task<dynamic> task = Task.Run(() =>
            {
                var facets = new Dictionary<string, FacetResultModel[]>();
                facets.Add("OverallPhase", new FacetResultModel[] { new FacetResultModel() { Value = "Primary", Count = 2 }, new FacetResultModel() { Value = "Secondary", Count = 1 }, new FacetResultModel() { Value = "All through", Count = 1 } });
                facets.Add("TypeOfEstablishment", new FacetResultModel[] { new FacetResultModel() { Value = "Pupil Referral Unit", Count = 2 }, new FacetResultModel() { Value = "Nursery", Count = 1 }, new FacetResultModel() { Value = "Primary", Count = 1 } });
                facets.Add("OfstedRating", new FacetResultModel[] { new FacetResultModel() { Value = "Outstanding", Count = 2 }, new FacetResultModel() { Value = "Good", Count = 1 }, new FacetResultModel() { Value = "Requires Improvement", Count = 1 } });
                facets.Add("ReligiousCharacter", new FacetResultModel[] { new FacetResultModel() { Value = "Hindu", Count = 2 }, new FacetResultModel() { Value = "Church of England", Count = 1 }, new FacetResultModel() { Value = "Roman Catholic", Count = 1 } });

                var matchedResults = new Dictionary<string, object>();
                matchedResults.Add("test1", 1);
                matchedResults.Add("test2", 2);
                var matches = new List<Dictionary<string, object>>();
                matches.Add(matchedResults);
                dynamic results = new QueryResultsModel(5, facets, matches, 5, 0);
                return results;
            });

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByName("Test", 0, 50, null, null)).Returns((string name, int skip, int take, string orderby, NameValueCollection queryParams) => task);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, new FilterBuilder(),
                _valService, _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("Test", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 1);

            var filters = ((result as ViewResult).Model as SearchedSchoolListViewModel).Filters;

            var TypeFilter = filters.First(l => l.Id == "schoolType");
            
            Assert.IsTrue(TypeFilter.Metadata[0].Label.CompareTo(TypeFilter.Metadata[1].Label) < 0);
            Assert.IsTrue(TypeFilter.Metadata[1].Label.CompareTo(TypeFilter.Metadata[2].Label) < 0);            
        }

        [Test]
        public async Task SearchActionReturnsAlphabeticalOrderedFacetFiltersForReligiousCharacter()
        {
            Task<dynamic> task = Task.Run(() =>
            {
                var facets = new Dictionary<string, FacetResultModel[]>();
                facets.Add("OverallPhase", new FacetResultModel[] { new FacetResultModel() { Value = "Primary", Count = 2 }, new FacetResultModel() { Value = "Secondary", Count = 1 }, new FacetResultModel() { Value = "All through", Count = 1 } });
                facets.Add("TypeOfEstablishment", new FacetResultModel[] { new FacetResultModel() { Value = "Pupil Referral Unit", Count = 2 }, new FacetResultModel() { Value = "Nursery", Count = 1 }, new FacetResultModel() { Value = "Primary", Count = 1 } });
                facets.Add("OfstedRating", new FacetResultModel[] { new FacetResultModel() { Value = "Outstanding", Count = 2 }, new FacetResultModel() { Value = "Good", Count = 1 }, new FacetResultModel() { Value = "Requires Improvement", Count = 1 } });
                facets.Add("ReligiousCharacter", new FacetResultModel[] { new FacetResultModel() { Value = "Hindu", Count = 2 }, new FacetResultModel() { Value = "Church of England", Count = 1 }, new FacetResultModel() { Value = "Roman Catholic", Count = 1 } });

                var matchedResults = new Dictionary<string, object>();
                matchedResults.Add("test1", 1);
                matchedResults.Add("test2", 2);
                var matches = new List<Dictionary<string, object>>();
                matches.Add(matchedResults);
                dynamic results = new QueryResultsModel(5, facets, matches, 5, 0);
                return results;
            });

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByName("Test", 0, 50, null, null)).Returns((string name, int skip, int take, string orderby, NameValueCollection queryParams) => task);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, new FilterBuilder(),
                _valService, _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("Test", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 1);

            var filters = ((result as ViewResult).Model as SearchedSchoolListViewModel).Filters;

            var TypeFilter = filters.First(l => l.Id == "faith");

            Assert.IsTrue(TypeFilter.Metadata[0].Label.CompareTo(TypeFilter.Metadata[1].Label) < 0);
            Assert.IsTrue(TypeFilter.Metadata[1].Label.CompareTo(TypeFilter.Metadata[2].Label) < 0);
        }

        [Test]
        public async Task SearchActionReturnsAlphabeticalOrderedFacetFiltersForPhase()
        {
            Task<dynamic> task = Task.Run(() =>
            {
                var facets = new Dictionary<string, FacetResultModel[]>();
                facets.Add("OverallPhase", new FacetResultModel[] { new FacetResultModel() { Value = "Primary", Count = 2 }, new FacetResultModel() { Value = "Secondary", Count = 1 }, new FacetResultModel() { Value = "All through", Count = 1 } });
                facets.Add("TypeOfEstablishment", new FacetResultModel[] { new FacetResultModel() { Value = "Pupil Referral Unit", Count = 2 }, new FacetResultModel() { Value = "Nursery", Count = 1 }, new FacetResultModel() { Value = "Primary", Count = 1 } });
                facets.Add("OfstedRating", new FacetResultModel[] { new FacetResultModel() { Value = "Outstanding", Count = 2 }, new FacetResultModel() { Value = "Good", Count = 1 }, new FacetResultModel() { Value = "Requires Improvement", Count = 1 } });
                facets.Add("ReligiousCharacter", new FacetResultModel[] { new FacetResultModel() { Value = "Hindu", Count = 2 }, new FacetResultModel() { Value = "Church of England", Count = 1 }, new FacetResultModel() { Value = "Roman Catholic", Count = 1 } });
                facets.Add("EstablishmentStatus", new FacetResultModel[] { new FacetResultModel() { Value = "Open", Count = 2 }, new FacetResultModel() { Value = "Closed", Count = 1 } });

                var matchedResults = new Dictionary<string, object>();
                matchedResults.Add("test1", 1);
                matchedResults.Add("test2", 2);
                var matches = new List<Dictionary<string, object>>();
                matches.Add(matchedResults);
                dynamic results = new QueryResultsModel(5, facets, matches, 5, 0);
                return results;
            });

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByName("Test", 0, 50, null, null)).Returns((string name, int skip, int take, string orderby, NameValueCollection queryParams) => task);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, new FilterBuilder(),
                _valService, _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("Test", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 1);

            var filters = ((result as ViewResult).Model as SearchedSchoolListViewModel).Filters;

            var TypeFilter = filters.First(l => l.Id == "schoolLevel");

            Assert.IsTrue(TypeFilter.Metadata[2].Label.CompareTo(TypeFilter.Metadata[1].Label) < 0);
            Assert.IsTrue(TypeFilter.Metadata[2].Label.CompareTo(TypeFilter.Metadata[0].Label) < 0);            
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

            _mockLaSearchService.Setup(m => m.SearchExactMatch("Test")).Returns(() => new LaModel(){ Id="123", LaName = "Test"});

            var testDictionary = new Dictionary<string, object>();
            testDictionary.Add("URN", "654321");
            dynamic edubaseSearchResponse = new QueryResultsModel(2, null, new List<IDictionary<string, object>>() { testDictionary }, 50, 0);
            Task<dynamic> task = Task.Run(() =>
            {
                return edubaseSearchResponse;
            });

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByLaCode("123", 0, 50, "EstablishmentName", null)).Returns((string name, int skip, int take, string orderby, NameValueCollection queryParams) => task);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, _valService, _mockContextDataService.Object, 
                _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search(null, SearchTypes.SEARCH_BY_LA_CODE_NAME, null, null, null, "Test", null, false, null);

            Assert.IsTrue(result is ViewResult);
            Assert.AreEqual("SearchResults", (result as ViewResult).ViewName);
        }


        [Test]
        public async Task SearchActionReturnsHomeViewIfNotValid()
        {
            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object,
                _mockFilterBuilder.Object, _valService, _mockContextDataService.Object, _mockSchoolSearchService.Object, 
                _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("" , SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("../home/index", (result as ViewResult).ViewName);            
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfValidUrnProvided()
        {
            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, _valService, 
                _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            var result = await controller.Search("", SearchTypes.SEARCH_BY_NAME_ID, "123456", null, null, null, null, false, null, 0);

            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("123456", (result as RedirectToRouteResult).RouteValues["urn"]);
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfUrnIsUsedAsId()
        {            
            var testResult = new EdubaseDataObject();
            testResult.URN = 123456;

            _mockContextDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(123456)).Returns((int urn) => testResult);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, _valService, 
                _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            var result = await controller.Search("123456", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual(123456, (result as RedirectToRouteResult).RouteValues["urn"]);
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfLaEstabIsUsedAsIdAndOneResultFound()
        {
            var testResult = new List<EdubaseDataObject>() { new EdubaseDataObject() { URN = 1234567 } };

            Task<List<EdubaseDataObject>> task = Task.Run(() =>
            {
                return testResult;
            });

            _mockContextDataService.Setup(m => m.GetSchoolDataObjectByLaEstabAsync("1234567", false)).Returns((string urn, bool openOnly) => task);
                       
            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, 
                _valService, _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("1234567", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("1234567", (result as RedirectToRouteResult).RouteValues["urn"].ToString());
        }

        [Test]
        public async Task SearchActionRedirectsToSearchViewIfLaEstabIsUsedAsIdAndMultipleResultsFound()
        {
            _mockContextDataService.Setup(m => m.GetSchoolDataObjectByLaEstabAsync("1234567", false)).Returns((string urn, bool openOnly) => task);

            Task<dynamic> task = Task.Run(() =>
            {
                var facets = new Dictionary<string, FacetResultModel[]>();
                facets.Add("OverallPhase", new FacetResultModel[] { new FacetResultModel() { Value = "Primary", Count = 2 }, new FacetResultModel() { Value = "Secondary", Count = 1 }, new FacetResultModel() { Value = "All through", Count = 1 } });
                facets.Add("TypeOfEstablishment", new FacetResultModel[] { new FacetResultModel() { Value = "Pupil Referral Unit", Count = 2 }, new FacetResultModel() { Value = "Nursery", Count = 1 }, new FacetResultModel() { Value = "Primary", Count = 1 } });
                facets.Add("OfstedRating", new FacetResultModel[] { new FacetResultModel() { Value = "Outstanding", Count = 2 }, new FacetResultModel() { Value = "Good", Count = 1 }, new FacetResultModel() { Value = "Requires Improvement", Count = 1 } });
                facets.Add("ReligiousCharacter", new FacetResultModel[] { new FacetResultModel() { Value = "Hindu", Count = 2 }, new FacetResultModel() { Value = "Church of England", Count = 1 }, new FacetResultModel() { Value = "Roman Catholic", Count = 1 } });

                var matchedResults = new Dictionary<string, object>();
                matchedResults.Add("test1", 1);
                matchedResults.Add("test2", 2);
                var matches = new List<Dictionary<string, object>>();
                matches.Add(matchedResults);
                dynamic results = new QueryResultsModel(5, facets, matches, 5, 0);
                return results;
            });

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByLaEstab("1234567", 0, SearchDefaults.RESULTS_PER_PAGE, null, null))
                .Returns((string laEstab, int skip, int take, string @orderby, NameValueCollection queryParams) => task);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, 
                _valService, _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("1234567", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 1);
            
            Assert.IsTrue(result is ViewResult);
            Assert.AreEqual("SearchResults", (result as ViewResult).ViewName);
        }

        [Test]
        public async Task SearchActionRedirectsToEmptyViewIfLaEstabIsUsedAsIdAndNoResultsFound()
        {
            var testResult = new List<EdubaseDataObject>() {  };

            Task<List<EdubaseDataObject>> dataObjectTask = Task.Run(() =>
            {
                return testResult;
            });

            _mockContextDataService.Setup(m => m.GetSchoolDataObjectByLaEstabAsync("1234567", false)).Returns((string urn, bool openOnly) => dataObjectTask);

            Task<dynamic> schoolTask = Task.Run(() =>
            {
                var facets = new Dictionary<string, FacetResultModel[]>();

                var matchedResults = new Dictionary<string, object>();
                matchedResults.Add("test1", 1);
                matchedResults.Add("test2", 2);
                var matches = new List<Dictionary<string, object>>();
                matches.Add(matchedResults);
                dynamic results = new QueryResultsModel(5, facets, matches, 5, 0);
                return results;
            });

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByLaEstab("1234567", 0, SearchDefaults.RESULTS_PER_PAGE, null, null))
                .Returns((string laEstab, int skip, int take, string @orderby, NameValueCollection queryParams) => schoolTask);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object,
                _valService, _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("1234567", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 1);

            Assert.IsTrue(result is ViewResult);
            Assert.AreEqual("EmptyResult", (result as ViewResult).ViewName);
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfLaEstabWithDashIsUsedAsId()
        {
            var testResult = new List<EdubaseDataObject>() { new EdubaseDataObject() { URN = 1234567 } };

            Task<List<EdubaseDataObject>> dataObjectTask = Task.Run(() =>
            {
                return testResult;
            });

            _mockContextDataService.Setup(m => m.GetSchoolDataObjectByLaEstabAsync("1234567", false)).Returns((string urn, bool openOnly) => dataObjectTask);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, 
                _valService, _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("123-4567", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 0);

            Assert.IsNotNull(result);
            Assert.AreEqual("School", (result as RedirectToRouteResult).RouteValues["controller"]);
            Assert.AreEqual("Detail", (result as RedirectToRouteResult).RouteValues["action"]);
            Assert.AreEqual("1234567", (result as RedirectToRouteResult).RouteValues["urn"].ToString());
        }

        [Test]
        public async Task SearchActionRedirectsToSchoolViewIfLaEstabWithSlashIsUsedAsId()
        {
            var testResult = new List<EdubaseDataObject>() { new EdubaseDataObject() { URN = 1234567 } };

            Task<List<EdubaseDataObject>> dataObjectTask = Task.Run(() =>
            {
                return testResult;
            });

            _mockContextDataService.Setup(m => m.GetSchoolDataObjectByLaEstabAsync("1234567", false)).Returns((string urn, bool openOnly) => dataObjectTask);

            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, _valService, 
                _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("123/4567", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, null, 0);

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

            _mockSchoolSearchService.Setup(m => m.SearchSchoolByName("Test", 50, 50, It.IsAny<string>(), It.IsAny<NameValueCollection>()))
                .Returns((string name, int skip, int take, string orderby, NameValueCollection queryParams) => task);            
            
            var controller = new SchoolSearchController(_mockLaService.Object, _mockLaSearchService.Object, _mockLocationSearchService.Object, _mockFilterBuilder.Object, _valService, 
                _mockContextDataService.Object, _mockSchoolSearchService.Object,  _mockCookieManager.Object);
            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.Search("Test", SearchTypes.SEARCH_BY_NAME_ID, null, null, null, null, null, false, string.Empty, 2);

            _mockSchoolSearchService.Verify(req => req.SearchSchoolByName("Test", 50, 50, string.Empty, null), Times.Once());
        }
      
    }
}
