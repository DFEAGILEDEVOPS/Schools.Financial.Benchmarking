﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SFB.Web.Common;

namespace SFB.Web.UI.Models
{
    [Serializable]
    public class TrustComparisonListModel
    {
        [JsonProperty(PropertyName = "DTMN")]
        public string DefaultTrustMatNo { get; set; }

        [JsonProperty(PropertyName = "DTCN")]
        public int DefaultTrustCompanyNo { get; set; }

        [JsonProperty(PropertyName = "DTN")]
        public string DefaultTrustName { get; set; }

        [JsonProperty(PropertyName = "T")]
        public List<BenchmarkTrustModel> Trusts { get; set; }

        public TrustComparisonListModel(int defaultTrustCompanyNo, string defaultTrustName)
        {
            this.DefaultTrustCompanyNo = defaultTrustCompanyNo;
            this.DefaultTrustName = defaultTrustName;
            this.Trusts = new List<BenchmarkTrustModel>();
        }
    }

    [Serializable]
    public class BenchmarkTrustModel : CompareEntityBase, IEquatable<BenchmarkTrustModel>
    {
        [JsonProperty(PropertyName = "N")]
        public string MatNo { get; }

        [JsonProperty(PropertyName = "CN")]
        public int CompanyNo { get; set; }

        [JsonProperty(PropertyName = "NA")]
        public string MatName { get; set; }

        [JsonIgnore]
        public override string Id => CompanyNo.ToString();

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

        public BenchmarkTrustModel(int companyNo, string matName = null, string matNo = null)
        {
            this.CompanyNo = companyNo;
            this.MatNo = matNo;
            this.MatName = matName;
            this.Type = EstablishmentType.MAT.ToString();
        }

        public bool Equals(BenchmarkTrustModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(CompanyNo, other.CompanyNo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BenchmarkTrustModel) obj);
        }

        public override int GetHashCode()
        {
            return CompanyNo.GetHashCode();
        }
    }
}