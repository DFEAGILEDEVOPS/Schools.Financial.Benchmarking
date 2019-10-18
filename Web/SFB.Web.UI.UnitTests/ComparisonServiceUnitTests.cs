using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFB.Web.Common;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services.Comparison;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Common.Entities;

namespace SFB.Web.UI.UnitTests
{
    public class ComparisonServiceUnitTests
    {
        [Test]
        public async Task GenerateBenchmarkListWithSimpleComparisonAsyncShouldExpandTheUrbanRuralIfNotEnoughSchoolsFound()
        {
            var mockFinancialDataService = new Mock<IFinancialDataService>();
            var testResult = new SchoolTrustFinancialDataObject();
            testResult.URN = 321;
            testResult.SchoolName = "test";
            testResult.FinanceType = "Academies";
            testResult.UrbanRural = "Town and fringe";
            Task<List<SchoolTrustFinancialDataObject>> task = Task.Run(() =>
            {
                return new List<SchoolTrustFinancialDataObject> { testResult };
            });

            mockFinancialDataService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType) => task);

            var mockContextDataService = new Mock<IContextDataService>();

            var mockBenchmarkCriteriaBuilderService = new Mock<IBenchmarkCriteriaBuilderService>();
            mockBenchmarkCriteriaBuilderService.Setup(s => s.BuildFromSimpleComparisonCriteria(It.IsAny<FinancialDataModel>(), It.IsAny<SimpleCriteria>(), It.IsAny<int>()))
                .Returns((FinancialDataModel dm, SimpleCriteria sc, int percentage) => new BenchmarkCriteria() { Gender = new[] { "Male" } });

            var service = new ComparisonService(mockFinancialDataService.Object, mockContextDataService.Object, mockBenchmarkCriteriaBuilderService.Object);

            var comparisonResult = await service.GenerateBenchmarkListWithSimpleComparisonAsync(new BenchmarkCriteria(){ Gender = new []{"Male"}},
                EstablishmentType.Maintained, 15, new SimpleCriteria(), new FinancialDataModel("123","14-15",testResult,EstablishmentType.Maintained));

            mockFinancialDataService.Verify(s => s.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), EstablishmentType.Maintained), Times.AtLeast(11));
            Assert.AreEqual(5, comparisonResult.BenchmarkCriteria.UrbanRural.Length);
            Assert.IsTrue(comparisonResult.BenchmarkCriteria.UrbanRural.Contains("Rural and village"));
            Assert.IsTrue(comparisonResult.BenchmarkCriteria.UrbanRural.Contains("Town and fringe"));
            Assert.IsTrue(comparisonResult.BenchmarkCriteria.UrbanRural.Contains("Urban and city"));
            Assert.IsTrue(comparisonResult.BenchmarkCriteria.UrbanRural.Contains("Hamlet and isolated dwelling"));
            Assert.IsTrue(comparisonResult.BenchmarkCriteria.UrbanRural.Contains("Conurbation"));
        }

        //[Test]
        //public async Task GenerateBenchmarkListWithBestInClassComparisonAsyncShouldExpandTheUrbanRuralIfNotEnoughSchoolsFound()
        //{
        //    var mockFinancialDataService = new Mock<IFinancialDataService>();
        //    var testResult = new SchoolTrustFinancialDataObject();
        //    testResult.URN = 321;
        //    testResult.SchoolName = "test";
        //    testResult.FinanceType = "Maintained";
        //    testResult.UrbanRural = "Town and fringe";
        //    Task<List<SchoolTrustFinancialDataObject>> task = Task.Run(() =>
        //    {
        //        return new List<SchoolTrustFinancialDataObject> { testResult };
        //    });

        //    mockFinancialDataService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>()))
        //        .Returns((BenchmarkCriteria criteria, EstablishmentType estType) => task);

        //    var mockContextDataService = new Mock<IContextDataService>();

        //    var mockBenchmarkCriteriaBuilderService = new Mock<IBenchmarkCriteriaBuilderService>();
        //    mockBenchmarkCriteriaBuilderService.Setup(s => s.BuildFromBicComparisonCriteria(It.IsAny<FinancialDataModel>(), It.IsAny<BestInClassCriteria>(), It.IsAny<int>()))
        //        .Returns((FinancialDataModel dm, BestInClassCriteria bic, int percentage) => new BenchmarkCriteria() { Gender = new[] { "Male" } });

        //    var service = new ComparisonService(mockFinancialDataService.Object, mockContextDataService.Object, mockBenchmarkCriteriaBuilderService.Object);

        //    var comparisonResult = await service.GenerateBenchmarkListWithBestInClassComparisonAsync(EstablishmentType.Maintained, new BenchmarkCriteria() { Gender = new[] { "Male" } },
        //        new BestInClassCriteria(), new FinancialDataModel("123", "14-15", testResult, EstablishmentType.Maintained));

        //    mockFinancialDataService.Verify(s => s.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), EstablishmentType.Maintained), Times.AtLeast(11));
        //    Assert.AreEqual(5, comparisonResult.BenchmarkCriteria.UrbanRural.Length);
        //    Assert.IsTrue(comparisonResult.BenchmarkCriteria.UrbanRural.Contains("Rural and village"));
        //    Assert.IsTrue(comparisonResult.BenchmarkCriteria.UrbanRural.Contains("Town and fringe"));
        //    Assert.IsTrue(comparisonResult.BenchmarkCriteria.UrbanRural.Contains("Urban and city"));
        //    Assert.IsTrue(comparisonResult.BenchmarkCriteria.UrbanRural.Contains("Hamlet and isolated dwelling"));
        //    Assert.IsTrue(comparisonResult.BenchmarkCriteria.UrbanRural.Contains("Conurbation"));
        //}

        [Test]
        public async Task GenerateBenchmarkListWithBestInClassComparisonAsyncShouldExpandWhenNotEnoughSchoolsFound()
        {
            var testResult = new SchoolTrustFinancialDataObject();
            testResult.URN = 321;
            testResult.SchoolName = "test";
            testResult.FinanceType = "Maintained";
            testResult.UrbanRural = "Town and fringe";
            testResult.RevenueReserve = 10;
            testResult.TotalIncome = 100;
            Task<List<SchoolTrustFinancialDataObject>> task = Task.Run(() =>
            {
                var results = new List<SchoolTrustFinancialDataObject>();
                for(var i=0; i<25; i++)
                {
                    results.Add(testResult);
                }

                return results;
            });

            var mockFinancialDataService = new Mock<IFinancialDataService>();
            mockFinancialDataService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>(), true))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial) => task);
            
            var mockBenchmarkCriteriaBuilderService = new Mock<IBenchmarkCriteriaBuilderService>();
            mockBenchmarkCriteriaBuilderService.Setup(s => s.BuildFromBicComparisonCriteria(It.IsAny<FinancialDataModel>(), It.IsAny<BestInClassCriteria>(), It.IsAny<int>()))
                .Returns((FinancialDataModel dm, BestInClassCriteria bic, int percentage) => new BenchmarkCriteria() { Gender = new[] { "Male" } });

            var service = new ComparisonService(mockFinancialDataService.Object, new Mock<IContextDataService>().Object, mockBenchmarkCriteriaBuilderService.Object);

            var comparisonResult = await service.GenerateBenchmarkListWithBestInClassComparisonAsync(EstablishmentType.Maintained, new BenchmarkCriteria() { Gender = new[] { "Male" } },
                new BestInClassCriteria(), new FinancialDataModel("123", "14-15", testResult, EstablishmentType.Maintained));

            mockFinancialDataService.Verify(s => s.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), EstablishmentType.Maintained, true), Times.Exactly(16));
            Assert.AreEqual(15, comparisonResult.BenchmarkSchools.Count);            
        }


        [Test]
        public async Task GenerateBenchmarkListWithBestInClassComparisonAsyncShouldNotExpandWhenNotEnoughSchoolsFound()
        {
            var testResult = new SchoolTrustFinancialDataObject();
            testResult.URN = 321;
            testResult.SchoolName = "test";
            testResult.FinanceType = "Maintained";
            testResult.UrbanRural = "Town and fringe";
            testResult.RevenueReserve = 10;
            testResult.TotalIncome = 100;
            Task<List<SchoolTrustFinancialDataObject>> task = Task.Run(() =>
            {
                var results = new List<SchoolTrustFinancialDataObject>();
                for (var i = 0; i < 51; i++)
                {
                    results.Add(testResult);
                }

                return results;
            });

            var mockFinancialDataService = new Mock<IFinancialDataService>();
            mockFinancialDataService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>(), true))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial) => task);

            var mockBenchmarkCriteriaBuilderService = new Mock<IBenchmarkCriteriaBuilderService>();
            mockBenchmarkCriteriaBuilderService.Setup(s => s.BuildFromBicComparisonCriteria(It.IsAny<FinancialDataModel>(), It.IsAny<BestInClassCriteria>(), It.IsAny<int>()))
                .Returns((FinancialDataModel dm, BestInClassCriteria bic, int percentage) => new BenchmarkCriteria() { Gender = new[] { "Male" } });

            var service = new ComparisonService(mockFinancialDataService.Object, new Mock<IContextDataService>().Object, mockBenchmarkCriteriaBuilderService.Object);

            var comparisonResult = await service.GenerateBenchmarkListWithBestInClassComparisonAsync(EstablishmentType.Maintained, new BenchmarkCriteria() { Gender = new[] { "Male" } },
                new BestInClassCriteria(), new FinancialDataModel("123", "14-15", testResult, EstablishmentType.Maintained));

            mockFinancialDataService.Verify(s => s.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), EstablishmentType.Maintained, true), Times.Exactly(1));
            Assert.AreEqual(15, comparisonResult.BenchmarkSchools.Count);
        }
    }
}