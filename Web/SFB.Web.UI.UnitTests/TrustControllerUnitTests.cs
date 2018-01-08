using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Azure.Documents;
using Moq;
using NUnit.Framework;
using SFB.Web.Domain.Helpers.Enums;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Domain.Services.Search;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;

namespace SFB.Web.UI.UnitTests
{
    public class TrustControllerUnitTests
    {
        [Test]
        public void IndexMethodShouldBuildVMWithTrustAndAcademiesMatFinancingTypeByDefault()
        {
            var mockTrustSearchService = new Mock<ITrustSearchService>();
            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            var mockDocumentDbService = new Mock<IFinancialDataService>();
            var mockFCService = new Mock<IFinancialCalculationsService>();
            var mockEdubaseDataService = new Mock<IEdubaseDataService>();

            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            var requestCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var rc = new RequestContext(context.Object, new RouteData());

            var result = new ExpandoObject();
            ((dynamic) result).Results = new List<ExpandoObject>();

            mockDocumentDbService.Setup(m => m.GetAcademiesByMatNumber(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(result);
            mockDocumentDbService.Setup(m => m.GetLatestDataYearForTrusts())
                .Returns(2015);
            mockDocumentDbService.Setup(m => m.GetActiveTermsByDataGroup(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<string> {"2015"});

            mockHistoricalChartBuilder
                .Setup(m => m.Build(It.IsAny<RevenueGroupType>(), It.IsAny<SchoolFinancialType>()))
                .Returns(new List<ChartViewModel>());

            var controller = new TrustController(mockHistoricalChartBuilder.Object, 
                mockDocumentDbService.Object,
                mockFCService.Object,
                mockEdubaseDataService.Object, mockTrustSearchService.Object, null);

            controller.ControllerContext = new ControllerContext(rc, controller);

            controller.Index("123", "test", UnitType.AbsoluteMoney);

            mockDocumentDbService.Verify(m => m.GetMATDataDocument("123", "2014 / 2015", MatFinancingType.TrustAndAcademies));
        }
    }
}