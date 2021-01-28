using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFB.Web.ApplicationCore;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Services.Search;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers.Constants;
using System.Threading.Tasks;

namespace SFB.Web.UI.UnitTests
{
    public class TrustControllerUnitTests
    {
        [Test]
        public async Task IndexMethodShouldBuildVMWithTrustAndAcademiesMatFinancingTypeByDefaultAsync()
        {
            var mockTrustSearchService = new Mock<ITrustSearchService>();
            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            var mockFinancialDataService = new Mock<IFinancialDataService>();
            var mockFCService = new Mock<IFinancialCalculationsService>();
            var mockDataCollectionManager = new Mock<IDataCollectionManager>();
            var mockCookieManager = new Mock<IBenchmarkBasketService>();
            var mockEdubaseDataService = new Mock<IContextDataService>();
            var mockTrustHistoryService = new Mock<ITrustHistoryService>();

            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            var requestCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var rc = new RequestContext(context.Object, new RouteData());

            var result = new List<AcademySummaryDataObject>() {
                new AcademySummaryDataObject()
            };

            var GetAcademiesByCompanyNumberAsyncTask = Task.Run(() => result);

            mockFinancialDataService.Setup(m => m.GetAcademiesByCompanyNumberAsync(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(GetAcademiesByCompanyNumberAsyncTask);

            var GetActiveTermsForMatCentralAsyncTask = Task.Run(()=>  new List<string> { "2015" });
            mockFinancialDataService.Setup(m => m.GetActiveTermsForMatCentralAsync())
                .Returns(GetActiveTermsForMatCentralAsyncTask);

            var GetLatestDataYearPerEstabTypeAsyncTask = Task.Run(()=> 2015);
            mockFinancialDataService.Setup(m => m.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.MAT))
                .Returns(GetLatestDataYearPerEstabTypeAsyncTask);

            var GetLatestFinancialDataYearPerEstabTypeAsyncTask = Task.Run(() => 2015);
            mockDataCollectionManager.Setup(m => m.GetLatestFinancialDataYearPerEstabTypeAsync(EstablishmentType.MAT))
                .Returns(GetLatestFinancialDataYearPerEstabTypeAsyncTask);

            var GetActiveTermsByDataGroupAsyncTask = Task.Run(() => new List<string> { "2015" });
            mockDataCollectionManager.Setup(m => m.GetActiveTermsByDataGroupAsync(DataGroups.MATCentral))
                .Returns(GetActiveTermsByDataGroupAsyncTask);

            mockHistoricalChartBuilder
                .Setup(m => m.Build(It.IsAny<TabType>(), It.IsAny<EstablishmentType>()))
                .Returns(new List<ChartViewModel>());

            var controller = new TrustController(mockHistoricalChartBuilder.Object, 
                mockFinancialDataService.Object,
                mockFCService.Object,
                mockEdubaseDataService.Object, 
                null,
                mockCookieManager.Object,
                mockTrustHistoryService.Object);

            controller.ControllerContext = new ControllerContext(rc, controller);

            await controller.Index(123);

            mockFinancialDataService.Verify(m => m.GetTrustFinancialDataObjectByCompanyNoAsync(123, "2014 / 2015", MatFinancingType.TrustAndAcademies));
        }

        [Test]
        public void IndexMethodShouldRedirectToTrustSuggestionIfNoTrustFoundForCompanyNo()
        {
            var mockTrustSearchService = new Mock<ITrustSearchService>();
            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            var mockFinancialDataService = new Mock<IFinancialDataService>();
            var mockFCService = new Mock<IFinancialCalculationsService>();
            var mockDataCollectionManager = new Mock<IDataCollectionManager>();
            var mockCookieManager = new Mock<IBenchmarkBasketService>();
            var mockEdubaseDataService = new Mock<IContextDataService>();
            var mockTrustHistoryService = new Mock<ITrustHistoryService>();

            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            var requestCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var rc = new RequestContext(context.Object, new RouteData());

            var GetAcademiesByCompanyNumberAsyncTask = Task.Run(() => new List<AcademySummaryDataObject>());
            mockFinancialDataService.Setup(m => m.GetAcademiesByCompanyNumberAsync(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(GetAcademiesByCompanyNumberAsyncTask);

            var GetActiveTermsForMatCentralAsyncTask = Task.Run(()=> new List<string> { "2015" });
            mockFinancialDataService.Setup(m => m.GetActiveTermsForMatCentralAsync())
                .Returns(GetActiveTermsForMatCentralAsyncTask);

            var GetLatestDataYearPerEstabTypeAsyncTask = Task.Run(() => 2015);
            mockFinancialDataService.Setup(m => m.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.MAT))
                .Returns(GetLatestDataYearPerEstabTypeAsyncTask);

            var GetLatestFinancialDataYearPerEstabTypeAsyncTask = Task.Run(() => 2015);
            mockDataCollectionManager.Setup(m => m.GetLatestFinancialDataYearPerEstabTypeAsync(EstablishmentType.MAT))
                .Returns(GetLatestFinancialDataYearPerEstabTypeAsyncTask);

            var GetActiveTermsByDataGroupAsyncTask = Task.Run(() => new List<string> { "2015" });
            mockDataCollectionManager.Setup(m => m.GetActiveTermsByDataGroupAsync(DataGroups.MATCentral))
                .Returns(GetActiveTermsByDataGroupAsyncTask);

            mockHistoricalChartBuilder
                .Setup(m => m.Build(It.IsAny<TabType>(), It.IsAny<EstablishmentType>()))
                .Returns(new List<ChartViewModel>());

            var controller = new TrustController(mockHistoricalChartBuilder.Object,
                mockFinancialDataService.Object,
                mockFCService.Object,
                mockEdubaseDataService.Object,
                null,
                mockCookieManager.Object,
                mockTrustHistoryService.Object);

            controller.ControllerContext = new ControllerContext(rc, controller);

            var action = controller.Index(123);

            action.Wait();

            Assert.IsInstanceOf(typeof(RedirectToRouteResult), action.Result);
            Assert.AreEqual("TrustSearch", (action.Result as RedirectToRouteResult).RouteValues["Controller"]);
            Assert.AreEqual("SuggestTrust", (action.Result as RedirectToRouteResult).RouteValues["Action"]);
        }

        [Test]
        public void IndexMethodShouldRedirectToSchoolViewIfNoTrustFoundForCompanyNoButOneAcademyRelationIsFound()
        {
            var mockTrustSearchService = new Mock<ITrustSearchService>();
            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            var mockFinancialDataService = new Mock<IFinancialDataService>();
            var mockFCService = new Mock<IFinancialCalculationsService>();
            var mockDataCollectionManager = new Mock<IDataCollectionManager>();
            var mockCookieManager = new Mock<IBenchmarkBasketService>();
            var mockEdubaseDataService = new Mock<IContextDataService>();
            var mockTrustHistoryService = new Mock<ITrustHistoryService>();

            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            var requestCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var rc = new RequestContext(context.Object, new RouteData());

            var GetAcademiesByCompanyNumberAsyncTask = Task.Run(() => new List<AcademySummaryDataObject>() { new AcademySummaryDataObject() });
            mockFinancialDataService.Setup(m => m.GetAcademiesByCompanyNumberAsync(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(GetAcademiesByCompanyNumberAsyncTask);

            var GetActiveTermsForMatCentralAsyncTask = Task.Run(() => new List<string> { "2015" });
            mockFinancialDataService.Setup(m => m.GetActiveTermsForMatCentralAsync())
                .Returns(GetActiveTermsForMatCentralAsyncTask);

            var GetLatestDataYearPerEstabTypeAsyncTask = Task.Run(() => 2015);
            mockFinancialDataService.Setup(m => m.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.MAT))
                .Returns(GetLatestDataYearPerEstabTypeAsyncTask);

            var GetLatestFinancialDataYearPerEstabTypeAsyncTask = Task.Run(() => 2015);
            mockDataCollectionManager.Setup(m => m.GetLatestFinancialDataYearPerEstabTypeAsync(EstablishmentType.MAT))
                .Returns(GetLatestFinancialDataYearPerEstabTypeAsyncTask);

            var GetActiveTermsByDataGroupAsyncTask = Task.Run(() => new List<string> { "2015" });
            mockDataCollectionManager.Setup(m => m.GetActiveTermsByDataGroupAsync(DataGroups.MATCentral))
                .Returns(GetActiveTermsByDataGroupAsyncTask);

            mockHistoricalChartBuilder
                .Setup(m => m.Build(It.IsAny<TabType>(), It.IsAny<EstablishmentType>()))
                .Returns(new List<ChartViewModel>());

            var controller = new TrustController(mockHistoricalChartBuilder.Object,
                mockFinancialDataService.Object,
                mockFCService.Object,
                mockEdubaseDataService.Object,
                null,
                mockCookieManager.Object,
                mockTrustHistoryService.Object);

            controller.ControllerContext = new ControllerContext(rc, controller);

            var action = controller.Index(123);

            action.Wait();

            Assert.IsInstanceOf(typeof(RedirectToRouteResult), action.Result);
            Assert.AreEqual("School", (action.Result as RedirectToRouteResult).RouteValues["Controller"]);
            Assert.AreEqual("Detail", (action.Result as RedirectToRouteResult).RouteValues["Action"]);
        }
    }
}