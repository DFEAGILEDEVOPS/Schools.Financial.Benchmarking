using System;
using Newtonsoft.Json;
using SFB.Web.Domain.Models;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.Domain.Services;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Services
{
    public class FinancialCalculationsService : IFinancialCalculationsService
    {
        private readonly ILocalAuthoritiesService _localAuthoritiesService;

        public FinancialCalculationsService(ILocalAuthoritiesService localAuthoritiesService)
        {
            _localAuthoritiesService = localAuthoritiesService;
        }

        public void PopulateHistoricalChartsWithSchoolData(List<ChartViewModel> historicalCharts,
            List<SchoolFinancialDataModel> SchoolFinancialDataModels, string term, RevenueGroupType revgroup, UnitType unit,
            SchoolFinancialType schoolFinancialType)
        {
            foreach (var chart in historicalCharts)
            {
                BuildChart(SchoolFinancialDataModels, term, revgroup, unit, schoolFinancialType, chart);
                if (chart.SubCharts != null)
                {
                    foreach (var subChart in chart.SubCharts)
                    {
                        BuildChart(SchoolFinancialDataModels, term, revgroup, unit, schoolFinancialType, subChart);
                    }
                }
            }
        }

        private void BuildChart(List<SchoolFinancialDataModel> SchoolFinancialDataModels, string term, RevenueGroupType revgroup, UnitType unit,
            SchoolFinancialType schoolFinancialType, ChartViewModel chart)
        {
            var historicalChartData = new List<HistoricalChartData>();
            foreach (var schoolData in SchoolFinancialDataModels)
            {
                decimal? amount = null;
                decimal? rawAmount = null;
                switch (unit)
                {
                    case UnitType.AbsoluteMoney:
                    case UnitType.AbsoluteCount:
                        amount = schoolData.GetDecimal(chart.FieldName);
                        break;
                    case UnitType.PerTeacher:
                        rawAmount = schoolData.GetDecimal(chart.FieldName);
                        if (rawAmount == null)
                        {
                            break;
                        }
                        amount = (schoolData.TeacherCount == 0)
                            ? null
                            : (rawAmount / (decimal) schoolData.TeacherCount);
                        if (amount.HasValue)
                        {
                            amount = decimal.Round(amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                        }
                        break;
                    case UnitType.PerPupil:
                        rawAmount = schoolData.GetDecimal(chart.FieldName);
                        if (rawAmount == null)
                        {
                            break;
                        }
                        amount = (schoolData.PupilCount == 0)
                            ? null
                            : (rawAmount / (decimal) schoolData.PupilCount);
                        if (amount.HasValue)
                        {
                            amount = decimal.Round(amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                        }
                        break;
                    case UnitType.PercentageOfTotal:
                        decimal total = 0;
                        rawAmount = schoolData.GetDecimal(chart.FieldName);
                        if (rawAmount == null)
                        {
                            break;
                        }
                        switch (revgroup)
                        {
                            case RevenueGroupType.Expenditure:
                                total = schoolData.TotalExpenditure;
                                break;
                            case RevenueGroupType.Income:
                                total = schoolData.TotalIncome;
                                break;
                            case RevenueGroupType.Balance:
                                total = schoolData.InYearBalance;
                                break;
                        }

                        if (total == 0)
                        {
                            amount = 0;
                        }
                        else
                        {
                            amount = (total == 0) ? 0 : (rawAmount / total) * 100;
                            amount = decimal.Round(amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                        }
                        break;
                    case UnitType.NoOfPupilsPerMeasure:
                        rawAmount = schoolData.GetDecimal(chart.FieldName);
                        if (rawAmount == null || rawAmount == 0)
                        {
                            break;
                        }
                        amount = (schoolData.PupilCount == 0)
                            ? null
                            : ((decimal) schoolData.PupilCount / rawAmount);
                        amount = decimal.Round(amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                        break;

                    case UnitType.HeadcountPerFTE:
                        string fieldNameBase = chart.FieldName.Contains("FullTimeEquivalent")
                            ? chart.FieldName.Substring(0, chart.FieldName.Length - 18)
                            : chart.FieldName.Substring(0, chart.FieldName.Length - 9);
                        total = schoolData.GetDecimal(fieldNameBase + "Headcount").GetValueOrDefault();
                        rawAmount = schoolData.GetDecimal(fieldNameBase + "FullTimeEquivalent");
                        if (rawAmount == null)
                        {
                            break;
                        }
                        if (total == 0)
                        {
                            amount = 0;
                        }
                        else
                        {
                            amount = (total == 0) ? 0 : (total / rawAmount);
                            amount = decimal.Round(amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                        }
                        break;
                    case UnitType.FTERatioToTotalFTE:
                        total = schoolData.GetDecimal("TotalSchoolWorkforceFullTimeEquivalent").GetValueOrDefault();
                        rawAmount = schoolData.GetDecimal(chart.FieldName);
                        if (rawAmount == null)
                        {
                            break;
                        }
                        if (total == 0)
                        {
                            amount = 0;
                        }
                        else
                        {
                            amount = (total == 0) ? 0 : (rawAmount / total) * 100;
                            amount = decimal.Round(amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                        }
                        break;
                }

                historicalChartData.Add(new HistoricalChartData()
                {
                    Year = schoolData.Term,
                    Amount = amount,
                    TeacherCount = schoolData.TeacherCount,
                    PupilCount = schoolData.PupilCount,
                    Unit = unit.ToString()
                });
            }

            chart.HistoricalData = historicalChartData;
            chart.DataJson = GenerateJson(historicalChartData, schoolFinancialType);
            chart.LastYear = term;
            chart.LastYearBalance = historicalChartData.Find(d => d.Year == term).Amount;
            chart.ShowValue = unit;
        }

        private string GenerateJson(List<HistoricalChartData> historicalChartData,
            SchoolFinancialType schoolFinancialType)
        {
            var clonedHistoricalChartDataList = new List<HistoricalChartData>();
            foreach (var chartData in historicalChartData)
            {
                var clonedChartData = (HistoricalChartData) chartData.Clone();
                clonedChartData.Year = (schoolFinancialType == SchoolFinancialType.Academies)
                    ? clonedChartData.Year.Replace(" / ", "/")
                    : clonedChartData.Year.Replace(" / ", "-");
                clonedChartData.Year = clonedChartData.Year.Remove(5, 2);

                clonedHistoricalChartDataList.Add(clonedChartData);
            }
            return JsonConvert.SerializeObject(clonedHistoricalChartDataList);
        }

        public void PopulateBenchmarkChartsWithFinancialData(List<ChartViewModel> benchmarkCharts,
            List<SchoolFinancialDataModel> financialDataModels, IEnumerable<CompareEntityBase> bmEntities, string homeSchoolId,
            UnitType? unit, bool trimSchoolNames = false)
        {
            foreach (var chart in benchmarkCharts)
            {
                var chartUnit = unit.HasValue ? unit.GetValueOrDefault() : chart.ShowValue;
                if (!string.IsNullOrEmpty(chart.FieldName))
                {
                    var benchmarkChart = BuildBenchmarkChartModel(chart.FieldName, bmEntities, financialDataModels, chartUnit,
                        chart.RevenueGroup, homeSchoolId, trimSchoolNames);
                    chart.BenchmarkData = benchmarkChart.ChartData;
                    chart.DataJson = JsonConvert.SerializeObject(benchmarkChart.ChartData);
                    chart.BenchmarkSchoolIndex = benchmarkChart.BenchmarkSchoolIndex;
                    chart.IncompleteFinanceDataIndex = benchmarkChart.IncompleteFinanceDataIndex;
                    chart.IncompleteWorkforceDataIndex = benchmarkChart.IncompleteWorkforceDataIndex;
                    chart.ShowValue = unit.HasValue ? unit.GetValueOrDefault() : chart.ShowValue;
                }
                if (chart.TableColumns != null)
                {
                    foreach (var tableColumn in chart.TableColumns)
                    {
                        tableColumn.BenchmarkData =
                            BuildBenchmarkChartModel(tableColumn.FieldName,
                                bmEntities,
                                financialDataModels,
                                chartUnit,
                                chart.RevenueGroup,
                                homeSchoolId).ChartData;
                    }
                }
            }
        }
        
        private BenchmarkChartModel BuildBenchmarkChartModel(string fieldName, IEnumerable<CompareEntityBase> bmSchools,
            List<SchoolFinancialDataModel> financialDataModels, UnitType unit, RevenueGroupType revGroup, string homeSchoolId,
            bool trimSchoolNames = false)
        {
            var chartDataList = new List<BenchmarkChartData>();
            foreach (var school in bmSchools)
            {
                var dataModel = financialDataModels.First(f => f.Id.ToString() == school.Id);
                decimal total = 0;
                switch (revGroup)
                {
                    case RevenueGroupType.Expenditure:
                        total = dataModel.TotalExpenditure;
                        break;
                    case RevenueGroupType.Income:
                        total = dataModel.TotalIncome;
                        break;
                    case RevenueGroupType.Balance:
                        total = dataModel.InYearBalance;
                        break;
                }

                decimal? amountPerUnit = null;
                if (revGroup == RevenueGroupType.Workforce)
                {
                    amountPerUnit = CalculateWFAmount(dataModel, fieldName, unit);
                }
                else
                {
                    amountPerUnit = CalculateAmountPerUnit(dataModel, fieldName, unit, total);
                }

                chartDataList.Add(new BenchmarkChartData()
                {
                    School = trimSchoolNames ? $"{school.ShortName}#{school.Id}" : $"{school.Name}#{school.Id}",
                    Amount = amountPerUnit,
                    Urn = school.Id,
                    TeacherCount = dataModel.TeacherCount,
                    PupilCount = dataModel.PupilCount,
                    Term = dataModel.Term,
                    Type = school.Type,
                    La = _localAuthoritiesService.GetLaName(dataModel.LaNumber.ToString()),
                    IsCompleteYear = dataModel.PeriodCoveredByReturn >= 12,
                    IsWFDataPresent = dataModel.WorkforceDataPresent,
                    PartialYearsPresentInSubSchools = dataModel.PartialYearsPresentInSubSchools,
                    Unit = unit.ToString()
                });
            }

            var sortedChartData = chartDataList.OrderByDescending(a => a.Amount).ToList();
            var benchmarkSchoolIndex = !string.IsNullOrEmpty(homeSchoolId)
                ? sortedChartData.IndexOf(sortedChartData.Find(s => s.Urn.ToString() == homeSchoolId))
                : -1;


            var incompleteFinanceDataIndex = new List<int>();
            var incompleteWorkforceDataIndex = new List<int>();

            if (revGroup == RevenueGroupType.Workforce)
            {
                var incompleteWorkForce = sortedChartData.FindAll(s => !s.IsWFDataPresent);
                incompleteWorkForce.ForEach(i => incompleteWorkforceDataIndex.Add(sortedChartData.IndexOf(i)));
            }
            else
            {
                var incompleteFinances = sortedChartData.FindAll(s => !s.IsCompleteYear || s.PartialYearsPresentInSubSchools);
                incompleteFinances.ForEach(i => incompleteFinanceDataIndex.Add(sortedChartData.IndexOf(i)));
            }
            
            return new BenchmarkChartModel
            {
                ChartData = sortedChartData,
                BenchmarkSchoolIndex = benchmarkSchoolIndex,
                IncompleteFinanceDataIndex = incompleteFinanceDataIndex,
                IncompleteWorkforceDataIndex = incompleteWorkforceDataIndex,
            };
        }

        private decimal? CalculateWFAmount(SchoolFinancialDataModel schoolData, string fieldName, UnitType unit)
        {
            decimal? amount = null;
            decimal? rawAmount = null;
            switch (unit)
            {
                case UnitType.AbsoluteCount:
                    amount = schoolData.GetDecimal(fieldName);
                    break;
                case UnitType.NoOfPupilsPerMeasure:
                    rawAmount = schoolData.GetDecimal(fieldName);
                    if (rawAmount == null || rawAmount == 0)
                    {
                        break;
                    }
                    amount = (schoolData.PupilCount == 0) ? null : ((decimal) schoolData.PupilCount / rawAmount);
                    if (amount.HasValue)
                    {
                        amount = decimal.Round(amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                    }
                    break;

                case UnitType.HeadcountPerFTE:
                    string fieldNameBase = fieldName.Contains("FullTimeEquivalent")
                        ? fieldName.Substring(0, fieldName.Length - 18)
                        : fieldName.Substring(0, fieldName.Length - 9);
                    var total = schoolData.GetDecimal(fieldNameBase + "Headcount").GetValueOrDefault();
                    rawAmount = schoolData.GetDecimal(fieldNameBase + "FullTimeEquivalent");
                    if (rawAmount == null)
                    {
                        break;
                    }
                    if (total == 0)
                    {
                        amount = 0;
                    }
                    else
                    {
                        amount = (total == 0) ? 0 : (total / rawAmount);
                        amount = decimal.Round(amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                    }
                    break;
                case UnitType.FTERatioToTotalFTE:
                    total = schoolData.GetDecimal("TotalSchoolWorkforceFullTimeEquivalent").GetValueOrDefault();
                    rawAmount = schoolData.GetDecimal(fieldName);
                    if (rawAmount == null)
                    {
                        break;
                    }
                    if (total == 0)
                    {
                        amount = 0;
                    }
                    else
                    {
                        amount = (total == 0) ? 0 : (rawAmount / total) * 100;
                        amount = decimal.Round(amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                    }
                    break;
            }

            return amount;
        }

        private decimal? CalculateAmountPerUnit(SchoolFinancialDataModel dataModel, string fieldName, UnitType unit,
            decimal total)
        {
            var rawAmount = dataModel.GetDecimal(fieldName);
            var pupilCount = dataModel.PupilCount;
            var teacherCount = dataModel.TeacherCount;

            if (rawAmount == null)
            {
                return null;
            }

            switch (unit)
            {
                case UnitType.AbsoluteMoney:
                    return rawAmount;
                case UnitType.PerTeacher:
                    return (teacherCount == 0) ? null : (rawAmount / (decimal) teacherCount);
                case UnitType.PerPupil:
                    return (pupilCount == 0) ? null : (rawAmount / (decimal) pupilCount);
                case UnitType.PercentageOfTotal:
                    return (total == 0) ? 0 : ((rawAmount / total) * 100);
                default:
                    return rawAmount;
            }
        }
    }
}