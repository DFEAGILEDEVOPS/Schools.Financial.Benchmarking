using Moq;
using NUnit.Framework;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Services.Comparison;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.UnitTests
{
    public class ManualComparisonControllerUnitTests
    {
        [Test]
        public void WithoutBaseSchoolActionClearsBaseSchoolWhenManualComparisonWithoutBaseSchool()
        {
            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();

            var _mockDocumentDbService = new Mock<IFinancialDataService>();

            var _mockDataCollectionManager = new Mock<IDataCollectionManager>();

            var _mockEdubaseDataService = new Mock<IContextDataService>();

            var mockComparisonService = new Mock<IComparisonService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var controller = new ManualComparisonController(mockCookieManager.Object, mockLaService.Object, _mockEdubaseDataService.Object, null, null, null, null, null);

            var result = controller.WithoutBaseSchool();

            mockCookieManager.Verify(m => m.UpdateManualComparisonListCookie(CookieActions.UnsetDefault, null));
            mockCookieManager.Verify(m => m.UpdateManualComparisonListCookie(CookieActions.RemoveAll, null));
            mockCookieManager.Verify(m => m.UpdateSchoolComparisonListCookie(CookieActions.UnsetDefault, null));
        }
    }
}
