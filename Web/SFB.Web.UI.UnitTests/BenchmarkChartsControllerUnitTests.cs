using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFB.Web.Common;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services;
using SFB.Web.Domain.Services.Comparison;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Common.DataObjects;

namespace SFB.Web.UI.UnitTests
{
    public class BenchmarkChartsControllerUnitTests
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
            var listCookie = new SchoolComparisonListModel();
            listCookie.HomeSchoolUrn = "123";
            listCookie.HomeSchoolName = "test";
            listCookie.HomeSchoolType = "test";
            listCookie.HomeSchoolFinancialType = "Academies";
            listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>() { new BenchmarkSchoolModel() { Urn = "123", Name = "test", EstabType = "Academies" } };
            requestCookies.Add(new HttpCookie(CookieNames.COMPARISON_LIST, JsonConvert.SerializeObject(listCookie)));
            context.SetupGet(x => x.Request.Cookies).Returns(requestCookies);
            var responseCookies = new HttpCookieCollection();
            context.SetupGet(x => x.Response.Cookies).Returns(responseCookies);
            _rc = new RequestContext(context.Object, new RouteData());
        }

        [Test]
        public void GenerateFromAdvancedCriteriaWithOverwriteAddsTheBenchmarkSchoolToTheBenchmarkListIfNotAlreadyReturned()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();
            var testResult = new SchoolTrustFinancialDataObject();
            testResult.URN = 321;
            testResult.SchoolName = "test";
            testResult.FinanceType = "Academies";
            Task<List<SchoolTrustFinancialDataObject>> task = Task.Run(() =>
            {
                return new List<SchoolTrustFinancialDataObject> { testResult };
            });
            mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType) => task);

            var mockEdubaseDataService = new Mock<IContextDataService>();
            var testEduResult = new EdubaseDataObject();
            testEduResult.URN = 321;
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(321)).Returns((int urn) => testEduResult);

            var testEduHomeResult = new EdubaseDataObject();
            testEduHomeResult.URN = 123;
            testEduHomeResult.EstablishmentName = "test";
            testEduHomeResult.FinanceType = "Academies";
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(123)).Returns((int urn) => testEduHomeResult);

            var mockComparisonService = new Mock<IComparisonService>();
            Task<ComparisonResult> cTask = Task.Run(() =>
            {
                return new ComparisonResult() { BenchmarkSchools = new List<SchoolTrustFinancialDataObject>() { testResult } };
            });

            mockComparisonService.Setup(m =>
                    m.GenerateBenchmarkListWithAdvancedComparisonAsync(It.IsAny<BenchmarkCriteria>(),
                        It.IsAny<EstablishmentType>(), It.IsAny<Int32>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, int basketSize) => cTask);

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>(){ new ChartViewModel(){ChartGroup = ChartGroupType.Staff}});

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            mockCookieManager.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(fakeSchoolComparisonList);

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, mockEdubaseDataService.Object, null, mockComparisonService.Object, mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.GenerateFromAdvancedCriteria(new BenchmarkCriteria(), EstablishmentType.All, null, 123, ComparisonArea.All);

            result.Wait();

            mockCookieManager.Verify(m => m.UpdateSchoolComparisonListCookie(CookieActions.Add, It.IsAny<BenchmarkSchoolModel>()), Times.Exactly(2));
        }

        [Test]
        public void OneClickReportShouldBuildWithCorrectCriteria()
        {
            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            mockCookieManager.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(fakeSchoolComparisonList);

            var mockDocumentDbService = new Mock<IFinancialDataService>();
            var testResult = new SchoolTrustFinancialDataObject();
            testResult.URN = 321;
            testResult.SchoolName = "test";
            testResult.FinanceType = "academies";
            Task<List<SchoolTrustFinancialDataObject>> task = Task.Run(() =>
            {
                return new List<SchoolTrustFinancialDataObject> { testResult };
            });

            var mockEdubaseDataService = new Mock<IContextDataService>();
            var testEduResult = new EdubaseDataObject();
            testEduResult.URN = 321;
            testEduResult.EstablishmentName = "test";
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(321)).Returns((int urn) => testEduResult);

            var testEduHomeResult = new EdubaseDataObject();
            testEduHomeResult.URN = 123;
            testEduHomeResult.EstablishmentName = "test";
            testEduHomeResult.FinanceType = "Academies";
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(123)).Returns((int urn) => testEduHomeResult);

            mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType) => task);

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockComparisonService = new Mock<IComparisonService>();
            Task<ComparisonResult> cTask = Task.Run(() =>
            {
                return new ComparisonResult() { BenchmarkSchools = new List<SchoolTrustFinancialDataObject>() { testResult } };
            });

            mockComparisonService.Setup(m =>
                    m.GenerateBenchmarkListWithSimpleComparisonAsync(It.IsAny<BenchmarkCriteria>(),
                        It.IsAny<EstablishmentType>(), It.IsAny<Int32>(), It.IsAny<SimpleCriteria>(), It.IsAny<FinancialDataModel>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, int basketSize, SimpleCriteria simpleCr, FinancialDataModel financialDataModel) => cTask);

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var mockBenchmarkCriteriaBuilderService = new Mock<IBenchmarkCriteriaBuilderService>();
            mockBenchmarkCriteriaBuilderService
                .Setup(m => m.BuildFromSimpleComparisonCriteria(It.IsAny<FinancialDataModel>(), It.IsAny<SimpleCriteria>(), 0))
                .Returns(new BenchmarkCriteria());

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null,
                mockEdubaseDataService.Object, mockBenchmarkCriteriaBuilderService.Object, mockComparisonService.Object, mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.OneClickReport(123);

            result.Wait();

            var expectedSimpleCriteria = new SimpleCriteria()
            {
                IncludeEal = true,
                IncludeFsm = true,
                IncludeSen = true
            };

            mockBenchmarkCriteriaBuilderService.Verify(m => m.BuildFromSimpleComparisonCriteria(It.IsAny<FinancialDataModel>(), 
                It.Is<SimpleCriteria>(sc=> sc.Equals(expectedSimpleCriteria)), 
                It.IsAny<Int32>()));
        }

        [Test]
        public void GenerateFromAdvancedCriteriaWithAddAddsTheBenchmarkSchoolToTheBenchmarkListIfNotAlreadyReturned()
        {
            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            mockCookieManager.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(fakeSchoolComparisonList);

            var mockDocumentDbService = new Mock<IFinancialDataService>();
            var testResult = new SchoolTrustFinancialDataObject();
            testResult.URN = 321;
            testResult.SchoolName = "test";
            testResult.FinanceType = "academies";
            Task<List<SchoolTrustFinancialDataObject>> task = Task.Run(() =>
            {
                return new List<SchoolTrustFinancialDataObject> { testResult };
            });

            var mockEdubaseDataService = new Mock<IContextDataService>();
            var testEduResult = new EdubaseDataObject();
            testEduResult.URN = 321;
            testEduResult.EstablishmentName = "test";
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(321)).Returns((int urn) => testEduResult);

            var testEduHomeResult = new EdubaseDataObject();
            testEduHomeResult.URN = 123;
            testEduHomeResult.EstablishmentName = "test";
            testEduHomeResult.FinanceType = "Academies";
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(123)).Returns((int urn) => testEduHomeResult);

            mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType) => task);

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockComparisonService = new Mock<IComparisonService>();
            Task<ComparisonResult> cTask = Task.Run(() =>
                {
                    return new ComparisonResult() {BenchmarkSchools = new List<SchoolTrustFinancialDataObject>() {testResult}};
                });

            mockComparisonService.Setup(m =>
                    m.GenerateBenchmarkListWithAdvancedComparisonAsync(It.IsAny<BenchmarkCriteria>(),
                        It.IsAny<EstablishmentType>(), It.IsAny<Int32>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, int basketSize) => cTask);

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, 
                mockEdubaseDataService.Object, null, mockComparisonService.Object, mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.GenerateFromAdvancedCriteria(new BenchmarkCriteria(), EstablishmentType.All, null, 123, ComparisonArea.All, BenchmarkListOverwriteStrategy.Add);

            result.Wait();

            mockCookieManager.Verify(m => m.UpdateSchoolComparisonListCookie(CookieActions.Add, It.IsAny<BenchmarkSchoolModel>()), Times.Exactly(2));

        }

        [Test]
        public void TabChangeShouldKeepUnitTypeBetweenExpenditureAndIncomeTabs()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockEdubaseDataService = new Mock<IContextDataService>();

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var mockComparisonService = new Mock<IComparisonService>();

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel { Urn = "123", EstabType = "Academies" });
            mockCookieManager.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(fakeSchoolComparisonList);

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, 
                mockEdubaseDataService.Object, null, mockComparisonService.Object, mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.TabChange(EstablishmentType.Maintained, UnitType.PerPupil, RevenueGroupType.Income);
            
            financialCalculationsService.Verify(f=> f.PopulateBenchmarkChartsWithFinancialData(
                null,
                It.IsAny<List<FinancialDataModel>>(),
                It.IsAny<IEnumerable<CompareEntityBase>>(),
                It.IsAny<string>(),
                UnitType.PerPupil,
                It.IsAny<bool>()));
        }

        [Test]
        public void TabChangeShouldKeepUnitTypeBetweenExpenditureAndBalanceTabsIfPossible()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockEdubaseDataService = new Mock<IContextDataService>();

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var mockComparisonService = new Mock<IComparisonService>();

            IBenchmarkCriteriaBuilderService benchmarkCriteriaBuilderService = null;

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel { Urn = "123", EstabType = "Academies" });
            mockCookieManager.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(fakeSchoolComparisonList);

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, 
                mockEdubaseDataService.Object, benchmarkCriteriaBuilderService, mockComparisonService.Object, mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.TabChange(EstablishmentType.Maintained, UnitType.PerPupil, RevenueGroupType.Balance);

            financialCalculationsService.Verify(f => f.PopulateBenchmarkChartsWithFinancialData(
                null,
                It.IsAny<List<FinancialDataModel>>(),
                It.IsAny<IEnumerable<CompareEntityBase>>(),
                It.IsAny<string>(),
                UnitType.PerPupil,
                It.IsAny<bool>()));
        }

        [Test]
        public void TabChangeShouldResetUnitTypeBetweenExpenditureAndBalanceTabsWhenKeepingNotPossible()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockEdubaseDataService = new Mock<IContextDataService>();

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();

            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(),It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroup ,EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { Name = "Staff", ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var mockComparisonService = new Mock<IComparisonService>();

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel { Urn = "123", EstabType = "Academies" });
            mockCookieManager.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(fakeSchoolComparisonList);

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, 
                mockEdubaseDataService.Object, null, mockComparisonService.Object, mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.TabChange(EstablishmentType.Maintained, UnitType.PercentageOfTotal, RevenueGroupType.Balance);

            financialCalculationsService.Verify(f => f.PopulateBenchmarkChartsWithFinancialData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<FinancialDataModel>>(),
                It.IsAny<IEnumerable<CompareEntityBase>>(),
                It.IsAny<string>(),
                UnitType.AbsoluteMoney,
                It.IsAny<bool>()));
        }

        [Test]
        public void TabChangeShouldResetUnitTypeBetweenExpenditureAndWorkforceTabs()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockEdubaseDataService = new Mock<IContextDataService>();

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();

            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<RevenueGroupType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((RevenueGroupType revenueGroup, ChartGroupType chartGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { Name = "Staff", ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var mockComparisonService = new Mock<IComparisonService>();

            var mockCookieManager = new Mock<IBenchmarkBasketCookieManager>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel { Urn = "123", EstabType = "Academies" });
            mockCookieManager.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(fakeSchoolComparisonList);

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, 
                mockEdubaseDataService.Object, null, mockComparisonService.Object, mockCookieManager.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.TabChange(EstablishmentType.Maintained, UnitType.AbsoluteMoney, RevenueGroupType.Workforce);

            financialCalculationsService.Verify(f => f.PopulateBenchmarkChartsWithFinancialData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<FinancialDataModel>>(),
                It.IsAny<IEnumerable<CompareEntityBase>>(),
                It.IsAny<string>(),
                UnitType.AbsoluteCount,
                It.IsAny<bool>()));
        }
    }
}
