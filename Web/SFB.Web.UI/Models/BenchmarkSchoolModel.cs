using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Models;
using System;

namespace SFB.Web.UI.Models
{
    [Serializable]
    public class BenchmarkSchoolModel : CompareEntityBase, IEquatable<BenchmarkSchoolModel>
    {
        public BenchmarkSchoolModel() {}

        public BenchmarkSchoolModel(SchoolViewModel schoolViewModel)
        {
            Name = schoolViewModel.Name;
            Urn = schoolViewModel.Id.ToString();
            Type = schoolViewModel.Type;
            EstabType = schoolViewModel.EstablishmentType.ToString();
            ProgressScore = schoolViewModel.ProgressScore;
        }

        public BenchmarkSchoolModel(FederationViewModel federationViewModel)
        {
            Name = federationViewModel.Name;
            Urn = federationViewModel.Id.ToString();
            Type = federationViewModel.Type;
            EstabType = federationViewModel.EstablishmentType.ToString();
        }

        public BenchmarkSchoolModel(EdubaseDataObject schoolContextData)
        {
            if (schoolContextData.IsFederation)
            {
                Name = schoolContextData.FederationName;
                Type = "Federation";
                EstabType = "Maintained";
                Urn = schoolContextData.URN.ToString();
            }
            else
            {
                Name = schoolContextData.EstablishmentName;
                Type = schoolContextData.TypeOfEstablishment;
                EstabType = schoolContextData.FinanceType;
                Urn = schoolContextData.URN.ToString();
            }
        }        
        
        public BenchmarkSchoolModel(SchoolTrustFinancialDataObject schoolFinanceData)
        {
            Name = schoolFinanceData.SchoolName ?? schoolFinanceData.FederationName;
            Type = schoolFinanceData.Type;
            EstabType = schoolFinanceData.FinanceType;
            Urn = schoolFinanceData.URN.ToString();
        }

        public BenchmarkSchoolModel(SchoolComparisonListModel schoolComparisonList)
        {
            Urn = schoolComparisonList.HomeSchoolUrn;
            Name = schoolComparisonList.HomeSchoolName;
            Type = schoolComparisonList.HomeSchoolType;
            EstabType = schoolComparisonList.HomeSchoolFinancialType;
        }

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