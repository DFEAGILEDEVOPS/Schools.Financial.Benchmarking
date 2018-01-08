using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SFB.Web.UI.Helpers.Constants;

namespace SFB.Web.UI.UnitTests
{
    public class BaseControllerUnitTests
    {
        [Test]
        public void UpdateComparisonListCookieAddsDefaultSchoolToTheListByDefault()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var requestCookies = new HttpCookieCollection();
            var listCookie = new ComparisonListModel();
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>();
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            context.SetupGet(x => x.Response.Cookies).Returns(requestCookies);
            var rc = new RequestContext(context.Object, new RouteData());
            
            var controller = new BaseController();

            controller.ControllerContext = new ControllerContext(rc, controller);

            var result = controller.UpdateSchoolComparisonListCookie(CompareActions.MAKE_DEFAULT_BENCHMARK, new BenchmarkSchoolViewModel() { Urn = "123" });

            Assert.AreEqual(1, controller.Response.Cookies.Count);

            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);

            Assert.AreEqual(1, cookie.BenchmarkSchools.Count);

            Assert.AreEqual("123", cookie.BenchmarkSchools[0].Urn);
        }

        [Test]
        public void UpdateComparisonListCookieAddsDoesNotAddDefaultSchoolToTheListIfThere()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var requestCookies = new HttpCookieCollection();
            var listCookie = new ComparisonListModel();
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>() { new BenchmarkSchoolViewModel() { Urn = "123" } };
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            context.SetupGet(x => x.Response.Cookies).Returns(requestCookies);
            var rc = new RequestContext(context.Object, new RouteData());
            
            var controller = new BaseController();

            controller.ControllerContext = new ControllerContext(rc, controller);

            var result = controller.UpdateSchoolComparisonListCookie(CompareActions.MAKE_DEFAULT_BENCHMARK, new BenchmarkSchoolViewModel() { Urn = "123" });

            Assert.AreEqual(1, controller.Response.Cookies.Count);

            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);

            Assert.AreEqual(1, cookie.BenchmarkSchools.Count);

            Assert.AreEqual("123", cookie.BenchmarkSchools[0].Urn);
        }

        [Test]
        public void UpdateComparisonListUndefaultsTheSchoolWhenRemovedFromList()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var requestCookies = new HttpCookieCollection();
            var listCookie = new ComparisonListModel();
            listCookie.HomeSchoolUrn = "123";
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>() { new BenchmarkSchoolViewModel() { Urn = "123" } };
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            context.SetupGet(x => x.Response.Cookies).Returns(requestCookies);
            var rc = new RequestContext(context.Object, new RouteData());
            
            var controller = new BaseController();

            controller.ControllerContext = new ControllerContext(rc, controller);

            var result = controller.UpdateSchoolComparisonListCookie(CompareActions.REMOVE_FROM_COMPARISON_LIST, new BenchmarkSchoolViewModel() { Urn = "123" });

            Assert.AreEqual(1, controller.Response.Cookies.Count);

            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);

            Assert.AreEqual(0, cookie.BenchmarkSchools.Count);

            Assert.AreEqual(null, cookie.HomeSchoolUrn);
        }
    }
}
