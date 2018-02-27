using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Azure.Documents;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFB.Web.Common;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;

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
        public void DetailShouldKeepUnitTypeBetweenExpenditureAndIncomeTabs()
        {
            var mockEdubaseDataService = new Mock<IContextDataService>();
            dynamic testEduResult = new Document();
            testEduResult.URN = "123";
            mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123")).Returns((string urn) => testEduResult);

            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(), It.IsAny<SchoolFinancialType>(), It.IsAny<UnitType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroupType, SchoolFinancialType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<SchoolFinancialType>()))
                .Returns((RevenueGroupType revenueGroup, SchoolFinancialType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var controller = new SchoolController(mockHistoricalChartBuilder.Object, mockFinancialDataService.Object, financialCalculationsService.Object, mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.Detail(123, UnitType.PerPupil, CentralFinancingType.Exclude, RevenueGroupType.Income);

            financialCalculationsService.Verify(f => f.PopulateHistoricalChartsWithSchoolData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<SchoolDataModel>>(),
                It.IsAny<string>(),
                It.IsAny<RevenueGroupType>(),
                UnitType.PerPupil,
                It.IsAny<SchoolFinancialType>()));
        }

        [Test]
        public void DetailCallShouldKeepUnitTypeBetweenExpenditureAndBalanceTabsIfPossible()
        {
            var mockEdubaseDataService = new Mock<IContextDataService>();
            dynamic testEduResult = new Document();
            testEduResult.URN = "123";
            mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123")).Returns((string urn) => testEduResult);

            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(), It.IsAny<SchoolFinancialType>(), It.IsAny<UnitType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroupType, SchoolFinancialType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<SchoolFinancialType>()))
                .Returns((RevenueGroupType revenueGroup, SchoolFinancialType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var controller = new SchoolController(mockHistoricalChartBuilder.Object, mockFinancialDataService.Object, financialCalculationsService.Object, mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.Detail(123, UnitType.PerPupil, CentralFinancingType.Exclude, RevenueGroupType.Balance);

            financialCalculationsService.Verify(f => f.PopulateHistoricalChartsWithSchoolData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<SchoolDataModel>>(),
                It.IsAny<string>(),
                It.IsAny<RevenueGroupType>(),
                UnitType.PerPupil,
                It.IsAny<SchoolFinancialType>()));
        }

        [Test]
        public void DetailCallShouldResetUnitTypeBetweenExpenditureAndBalanceTabsWhenKeepingNotPossible()
        {
            var mockEdubaseDataService = new Mock<IContextDataService>();
            dynamic testEduResult = new Document();
            testEduResult.URN = "123";
            mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123")).Returns((string urn) => testEduResult);

            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(), It.IsAny<SchoolFinancialType>(), It.IsAny<UnitType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroupType, SchoolFinancialType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<SchoolFinancialType>()))
                .Returns((RevenueGroupType revenueGroup, SchoolFinancialType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var controller = new SchoolController(mockHistoricalChartBuilder.Object, mockFinancialDataService.Object, financialCalculationsService.Object, mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.Detail(123, UnitType.PercentageOfTotal, CentralFinancingType.Exclude, RevenueGroupType.Balance);

            financialCalculationsService.Verify(f => f.PopulateHistoricalChartsWithSchoolData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<SchoolDataModel>>(),
                It.IsAny<string>(),
                It.IsAny<RevenueGroupType>(),
                UnitType.AbsoluteMoney,
                It.IsAny<SchoolFinancialType>()));
        }

        [Test]
        public void DetailCallShouldResetUnitTypeBetweenExpenditureAndWorkforceTabs()
        {
            var mockEdubaseDataService = new Mock<IContextDataService>();
            dynamic testEduResult = new Document();
            testEduResult.URN = "123";
            mockEdubaseDataService.Setup(m => m.GetSchoolByUrn("123")).Returns((string urn) => testEduResult);

            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(), It.IsAny<SchoolFinancialType>(), It.IsAny<UnitType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroupType, SchoolFinancialType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<SchoolFinancialType>()))
                .Returns((RevenueGroupType revenueGroup, SchoolFinancialType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var controller = new SchoolController(mockHistoricalChartBuilder.Object, mockFinancialDataService.Object, financialCalculationsService.Object, mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.Detail(123, UnitType.PerPupil, CentralFinancingType.Exclude, RevenueGroupType.Workforce);

            financialCalculationsService.Verify(f => f.PopulateHistoricalChartsWithSchoolData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<SchoolDataModel>>(),
                It.IsAny<string>(),
                It.IsAny<RevenueGroupType>(),
                UnitType.AbsoluteCount,
                It.IsAny<SchoolFinancialType>()));
        }
    }
}
