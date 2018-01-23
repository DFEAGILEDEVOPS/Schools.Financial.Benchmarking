using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Azure.Documents;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFB.Web.Domain.Helpers.Enums;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using System.Dynamic;
using SFB.Web.Domain.Services.DataAccess;

namespace SFB.Web.UI.UnitTests
{
    public class BenchmarkChartsControllerUnitTests
    {
        private RequestContext _rc = null;

        [SetUp]
        public void SetupContext()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var requestCookies = new HttpCookieCollection();
            var listCookie = new ComparisonListModel();
            listCookie.HomeSchoolUrn = "123";
            listCookie.HomeSchoolName = "test";
            listCookie.HomeSchoolType = "test";
            listCookie.HomeSchoolFinancialType = "Academies";
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>() { new BenchmarkSchoolViewModel() { Urn = "123", Name = "test", FinancialType = "Academies" } };
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var responseCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Response.Cookies).Returns(responseCookies);
            _rc = new RequestContext(context.Object, new RouteData());
        }

        [Test]
        public void GenerateFromAdvancedCriteriaWithOverwriteAddsTheBenchmarkSchoolToTheBenchmarkListIfNotAlreadyReturned()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();
            var testResult = new Document();
            testResult.SetPropertyValue("URN", "321");
            testResult.SetPropertyValue("School Name", "test");
            testResult.SetPropertyValue("FinanceType", "A");
            Task<List<Document>> task = Task.Run(() =>
            {
                return new List<Document> { testResult };
            });
            mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteria(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType) => task);

            var mockEdubaseDataService = new Mock<IEdubaseDataService>();
            dynamic testEduResult = new Document();
            testEduResult.URN = "321";
            mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("321")).Returns((string urn) => testEduResult);

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>(){ new ChartViewModel(){ChartGroup = ChartGroupType.Staff}});

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockSchoolApiService  = new Mock<ILocalAuthoritiesService>();
            dynamic mockLaRecord = new ExpandoObject();
            mockLaRecord.id = "0";
            mockLaRecord.LANAME = "County Durham";
            mockLaRecord.REGION = "1";
            mockLaRecord.REGIONNAME = "North East A";
            dynamic laSearchResponse = new List<dynamic>
            {
                mockLaRecord
            };

            mockSchoolApiService.Setup(m => m.GetLocalAuthorities()).Returns(() => laSearchResponse);

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockSchoolApiService.Object, null, mockEdubaseDataService.Object, null);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.GenerateFromAdvancedCriteria(new BenchmarkCriteria(), EstablishmentType.All, null, "123", ComparisonArea.All);

            result.Wait();

            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);

            Assert.AreEqual(2, cookie.BenchmarkSchools.Count);

            Assert.AreEqual("123", cookie.BenchmarkSchools[1].Urn);
        }

        [Test]
        public void GenerateFromAdvancedCriteriaWithOverwriteDoNotAddTheBenchmarkSchoolToTheBenchmarkListIfAlreadyReturned()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();
            var testResult = new Document();
            testResult.SetPropertyValue("URN", "123");
            testResult.SetPropertyValue("School Name", "test");
            testResult.SetPropertyValue("FinanceType", "A");
            Task<List<Document>> task = Task.Run(() =>
            {
                return new List<Document> { testResult };
            });

            mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteria(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType) => task);

            var mockEdubaseDataService = new Mock<IEdubaseDataService>();
            dynamic testEduResult = new Document();
            testEduResult.URN = "123";
            mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123")).Returns((string urn) => testEduResult);

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            dynamic mockLaRecord = new ExpandoObject();
            mockLaRecord.id = "0";
            mockLaRecord.LANAME = "County Durham";
            mockLaRecord.REGION = "1";
            mockLaRecord.REGIONNAME = "North East A";
            dynamic laSearchResponse = new List<dynamic>
            {
                mockLaRecord
            };
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => laSearchResponse);

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, mockEdubaseDataService.Object, null);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.GenerateFromAdvancedCriteria(new BenchmarkCriteria(), EstablishmentType.All, null, "123", ComparisonArea.All);

            result.Wait();

            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);

            Assert.AreEqual(1, cookie.BenchmarkSchools.Count);

            Assert.AreEqual("123", cookie.BenchmarkSchools[0].Urn);
        }

        [Test]
        public void GenerateFromAdvancedCriteriaWithAddAddsTheBenchmarkSchoolToTheBenchmarkListIfNotAlreadyReturned()
        {
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var requestCookies = new HttpCookieCollection();
            var listCookie = new ComparisonListModel();
            listCookie.HomeSchoolUrn = "123";
            listCookie.HomeSchoolName = "test";
            listCookie.HomeSchoolType = "test";
            listCookie.HomeSchoolFinancialType = "Academies";
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>();
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var responseCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Response.Cookies).Returns(responseCookies);
            var rc = new RequestContext(context.Object, new RouteData());
            var mockDocumentDbService = new Mock<IFinancialDataService>();
            var testResult = new Document();
            testResult.SetPropertyValue("URN", "321");
            testResult.SetPropertyValue("School Name", "test");
            testResult.SetPropertyValue("FinanceType", "A");
            Task<List<Document>> task = Task.Run(() =>
            {
                return new List<Document> { testResult };
            });

            var mockEdubaseDataService = new Mock<IEdubaseDataService>();
            dynamic testEduResult = new Document();
            testEduResult.URN = "321";
            testEduResult.EstablishmentName = "test";
            mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("321")).Returns((string urn) => testEduResult);

            mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteria(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType) => task);

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            dynamic mockLaRecord = new ExpandoObject();
            mockLaRecord.id = "0";
            mockLaRecord.LANAME = "County Durham";
            mockLaRecord.REGION = "1";
            mockLaRecord.REGIONNAME = "North East A";
            dynamic laSearchResponse = new List<dynamic>
            {
                mockLaRecord
            };
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => laSearchResponse);

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, mockEdubaseDataService.Object, null);

            controller.ControllerContext = new ControllerContext(rc, controller);

            var result = controller.GenerateFromAdvancedCriteria(new BenchmarkCriteria(), EstablishmentType.All, null, "123", ComparisonArea.All, BenchmarkListOverwriteStrategy.Add);

            result.Wait();

            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);

            Assert.AreEqual(2, cookie.BenchmarkSchools.Count);

            Assert.AreEqual("123", cookie.BenchmarkSchools[1].Urn);
        }

        [Test]
        public void GenerateFromAdvancedCriteriaWithAddDoNotAddTheBenchmarkSchoolToTheBenchmarkListIfAlreadyReturned()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();
            var testResult = new Document();
            testResult.SetPropertyValue("URN", "123");
            testResult.SetPropertyValue("School Name", "test");
            testResult.SetPropertyValue("FinanceType", "A");
            Task<List<Document>> task = Task.Run(() =>
            {
                return new List<Document> { testResult };
            });
            mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteria(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType) => task);

            var mockEdubaseDataService = new Mock<IEdubaseDataService>();
            dynamic testEduResult = new Document();
            testEduResult.URN = "123";
            testEduResult.EstablishmentName = "test";
            mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123")).Returns((string urn) => testEduResult);

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            dynamic mockLaRecord = new ExpandoObject();
            mockLaRecord.id = "0";
            mockLaRecord.LANAME = "County Durham";
            mockLaRecord.REGION = "1";
            mockLaRecord.REGIONNAME = "North East A";
            dynamic laSearchResponse = new List<dynamic>
            {
                mockLaRecord
            };
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => laSearchResponse);

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, mockEdubaseDataService.Object, null);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.GenerateFromAdvancedCriteria(new BenchmarkCriteria(), EstablishmentType.All, null, "123", ComparisonArea.All);

            result.Wait();

            var cookie = JsonConvert.DeserializeObject<ComparisonListModel>(controller.Response.Cookies[CookieNames.COMPARISON_LIST].Value);

            Assert.AreEqual(1, cookie.BenchmarkSchools.Count);

            Assert.AreEqual("123", cookie.BenchmarkSchools[0].Urn);
        }

        [Test]
        public void TabChangeShouldKeepUnitTypeBetweenExpenditureAndIncomeTabs()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockEdubaseDataService = new Mock<IEdubaseDataService>();

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, mockEdubaseDataService.Object, null);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.TabChange(EstablishmentType.Maintained, UnitType.PerPupil, RevenueGroupType.Income);
            
            financialCalculationsService.Verify(f=> f.PopulateBenchmarkChartsWithFinancialData(
                null,
                It.IsAny<List<SchoolDataModel>>(),
                It.IsAny<IEnumerable<CompareEntityBase>>(),
                It.IsAny<string>(),
                UnitType.PerPupil,
                It.IsAny<bool>()));
        }

        [Test]
        public void TabChangeShouldKeepUnitTypeBetweenExpenditureAndBalanceTabsIfPossible()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockEdubaseDataService = new Mock<IEdubaseDataService>();

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, mockEdubaseDataService.Object, null);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.TabChange(EstablishmentType.Maintained, UnitType.PerPupil, RevenueGroupType.Balance);

            financialCalculationsService.Verify(f => f.PopulateBenchmarkChartsWithFinancialData(
                null,
                It.IsAny<List<SchoolDataModel>>(),
                It.IsAny<IEnumerable<CompareEntityBase>>(),
                It.IsAny<string>(),
                UnitType.PerPupil,
                It.IsAny<bool>()));
        }

        [Test]
        public void TabChangeShouldResetUnitTypeBetweenExpenditureAndBalanceTabsWhenKeepingNotPossible()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockEdubaseDataService = new Mock<IEdubaseDataService>();

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();

            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(),It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroup ,EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { Name = "Staff", ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, mockEdubaseDataService.Object, null);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.TabChange(EstablishmentType.Maintained, UnitType.PercentageOfTotal, RevenueGroupType.Balance);

            financialCalculationsService.Verify(f => f.PopulateBenchmarkChartsWithFinancialData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<SchoolDataModel>>(),
                It.IsAny<IEnumerable<CompareEntityBase>>(),
                It.IsAny<string>(),
                UnitType.AbsoluteMoney,
                It.IsAny<bool>()));
        }

        [Test]
        public void TabChangeShouldResetUnitTypeBetweenExpenditureAndWorkforceTabs()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockEdubaseDataService = new Mock<IEdubaseDataService>();

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();

            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { Name = "Staff", ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, mockEdubaseDataService.Object, null);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.TabChange(EstablishmentType.Maintained, UnitType.AbsoluteMoney, RevenueGroupType.Workforce);

            financialCalculationsService.Verify(f => f.PopulateBenchmarkChartsWithFinancialData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<SchoolDataModel>>(),
                It.IsAny<IEnumerable<CompareEntityBase>>(),
                It.IsAny<string>(),
                UnitType.AbsoluteCount,
                It.IsAny<bool>()));
        }
    }
}
