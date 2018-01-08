using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFB.Web.Domain.Services;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.UI.Helpers.Constants;

namespace SFB.Web.UI.UnitTests
{
    public class BenchmarkListControllerUnitTests
    {
        [Test]
        public void UpdateActionReturnsCookieWhenComparisonListCleared()
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

            var mockApiService = new Mock<ISchoolApiService>();
            dynamic apiResponse = new System.Dynamic.ExpandoObject();
            apiResponse.Results = new object[0];
            apiResponse.NumberOfResults = 0;
            apiResponse.Facets = string.Empty;

            var mockEdubaseDataService = new Mock<IEdubaseDataService>();
            dynamic serviceResponse = new System.Dynamic.ExpandoObject();
            serviceResponse.Results = new object[0];
            serviceResponse.NumberOfResults = 0;
            serviceResponse.Facets = string.Empty;

            var controller = new BenchmarkListController(mockApiService.Object, mockEdubaseDataService.Object);

            controller.ControllerContext = new ControllerContext(rc, controller);

            var result = controller.UpdateBenchmarkBasket(null, CompareActions.CLEAR_BENCHMARK_LIST);

            Assert.AreEqual(1, controller.Response.Cookies.Count);
            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);
            Assert.AreEqual(0, cookie.BenchmarkSchools.Count);
        }
    }
}
