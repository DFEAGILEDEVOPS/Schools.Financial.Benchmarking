using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFB.Web.Common;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using SFB.Web.Domain.ApiWrappers;
using SFB.Web.Common.DataObjects;
using SFB.Web.Domain;
using SFB.Web.Domain.Services;

namespace SFB.Web.UI.UnitTests
{
    public class SchoolControllerUnitTests
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
            var listCookie = new SchoolComparisonListModel();
            listCookie.HomeSchoolUrn = "123";
            listCookie.HomeSchoolName = "test";
            listCookie.HomeSchoolType = "test";
            listCookie.HomeSchoolFinancialType = "Academies";
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>() { new BenchmarkSchoolModel() { Urn = "123", Name = "test", EstabType = "Academies" } };
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var responseCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Response.Cookies).Returns(responseCookies);
            _rc = new RequestContext(context.Object, new RouteData());
        }

        [Test]
        public void DetailShouldKeepUnitTypeBetweenExpenditureAndIncomeTabs()
        {
            var mockEdubaseDataService = new Mock<IContextDataService>();
            var testEduResult = new EdubaseDataObject();
            testEduResult.URN = 123;
            testEduResult.FinanceType = "Maintained";
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(123)).Returns((int urn) => testEduResult);

            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>(), It.IsAny<UnitType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroupType, EstablishmentType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();

            var mockApiRequest = new Mock<IApiRequest>();

            var mockLaSearchService = new Mock<ILocalAuthoritiesService>();

            var mockActiveUrnsService = new Mock<IActiveUrnsService>();

            var controller = new SchoolController(mockHistoricalChartBuilder.Object, mockFinancialDataService.Object, financialCalculationsService.Object, 
                mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object, mockCookieManager.Object, mockApiRequest.Object, mockLaSearchService.Object, mockActiveUrnsService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.Detail(123, UnitType.PerPupil, CentralFinancingType.Exclude, RevenueGroupType.Income);

            financialCalculationsService.Verify(f => f.PopulateHistoricalChartsWithSchoolData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<FinancialDataModel>>(),
                It.IsAny<string>(),
                It.IsAny<RevenueGroupType>(),
                UnitType.PerPupil,
                It.IsAny<EstablishmentType>()));
        }

        [Test]
        public void DetailCallShouldKeepUnitTypeBetweenExpenditureAndBalanceTabsIfPossible()
        {
            var mockEdubaseDataService = new Mock<IContextDataService>();
            var testEduResult = new EdubaseDataObject();
            testEduResult.URN = 123;
            testEduResult.FinanceType = "Maintained";
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(123)).Returns((int urn) => testEduResult);

            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>(), It.IsAny<UnitType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroupType, EstablishmentType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();

            var mockApiRequest = new Mock<IApiRequest>();

            var mockLaSearchService = new Mock<ILocalAuthoritiesService>();

            var mockActiveUrnsService = new Mock<IActiveUrnsService>();

            var controller = new SchoolController(mockHistoricalChartBuilder.Object, mockFinancialDataService.Object, financialCalculationsService.Object, 
                mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object, mockCookieManager.Object, mockApiRequest.Object, mockLaSearchService.Object, mockActiveUrnsService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.Detail(123, UnitType.PerPupil, CentralFinancingType.Exclude, RevenueGroupType.Balance);

            financialCalculationsService.Verify(f => f.PopulateHistoricalChartsWithSchoolData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<FinancialDataModel>>(),
                It.IsAny<string>(),
                It.IsAny<RevenueGroupType>(),
                UnitType.PerPupil,
                It.IsAny<EstablishmentType>()));
        }

        [Test]
        public void DetailCallShouldResetUnitTypeBetweenExpenditureAndBalanceTabsWhenKeepingNotPossible()
        {
            var mockEdubaseDataService = new Mock<IContextDataService>();
            var testEduResult = new EdubaseDataObject();
            testEduResult.URN = 123;
            testEduResult.FinanceType = "Maintained";
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(123)).Returns((int urn) => testEduResult);

            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>(), It.IsAny<UnitType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroupType, EstablishmentType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();

            var mockApiRequest = new Mock<IApiRequest>();

            var mockLaSearchService = new Mock<ILocalAuthoritiesService>();

            var mockActiveUrnsService = new Mock<IActiveUrnsService>();

            var controller = new SchoolController(mockHistoricalChartBuilder.Object, mockFinancialDataService.Object, financialCalculationsService.Object, 
                mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object, mockCookieManager.Object, mockApiRequest.Object, mockLaSearchService.Object, mockActiveUrnsService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.Detail(123, UnitType.PercentageOfTotal, CentralFinancingType.Exclude, RevenueGroupType.Balance);

            financialCalculationsService.Verify(f => f.PopulateHistoricalChartsWithSchoolData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<FinancialDataModel>>(),
                It.IsAny<string>(),
                It.IsAny<RevenueGroupType>(),
                UnitType.AbsoluteMoney,
                It.IsAny<EstablishmentType>()));
        }

        [Test]
        public void DetailCallShouldResetUnitTypeBetweenExpenditureAndWorkforceTabs()
        {
            var mockEdubaseDataService = new Mock<IContextDataService>();
            var testEduResult = new EdubaseDataObject();
            testEduResult.URN = 123;
            testEduResult.FinanceType = "Maintained";
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(123)).Returns((int urn) => testEduResult);

            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>(), It.IsAny<UnitType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroupType, EstablishmentType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();

            var mockApiRequest = new Mock<IApiRequest>();

            var mockLaSearchService = new Mock<ILocalAuthoritiesService>();

            var mockActiveUrnsService = new Mock<IActiveUrnsService>();

            var controller = new SchoolController(mockHistoricalChartBuilder.Object, mockFinancialDataService.Object, financialCalculationsService.Object, 
                mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object, mockCookieManager.Object, mockApiRequest.Object, mockLaSearchService.Object, mockActiveUrnsService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.Detail(123, UnitType.PerPupil, CentralFinancingType.Exclude, RevenueGroupType.Workforce);

            financialCalculationsService.Verify(f => f.PopulateHistoricalChartsWithSchoolData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<FinancialDataModel>>(),
                It.IsAny<string>(),
                It.IsAny<RevenueGroupType>(),
                UnitType.AbsoluteCount,
                It.IsAny<EstablishmentType>()));
        }

        [Test]
        public void DetailCallShouldSetSptReportExistsPropertyTrue()
        {
            var mockEdubaseDataService = new Mock<IContextDataService>();
            var testEduResult = new EdubaseDataObject();
            testEduResult.URN = 123;
            testEduResult.FinanceType = "Maintained";
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(123)).Returns((int urn) => testEduResult);

            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>(), It.IsAny<UnitType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroupType, EstablishmentType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();

            var mockApiRequest = new Mock<IApiRequest>();

            var mockLaSearchService = new Mock<ILocalAuthoritiesService>();

            var mockActiveUrnsService = new Mock<IActiveUrnsService>();

            mockApiRequest
                .Setup(ar => ar.Head(It.IsAny<string>(), It.IsAny<List<string>>()))
                .Returns((string endpoint, List<string> parameters) => new ApiResponse(System.Net.HttpStatusCode.OK, null));

            var controller = new SchoolController(mockHistoricalChartBuilder.Object, mockFinancialDataService.Object, financialCalculationsService.Object, 
                mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object, mockCookieManager.Object, mockApiRequest.Object, mockLaSearchService.Object, mockActiveUrnsService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var view = controller.Detail(123, UnitType.PerPupil, CentralFinancingType.Exclude, RevenueGroupType.Workforce);
            view.Wait();
            
            Assert.AreEqual(((ViewResult)view.Result).ViewBag.SptReportExists, true);
        }

        [Test]
        public void DetailCallShouldSetSptReportExistsPropertyFalse()
        {
            var mockEdubaseDataService = new Mock<IContextDataService>();
            var testEduResult = new EdubaseDataObject();
            testEduResult.URN = 123;
            testEduResult.FinanceType = "Maintained";
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(123)).Returns((int urn) => testEduResult);

            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>(), It.IsAny<UnitType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroupType, EstablishmentType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();

            var mockActiveUrnsService = new Mock<IActiveUrnsService>();

            var mockApiRequest = new Mock<IApiRequest>();

            var mockLaSearchService = new Mock<ILocalAuthoritiesService>();

            mockApiRequest
                .Setup(ar => ar.Head(It.IsAny<string>(), It.IsAny<List<string>>()))
                .Returns((string endpoint, List<string> parameters) => new ApiResponse(System.Net.HttpStatusCode.NotFound, null));

            var controller = new SchoolController(mockHistoricalChartBuilder.Object, mockFinancialDataService.Object, financialCalculationsService.Object, 
                mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object, mockCookieManager.Object, mockApiRequest.Object, mockLaSearchService.Object, mockActiveUrnsService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var view = controller.Detail(123, UnitType.PerPupil, CentralFinancingType.Exclude, RevenueGroupType.Workforce);
            view.Wait();

            Assert.AreEqual(((ViewResult)view.Result).ViewBag.SptReportExists, false);
        }
    }
}
