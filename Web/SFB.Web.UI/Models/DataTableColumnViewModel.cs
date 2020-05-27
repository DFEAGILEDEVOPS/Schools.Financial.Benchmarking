using System;
using System.Collections.Generic;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public class DataTableColumnViewModel : ICloneable
    {
        public string Name { get; set; }
        public string FieldName { get; set; }
        public ChartSchoolType ChartSchoolType { get; set; }
        public string MoreInfo { get; set; }
        public List<BenchmarkChartData> BenchmarkData { get; set; }
        public string HelpTooltip { get; set; }

        public object Clone()
        {
            return new DataTableColumnViewModel()
            {
                Name = this.Name,
                FieldName = this.FieldName,
                ChartSchoolType = this.ChartSchoolType,
                MoreInfo = this.MoreInfo,
                HelpTooltip = this.HelpTooltip
            };
        }
    }
}