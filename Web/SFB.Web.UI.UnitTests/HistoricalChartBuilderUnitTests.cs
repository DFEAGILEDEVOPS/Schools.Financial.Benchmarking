using System.Linq;
using NUnit.Framework;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.UnitTests
{
    public class HistoricalChartBuilderUnitTests
    {
        [Test]
        public void TotalExpenditureChartShouldNotBeInPercentageOfTotalExpenditureMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(TabType.Expenditure, ChartGroupType.All, EstablishmentType.Academies, UnitType.PercentageOfTotalExpenditure);

            Assert.False(chartVMs.Any(c => c.Name == "Total expenditure"));
        }

        [Test]
        public void TotalIncomeChartShouldNotBeInPercentageOfTotalIncomeMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(TabType.Income, ChartGroupType.All, EstablishmentType.Academies, UnitType.PercentageOfTotalIncome);

            Assert.False(chartVMs.Any(c => c.Name == "Total income"));
        }

        [Test]
        public void WorkforceHeadCountChartShouldNotBeInHeadcountPerFTEMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(TabType.Workforce, ChartGroupType.All, EstablishmentType.Academies, UnitType.HeadcountPerFTE);

            Assert.False(chartVMs.Any(c => c.Name == "School workforce (headcount)"));
        }

        [Test]
        public void WorkforceHeadCountChartShouldNotBeInFTERatioToTotalFTEMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(TabType.Workforce, ChartGroupType.All, EstablishmentType.Academies, UnitType.FTERatioToTotalFTE);

            Assert.False(chartVMs.Any(c => c.Name == "School workforce (headcount)"));
        }

        [Test]
        public void WorkforceFTEChartShouldNotBeInFTERatioToTotalFTEMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(TabType.Workforce, ChartGroupType.All, EstablishmentType.Academies, UnitType.FTERatioToTotalFTE);

            Assert.False(chartVMs.Any(c => c.Name == "School workforce (Full Time Equivalent)"));
        }

        [Test]
        public void QTSChartShouldNotBeInHeadcountPerFTEMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(TabType.Workforce, ChartGroupType.All, EstablishmentType.Academies, UnitType.HeadcountPerFTE);

            Assert.False(chartVMs.Any(c => c.Name == "Teachers with Qualified Teacher Status (%)"));
        }

        [Test]
        public void QTSChartShouldNotBeInRatioFTEMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(TabType.Workforce, ChartGroupType.All, EstablishmentType.Academies, UnitType.FTERatioToTotalFTE);

            Assert.False(chartVMs.Any(c => c.Name == "Teachers with Qualified Teacher Status (%)"));
        }

        [Test]
        public void QTSChartShouldNotBeInRatioNumberOfPupilsPerMeasureMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(TabType.Workforce, ChartGroupType.All, EstablishmentType.Academies, UnitType.NoOfPupilsPerMeasure);

            Assert.False(chartVMs.Any(c => c.Name == "Teachers with Qualified Teacher Status (%)"));
        }
    }
}
