using System.Collections.Generic;
using Newtonsoft.Json;
using System;
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

    [Serializable]
    public class BenchmarkSchoolModel : CompareEntityBase, IEquatable<BenchmarkSchoolModel>
    {
        [JsonIgnore]
        public override string Id => Urn;

        [JsonProperty(PropertyName = "U")]
        public string Urn { get; set; }

        [JsonProperty(PropertyName = "N")]
        public override string Name { get; set; }

        [JsonProperty(PropertyName = "FT")]
        public string EstabType;

        [JsonProperty(PropertyName = "T")]
        public override string Type { get; set; }

        [JsonProperty(PropertyName = "P")]
        public decimal? ProgressScore { get; set; }

        [JsonIgnore]
        public int La;

        [JsonIgnore]
        public string LaName;

        [JsonIgnore]
        public int Estab;

        [JsonIgnore]
        public string Address;

        [JsonIgnore]
        public string Phase;

        [JsonIgnore]
        public bool IsReturnsComplete;

        [JsonIgnore]
        public bool WorkforceDataPresent;

        [JsonIgnore]
        public float NumberOfPupils;

        [JsonIgnore]
        public override string ShortName => !string.IsNullOrEmpty(Name) && Name.Length >= 20 ? $"{Name.Substring(0, 17)}..." : Name;

        public bool Equals(BenchmarkSchoolModel other)
        {
            return this.Urn == other.Urn;
        }
    }
}