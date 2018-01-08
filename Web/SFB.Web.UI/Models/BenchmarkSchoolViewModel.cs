using System;
using Newtonsoft.Json;
using SFB.Web.Domain.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    [Serializable]
    public class BenchmarkSchoolViewModel : CompareEntityBase, IEquatable<BenchmarkSchoolViewModel>
    {
        [JsonIgnore]
        public override string Id => Urn;

        [JsonProperty(PropertyName = "U")]
        public string Urn { get; set; }

        [JsonProperty(PropertyName = "N")]
        public override string Name { get; set; }

        [JsonProperty(PropertyName = "FT")]
        public string FinancialType;

        [JsonIgnore]
        public override string ShortName => !string.IsNullOrEmpty(Name) && Name.Length >= 20 ? $"{Name.Substring(0, 17)}..." : Name;

        [JsonIgnore]
        public override string Type { get; set; }

        [JsonIgnore]
        public int La;

        [JsonIgnore]
        public int Estab;

        [JsonIgnore]
        public string Address;

        [JsonIgnore]
        public string Phase;

        public bool Equals(BenchmarkSchoolViewModel other)
        {
            return this.Urn == other.Urn;
        }
    }

    [Serializable]
    public class BenchmarkSchoolViewModelOld : IEquatable<BenchmarkSchoolViewModelOld>
    {
        public string Urn;

        public string Name;

        public string ShortName => !string.IsNullOrEmpty(Name) && Name.Length >= 20 ? $"{Name.Substring(0, 17)}..." : Name;

        [NonSerialized]
        public int La;

        [NonSerialized]
        public int Estab;

        [NonSerialized]
        public string Address;

        [NonSerialized]
        public string Phase;

        public string Type;

        public string FinancialType;

        public bool Equals(BenchmarkSchoolViewModelOld other)
        {
            return this.Urn == other.Urn;
        }
    }

}