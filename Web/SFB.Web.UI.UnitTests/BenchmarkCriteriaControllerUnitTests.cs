using Moq;
using NUnit.Framework;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Services.Comparison;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services.LocalAuthorities;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.UI.Services;

namespace SFB.Web.UI.UnitTests
{
    public class BenchmarkCriteriaControllerUnitTests
    {
        [Test]
        public async Task AskForOverwriteStrategyIfMultipleSchoolsInComparisonListAsync()
        {
            var mockCookieManager = new Mock<ISchoolBenchmarkListService>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel() { Name = "test" });
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel() { Name = "test" });
            mockCookieManager.Setup(m => m.GetSchoolBenchmarkList()).Returns(fakeSchoolComparisonList);

            var mockComparisonService = new Mock<IComparisonService>();

            var controller = new BenchmarkCriteriaController(null, null, null, null, mockCookieManager.Object, mockComparisonService.Object, new ValidationService());

            var response = await controller.OverwriteStrategy(10000, ComparisonType.Advanced, EstablishmentType.Maintained, new BenchmarkCriteriaVM(new BenchmarkCriteria() { Gender = new[] { "Boys" } }), ComparisonArea.All, 306, "test", 10);

            Assert.IsNotNull(response);
            Assert.IsNotNull((response as ViewResult).Model);
            Assert.AreEqual("", (response as ViewResult).ViewName);
        }

        [Test]
        public async Task DoNotAskForOverwriteStrategyIfOnlyBenchmarkSchoolInListAsync()
        {
            var mockCookieManager = new Mock<ISchoolBenchmarkListService>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "100";
            fakeSchoolComparisonList.HomeSchoolName = "home school";
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel() { Name = "test", Urn = "100" });

            mockCookieManager.Setup(m => m.GetSchoolBenchmarkList()).Returns(fakeSchoolComparisonList);

            var _mockDocumentDbService = new Mock<IFinancialDataService>();
            var testResult = new SchoolTrustFinancialDataObject();
            testResult.URN = 321;
            testResult.SchoolName = "test";
            Task<List<SchoolTrustFinancialDataObject>> task = Task.Run(() =>
            {
                return new List<SchoolTrustFinancialDataObject> { testResult };
            });

            _mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>(), false, true))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial, bool excludeFeds) => task);

            Task<int> GetLatestFinancialDataYearPerEstabTypeAsyncTask = Task.Run(()=> {
                return 2015;
            });

            var _mockDataCollectionManager = new Mock<IDataCollectionManager>();
            _mockDataCollectionManager.Setup(m => m.GetLatestFinancialDataYearPerEstabTypeAsync(It.IsAny<EstablishmentType>()))
                .Returns(GetLatestFinancialDataYearPerEstabTypeAsyncTask);

            var _mockEdubaseDataService = new Mock<IContextDataService>();
            Task<EdubaseDataObject> GetSchoolDataObjectByUrnAsyncTask = Task.Run(()=> {
                return new EdubaseDataObject
                {
                    URN = 100,
                    EstablishmentName = "test"
                };
            });
            _mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(100)).Returns((string urn) => GetSchoolDataObjectByUrnAsyncTask);

            var mockComparisonService = new Mock<IComparisonService>();

            var controller = new BenchmarkCriteriaController(null, _mockDocumentDbService.Object, _mockEdubaseDataService.Object, null, mockCookieManager.Object, mockComparisonService.Object, new ValidationService());

            var result = await controller.OverwriteStrategy(10000, ComparisonType.Advanced, EstablishmentType.Maintained, new BenchmarkCriteriaVM(new BenchmarkCriteria() { Gender = new[] { "Boys" } }), ComparisonArea.All, 306, "test", 10);

            Assert.AreEqual("BenchmarkCharts", (result as RedirectToRouteResult).RouteValues["Controller"]);
            Assert.AreEqual("GenerateNewFromAdvancedCriteria", (result as RedirectToRouteResult).RouteValues["Action"]);
        }

        [Test]
        public async Task DisplayReplaceViewIfBasketLimitExceedAsync()
        {
            var mockCookieManager = new Mock<ISchoolBenchmarkListService>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel() { Name = "test" });
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel() { Name = "test" });
            mockCookieManager.Setup(m => m.GetSchoolBenchmarkList()).Returns(fakeSchoolComparisonList);

            var mockComparisonService = new Mock<IComparisonService>();

            var controller = new BenchmarkCriteriaController(null, null, null, null, mockCookieManager.Object, mockComparisonService.Object, new ValidationService());

            var response = await controller.OverwriteStrategy(10000, ComparisonType.Advanced, EstablishmentType.Maintained, new BenchmarkCriteriaVM(new BenchmarkCriteria() { Gender = new[] { "Boys" } }), ComparisonArea.All, 306, "test", 29);

            Assert.IsNotNull(response);
            Assert.AreEqual("OverwriteReplace", (response as ViewResult).ViewName);
        }

        [Test]
        public void SelectSchoolTypeActionClearsBaseSchoolWhenAdvancedComparisonWithoutBaseSchool()
        {
            var mockCookieManager = new Mock<ISchoolBenchmarkListService>();

            var _mockDocumentDbService = new Mock<IFinancialDataService>();

            var _mockDataCollectionManager = new Mock<IDataCollectionManager>();

            var _mockEdubaseDataService = new Mock<IContextDataService>();

            var mockComparisonService = new Mock<IComparisonService>();

            var controller = new BenchmarkCriteriaController(null, _mockDocumentDbService.Object, _mockEdubaseDataService.Object, null, mockCookieManager.Object, mockComparisonService.Object, new ValidationService());

            var result = controller.SelectSchoolType(null, null, ComparisonType.Advanced, EstablishmentType.Maintained, 15);

            mockCookieManager.Verify(m => m.UnsetDefaultSchool());
        }

        [Test]
        public async Task AdvancedCharacteristicsShouldReturnErrorIfLaCodeIsNotFoundAsync()
        {
            var mockCookieManager = new Mock<ISchoolBenchmarkListService>();
            //mockCookieManager.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(new SchoolComparisonListModel());

            var _mockDocumentDbService = new Mock<IFinancialDataService>();

            var _mockDataCollectionManager = new Mock<IDataCollectionManager>();

            var _mockEdubaseDataService = new Mock<IContextDataService>();
            //_mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(123456)).Returns(Task.Run(() => new EdubaseDataObject()));

            var mockComparisonService = new Mock<IComparisonService>();

            var mockLaSearchService = new Mock<ILaSearchService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            mockLaSearchService.Setup(m => m.LaCodesContain(123)).Returns(false);

            var controller = new BenchmarkCriteriaController(mockLaService.Object, _mockDocumentDbService.Object, _mockEdubaseDataService.Object, mockLaSearchService.Object, mockCookieManager.Object, mockComparisonService.Object, new ValidationService());

            var response = await controller.AdvancedCharacteristics(123456, ComparisonType.Advanced, EstablishmentType.All, ComparisonArea.LaCodeName, "123", null);

            Assert.IsNotNull(response);
            Assert.IsNotNull((response as ViewResult).Model);
            Assert.IsTrue(((response as ViewResult).Model as SchoolViewModel).HasError());
            Assert.AreEqual(SearchErrorMessages.NO_LA_RESULTS, ((response as ViewResult).Model as SchoolViewModel).ErrorMessage);
        }

    }
}
