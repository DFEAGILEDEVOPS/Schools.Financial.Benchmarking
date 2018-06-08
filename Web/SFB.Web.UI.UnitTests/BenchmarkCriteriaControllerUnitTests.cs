using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFB.Web.Domain.Models;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using Microsoft.Azure.Documents;
using SFB.Web.Common;
using SFB.Web.DAL;
using SFB.Web.DAL.Helpers;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.UI.Helpers;

namespace SFB.Web.UI.UnitTests
{
    public class BenchmarkCriteriaControllerUnitTests
    {
        [Test]
        public void AskForOverwriteStrategyIfMultipleSchoolsInComparisonList()
        {
            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel() { Name = "test" });
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel() { Name = "test" });
            mockCookieManager.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(fakeSchoolComparisonList);

            var controller = new BenchmarkCriteriaController(null, null, null, null, mockCookieManager.Object);

            var response = controller.OverwriteStrategy(10000, ComparisonType.Advanced, EstablishmentType.Maintained, new BenchmarkCriteriaVM(new BenchmarkCriteria() { Gender = new [] { "Boys"} }), ComparisonArea.All, 306, "test");

            Assert.IsNotNull(response);
            Assert.IsNotNull((response as ViewResult).Model);
            Assert.AreEqual("", (response as ViewResult).ViewName);
        }

        [Test]
        public void DoNotAskForOverwriteStrategyIfOnlyBenchmarkSchoolInList()
        {
            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "100";
            fakeSchoolComparisonList.HomeSchoolName = "home school";
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel() { Name = "test", Urn = "100" });
            
            mockCookieManager.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(fakeSchoolComparisonList);

            var _mockDocumentDbService = new Mock<IFinancialDataService>();
            var testResult = new Document();
            testResult.SetPropertyValue("URN", "321");
            testResult.SetPropertyValue("School Name", "test");
            Task<List<Document>> task = Task.Run(() =>
            {
                return new List<Document> { testResult};
            });

            _mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType) => task);

            var _mockDataCollectionManager = new Mock<IDataCollectionManager>();
            _mockDataCollectionManager.Setup(m => m.GetLatestFinancialDataYearPerEstabType(It.IsAny<EstablishmentType>()))
                .Returns(2015);

            var _mockEdubaseDataService = new Mock<IContextDataService>();
            dynamic testEduResult = new Document();
            testEduResult.URN = "100";
            testEduResult.EstablishmentName = "test";
            _mockEdubaseDataService.Setup(m => m.GetSchoolByUrn(100)).Returns((string urn) => testEduResult);

            var controller = new BenchmarkCriteriaController(null, _mockDocumentDbService.Object, _mockEdubaseDataService.Object, null, mockCookieManager.Object);

            var result = controller.OverwriteStrategy(10000, ComparisonType.Advanced, EstablishmentType.Maintained, new BenchmarkCriteriaVM(new BenchmarkCriteria() { Gender = new [] { "Boys" } }), ComparisonArea.All, 306, "test");

            Assert.AreEqual("BenchmarkCharts", (result as RedirectToRouteResult).RouteValues["Controller"]);
            Assert.AreEqual("GenerateNewFromAdvancedCriteria", (result as RedirectToRouteResult).RouteValues["Action"]);
        }
    }
}
