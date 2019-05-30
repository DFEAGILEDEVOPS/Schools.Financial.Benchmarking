using Newtonsoft.Json;
using System;

namespace SFB.Web.UI.Models
{
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
        
        public bool Equals(BenchmarkSchoolModel other)
        {
            return this.Urn == other.Urn;
        }
    }
}