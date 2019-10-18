using System;
using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using SFB.Web.ApplicationCore;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers.Constants;

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
            List<FinancialDataModel> SchoolFinancialDataModels, string term, RevenueGroupType revgroup, UnitType unit,
            EstablishmentType estabType)
        {
            foreach (var chart in historicalCharts)
            {
                BuildChart(SchoolFinancialDataModels, term, revgroup, unit, estabType, chart);
                if (chart.SubCharts != null)
                {
                    foreach (var subChart in chart.SubCharts)
                    {
                        BuildChart(SchoolFinancialDataModels, term, revgroup, unit, estabType, subChart);
                    }
                }
            }
        }

        private Decimal? GetFinancialDataValueForChartField(string chartFieldName, SchoolTrustFinancialDataObject dataObject)
        {
            if (dataObject == null)
            {
                return null;
            }

            return (Decimal?)typeof(SchoolTrustFinancialDataObject).GetProperties()
                    .First(p => (p.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First() as JsonPropertyAttribute).PropertyName == chartFieldName)
                    .GetValue(dataObject);
        }

        private void BuildChart(List<FinancialDataModel> SchoolFinancialDataModels, string term, RevenueGroupType revgroup, UnitType unit,
            EstablishmentType estabType, ChartViewModel chart)
        {
            var historicalChartData = new List<HistoricalChartData>();
            foreach (var schoolData in SchoolFinancialDataModels)
            {
                decimal? amount = null;
                decimal? rawAmount = null;
                try
                {
                    switch (unit)
                    {
                        case UnitType.AbsoluteMoney:
                        case UnitType.AbsoluteCount:
                        amount = GetFinancialDataValueForChartField(chart.FieldName, schoolData.FinancialDataObjectModel);
                            break;
                        case UnitType.PerTeacher:
                        rawAmount = GetFinancialDataValueForChartField(chart.FieldName, schoolData.FinancialDataObjectModel);
                            if (rawAmount == null)
                            {
                                break;
                            }
                            amount = (schoolData.TeacherCount == 0)
                                ? null
                                : (rawAmount / (decimal)schoolData.TeacherCount);
                            if (amount.HasValue)
                            {
                                amount = decimal.Round(amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                            }
                            break;
                        case UnitType.PerPupil:
                        rawAmount = GetFinancialDataValueForChartField(chart.FieldName, schoolData.FinancialDataObjectModel);
                            if (rawAmount == null)
                            {
                                break;
                            }
                            amount = (schoolData.PupilCount == 0)
                                ? null
                                : (rawAmount / (decimal)schoolData.PupilCount);
                            if (amount.HasValue)
                            {
                                amount = decimal.Round(amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                            }
                            break;
                        case UnitType.PercentageOfTotal:
                        decimal? total = 0;
                        rawAmount = GetFinancialDataValueForChartField(chart.FieldName, schoolData.FinancialDataObjectModel);
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
                        rawAmount = GetFinancialDataValueForChartField(chart.FieldName, schoolData.FinancialDataObjectModel);
                            if (rawAmount == null || rawAmount == 0)
                            {
                                break;
                            }
                            amount = (schoolData.PupilCount == 0)
                                ? null
                                : ((decimal)schoolData.PupilCount / rawAmount);
                            amount = decimal.Round(amount.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                            break;

                        case UnitType.HeadcountPerFTE:
                            string fieldNameBase = chart.FieldName.Contains("FullTimeEquivalent")
                                ? chart.FieldName.Substring(0, chart.FieldName.Length - 18)
                            : chart.FieldName.Substring(0, chart.FieldName.Length - 9);                        
                        total = GetFinancialDataValueForChartField(fieldNameBase + "Headcount", schoolData.FinancialDataObjectModel);                                                
                        rawAmount = GetFinancialDataValueForChartField(fieldNameBase + "FullTimeEquivalent", schoolData.FinancialDataObjectModel);
                            if (total == null || rawAmount == null || rawAmount == 0)
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
                        total = GetFinancialDataValueForChartField(SchoolTrustFinanceDataFieldNames.WORKFORCE_TOTAL, schoolData.FinancialDataObjectModel);                        
                        rawAmount = GetFinancialDataValueForChartField(chart.FieldName, schoolData.FinancialDataObjectModel);
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
                }catch(DivideByZeroException)
                {
                    amount = null;
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
            chart.DataJson = GenerateJson(historicalChartData, estabType);
            chart.LastYear = term;
            chart.LastYearBalance = historicalChartData.Find(d => d.Year == term).Amount;
            chart.ShowValue = unit;
        }

        private string GenerateJson(List<HistoricalChartData> historicalChartData,
            EstablishmentType estabType)
        {
            var clonedHistoricalChartDataList = new List<HistoricalChartData>();
            foreach (var chartData in historicalChartData)
            {
                var clonedChartData = (HistoricalChartData) chartData.Clone();
                clonedChartData.Year = (estabType == EstablishmentType.Academies || estabType == EstablishmentType.MAT)
                    ? clonedChartData.Year.Replace(" / ", "/")
                    : clonedChartData.Year.Replace(" / ", "-");
                clonedChartData.Year = clonedChartData.Year.Remove(5, 2);

                clonedHistoricalChartDataList.Add(clonedChartData);
            }
            return JsonConvert.SerializeObject(clonedHistoricalChartDataList);
        }

        public void PopulateBenchmarkChartsWithFinancialData(List<ChartViewModel> benchmarkCharts,
            List<FinancialDataModel> financialDataModels, IEnumerable<CompareEntityBase> bmEntities, string homeSchoolId,
            UnitType? unit)
        {
            foreach (var chart in benchmarkCharts)
            {
                var chartUnit = unit.HasValue ? unit.GetValueOrDefault() : chart.ShowValue;
                if (!string.IsNullOrEmpty(chart.FieldName))
                {
                    var benchmarkChart = BuildBenchmarkChartModel(chart.FieldName, bmEntities, financialDataModels, chartUnit,
                        chart.RevenueGroup, homeSchoolId);
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
            List<FinancialDataModel> financialDataModels, UnitType unit, RevenueGroupType revGroup, string homeSchoolId)
        {
            var chartDataList = new List<BenchmarkChartData>();
            foreach (var school in bmSchools)
            {
                var dataModel = financialDataModels.First(f => f.Id.ToString() == school.Id);
                decimal? total = 0;
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
                    School = $"{school.Name}",
                    Amount = amountPerUnit,
                    Urn = school.Id,
                    TeacherCount = dataModel.TeacherCount,
                    PupilCount = dataModel.PupilCount,
                    Term = dataModel.Term,
                    Type = school.Type,
                    La = _localAuthoritiesService.GetLaName(dataModel.LaNumber.ToString()),
                    IsCompleteYear = dataModel.IsReturnsComplete,
                    IsWFDataPresent = dataModel.WorkforceDataPresent,
                    PartialYearsPresentInSubSchools = dataModel.PartialYearsPresentInSubSchools,
                    Unit = unit.ToString(),
                    ProgressScore = school is BenchmarkSchoolModel ? (school as BenchmarkSchoolModel).ProgressScore : null
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

        private decimal? CalculateWFAmount(FinancialDataModel schoolData, string fieldName, UnitType unit)
        {
            decimal? amount = null;
            decimal? rawAmount = null;
            switch (unit)
            {
                case UnitType.AbsoluteCount:                    
                    amount = GetFinancialDataValueForChartField(fieldName, schoolData.FinancialDataObjectModel);
                    break;
                case UnitType.NoOfPupilsPerMeasure:                    
                    rawAmount = GetFinancialDataValueForChartField(fieldName, schoolData.FinancialDataObjectModel);
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
                    var total = GetFinancialDataValueForChartField(fieldNameBase + "Headcount", schoolData.FinancialDataObjectModel);
                    rawAmount = GetFinancialDataValueForChartField(fieldNameBase + "FullTimeEquivalent", schoolData.FinancialDataObjectModel);
                    if (rawAmount == null || rawAmount == 0)
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
                    total = GetFinancialDataValueForChartField(SchoolTrustFinanceDataFieldNames.WORKFORCE_TOTAL, schoolData.FinancialDataObjectModel);
                    rawAmount = GetFinancialDataValueForChartField(fieldName, schoolData.FinancialDataObjectModel);
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

        private decimal? CalculateAmountPerUnit(FinancialDataModel dataModel, string fieldName, UnitType unit,
            decimal? total)
        {            
            var rawAmount = GetFinancialDataValueForChartField(fieldName, dataModel.FinancialDataObjectModel);
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