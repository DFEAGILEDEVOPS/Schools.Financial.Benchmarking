using Newtonsoft.Json;

namespace SFB.Web.Domain.Models
{
    public class BenchmarkChartData
    {
        [JsonProperty(PropertyName = "school")]
        public string School { get; set; }

        [JsonProperty(PropertyName = "urn")]
        public string Urn { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public decimal? Amount { get; set; }

        [JsonProperty(PropertyName = "teacherCount")]
        public decimal? TeacherCount { get; set; }

        [JsonProperty(PropertyName = "pupilCount")]
        public decimal? PupilCount { get; set; }

        [JsonProperty(PropertyName = "iscompleteyear")]
        public bool IsCompleteYear { get; set; }

        [JsonProperty(PropertyName = "isworkforcepresent")]
        public bool IsWFDataPresent { get; set; }

        [JsonProperty(PropertyName = "partialyearspresent")]
        public bool PartialYearsPresentInSubSchools { get; set; }

        [JsonProperty(PropertyName = "term")]
        public string Term { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "unit")]
        public string Unit { get; set; }

        [JsonProperty(PropertyName = "la")]
        public string La { get; set; }
    }
}
