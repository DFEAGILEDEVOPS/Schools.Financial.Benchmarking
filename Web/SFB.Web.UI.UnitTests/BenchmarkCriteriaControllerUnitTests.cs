using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFB.Web.Domain.Models;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SFB.Web.Domain.Helpers.Enums;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.Domain.Services;
using Microsoft.Azure.Documents;
using SFB.Web.Domain.Services.DataAccess;

namespace SFB.Web.UI.UnitTests
{
    public class BenchmarkCriteriaControllerUnitTests
    {
        [Test]
        public void AskForOverwriteStrategyIfMultipleSchoolsInComparisonList()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            var requestCookies = new HttpCookieCollection();
            var listCookie = new ComparisonListModel();
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>() { new BenchmarkSchoolViewModel() {Name = "test"}, new BenchmarkSchoolViewModel(){Name = "test"} };
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var rc = new RequestContext(context.Object, new RouteData());

            var controller = new BenchmarkCriteriaController(null, null, null);
            controller.ControllerContext = new ControllerContext(rc, controller);

            var response = controller.OverwriteStrategy("10000", ComparisonType.Advanced, EstablishmentType.Maintained, new BenchmarkCriteriaVM(new BenchmarkCriteria() { Gender = new [] { "Boys"} }), ComparisonArea.All, 306);
            

            Assert.IsNotNull(response);
            Assert.IsNotNull((response as ViewResult).Model);
            Assert.AreEqual("", (response as ViewResult).ViewName);
        }

        [Test]
        public void DoNotAskForOverwriteStrategyIfOnlyBenchmarkSchoolInList()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var requestCookies = new HttpCookieCollection();
            var responseCookies = new HttpCookieCollection();
            var listCookie = new ComparisonListModel();
            listCookie.HomeSchoolUrn = "100";
            listCookie.HomeSchoolName = "home school";
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>() { new BenchmarkSchoolViewModel() {Urn = "100", Name = "test"} };
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            context.SetupGet(x => x.Response.Cookies).Returns(responseCookies);
            var rc = new RequestContext(context.Object, new RouteData());

            var _mockDocumentDbService = new Mock<IFinancialDataService>();
            var testResult = new Document();
            testResult.SetPropertyValue("URN", "321");
            testResult.SetPropertyValue("School Name", "test");
            Task<List<Document>> task = Task.Run(() =>
            {
                return new List<Document> { testResult};
            });
            _mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteria(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType) => task);
            _mockDocumentDbService.Setup(m => m.GetLatestDataYearPerSchoolType(It.IsAny<SchoolFinancialType>()))
                .Returns(2015);

            var _mockEdubaseDataService = new Mock<IEdubaseDataService>();
            dynamic testEduResult = new Document();
            testEduResult.URN = "100";
            testEduResult.EstablishmentName = "test";
            _mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("100")).Returns((string urn) => testEduResult);

            var controller = new BenchmarkCriteriaController(null, _mockDocumentDbService.Object, _mockEdubaseDataService.Object);
            controller.ControllerContext = new ControllerContext(rc, controller);

            var result = controller.OverwriteStrategy("10000", ComparisonType.Advanced, EstablishmentType.Maintained, new BenchmarkCriteriaVM(new BenchmarkCriteria() { Gender = new [] { "Boys" } }), ComparisonArea.All, 306);

            Assert.IsNotNull(response);
            Assert.AreEqual("BenchmarkCharts", (result as RedirectToRouteResult).RouteValues["Controller"]);
            Assert.AreEqual("GenerateNewFromAdvancedCriteria", (result as RedirectToRouteResult).RouteValues["Action"]);
        }

        [Test]
        public void ComparionStrategyActionMarksSchoolAsBenchmarkSchool()
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

            var _mockEdubaseDataService = new Mock<IEdubaseDataService>();
            dynamic testResult = new Document();
            testResult.URN = "123";
            testResult.EstablishmentName = "test";
            _mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123")).Returns((string urn) => testResult);

            var controller = new BenchmarkCriteriaController(null, null, _mockEdubaseDataService.Object);

            controller.ControllerContext = new ControllerContext(rc, controller);

            var result = controller.ComparisonStrategy(123);

            Assert.AreEqual(1, controller.Response.Cookies.Count);

            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);

            Assert.AreEqual(1, cookie.BenchmarkSchools.Count);

            Assert.AreEqual("123", cookie.BenchmarkSchools[0].Urn);
        }
    }
}
