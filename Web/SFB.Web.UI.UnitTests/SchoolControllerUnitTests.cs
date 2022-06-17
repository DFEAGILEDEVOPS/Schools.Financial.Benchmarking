using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.UI.Controllers;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers.Constants;
using System.Threading.Tasks;

namespace SFB.Web.UI.UnitTests
{
    public class SchoolControllerUnitTests
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
        public async Task DetailShouldKeepUnitTypeBetweenExpenditureAndIncomeTabsAsync()
        {
            var fakeEdubaseObject = new EdubaseDataObject
            {
                URN = 123,
                FinanceType = "Maintained"
            };

            var mockEdubaseDataService = new Mock<IContextDataService>();

            var GetSchoolDataObjectByUrnAsyncTask = Task.Run(() =>
            {
                return fakeEdubaseObject;
            });

            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(123)).Returns((long urn) => GetSchoolDataObjectByUrnAsyncTask);

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var mockCookieManager = new Mock<ISchoolBenchmarkListService>();

            var mockLaSearchService = new Mock<ILocalAuthoritiesService>();

            var mockActiveUrnsService = new Mock<IActiveEstablishmentsService>();

            var mockCscpLookupService = new Mock<ICscpLookupService>();

            var mockGiasLookupService = new Mock<IGiasLookupService>();

            var mockSchoolVMBuilder = new Mock<ISchoolVMBuilder>();
            var fakeSVM = new SchoolViewModel(fakeEdubaseObject);
            fakeSVM.HistoricalFinancialDataModels = new List<FinancialDataModel> { new FinancialDataModel() };
            fakeSVM.HistoricalCharts = new List<ChartViewModel> { new ChartViewModel() };
            mockSchoolVMBuilder.Setup(m => m.GetResult()).Returns(fakeSVM);

            var controller = new SchoolController(mockFinancialDataService.Object, financialCalculationsService.Object, 
                mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object, mockCookieManager.Object, mockActiveUrnsService.Object,
                mockSchoolVMBuilder.Object,mockGiasLookupService.Object, mockCscpLookupService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            await controller.Index(123, UnitType.PerPupil, CentralFinancingType.Exclude, TabType.Income);

            mockSchoolVMBuilder.Verify(f => f.AddHistoricalChartsAsync(
                It.IsAny<TabType>(),
                It.IsAny<ChartGroupType>(),
                It.IsAny<CentralFinancingType>(),
                UnitType.PerPupil));
        }

        [Test]
        public async Task DetailCallShouldKeepUnitTypeBetweenExpenditureAndBalanceTabsIfPossibleAsync()
        {
            var fakeEdubaseObject = new EdubaseDataObject
            {
                URN = 123,
                FinanceType = "Maintained"
            };

            var mockEdubaseDataService = new Mock<IContextDataService>();
            var GetSchoolDataObjectByUrnAsyncTask = Task.Run(() =>
            {
                return fakeEdubaseObject;
            });
            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(123)).Returns((long urn) => GetSchoolDataObjectByUrnAsyncTask);

            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var mockCookieManager = new Mock<ISchoolBenchmarkListService>();

            var mockLaSearchService = new Mock<ILocalAuthoritiesService>();

            var mockActiveUrnsService = new Mock<IActiveEstablishmentsService>();
            
            var mockCscpLookupService = new Mock<ICscpLookupService>();

            var mockGiasLookupService = new Mock<IGiasLookupService>();
            
            var mockSchoolVMBuilder = new Mock<ISchoolVMBuilder>();
            var fakeSVM = new SchoolViewModel(fakeEdubaseObject);
            fakeSVM.HistoricalFinancialDataModels = new List<FinancialDataModel> { new FinancialDataModel() };
            fakeSVM.HistoricalCharts = new List<ChartViewModel> { new ChartViewModel() };
            mockSchoolVMBuilder.Setup(m => m.GetResult()).Returns(fakeSVM);
            
            var controller = new SchoolController(mockFinancialDataService.Object, financialCalculationsService.Object, 
                mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object, mockCookieManager.Object, mockActiveUrnsService.Object,
                mockSchoolVMBuilder.Object, mockGiasLookupService.Object, mockCscpLookupService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            await controller.Index(123, UnitType.PerPupil, CentralFinancingType.Exclude, TabType.Balance);

            mockSchoolVMBuilder.Verify(f => f.AddHistoricalChartsAsync(
                It.IsAny<TabType>(),
                It.IsAny<ChartGroupType>(),
                It.IsAny<CentralFinancingType>(),
                UnitType.PerPupil));
        }

        [Test]
        public void DetailCallShouldResetUnitTypeBetweenExpenditureAndWorkforceTabs()
        {
            var mockEdubaseDataService = new Mock<IContextDataService>();
            var GetSchoolDataObjectByUrnAsyncTask = Task.Run(() =>
            {
                return new EdubaseDataObject
                {
                    URN = 123,
                    FinanceType = "Maintained"
                };
            });

            mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrnAsync(123)).Returns((long urn) => GetSchoolDataObjectByUrnAsyncTask);

            var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<TabType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>(), It.IsAny<UnitType>()))
                .Returns((TabType TabNames, ChartGroupType chartGroupType, EstablishmentType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

            mockHistoricalChartBuilder
                .Setup(cb => cb.Build(It.IsAny<TabType>(), It.IsAny<EstablishmentType>()))
                .Returns((TabType TabNames, EstablishmentType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


            var financialCalculationsService = new Mock<IFinancialCalculationsService>();

            var mockFinancialDataService = new Mock<IFinancialDataService>();

            var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

            var mockCookieManager = new Mock<ISchoolBenchmarkListService>();

            var mockLaSearchService = new Mock<ILocalAuthoritiesService>();

            var mockActiveUrnsService = new Mock<IActiveEstablishmentsService>();

            var mockSchoolVMBuilder = new Mock<ISchoolVMBuilder>();

            var mockCscpLookupService = new Mock<ICscpLookupService>();

            var mockGiasLookupService = new Mock<IGiasLookupService>();

            var controller = new SchoolController(mockFinancialDataService.Object, financialCalculationsService.Object, 
                mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object, mockCookieManager.Object, mockActiveUrnsService.Object,
                mockSchoolVMBuilder.Object, mockGiasLookupService.Object, mockCscpLookupService.Object);

            controller.ControllerContext = new ControllerContext(_rc, controller);

            _ = controller.Index(123, UnitType.PerPupil, CentralFinancingType.Exclude, TabType.Workforce);

            mockSchoolVMBuilder.Verify(f => f.AddHistoricalChartsAsync(
                It.IsAny<TabType>(),
                It.IsAny<ChartGroupType>(),
                It.IsAny<CentralFinancingType>(),
                UnitType.AbsoluteCount));
        }

        //[Test]
        //public void DetailCallShouldSetSptReportExistsPropertyTrue()
        //{
        //    var mockEdubaseDataService = new Mock<IContextDataService>();
        //    var testEduResult = new EdubaseDataObject();
        //    testEduResult.URN = 123;
        //    testEduResult.FinanceType = "Maintained";
        //    mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(123)).Returns((long urn) => testEduResult);

        //    var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
        //    mockHistoricalChartBuilder
        //        .Setup(cb => cb.Build(It.IsAny<TabNamesType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>(), It.IsAny<UnitType>()))
        //        .Returns((TabNamesType TabNames, ChartGroupType chartGroupType, EstablishmentType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

        //    mockHistoricalChartBuilder
        //        .Setup(cb => cb.Build(It.IsAny<TabNamesType>(), It.IsAny<EstablishmentType>()))
        //        .Returns((TabNamesType TabNames, EstablishmentType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


        //    var financialCalculationsService = new Mock<IFinancialCalculationsService>();

        //    var mockFinancialDataService = new Mock<IFinancialDataService>();

        //    var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

        //    var mockCookieManager = new Mock<IBenchmarkBasketService>();

        //    var mockApiRequest = new Mock<ISptReportService>();

        //    var mockLaSearchService = new Mock<ILocalAuthoritiesService>();

        //    var mockActiveUrnsService = new Mock<IActiveUrnsService>();

        //    mockApiRequest
        //        .Setup(ar => ar.SptReportExists(It.IsAny<int>()))
        //        .Returns((long urn) => true);

        //    var controller = new SchoolController(mockHistoricalChartBuilder.Object, mockFinancialDataService.Object, financialCalculationsService.Object, 
        //        mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object, mockCookieManager.Object, mockApiRequest.Object, mockLaSearchService.Object, mockActiveUrnsService.Object);

        //    controller.ControllerContext = new ControllerContext(_rc, controller);

        //    var view = controller.Detail(123, UnitType.PerPupil, CentralFinancingType.Exclude, TabNamesType.Workforce);
        //    view.Wait();
            
        //    Assert.AreEqual(((ViewResult)view.Result).ViewBag.SptReportExists, true);
        //}

        //[Test]
        //public void DetailCallShouldSetSptReportExistsPropertyFalse()
        //{
        //    var mockEdubaseDataService = new Mock<IContextDataService>();
        //    var testEduResult = new EdubaseDataObject();
        //    testEduResult.URN = 123;
        //    testEduResult.FinanceType = "Maintained";
        //    mockEdubaseDataService.Setup(m => m.GetSchoolDataObjectByUrn(123)).Returns((long urn) => testEduResult);

        //    var mockHistoricalChartBuilder = new Mock<IHistoricalChartBuilder>();
        //    mockHistoricalChartBuilder
        //        .Setup(cb => cb.Build(It.IsAny<TabNamesType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>(), It.IsAny<UnitType>()))
        //        .Returns((TabNamesType TabNames, ChartGroupType chartGroupType, EstablishmentType schoolFinancialType, UnitType unit) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });

        //    mockHistoricalChartBuilder
        //        .Setup(cb => cb.Build(It.IsAny<TabNamesType>(), It.IsAny<EstablishmentType>()))
        //        .Returns((TabNamesType TabNames, EstablishmentType schoolFinancialType) => new List<ChartViewModel>() { new ChartViewModel() { ChartGroup = ChartGroupType.Staff } });


        //    var financialCalculationsService = new Mock<IFinancialCalculationsService>();

        //    var mockFinancialDataService = new Mock<IFinancialDataService>();

        //    var mockDownloadCsvBuilder = new Mock<IDownloadCSVBuilder>();

        //    var mockCookieManager = new Mock<IBenchmarkBasketService>();

        //    var mockActiveUrnsService = new Mock<IActiveUrnsService>();

        //    var mockApiRequest = new Mock<ISptReportService>();

        //    var mockLaSearchService = new Mock<ILocalAuthoritiesService>();

        //    mockApiRequest
        //        .Setup(ar => ar.SptReportExists(It.IsAny<int>()))
        //        .Returns((long urn) => false);

        //    var controller = new SchoolController(mockHistoricalChartBuilder.Object, mockFinancialDataService.Object, financialCalculationsService.Object, 
        //        mockEdubaseDataService.Object, mockDownloadCsvBuilder.Object, mockCookieManager.Object, mockApiRequest.Object, mockLaSearchService.Object, mockActiveUrnsService.Object);

        //    controller.ControllerContext = new ControllerContext(_rc, controller);

        //    var view = controller.Detail(123, UnitType.PerPupil, CentralFinancingType.Exclude, TabNamesType.Workforce);
        //    view.Wait();

        //    Assert.AreEqual(((ViewResult)view.Result).ViewBag.SptReportExists, false);
        //}
    }
}
