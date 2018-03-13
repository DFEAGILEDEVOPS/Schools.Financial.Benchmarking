using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFB.Web.Common;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Helpers
{
    public interface IDownloadCSVBuilder
    {
        string BuildCSVContentForSchools(ComparisonListModel comparisonList, List<ChartViewModel> benchmarkCharts);
        string BuildCSVContentForTrusts(TrustComparisonViewModel comparisonList, List<ChartViewModel> benchmarkCharts);
        string BuildCSVContentHistorically(EstablishmentViewModelBase estabVM, int latestYear);
    }

    public class DownloadCSVBuilder : IDownloadCSVBuilder
    {
        public string BuildCSVContentHistorically(EstablishmentViewModelBase estabVM, int latestYear)
        {
            var csv = new StringBuilder();
            var header = new StringBuilder();

            header.Append(estabVM.Type == "MAT" ? "Trust Name," : "School Name,");
            header.Append("Period,Number of Pupils,Number of Teachers,");
            foreach (var chart in estabVM.HistoricalCharts)
            {
                header.Append(chart.Name);
                header.Append(",");
            }

            csv.AppendLine(header.ToString().TrimEnd(','));

            for (int i = 0; i < ChartHistory.YEARS_OF_HISTORY; i++)
            {
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var termFormatted = FormatTerm(term, estabVM.FinancialType);
                var valuesLine = new StringBuilder();
                var data = estabVM.HistoricalCharts.First().HistoricalData.Find(d => d.Year == term);
                if (data?.Amount != null)
                {
                    valuesLine.Append($"\"{estabVM.Name}\",{termFormatted},{data.PupilCount},{data.TeacherCount},");
                    foreach (var chart in estabVM.HistoricalCharts)
                    {
                        valuesLine.Append(chart.HistoricalData.First(d => d.Year == term).Amount);
                        valuesLine.Append(",");
                    }

                    csv.AppendLine(valuesLine.ToString().TrimEnd(','));
                }
            }

            return csv.ToString();
        }

        public string BuildCSVContentForSchools(ComparisonListModel comparisonList, List<ChartViewModel> benchmarkCharts)
        {
            var csv = new StringBuilder();

            BuildHeaderLine(benchmarkCharts, csv, EstablishmentType.Maintained);

            BuildHomeSchoolLine(comparisonList, benchmarkCharts, csv);

            BuildOtherSchoolsLines(comparisonList, benchmarkCharts, csv);

            return csv.ToString();
        }

        public string BuildCSVContentForTrusts(TrustComparisonViewModel comparisonList, List<ChartViewModel> benchmarkCharts)
        {
            var csv = new StringBuilder();

            BuildHeaderLine(benchmarkCharts, csv, EstablishmentType.MAT);

            BuildDefaultTrustLine(comparisonList, benchmarkCharts, csv);

            BuildOtherTrustsLines(comparisonList, benchmarkCharts, csv);

            return csv.ToString();
        }

        private void BuildHeaderLine(List<ChartViewModel> benchmarkCharts, StringBuilder csv, EstablishmentType establishmentType)
        {
            var header = new StringBuilder();
            header.Append(establishmentType == EstablishmentType.MAT ? "Trust Name" : "School Name");
            header.Append(",URN,Analysis Period,Number of Pupils,Number of Teachers,");
            foreach (var chart in benchmarkCharts.Where(bc => bc.Downloadable))
            {
                header.Append(chart.Name);
                header.Append(",");
            }

            foreach (var col in benchmarkCharts.Where(bc => bc.TableColumns != null).SelectMany(bc => bc.TableColumns))
            {
                header.Append(col.Name);
                header.Append(",");
            }
            csv.AppendLine(header.ToString().TrimEnd(','));
        }

        private void BuildHomeSchoolLine(ComparisonListModel comparisonList, List<ChartViewModel> benchmarkCharts, StringBuilder csv)
        {
            if (!string.IsNullOrEmpty(comparisonList.HomeSchoolUrn) && comparisonList.BenchmarkSchools.Any(s => s.Urn == comparisonList.HomeSchoolUrn))
            {
                var valuesLine = new StringBuilder();
                var data = benchmarkCharts.First().BenchmarkData.Find(d => d.Urn == comparisonList.HomeSchoolUrn);
                var term = data.Term;

                var formattedTerm = FormatTerm(term, (SchoolFinancialType)Enum.Parse(typeof(SchoolFinancialType), comparisonList.HomeSchoolFinancialType));
                valuesLine.Append($"Your school,{data.Urn},{formattedTerm},{data.PupilCount},{data.TeacherCount},");

                foreach (var chart in benchmarkCharts.Where(bc => bc.Downloadable))
                {
                    var amount = chart.BenchmarkData.Find(d => d.Urn == comparisonList.HomeSchoolUrn).Amount;
                    valuesLine.Append(amount == null ? "N/A" : amount.ToString());
                    valuesLine.Append(",");
                }

                foreach (var col in benchmarkCharts.Where(bc => bc.TableColumns != null).SelectMany(bc => bc.TableColumns))
                {
                    var amount = col.BenchmarkData.Find(d => d.Urn == comparisonList.HomeSchoolUrn).Amount;
                    valuesLine.Append(amount == null ? "N/A" : amount.ToString());
                    valuesLine.Append(",");
                }

                csv.AppendLine(valuesLine.ToString().TrimEnd(','));
            }
        }

        private void BuildDefaultTrustLine(TrustComparisonViewModel comparisonList, List<ChartViewModel> benchmarkCharts, StringBuilder csv)
        {
            if (!string.IsNullOrEmpty(comparisonList.DefaultTrustMatNo) && comparisonList.Trusts.Any(s => s.MatNo == comparisonList.DefaultTrustMatNo))
            {
                var valuesLine = new StringBuilder();
                var data = benchmarkCharts.First().BenchmarkData.Find(d => d.Urn == comparisonList.DefaultTrustMatNo);
                var term = data.Term;

                valuesLine.Append($"Your trust,{data.Urn},{term},{data.PupilCount},{data.TeacherCount},");

                foreach (var chart in benchmarkCharts.Where(bc => bc.Downloadable))
                {
                    var amount = chart.BenchmarkData.Find(d => d.Urn == comparisonList.DefaultTrustMatNo).Amount;
                    valuesLine.Append(amount == null ? "N/A" : amount.ToString());
                    valuesLine.Append(",");
                }

                foreach (var col in benchmarkCharts.Where(bc => bc.TableColumns != null).SelectMany(bc => bc.TableColumns))
                {
                    var amount = col.BenchmarkData.Find(d => d.Urn == comparisonList.DefaultTrustMatNo).Amount;
                    valuesLine.Append(amount == null ? "N/A" : amount.ToString());
                    valuesLine.Append(",");
                }

                csv.AppendLine(valuesLine.ToString().TrimEnd(','));
            }
        }

        private void BuildOtherSchoolsLines(ComparisonListModel comparisonList, List<ChartViewModel> benchmarkCharts, StringBuilder csv)
        {
            var otherSchools = comparisonList.BenchmarkSchools.Where(b => b.Urn != comparisonList.HomeSchoolUrn);

            foreach (var school in otherSchools)
            {
                var valuesLine = new StringBuilder();
                var data = benchmarkCharts.First().BenchmarkData.Find(d => d.Urn == school.Urn);
                var term = benchmarkCharts.First().BenchmarkData.Find(d => d.Urn == school.Urn).Term;
                var formattedTerm = FormatTerm(term, (SchoolFinancialType)Enum.Parse(typeof(SchoolFinancialType), school.FinancialType));
                valuesLine.Append($"\"{school.Name}\",{data.Urn},{formattedTerm},{data.PupilCount},{data.TeacherCount},");

                foreach (var chart in benchmarkCharts.Where(bc => bc.Downloadable))
                {
                    var amount = chart.BenchmarkData.Find(d => d.Urn == school.Urn).Amount;
                    valuesLine.Append(amount == null ? "N/A" : amount.ToString());
                    valuesLine.Append(",");
                }

                foreach (var col in benchmarkCharts.Where(bc => bc.TableColumns != null).SelectMany(bc => bc.TableColumns))
                {
                    var amount = col.BenchmarkData.Find(d => d.Urn == school.Urn).Amount;
                    valuesLine.Append(amount == null ? "N/A" : amount.ToString());
                    valuesLine.Append(",");
                }

                csv.AppendLine(valuesLine.ToString().TrimEnd(','));
            }
        }

        private void BuildOtherTrustsLines(TrustComparisonViewModel comparisonList, List<ChartViewModel> benchmarkCharts, StringBuilder csv)
        {
            var otherTrusts = comparisonList.Trusts.Where(b => b.MatNo != comparisonList.DefaultTrustMatNo);

            foreach (var trust in otherTrusts)
            {
                var valuesLine = new StringBuilder();
                var data = benchmarkCharts.First().BenchmarkData.Find(d => d.Urn == trust.MatNo);
                var term = benchmarkCharts.First().BenchmarkData.Find(d => d.Urn == trust.MatNo).Term;
                valuesLine.Append($"\"{trust.Name}\",{data.Urn},{term},{data.PupilCount},{data.TeacherCount},");

                foreach (var chart in benchmarkCharts.Where(bc => bc.Downloadable))
                {
                    var amount = chart.BenchmarkData.Find(d => d.Urn == trust.MatNo).Amount;
                    valuesLine.Append(amount == null ? "N/A" : amount.ToString());
                    valuesLine.Append(",");
                }

                foreach (var col in benchmarkCharts.Where(bc => bc.TableColumns != null).SelectMany(bc => bc.TableColumns))
                {
                    var amount = col.BenchmarkData.Find(d => d.Urn == trust.MatNo).Amount;
                    valuesLine.Append(amount == null ? "N/A" : amount.ToString());
                    valuesLine.Append(",");
                }

                csv.AppendLine(valuesLine.ToString().TrimEnd(','));
            }
        }

        private string FormatTerm(string term, SchoolFinancialType financialType)
        {
            return financialType == SchoolFinancialType.Academies ? term : term.Replace('/', '-');
        }
    }
}