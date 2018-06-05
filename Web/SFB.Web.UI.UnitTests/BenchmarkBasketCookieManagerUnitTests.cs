using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Web;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers;

namespace SFB.Web.UI.UnitTests
{
    //public class BenchmarkBasketCookieManagerUnitTests
    //{
    //    [Test]
    //    public void UpdateComparisonListCookieAddsDefaultSchoolToTheListByDefault()
    //    {
    //        var request = new Mock<HttpRequestBase>(MockBehavior.Loose);            
    //        var response = new Mock<HttpResponseBase>(MockBehavior.Loose);            
    //        var requestCookies = new HttpCookieCollection();
    //        var listCookie = new SchoolComparisonListModel();
    //        listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>();
    //        requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
    //        request.SetupGet(x => x.Cookies).Returns(requestCookies);
    //        response.SetupGet(x => x.Cookies).Returns(requestCookies);

    //        var http = new Mock<HttpContextBase>(MockBehavior.Loose);
    //        http.SetupGet(c => c.Request).Returns(request.Object);
    //        http.SetupGet(c => c.Response).Returns(response.Object);

    //        HttpContext.Current = new HttpContext(request.Object, response.Object);

    //        var manager = new BenchmarkBasketCookieManager();

    //        var result = manager.UpdateSchoolComparisonListCookie(CompareActions.MAKE_DEFAULT_BENCHMARK, new BenchmarkSchoolModel() { Urn = "123" });

    //        Assert.AreEqual(1, HttpContext.Current.Response.Cookies.Count);

    //        var cookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(HttpContext.Current.Response.Cookies[CookieNames.COMPARISON_LIST].Value);

    //        Assert.AreEqual(1, cookie.BenchmarkSchools.Count);

    //        Assert.AreEqual("123", cookie.BenchmarkSchools[0].Urn);
    //    }

    //    [Test]
    //    public void UpdateComparisonListCookieAddsDoesNotAddDefaultSchoolToTheListIfThere()
    //    {
    //        var request = new Mock<HttpRequest>(MockBehavior.Strict);
    //        var response = new Mock<HttpResponse>(MockBehavior.Strict);
    //        var requestCookies = new HttpCookieCollection();
    //        var listCookie = new SchoolComparisonListModel();
    //        listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>() { new BenchmarkSchoolModel() { Urn = "123" } };
    //        requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
    //        request.SetupGet(x => x.Cookies).Returns(requestCookies);
    //        response.SetupGet(x => x.Cookies).Returns(requestCookies);

    //        HttpContext.Current = new HttpContext(request.Object, response.Object);

    //        var manager = new BenchmarkBasketCookieManager();

    //        var result = manager.UpdateSchoolComparisonListCookie(CompareActions.MAKE_DEFAULT_BENCHMARK, new BenchmarkSchoolModel() { Urn = "123" });

    //        Assert.AreEqual(1, HttpContext.Current.Response.Cookies.Count);

    //        var cookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(HttpContext.Current.Response.Cookies[CookieNames.COMPARISON_LIST].Value);

    //        Assert.AreEqual(1, cookie.BenchmarkSchools.Count);

    //        Assert.AreEqual("123", cookie.BenchmarkSchools[0].Urn);
    //    }

    //    [Test]
    //    public void UpdateComparisonListUndefaultsTheSchoolWhenRemovedFromList()
    //    {
    //        var request = new Mock<HttpRequest>(MockBehavior.Strict);
    //        var response = new Mock<HttpResponse>(MockBehavior.Strict);
    //        var requestCookies = new HttpCookieCollection();
    //        var listCookie = new SchoolComparisonListModel();
    //        listCookie.HomeSchoolUrn = "123";
    //        listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>() { new BenchmarkSchoolModel() { Urn = "123" } };
    //        requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
    //        request.SetupGet(x => x.Cookies).Returns(requestCookies);
    //        response.SetupGet(x => x.Cookies).Returns(requestCookies);

    //        HttpContext.Current = new HttpContext(request.Object, response.Object);

    //        var manager = new BenchmarkBasketCookieManager();

    //        var result = manager.UpdateSchoolComparisonListCookie(CompareActions.REMOVE_FROM_COMPARISON_LIST, new BenchmarkSchoolModel() { Urn = "123" });

    //        Assert.AreEqual(1, HttpContext.Current.Response.Cookies.Count);

    //        var cookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(HttpContext.Current.Response.Cookies[CookieNames.COMPARISON_LIST].Value);

    //        Assert.AreEqual(0, cookie.BenchmarkSchools.Count);

    //        Assert.AreEqual(null, cookie.HomeSchoolUrn);
    //    }
    //}
}
