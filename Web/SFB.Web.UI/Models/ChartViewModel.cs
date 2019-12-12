using System;
using System.Collections.Generic;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public class ChartViewModel : ViewModelBase, ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FieldName { get; set; }
        public ChartSchoolType ChartSchoolType { get; set;}
        public ChartType ChartType { get; set;}
        public ChartGroupType ChartGroup { get; set; }
        public ChartGroupType? DrillInto { get; set; }

        public string ChartGroupName {
            get
            {
                switch (ChartGroup)
                {
                    case ChartGroupType.GrantFunding:
                        return ChartGroupNames.GRANT_FUNDING;
                    case ChartGroupType.SelfGenerated:
                        return ChartGroupNames.SELF_GENERATED;
                    case ChartGroupType.Staff:
                        return ChartGroupNames.STAFF;
                    case ChartGroupType.Premises:
                        return ChartGroupNames.PREMISES;
                    case ChartGroupType.Occupation:
                        return ChartGroupNames.OCCUPATION;
                    case ChartGroupType.SuppliesAndServices:
                        return ChartGroupNames.SUPPLIES_AND_SERVICES;
                    case ChartGroupType.CostOfFinance:
                        return ChartGroupNames.COST_OF_FINANCE;
                    case ChartGroupType.Community:
                        return ChartGroupNames.COMMUNITY;
                    case ChartGroupType.Other:
                        return ChartGroupNames.OTHER;
                    case ChartGroupType.SpecialFacilities:
                        return ChartGroupNames.SPECIAL_FACILITIES;
                    case ChartGroupType.InYearBalance:
                        return ChartGroupNames.IN_YEAR_BALANCE;
                    case ChartGroupType.TotalIncome:
                        return ChartGroupNames.TOTAL_INCOME;
                    case ChartGroupType.TotalExpenditure:
                        return ChartGroupNames.TOTAL_EXPENDITURE;
                    default:
                        return ChartGroupNames.OTHER;
                 }
            }
        }

        public TabType TabType { get; set; }

        public string TabName
        {
            get
            {
                switch (TabType)
                {
                    case TabType.Expenditure:
                        return TabNames.EXPENDITURE;
                    case TabType.Income:
                        return TabNames.INCOME;
                    case TabType.Balance:
                        return TabNames.BALANCE;
                    default:
                        return null;
                }
            }
        }
        
        public UnitType ShowValue { get; set; }

        public string MoreInfo
        {
            get
            {
                if (string.IsNullOrEmpty(_moreInfo))
                {
                    return _moreInfo;
                }
                return $"{_moreInfo}<p>{MoreInfoText.INTERPRET_CHARTS}</p>";
            }
            set { _moreInfo = value; }
        }

        public List<DataTableColumnViewModel> TableColumns { get; set; }
        public List<ChartViewModel> SubCharts { get; set; }

        public List<HistoricalChartData> HistoricalData { get; set; }
        public List<BenchmarkChartData> BenchmarkData { get; set; }

        public string DataJson { get; set; }

        public string LastYear { get; set; }
        public decimal? LastYearBalance { get; set; }

        public int BenchmarkSchoolIndex { get; set; }

        private string _moreInfo;

        public List<int> IncompleteFinanceDataIndex { get; set; }

        public List<int> IncompleteWorkforceDataIndex { get; set; }

        public bool Downloadable { get; set; }

        public string HelpTooltip { get; set; }

        public string DealsForSchoolsMessage { get; set; }

        public string DealsForSchoolsCategory { get; set; }

        public object Clone()
        {
            var copy = new ChartViewModel
            {
                Name = this.Name,
                FieldName = this.FieldName,
                ChartSchoolType = this.ChartSchoolType,
                ChartType = this.ChartType,
                ChartGroup = this.ChartGroup,
                TabType = this.TabType,
                ShowValue = this.ShowValue,
                MoreInfo = this.MoreInfo,
                LastYear = this.LastYear,
                Downloadable = this.Downloadable,
                HelpTooltip = this.HelpTooltip,
                TableColumns = new List<DataTableColumnViewModel>(),                
            };

            if (TableColumns != null)
            {
                TableColumns.ForEach(tc => copy.TableColumns.Add((DataTableColumnViewModel) tc.Clone()));
            }

            return copy;
        }
    }
}