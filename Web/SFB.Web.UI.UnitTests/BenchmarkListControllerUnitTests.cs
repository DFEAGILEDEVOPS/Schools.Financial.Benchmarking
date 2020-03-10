using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.ApplicationCore;
using SFB.Web.ApplicationCore.Helpers.Constants;

namespace SFB.Web.UI.UnitTests
{
    public class BenchmarkListControllerUnitTests
    {
        [Test]
        public void UpdateActionCallsCookieManagerWhenComparisonListCleared()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var requestCookies = new HttpCookieCollection();
            var listCookie = new SchoolComparisonListModel();
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>() { new BenchmarkSchoolModel() { Urn = "123", Name = "test" } };
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var responseCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Response.Cookies).Returns(responseCookies);
            var rc = new RequestContext(context.Object, new RouteData());

            dynamic apiResponse = new System.Dynamic.ExpandoObject();
            apiResponse.Results = new object[0];
            apiResponse.NumberOfResults = 0;
            apiResponse.Facets = string.Empty;

            var mockEdubaseDataService = new Mock<IContextDataService>();
            dynamic serviceResponse = new System.Dynamic.ExpandoObject();
            serviceResponse.Results = new object[0];
            serviceResponse.NumberOfResults = 0;
            serviceResponse.Facets = string.Empty;

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel { Urn = "123", EstabType = "Academies" });
            mockCookieManager.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(fakeSchoolComparisonList);

            var controller = new BenchmarkListController(mockEdubaseDataService.Object, mockCookieManager.Object, mockFinancialDataService.Object);

            controller.ControllerContext = new ControllerContext(rc, controller);

            var result = controller.UpdateBenchmarkBasketAsync(null, CookieActions.RemoveAll);

            mockCookieManager.Verify(m => m.UpdateSchoolComparisonListCookie(CookieActions.RemoveAll, It.IsAny<BenchmarkSchoolModel>()),Times.Once);


        }
    }
}
