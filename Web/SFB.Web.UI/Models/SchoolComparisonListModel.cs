using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace SFB.Web.UI.Models
{
    public class SchoolComparisonListModel
    {
        public SchoolComparisonListModel()
        {
            BenchmarkSchools = new List<BenchmarkSchoolModel>();
        }
        
        [JsonProperty(PropertyName = "HSU")]
        public string HomeSchoolUrn { get; set; }

        [JsonProperty(PropertyName = "HSFT")]
        public string HomeSchoolFinancialType { get; set; }

        [JsonProperty(PropertyName = "HSN")]
        public string HomeSchoolName { get; set; }

        [JsonProperty(PropertyName = "HST")]
        public string HomeSchoolType { get; set; }

        [JsonProperty(PropertyName = "BS")]
        public List<BenchmarkSchoolModel> BenchmarkSchools { get; set; }

        [JsonIgnore]
        public bool HasIncompleteFinancialData {
            get {
                return BenchmarkSchools.Any(s => !s.IsReturnsComplete);
            }
        }

        [JsonIgnore]
        public bool HasIncompleteWorkforceData
        {
            get
            {
                return BenchmarkSchools.Any(s => !s.WorkforceDataPresent);
            }
        }
    }
}