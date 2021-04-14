using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Services.Comparison;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers.Constants;

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
            testResult.Ks2Progress = 1;

            Task<List<SchoolTrustFinancialDataObject>> SearchSchoolsByCriteriaAsyncTask = Task.Run(() =>
            {
                return new List<SchoolTrustFinancialDataObject> { testResult };
            });

            Task<FinancialDataModel> GetSchoolsLatestFinancialDataModelAsyncTask = Task.Run(() =>
            {
                return new FinancialDataModel("321", "2017-18", new SchoolTrustFinancialDataObject(), EstablishmentType.Academies);
            });

            Task<EdubaseDataObject> GetSchoolDataObjectByUrnAsyncTask = Task.Run(() =>
            {
                return new EdubaseDataObject{ URN = 321 };
            });

            Task<EdubaseDataObject> GetSchoolDataObjectByUrnAsyncTask_2 = Task.Run(() =>
            {
                return new EdubaseDataObject
                {
                    URN = 123,
                    EstablishmentName = "test",
                    FinanceType = "Academies"
                };
            });

            mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>(), false, true))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial, bool excludeFeds) => SearchSchoolsByCriteriaAsyncTask);
            mockDocumentDbService.Setup(m => m.GetSchoolsLatestFinancialDataModelAsync(It.IsAny<int>(), It.IsAny<EstablishmentType>()))
                .Returns((int urn, EstablishmentType estType) => GetSchoolsLatestFinancialDataModelAsyncTask);

            var mockEdubaseDataService = new Mock<IContextDataService>();
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(321)).Returns((int urn) => GetSchoolDataObjectByUrnAsyncTask);

            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(123)).Returns((int urn) => GetSchoolDataObjectByUrnAsyncTask_2);

            var mockComparisonService = new Mock<IComparisonService>();
            Task<ComparisonResult> cTask = Task.Run(() =>
            {
                return new ComparisonResult() { BenchmarkSchools = new List<SchoolTrustFinancialDataObject>() { testResult } };
            });

            mockComparisonService.Setup(m =>
                    m.GenerateBenchmarkListWithAdvancedComparisonAsync(It.IsAny<BenchmarkCriteria>(),
                        It.IsAny<EstablishmentType>(), false, It.IsAny<Int32>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial, int basketSize) => cTask);

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<TabType>(), It.IsAny<EstablishmentType>()))
                .Returns((TabType TabNames, EstablishmentType schoolType) => new List<ChartViewModel>(){ new ChartViewModel(){ChartGroup = ChartGroupType.Staff}});

            var mockFinancialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockEfficiencyMetricService = new Mock<IEfficiencyMetricDataService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var mockBenchmarkBasketService = new Mock<ISchoolBenchmarkListService>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";            
            mockBenchmarkBasketService.Setup(m => m.GetSchoolBenchmarkList()).Returns(fakeSchoolComparisonList);

            var mockTrustBasketService = new Mock<ITrustBenchmarkListService>();

            var mockBicComparisonResultCachingService = new Mock<IBicComparisonResultCachingService>();

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object,
                mockDocumentDbService.Object,
                mockFinancialCalculationsService.Object,
                mockLaService.Object,
                null,
                mockEdubaseDataService.Object,
                null,
                mockComparisonService.Object,
                mockBicComparisonResultCachingService.Object,
                mockEfficiencyMetricService.Object,
                mockBenchmarkBasketService.Object,
                mockTrustBasketService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.GenerateFromAdvancedCriteria(new BenchmarkCriteria(), EstablishmentType.All, null, 123, ComparisonArea.All, BenchmarkListOverwriteStrategy.Overwrite);

            result.Wait();

            mockBenchmarkBasketService.Verify(m => m.AddSchoolsToBenchmarkList(It.IsAny<ComparisonResult>()), Times.Exactly(1));
        }

        [Test]
        public async Task OneClickReportShouldBuildCorrectViewModelAsync()
        {
            var mockBenchmarkBasketService = new Mock<ISchoolBenchmarkListService>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            mockBenchmarkBasketService.Setup(m => m.GetSchoolBenchmarkList()).Returns(fakeSchoolComparisonList);


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

            Task<EdubaseDataObject> GetSchoolDataObjectByUrnAsyncTask = Task.Run(() =>
            {
                return new EdubaseDataObject { URN = 321, EstablishmentName = "test" };
            });

            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(321)).Returns((int urn) => GetSchoolDataObjectByUrnAsyncTask);

            Task<EdubaseDataObject> GetSchoolDataObjectByUrnAsyncTask_2 = Task.Run(() =>
            {
                return new EdubaseDataObject
                {
                    URN = 123,
                    EstablishmentName = "test",
                    FinanceType = "Academies"
                }; 
            });

            Task<FinancialDataModel> GetSchoolsLatestFinancialDataModelAsyncTask = Task.Run(()=> {
                return new FinancialDataModel("321", "2017-18", new SchoolTrustFinancialDataObject(), EstablishmentType.Academies);
            });

            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(123)).Returns((int urn) => GetSchoolDataObjectByUrnAsyncTask_2);

            mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>(), false, true))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial, bool excludeFeds) => task);
            mockDocumentDbService.Setup(m => m.GetSchoolsLatestFinancialDataModelAsync(It.IsAny<int>(), It.IsAny<EstablishmentType>()))
                .Returns((int urn, EstablishmentType estType) => GetSchoolsLatestFinancialDataModelAsyncTask);

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<TabType>(), It.IsAny<EstablishmentType>()))
                .Returns((TabType TabNames, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockComparisonService = new Mock<IComparisonService>();
            Task<ComparisonResult> GenerateBenchmarkListWithOneClickComparisonAsyncTask = Task.Run(() =>
            {
                return new ComparisonResult() { BenchmarkSchools = new List<SchoolTrustFinancialDataObject>() { testResult } };
            });

            mockComparisonService.Setup(m =>
                    m.GenerateBenchmarkListWithOneClickComparisonAsync(It.IsAny<BenchmarkCriteria>(),
                        It.IsAny<EstablishmentType>(), It.IsAny<int>(), It.IsAny<FinancialDataModel>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, int basketSize, FinancialDataModel financialDataModel) => GenerateBenchmarkListWithOneClickComparisonAsyncTask);

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var mockBenchmarkCriteriaBuilderService = new Mock<IBenchmarkCriteriaBuilderService>();
            mockBenchmarkCriteriaBuilderService
                .Setup(m => m.BuildFromSimpleComparisonCriteria(It.IsAny<FinancialDataModel>(), It.IsAny<SimpleCriteria>(), 0))
                .Returns(new BenchmarkCriteria());

            var mockBicComparisonResultCachingService = new Mock<IBicComparisonResultCachingService>();

            var mockEfficiencyMetricService = new Mock<IEfficiencyMetricDataService>();

            var mockTrustBasketService = new Mock<ITrustBenchmarkListService>();

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object,
                mockDocumentDbService.Object,
                financialCalculationsService.Object,
                mockLaService.Object,
                null,
                mockEdubaseDataService.Object,
                mockBenchmarkCriteriaBuilderService.Object,
                mockComparisonService.Object,
                mockBicComparisonResultCachingService.Object,
                mockEfficiencyMetricService.Object,
                mockBenchmarkBasketService.Object,
                mockTrustBasketService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.OneClickReport(123);
           
            Assert.AreEqual(ChartFormat.Charts, result.ViewBag.ChartFormat);
            Assert.AreEqual("123", result.ViewBag.HomeSchoolId);
            Assert.AreEqual(ComparisonType.OneClick, (result.Model as BenchmarkChartListViewModel).ComparisonType);
            Assert.AreEqual(EstablishmentType.Academies, (result.Model as BenchmarkChartListViewModel).EstablishmentType);
        }

        [Test]
        public async Task GenerateFromAdvancedCriteriaWithAddAddsTheBenchmarkSchoolToTheBenchmarkListIfNotAlreadyReturnedAsync()
        {
            var mockBenchmarkBasketService = new Mock<ISchoolBenchmarkListService>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            mockBenchmarkBasketService.Setup(m => m.GetSchoolBenchmarkList()).Returns(fakeSchoolComparisonList);

            var mockFinancialDataService = new Mock<IFinancialDataService>();
            var testResult = new SchoolTrustFinancialDataObject();
            testResult.URN = 321;
            testResult.SchoolName = "test";
            testResult.FinanceType = "academies";
            Task<List<SchoolTrustFinancialDataObject>> task = Task.Run(() =>
            {
                return new List<SchoolTrustFinancialDataObject> { testResult };
            });

            var mockContextDataService = new Mock<IContextDataService>();

            Task<EdubaseDataObject> GetSchoolDataObjectByUrnAsyncTask = Task.Run(() =>
            {
                return new EdubaseDataObject { URN = 321, EstablishmentName = "test" };
            });

            mockContextDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(321)).Returns((int urn) => GetSchoolDataObjectByUrnAsyncTask);

            Task<EdubaseDataObject> GetSchoolDataObjectByUrnAsyncTask_2 = Task.Run(() =>
            {
                return new EdubaseDataObject
                {
                    URN = 123,
                    EstablishmentName = "test",
                    FinanceType = "Academies"
                };
            });
            mockContextDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(123)).Returns((int urn) => GetSchoolDataObjectByUrnAsyncTask_2);

            mockFinancialDataService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>(), false, true))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial, bool excludeFeds) => task);

            Task<FinancialDataModel> GetSchoolsLatestFinancialDataModelAsyncTask =  Task.Run(() => {
                return new FinancialDataModel("321", "2017-18", new SchoolTrustFinancialDataObject(), EstablishmentType.Academies);
            });

            mockFinancialDataService.Setup(m => m.GetSchoolsLatestFinancialDataModelAsync(It.IsAny<int>(), It.IsAny<EstablishmentType>()))
                .Returns(GetSchoolsLatestFinancialDataModelAsyncTask);

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<TabType>(), It.IsAny<EstablishmentType>()))
                .Returns((TabType TabNames, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockComparisonService = new Mock<IComparisonService>();
            Task<ComparisonResult> cTask = Task.Run(() =>
                {
                    return new ComparisonResult() {BenchmarkSchools = new List<SchoolTrustFinancialDataObject>() {testResult}};
                });

            mockComparisonService.Setup(m =>
                    m.GenerateBenchmarkListWithAdvancedComparisonAsync(It.IsAny<BenchmarkCriteria>(),
                        It.IsAny<EstablishmentType>(), false, It.IsAny<Int32>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial, int basketSize) => cTask);

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var mockBicComparisonResultCachingService = new Mock<IBicComparisonResultCachingService>();

            var mockEfficiencyMetricService = new Mock<IEfficiencyMetricDataService>();

            var mockTrustBasketService = new Mock<ITrustBenchmarkListService>();

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object,
                mockFinancialDataService.Object,
                financialCalculationsService.Object,
                mockLaService.Object,
                null,
                mockContextDataService.Object,
                null, mockComparisonService.Object,
                mockBicComparisonResultCachingService.Object,
                mockEfficiencyMetricService.Object,
                mockBenchmarkBasketService.Object,
                mockTrustBasketService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = await controller.GenerateFromAdvancedCriteria(new BenchmarkCriteria(), EstablishmentType.All, null, 123, ComparisonArea.All, BenchmarkListOverwriteStrategy.Add);

            mockBenchmarkBasketService.Verify(m => m.TryAddSchoolToBenchmarkList(It.IsAny<SchoolViewModel>()), Times.Exactly(1));

        }

        [Test]
        public void GenerateFromAdvancedCriteriaWithAddReturnsBackToPageWithErrorIfTotalLimitExceeds()
        {
            var mockBenchmarkBasketService = new Mock<ISchoolBenchmarkListService>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            for(int i=0; i < 30; i++)
            {
                fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel() {
                    Urn = i.ToString(), Name = "test", EstabType = "Academies"
                });
            }
            mockBenchmarkBasketService.Setup(m => m.GetSchoolBenchmarkList()).Returns(fakeSchoolComparisonList);

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
            Task<EdubaseDataObject> GetSchoolDataObjectByUrnAsyncTask = Task.Run(() =>
            {
                return new EdubaseDataObject { URN = 321, EstablishmentName = "test" };
            });
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(321)).Returns((int urn) => GetSchoolDataObjectByUrnAsyncTask);

            Task<EdubaseDataObject> GetSchoolDataObjectByUrnAsyncTask_2 = Task.Run(() =>
            {
                return new EdubaseDataObject
                {
                    URN = 1234,
                    EstablishmentName = "testResult",
                    FinanceType = "Academies"
                };
            });
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(123)).Returns((int urn) => GetSchoolDataObjectByUrnAsyncTask_2);

            mockDocumentDbService.Setup(m => m.SearchSchoolsByCriteriaAsync(It.IsAny<BenchmarkCriteria>(), It.IsAny<EstablishmentType>(), false, true))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial, bool excludeFeds) => task);

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<TabType>(), It.IsAny<EstablishmentType>()))
                .Returns((TabType TabNames, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockComparisonService = new Mock<IComparisonService>();
            Task<ComparisonResult> cTask = Task.Run(() =>
            {
                return new ComparisonResult() { BenchmarkSchools = new List<SchoolTrustFinancialDataObject>() { testResult } };
            });

            mockComparisonService.Setup(m =>
                    m.GenerateBenchmarkListWithAdvancedComparisonAsync(It.IsAny<BenchmarkCriteria>(),
                        It.IsAny<EstablishmentType>(), false, It.IsAny<Int32>()))
                .Returns((BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial, int basketSize) => cTask);

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var mockBicComparisonResultCachingService = new Mock<IBicComparisonResultCachingService>();

            var mockEfficiencyMetricService = new Mock<IEfficiencyMetricDataService>();

            var mockTrustBasketService = new Mock<ITrustBenchmarkListService>();

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object,
                mockDocumentDbService.Object,
                financialCalculationsService.Object,
                mockLaService.Object,
                null,
                mockEdubaseDataService.Object,
                null,
                mockComparisonService.Object,
                mockBicComparisonResultCachingService.Object,
                mockEfficiencyMetricService.Object,
                mockBenchmarkBasketService.Object,
                mockTrustBasketService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.GenerateFromAdvancedCriteria(new BenchmarkCriteria(), EstablishmentType.All, null, 123, ComparisonArea.All, BenchmarkListOverwriteStrategy.Add);

            result.Wait();

            Assert.AreEqual("~/Views/BenchmarkCriteria/OverwriteStrategy.cshtml",(result.Result as ViewResult).ViewName);
            Assert.IsFalse(string.IsNullOrEmpty(((result.Result as ViewResult).Model as BenchmarkCriteriaVM).ErrorMessage));

        }

        [Test]
        public void TabChangeShouldKeepUnitTypeBetweenExpenditureAndIncomeTabs()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockEdubaseDataService = new Mock<IContextDataService>();

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<TabType>(), It.IsAny<EstablishmentType>()))
                .Returns((TabType TabNames, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var mockComparisonService = new Mock<IComparisonService>();

            var mockBenchmarkBasketService = new Mock<ISchoolBenchmarkListService>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel { Urn = "123", EstabType = "Academies" });
            mockBenchmarkBasketService.Setup(m => m.GetSchoolBenchmarkList()).Returns(fakeSchoolComparisonList);

            var mockBicComparisonResultCachingService = new Mock<IBicComparisonResultCachingService>();

            var mockEfficiencyMetricService = new Mock<IEfficiencyMetricDataService>();

            var mockTrustBasketService = new Mock<ITrustBenchmarkListService>();

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object,
                mockDocumentDbService.Object,
                financialCalculationsService.Object,
                mockLaService.Object,
                null,
                mockEdubaseDataService.Object,
                null,
                mockComparisonService.Object,
                mockBicComparisonResultCachingService.Object,
                mockEfficiencyMetricService.Object,
                mockBenchmarkBasketService.Object,
                mockTrustBasketService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.TabChange(EstablishmentType.Maintained, UnitType.PerPupil, TabType.Income);
            
            financialCalculationsService.Verify(f=> f.PopulateBenchmarkChartsWithFinancialData(
                null,
                It.IsAny<List<FinancialDataModel>>(),
                It.IsAny<IEnumerable<CompareEntityBase>>(),
                It.IsAny<string>(),
                UnitType.PerPupil));
        }

        [Test]
        public void TabChangeShouldKeepUnitTypeBetweenExpenditureAndBalanceTabsIfPossible()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockEdubaseDataService = new Mock<IContextDataService>();

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();
            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<TabType>(), It.IsAny<EstablishmentType>()))
                .Returns((TabType TabNames, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var mockComparisonService = new Mock<IComparisonService>();

            IBenchmarkCriteriaBuilderService benchmarkCriteriaBuilderService = null;

            var mockBenchmarkBasketService = new Mock<ISchoolBenchmarkListService>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel { Urn = "123", EstabType = "Academies" });
            mockBenchmarkBasketService.Setup(m => m.GetSchoolBenchmarkList()).Returns(fakeSchoolComparisonList);

            var mockBicComparisonResultCachingService = new Mock<IBicComparisonResultCachingService>();

            var mockEfficiencyMetricService = new Mock<IEfficiencyMetricDataService>();

            var mockTrustBasketService = new Mock<ITrustBenchmarkListService>();

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object,
                mockDocumentDbService.Object,
                financialCalculationsService.Object,
                mockLaService.Object,
                null,
                mockEdubaseDataService.Object,
                benchmarkCriteriaBuilderService,
                mockComparisonService.Object,
                mockBicComparisonResultCachingService.Object,
                mockEfficiencyMetricService.Object,
                mockBenchmarkBasketService.Object,
                mockTrustBasketService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.TabChange(EstablishmentType.Maintained, UnitType.PerPupil, TabType.Balance);

            financialCalculationsService.Verify(f => f.PopulateBenchmarkChartsWithFinancialData(
                null,
                It.IsAny<List<FinancialDataModel>>(),
                It.IsAny<IEnumerable<CompareEntityBase>>(),
                It.IsAny<string>(),
                UnitType.PerPupil));
        }

        //[Test]
        //public void TabChangeShouldResetUnitTypeBetweenExpenditureAndBalanceTabsWhenKeepingNotPossible()
        //{
        //    var mockDocumentDbService = new Mock<IFinancialDataService>();

        //    var mockEdubaseDataService = new Mock<IContextDataService>();

        //    var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();

        //    mockBenchmarkChartBuilder
        //        .Setup(cb => cb.Build(It.IsAny<TabType>(), It.IsAny<EstablishmentType>()))
        //        .Returns((TabType TabNames, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

        //    mockBenchmarkChartBuilder
        //        .Setup(cb => cb.Build(It.IsAny<TabType>(), It.IsAny<ChartGroupType>(),It.IsAny<EstablishmentType>()))
        //        .Returns((TabType TabNames, ChartGroupType chartGroup ,EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { Name = "Staff", ChartGroup = ChartGroupType.Staff } });

        //    var financialCalculationsService = new Mock<IFinancialCalculationsService>();

        //    var mockLaService = new Mock<ILocalAuthoritiesService>();

        //    var mockComparisonService = new Mock<IComparisonService>();

        //    var mockBenchmarkBasketService = new Mock<IBenchmarkBasketService>();
        //    var fakeSchoolComparisonList = new SchoolComparisonListModel();
        //    fakeSchoolComparisonList.HomeSchoolUrn = "123";
        //    fakeSchoolComparisonList.HomeSchoolName = "test";
        //    fakeSchoolComparisonList.HomeSchoolType = "test";
        //    fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
        //    fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel { Urn = "123", EstabType = "Academies" });
        //    mockBenchmarkBasketService.Setup(m => m.ExtractSchoolComparisonListFromCookie()).Returns(fakeSchoolComparisonList);
        //    var mockBicComparisonResultCachingService = new Mock<IBicComparisonResultCachingService>();

        //    var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object, mockDocumentDbService.Object, financialCalculationsService.Object, mockLaService.Object, null, 
        //        mockEdubaseDataService.Object, null, mockComparisonService.Object, mockBenchmarkBasketService.Object, mockBicComparisonResultCachingService.Object);

        //    controller.ControllerContext = new ControllerContext(_rc, controller);

        //    controller.TabChange(EstablishmentType.Maintained, UnitType.PercentageOfTotalExpenditure, TabType.Balance);

        //    financialCalculationsService.Verify(f => f.PopulateBenchmarkChartsWithFinancialData(
        //        It.IsAny<List<ChartViewModel>>(),
        //        It.IsAny<List<FinancialDataModel>>(),
        //        It.IsAny<IEnumerable<CompareEntityBase>>(),
        //        It.IsAny<string>(),
        //        UnitType.AbsoluteMoney
        //        ));
        //}

        [Test]
        public void TabChangeShouldResetUnitTypeBetweenExpenditureAndWorkforceTabs()
        {
            var mockDocumentDbService = new Mock<IFinancialDataService>();

            var mockEdubaseDataService = new Mock<IContextDataService>();

            var mockBenchmarkChartBuilder = new Mock<IBenchmarkChartBuilder>();

            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<TabType>(), It.IsAny<EstablishmentType>()))
                .Returns((TabType TabNames, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockBenchmarkChartBuilder
                .Setup(cb => cb.Build(It.IsAny<TabType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>()))
                .Returns((TabType TabNames, ChartGroupType chartGroup, EstablishmentType schoolType) => new List<ChartViewModel>() { new ChartViewModel() { Name = "Staff", ChartGroup = ChartGroupType.Staff } });

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockLaService = new Mock<ILocalAuthoritiesService>();

            var mockComparisonService = new Mock<IComparisonService>();

            var mockBenchmarkBasketService = new Mock<ISchoolBenchmarkListService>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel { Urn = "123", EstabType = "Academies" });
            mockBenchmarkBasketService.Setup(m => m.GetSchoolBenchmarkList()).Returns(fakeSchoolComparisonList);

            var mockBicComparisonResultCachingService = new Mock<IBicComparisonResultCachingService>();

            var mockEfficiencyMetricService = new Mock<IEfficiencyMetricDataService>();

            var mockTrustBasketService = new Mock<ITrustBenchmarkListService>();

            var controller = new BenchmarkChartsController(mockBenchmarkChartBuilder.Object,
                mockDocumentDbService.Object,
                financialCalculationsService.Object,
                mockLaService.Object,
                null,
                mockEdubaseDataService.Object,
                null, mockComparisonService.Object,
                mockBicComparisonResultCachingService.Object,
                mockEfficiencyMetricService.Object,
                mockBenchmarkBasketService.Object,
                mockTrustBasketService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            controller.TabChange(EstablishmentType.Maintained, UnitType.AbsoluteMoney, TabType.Workforce);

            financialCalculationsService.Verify(f => f.PopulateBenchmarkChartsWithFinancialData(
                It.IsAny<List<ChartViewModel>>(),
                It.IsAny<List<FinancialDataModel>>(),
                It.IsAny<IEnumerable<CompareEntityBase>>(),
                It.IsAny<string>(),
                UnitType.AbsoluteCount));
        }

        [Test]
        public void GenerateFromSavedBasketReturnsWarningPageIfThereIsAnExistingListAndWouldReplace()
        {
            var mockBenchmarkBasketService = new Mock<ISchoolBenchmarkListService>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            for (int i = 0; i < 29; i++)
            {
                fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel()
                {
                    Urn = i.ToString(),
                    Name = "test",
                    EstabType = "Academies"
                });
            }
            mockBenchmarkBasketService.Setup(m => m.GetSchoolBenchmarkList()).Returns(fakeSchoolComparisonList);

            var mockBicComparisonResultCachingService = new Mock<IBicComparisonResultCachingService>();

            var mockEfficiencyMetricService = new Mock<IEfficiencyMetricDataService>();

            var mockTrustBasketService = new Mock<ITrustBenchmarkListService>();

            var controller = new BenchmarkChartsController(new Mock<IBenchmarkChartBuilder>().Object,
                new Mock<IFinancialDataService>().Object, new Mock<IFinancialCalculationsService>().Object,
                new Mock<ILocalAuthoritiesService>().Object, null,
                new Mock<IContextDataService>().Object, null,
                new Mock<IComparisonService>().Object,
                mockBicComparisonResultCachingService.Object,
                mockEfficiencyMetricService.Object,
                mockBenchmarkBasketService.Object,
                mockTrustBasketService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.GenerateFromSavedBasket("123-456", null, 123, null);

            result.Wait();

            Assert.AreEqual("ReplaceWithSavedBasket?savedUrns=123-456&default=123", (result.Result as RedirectResult).Url);

            var resultWithNoDefault = controller.GenerateFromSavedBasket("123-456", null, null, null);

            resultWithNoDefault.Wait();

            Assert.AreEqual("ReplaceWithSavedBasket?savedUrns=123-456", (resultWithNoDefault.Result as RedirectResult).Url);
        }

        [Test]
        public void GenerateFromSavedBasketReturnsWarningPageIfThereIsAnExistingListAndWouldReplaceTrustList()
        {
            var mockBenchmarkBasketService = new Mock<ISchoolBenchmarkListService>();
            var mockTrustBasketService = new Mock<ITrustBenchmarkListService>();

            var fakeTrustComparisonList = new TrustComparisonListModel();
            fakeTrustComparisonList.DefaultTrustCompanyNo = 123;
            fakeTrustComparisonList.DefaultTrustName = "test";
            for (int i = 0; i < 19; i++)
            {
                fakeTrustComparisonList.Trusts.Add(new BenchmarkTrustModel(i, "test"));
            }

            mockTrustBasketService.Setup(m => m.GetTrustBenchmarkList()).Returns(fakeTrustComparisonList);

            var mockBicComparisonResultCachingService = new Mock<IBicComparisonResultCachingService>();

            var mockEfficiencyMetricService = new Mock<IEfficiencyMetricDataService>();

            var controller = new BenchmarkChartsController(new Mock<IBenchmarkChartBuilder>().Object,
                new Mock<IFinancialDataService>().Object, new Mock<IFinancialCalculationsService>().Object,
                new Mock<ILocalAuthoritiesService>().Object, null,
                new Mock<IContextDataService>().Object, null,
                new Mock<IComparisonService>().Object,
                mockBicComparisonResultCachingService.Object,
                mockEfficiencyMetricService.Object,
                mockBenchmarkBasketService.Object,
                mockTrustBasketService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.GenerateFromSavedBasket(null, "123-456", 123, null);

            result.Wait();

            Assert.AreEqual("ReplaceWithSavedBasket?savedCompanyNos=123-456&default=123", (result.Result as RedirectResult).Url);

            var resultWithNoDefault = controller.GenerateFromSavedBasket(null, "123-456", null, null);

            resultWithNoDefault.Wait();

            Assert.AreEqual("ReplaceWithSavedBasket?savedCompanyNos=123-456", (resultWithNoDefault.Result as RedirectResult).Url);
        }

        [Test]
        public void GenerateFromSavedBasketReturnsConfirmationPageIfThereIsAnExistingListAndCouldReplaceOrAdd()
        {
            var mockBenchmarkBasketService = new Mock<ISchoolBenchmarkListService>();
            var fakeSchoolComparisonList = new SchoolComparisonListModel();
            fakeSchoolComparisonList.HomeSchoolUrn = "123";
            fakeSchoolComparisonList.HomeSchoolName = "test";
            fakeSchoolComparisonList.HomeSchoolType = "test";
            fakeSchoolComparisonList.HomeSchoolFinancialType = "Academies";
            for (int i = 0; i < 28; i++)
            {
                fakeSchoolComparisonList.BenchmarkSchools.Add(new BenchmarkSchoolModel()
                {
                    Urn = i.ToString(),
                    Name = "test",
                    EstabType = "Academies"
                });
            }
            mockBenchmarkBasketService.Setup(m => m.GetSchoolBenchmarkList()).Returns(fakeSchoolComparisonList);

            var mockBicComparisonResultCachingService = new Mock<IBicComparisonResultCachingService>();

            var mockEfficiencyMetricService = new Mock<IEfficiencyMetricDataService>();

            var mockTrustBasketService = new Mock<ITrustBenchmarkListService>();

            var controller = new BenchmarkChartsController(new Mock<IBenchmarkChartBuilder>().Object,
                new Mock<IFinancialDataService>().Object, new Mock<IFinancialCalculationsService>().Object,
                new Mock<ILocalAuthoritiesService>().Object, null,
                new Mock<IContextDataService>().Object, null, new Mock<IComparisonService>().Object,
                mockBicComparisonResultCachingService.Object, mockEfficiencyMetricService.Object,
                mockBenchmarkBasketService.Object,
                mockTrustBasketService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.GenerateFromSavedBasket("123-456", null, 123, null);

            result.Wait();

            Assert.AreEqual("SaveOverwriteStrategy?savedUrns=123-456&default=123", (result.Result as RedirectResult).Url);

            var resultWithNoDefault = controller.GenerateFromSavedBasket("123-456", null, null, null);

            resultWithNoDefault.Wait();

            Assert.AreEqual("SaveOverwriteStrategy?savedUrns=123-456", (resultWithNoDefault.Result as RedirectResult).Url);
        }

        [Test]
        public void GenerateFromSavedBasketReturnsConfirmationPageIfThereIsAnExistingListAndCouldReplaceOrAddTrustList()
        {
            var mockBenchmarkBasketService = new Mock<ISchoolBenchmarkListService>();
            var mockTrustBasketService = new Mock<ITrustBenchmarkListService>();

            var fakeTrustComparisonList = new TrustComparisonListModel();
            fakeTrustComparisonList.DefaultTrustCompanyNo = 123;
            fakeTrustComparisonList.DefaultTrustName = "test";
            for (int i = 0; i < 5; i++)
            {
                fakeTrustComparisonList.Trusts.Add(new BenchmarkTrustModel(i, "test"));
            }
            mockTrustBasketService.Setup(m => m.GetTrustBenchmarkList()).Returns(fakeTrustComparisonList);

            var mockBicComparisonResultCachingService = new Mock<IBicComparisonResultCachingService>();

            var mockEfficiencyMetricService = new Mock<IEfficiencyMetricDataService>();

            var controller = new BenchmarkChartsController(new Mock<IBenchmarkChartBuilder>().Object,
                new Mock<IFinancialDataService>().Object, new Mock<IFinancialCalculationsService>().Object,
                new Mock<ILocalAuthoritiesService>().Object, null,
                new Mock<IContextDataService>().Object, null, new Mock<IComparisonService>().Object,
                mockBicComparisonResultCachingService.Object, mockEfficiencyMetricService.Object,
                mockBenchmarkBasketService.Object,
                mockTrustBasketService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            var result = controller.GenerateFromSavedBasket(null, "123-456", 123, null);

            result.Wait();

            Assert.AreEqual("SaveOverwriteStrategy?savedCompanyNos=123-456&default=123", (result.Result as RedirectResult).Url);

            var resultWithNoDefault = controller.GenerateFromSavedBasket(null, "123-456", null, null);

            resultWithNoDefault.Wait();

            Assert.AreEqual("SaveOverwriteStrategy?savedCompanyNos=123-456", (resultWithNoDefault.Result as RedirectResult).Url);
        }
    }
}
