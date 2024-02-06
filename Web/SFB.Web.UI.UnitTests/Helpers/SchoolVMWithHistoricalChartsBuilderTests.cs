using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Helpers;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;

namespace SFB.Web.UI.UnitTests.Helpers
{
    public class SchoolVMWithHistoricalChartsBuilderTests
    {
        private SchoolVMWithHistoricalChartsBuilder _sut;
        private Mock<IHistoricalChartBuilder> _historicalChartBuilder;
        private Mock<IFinancialDataService> _financialDataService;
        private Mock<IFinancialCalculationsService> _financialCalculationsService;
        private Mock<IContextDataService> _contextDataService;
        private Mock<ISchoolBenchmarkListService> _schoolBenchmarkListService;
        private Mock<ILocalAuthoritiesService> _localAuthoritiesService;
        private const CentralFinancingType FINANCING_TYPE = CentralFinancingType.Include;
        private const EstablishmentType ESTABLISHMENT_TYPE = EstablishmentType.Maintained;
        private const long URN = 123456789;
        private const int TERM = 2023;

        [SetUp]
        public void SetupContext()
        {
            _historicalChartBuilder = new Mock<IHistoricalChartBuilder>();
            _historicalChartBuilder
                .Setup(b => b.Build(It.IsAny<TabType>(), It.IsAny<ChartGroupType>(), It.IsAny<EstablishmentType>(),
                    It.IsAny<UnitType>())).Returns(new List<ChartViewModel>());

            _financialDataService = new Mock<IFinancialDataService>();
            _financialDataService.Setup(f => f.GetLatestDataYearPerEstabTypeAsync(ESTABLISHMENT_TYPE))
                .ReturnsAsync(TERM);
            _financialDataService
                .Setup(f => f.GetSchoolFinancialDataObjectAsync(URN,
                    SchoolFormatHelpers.FinancialTermFormatAcademies(TERM), ESTABLISHMENT_TYPE, FINANCING_TYPE))
                .ReturnsAsync(new SchoolTrustFinancialDataObject());

            _financialCalculationsService = new Mock<IFinancialCalculationsService>();
            _financialCalculationsService.Setup(f =>
                f.PopulateHistoricalChartsWithFinancialData(It.IsAny<List<ChartViewModel>>(),
                    It.IsAny<List<FinancialDataModel>>(), It.IsAny<string>(), It.IsAny<TabType>(), It.IsAny<UnitType>(),
                    It.IsAny<EstablishmentType>()));

            _contextDataService = new Mock<IContextDataService>();
            _contextDataService.Setup(d => d.GetSchoolDataObjectByUrnAsync(URN)).ReturnsAsync(new EdubaseDataObject
            {
                URN = URN,
                FinanceType = ESTABLISHMENT_TYPE.ToString()
            });

            _schoolBenchmarkListService = new Mock<ISchoolBenchmarkListService>();
            _schoolBenchmarkListService.Setup(s => s.GetSchoolBenchmarkList()).Returns(new SchoolComparisonListModel());

            _localAuthoritiesService = new Mock<ILocalAuthoritiesService>();

            _sut = new SchoolVMWithHistoricalChartsBuilder(_historicalChartBuilder.Object, _financialDataService.Object,
                _financialCalculationsService.Object, _contextDataService.Object, _schoolBenchmarkListService.Object,
                _localAuthoritiesService.Object);
        }

        [Test]
        public async Task
            AddHistoricalChartsAsync_ForExpenditureTabType_PopulatesUniqueHistoricalFinancialDataModelsBasedOnId()
        {
            // arrange
            await _sut.BuildCoreAsync(URN);
            await _sut.AddHistoricalChartsAsync(TabType.Expenditure, ChartGroupType.TotalExpenditure, FINANCING_TYPE,
                UnitType.AbsoluteMoney);
            
            // act
            await _sut.AddHistoricalChartsAsync(TabType.Expenditure, ChartGroupType.TotalExpenditure, FINANCING_TYPE,
                UnitType.AbsoluteMoney);
            var result = _sut.GetResult();

            // assert
            Assert.IsTrue(result.HistoricalFinancialDataModels.Count == ChartHistory.YEARS_OF_HISTORY * 2);
        }

        [Test]
        public async Task
            AddHistoricalChartsAsync_ForWorkforceTabType_PopulatesUniqueHistoricalFinancialDataModelsBasedOnTerm()
        {
            // arrange
            await _sut.BuildCoreAsync(URN);
            await _sut.AddHistoricalChartsAsync(TabType.Expenditure, ChartGroupType.TotalExpenditure, FINANCING_TYPE,
                UnitType.AbsoluteMoney);
            
            // act
            await _sut.AddHistoricalChartsAsync(TabType.Workforce, ChartGroupType.Workforce, FINANCING_TYPE,
                UnitType.AbsoluteCount);
            var result = _sut.GetResult();

            // assert
            Assert.IsTrue(result.HistoricalFinancialDataModels.Count == ChartHistory.YEARS_OF_HISTORY);
        }
    }
}