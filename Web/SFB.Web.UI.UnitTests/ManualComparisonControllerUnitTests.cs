using Moq;
using NUnit.Framework;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Services.Comparison;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Web.Mvc;

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


        [Test]
        public void OverwriteStrategyRendersReplaceBasketViewWhenOnlyOption()
        {
            var fakeBMSchools = new List<BenchmarkSchoolModel>();
            for (int i = 0; i < 30; i++)
            {
                fakeBMSchools.Add(new BenchmarkSchoolModel());
            }

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
            mockCookieManager.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(new SchoolComparisonListModel() { HomeSchoolUrn = "123", BenchmarkSchools = fakeBMSchools });
            mockCookieManager.Setup(m => m.ExtractManualComparisonListFromCookie()).Returns(new SchoolComparisonListModel() { HomeSchoolUrn = "123", BenchmarkSchools = fakeBMSchools });
            
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockDataCollectionManager = new Mock<IDataCollectionManager>();

            var mockEdubaseDataService = new Mock<IContextDataService>();

            var mockComparisonService = new Mock<IComparisonService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var controller = new ManualComparisonController(mockCookieManager.Object, mockLaService.Object, mockEdubaseDataService.Object, null, null, null, null, null);

            var result = controller.OverwriteStrategy();

             Assert.AreEqual("OverwriteReplace", (result as ViewResult).ViewName);
        }
    }
}