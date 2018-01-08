using System;

namespace SFB.Web.UI.Models
{
    public class BenchmarkCriteriaRangeVM
    {
        public string Question { get; set; }
        public string HomeSchoolValue { get; set; }
        public string ElementName { get; set; }
        public string MinElementName { get { return $"Min{ElementName}"; } }
        public double? MinValue { get; set; }
        public string MaxElementName { get { return $"Max{ElementName}"; } }
        public double? MaxValue { get; set; }
        public int? MinLimit { get; set; }
        public int? MaxLimit { get; set; }
        public string Format { get; set; }

        public BenchmarkCriteriaRangeVM(string question, string homeSchoolValue, string elementName, double? minValue,
            double? maxValue, int? minLimit = 0, int? maxLimit = 100000, string format = null)
        {
            Question = question;
            HomeSchoolValue = homeSchoolValue;
            ElementName = elementName;
            MinValue = minValue;
            MaxValue = maxValue;
            MinLimit = minLimit;
            MaxLimit = maxLimit;
            Format = format;
        }
    }
}