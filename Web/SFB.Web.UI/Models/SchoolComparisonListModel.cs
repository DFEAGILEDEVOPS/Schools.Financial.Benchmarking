using System.Collections.Generic;
using Newtonsoft.Json;
using System;

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

        [JsonProperty(PropertyName = "BS")]
        public List<BenchmarkSchoolModel> BenchmarkSchools { get; set; }

        [JsonIgnore]
        public string HomeSchoolType { get; set; }
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

        [JsonIgnore]
        public override string ShortName => !string.IsNullOrEmpty(Name) && Name.Length >= 20 ? $"{Name.Substring(0, 17)}..." : Name;

        [JsonProperty(PropertyName = "T")]
        public override string Type { get; set; }

        [JsonIgnore]
        public int La;

        [JsonIgnore]
        public int Estab;

        [JsonIgnore]
        public string Address;

        [JsonIgnore]
        public string Phase;

        public bool Equals(BenchmarkSchoolModel other)
        {
            return this.Urn == other.Urn;
        }
    }
}