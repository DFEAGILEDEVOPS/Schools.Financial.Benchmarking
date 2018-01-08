using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using SFB.Web.Domain.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    [Serializable]
    public class TrustComparisonViewModel
    {
        [JsonProperty(PropertyName = "DTMN")]
        public string DefaultTrustMatNo { get; set; }

        [JsonProperty(PropertyName = "DTN")]
        public string DefaultTrustName { get; set; }

        [JsonProperty(PropertyName = "T")]
        public List<TrustToCompareViewModel> Trusts { get; set; }

        public TrustComparisonViewModel(string defaultTrustMatNo, string defaultTrustName)
        {
            this.DefaultTrustMatNo = defaultTrustMatNo;
            this.DefaultTrustName = defaultTrustName;
            this.Trusts = new List<TrustToCompareViewModel>();
        }
    }

    [Serializable]
    public class TrustToCompareViewModel : CompareEntityBase, IEquatable<TrustToCompareViewModel>
    {
        [JsonProperty(PropertyName = "N")]
        public string MatNo { get; }

        [JsonProperty(PropertyName = "NA")]
        public string MatName { get; set; }

        [JsonIgnore]
        public override string Id => MatNo;

        [JsonIgnore]
        public override string ShortName => !string.IsNullOrEmpty(Name) && Name.Length >= 20 ? $"{Name.Substring(0, 17)}..." : Name;

        [JsonIgnore]
        public override string Name
        {
            get { return MatName; }
            set { MatName = value; }
        }

        [JsonIgnore]
        public override string Type { get; set; }

        public TrustToCompareViewModel(string matNo, string matName = null)
        {
            this.MatNo = matNo;
            this.MatName = matName;
            this.Type = SchoolFinancialType.Academies.ToString();
        }

        public bool Equals(TrustToCompareViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(MatNo, other.MatNo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TrustToCompareViewModel) obj);
        }

        public override int GetHashCode()
        {
            return (MatNo != null ? MatNo.GetHashCode() : 0);
        }
    }
}