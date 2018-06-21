using Newtonsoft.Json;
using System;
using System.Web.UI.WebControls;


namespace SFB.Web.Domain.Models
{
    public class HistoricalChartData : ICloneable
    {
        [JsonProperty(PropertyName = "year")]
        public string Year { get; set; }

        private decimal? _amountRaw;
        [JsonProperty(PropertyName = "amount")]
        public decimal? Amount
        {
            get
            {
                return _amountRaw;
            }
            set
            {
                _amountRaw = (value == null) ? value : Decimal.Round(value.GetValueOrDefault(), 2);
            }
        }

        [JsonProperty(PropertyName = "teacherCount")]
        public double? TeacherCount { get; set; }

        [JsonProperty(PropertyName = "pupilCount")]
        public double? PupilCount { get; set; }

        [JsonProperty(PropertyName = "unit")]
        public string Unit { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
