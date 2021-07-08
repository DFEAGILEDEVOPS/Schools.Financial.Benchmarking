using Moq;
using NUnit.Framework;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Services.Comparison;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFB.Web.UI.UnitTests
{
    public class ManualComparisonControllerUnitTests
    {
        [Test]
        public void WithoutBaseSchoolActionClearsBaseSchoolWhenManualComparisonWithoutBaseSchool()
        {
            var mockSchoolCookieManager = new Mock<ISchoolBenchmarkListService>();
            
            var mockManualCookieManager = new Mock<IManualBenchmarkListService>();

            var _mockDocumentDbService = new Mock<IFinancialDataService>();

            var _mockDataCollectionManager = new Mock<IDataCollectionManager>();

            var _mockEdubaseDataService = new Mock<IContextDataService>();

            var mockComparisonService = new Mock<IComparisonService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var controller = new ManualComparisonController(mockSchoolCookieManager.Object, mockLaService.Object, _mockEdubaseDataService.Object, null, null, null, null, null, mockManualCookieManager.Object);

            var result = controller.WithoutBaseSchool();

            mockManualCookieManager.Verify(m => m.UnsetDefaultSchoolInManualBenchmarkList());
            mockManualCookieManager.Verify(m => m.ClearManualBenchmarkList());
            mockSchoolCookieManager.Verify(m => m.UnsetDefaultSchool());
        }


        [Test]
        public async System.Threading.Tasks.Task OverwriteStrategyRendersReplaceBasketViewWhenOnlyOptionAsync()
        {
            var fakeBMSchools = new List<BenchmarkSchoolModel>();
            for (int i = 0; i < 30; i++)
            {
                fakeBMSchools.Add(new BenchmarkSchoolModel());
            }

            var mockSchoolCookieManager = new Mock<ISchoolBenchmarkListService>();
            mockSchoolCookieManager.Setup(m => m.GetSchoolBenchmarkList()).Returns(new SchoolComparisonListModel() { HomeSchoolUrn = "123", BenchmarkSchools = fakeBMSchools });
            
            var mockManualCookieManager = new Mock<IManualBenchmarkListService>();
            mockManualCookieManager.Setup(m => m.GetManualBenchmarkList()).Returns(new SchoolComparisonListModel() { HomeSchoolUrn = "123", BenchmarkSchools = fakeBMSchools });
            
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockDataCollectionManager = new Mock<IDataCollectionManager>();

            var mockEdubaseDataService = new Mock<IContextDataService>();
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(123)).Returns(Task.Run(() => new EdubaseDataObject()));

            var mockComparisonService = new Mock<IComparisonService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var controller = new ManualComparisonController(mockSchoolCookieManager.Object, mockLaService.Object, mockEdubaseDataService.Object, null, null, null, null, null, mockManualCookieManager.Object);

            var result = await controller.OverwriteStrategy();

             Assert.AreEqual("OverwriteReplace", (result as ViewResult).ViewName);
        }
    }
}