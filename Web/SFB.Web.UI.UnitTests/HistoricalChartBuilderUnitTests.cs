using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFB.Web.Domain.Helpers.Enums;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.UnitTests
{
    public class HistoricalChartBuilderUnitTests
    {
        [Test]
        public void TotalExpenditureChartShouldNotBeInPercentageOfTotalMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(RevenueGroupType.Expenditure, ChartGroupType.All, SchoolFinancialType.Academies, UnitType.PercentageOfTotal);

            Assert.False(chartVMs.Any(c => c.Name == "Total expenditure"));
        }

        [Test]
        public void TotalIncomeChartShouldNotBeInPercentageOfTotalMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(RevenueGroupType.Income, ChartGroupType.All, SchoolFinancialType.Academies, UnitType.PercentageOfTotal);

            Assert.False(chartVMs.Any(c => c.Name == "Total income"));
        }

        [Test]
        public void WorkforceHeadCountChartShouldNotBeInHeadcountPerFTEMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(RevenueGroupType.Workforce, ChartGroupType.All, SchoolFinancialType.Academies, UnitType.HeadcountPerFTE);

            Assert.False(chartVMs.Any(c => c.Name == "School workforce (headcount)"));
        }

        [Test]
        public void WorkforceHeadCountChartShouldNotBeInFTERatioToTotalFTEMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(RevenueGroupType.Workforce, ChartGroupType.All, SchoolFinancialType.Academies, UnitType.FTERatioToTotalFTE);

            Assert.False(chartVMs.Any(c => c.Name == "School workforce (headcount)"));
        }

        [Test]
        public void WorkforceFTEChartShouldNotBeInFTERatioToTotalFTEMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(RevenueGroupType.Workforce, ChartGroupType.All, SchoolFinancialType.Academies, UnitType.FTERatioToTotalFTE);

            Assert.False(chartVMs.Any(c => c.Name == "School workforce (FTE)"));
        }

        [Test]
        public void QTSChartShouldNotBeInHeadcountPerFTEMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(RevenueGroupType.Workforce, ChartGroupType.All, SchoolFinancialType.Academies, UnitType.HeadcountPerFTE);

            Assert.False(chartVMs.Any(c => c.Name == "Teachers with QTS (%)"));
        }

        [Test]
        public void QTSChartShouldNotBeInRatioFTEMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(RevenueGroupType.Workforce, ChartGroupType.All, SchoolFinancialType.Academies, UnitType.FTERatioToTotalFTE);

            Assert.False(chartVMs.Any(c => c.Name == "Teachers with QTS (%)"));
        }

        [Test]
        public void QTSChartShouldNotBeInRatioNumberOfPupilsPerMeasureMetricCharts()
        {
            var builder = new HistoricalChartBuilder();

            var chartVMs = builder.Build(RevenueGroupType.Workforce, ChartGroupType.All, SchoolFinancialType.Academies, UnitType.NoOfPupilsPerMeasure);

            Assert.False(chartVMs.Any(c => c.Name == "Teachers with QTS (%)"));
        }
    }
}
