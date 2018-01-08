using System.Collections.Generic;
using Newtonsoft.Json;
using SFB.Web.Domain.Helpers.Enums;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public class ComparisonListModel
    {
        public ComparisonListModel()
        {
            BenchmarkSchools = new List<BenchmarkSchoolViewModel>();
        }
        
        [JsonProperty(PropertyName = "HSU")]
        public string HomeSchoolUrn { get; set; }

        [JsonProperty(PropertyName = "HSFT")]
        public string HomeSchoolFinancialType { get; set; }

        [JsonProperty(PropertyName = "HSN")]
        public string HomeSchoolName { get; set; }

        [JsonProperty(PropertyName = "BS")]
        public List<BenchmarkSchoolViewModel> BenchmarkSchools { get; set; }

        [JsonIgnore]
        public string HomeSchoolType { get; set; }
    }

    public class ComparisonListModelOld
    {
        public ComparisonListModelOld()
        {
            BenchmarkSchools = new List<BenchmarkSchoolViewModelOld>();
            ListType = ComparisonListType.UserCustomized;
        }

        public ComparisonListType ListType { get; set; }
        public string HomeSchoolUrn { get; set; }
        public string HomeSchoolType { get; set; }
        public string HomeSchoolFinancialType { get; set; }
        public string HomeSchoolName { get; set; }
        public List<BenchmarkSchoolViewModelOld> BenchmarkSchools { get; set; }
    }
}