using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFB.Web.Common;
using SFB.Web.DAL;
using SFB.Web.DAL.Helpers;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Domain.Services.Search;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using SFB.Web.Common.DataObjects;

namespace SFB.Web.UI.UnitTests
{
    public class TrustControllerUnitTests
    {
        [Test]
        public void IndexMethodShouldBuildVMWithTrustAndAcademiesMatFinancingTypeByDefault()
        {
            var mockTrustSearchService = new Mock<ITrustSearchService>();
            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            var mockFinancialDataService = new Mock<IFinancialDataService>();
            var mockFCService = new Mock<IFinancialCalculationsService>();
            var mockDataCollectionManager = new Mock<IDataCollectionManager>();
            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();

            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            var requestCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var rc = new RequestContext(context.Object, new RouteData());

            var result = new List<AcademiesContextualDataObject>();

            mockFinancialDataService.Setup(m => m.GetAcademiesByMatNumber(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(result);

            mockFinancialDataService.Setup(m => m.GetActiveTermsForMatCentral())
                .Returns(new List<string> { "2015" });

            mockFinancialDataService.Setup(m => m.GetLatestDataYearPerEstabType(EstablishmentType.MAT))
                .Returns(2015);

            mockDataCollectionManager.Setup(m => m.GetLatestFinancialDataYearPerEstabType(EstablishmentType.MAT))
                .Returns(2015);

            mockDataCollectionManager.Setup(m => m.GetActiveTermsByDataGroup(DataGroups.MATCentral))
                .Returns(new List<string> {"2015"});

            mockHistoricalChartBuilder
                .Setup(m => m.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns(new List<ChartViewModel>());

            var controller = new TrustController(mockHistoricalChartBuilder.Object, 
                mockFinancialDataService.Object,
                mockFCService.Object,
                mockTrustSearchService.Object, 
                null,
                mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(rc, controller);

            controller.Index("123", "test");

            mockFinancialDataService.Verify(m => m.GetMATDataDocumentAsync("123", "2014 / 2015", MatFinancingType.TrustAndAcademies));
        }
    }
}