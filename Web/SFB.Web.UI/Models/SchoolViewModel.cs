using System;
using System.Linq;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.ApplicationCore.Entities;
using System.Globalization;
using SFB.Web.ApplicationCore.Helpers.Enums;
using System.Collections.Generic;
using SFB.Web.ApplicationCore.Models;

namespace SFB.Web.UI.Models
{
    public class SchoolViewModel : EstablishmentViewModelBase
    {
        public SchoolComparisonListModel ManualComparisonList { get; set; }

        public SchoolViewModel(EdubaseDataObject contextDataModel)
        {
            this.ContextData = contextDataModel;
        }

        public SchoolViewModel(EdubaseDataObject schoolContextDataModel, SchoolComparisonListModel comparisonList)
        {
            this.ContextData = schoolContextDataModel;
            base.ComparisonList = comparisonList;
        }

        public SchoolViewModel(EdubaseDataObject schoolContextDataModel, SchoolComparisonListModel comparisonList, SchoolComparisonListModel manualComparisonList)
        {
            this.ContextData = schoolContextDataModel;
            base.ComparisonList = comparisonList;
            this.ManualComparisonList = manualComparisonList;
        }

        public LocationDataObject GetLocation()
        {
            return ContextData.Location;
        }

        public bool IsInComparisonList
        {
            get
            {
                if (ComparisonList == null)
                {
                    return false;
                }
                return base.ComparisonList.BenchmarkSchools.Any(s => s.Urn == ContextData.URN.ToString());
            }
        }

        public bool IsDefaultBenchmark => base.ComparisonList.HomeSchoolUrn == ContextData.URN.ToString();

        public int Id => ContextData.URN;

        public string LaEstab => $"{ContextData.LACode} {ContextData.EstablishmentNumber}";

        public override string Name
        {
            get
            {
                return ContextData.EstablishmentName;
            }

            set { }
        }

        public int La => ContextData.LACode;

        public string LaName { get; set; }

        public int Estab => ContextData.EstablishmentNumber;

        public string OverallPhase => ContextData.OverallPhase;

        public string Phase => ContextData.PhaseOfEducation;

        public string SchoolWebSite
        {
            get {
                var url = ContextData.SchoolWebsite;
                if (!string.IsNullOrEmpty(url) && !url.ToLower().StartsWith("http"))
                {
                    url = "http://" + url;
                }

                return url;
            }
        }

        public string AgeRange => ContextData.StatutoryLowAge == null ? null : $"{ContextData.StatutoryLowAge} to {ContextData.StatutoryHighAge}";

        public string HeadTeachFullName => $"{ContextData.HeadFirstName} {ContextData.HeadLastName}";
        public string TrustName => ContextData.Trusts;

        public string SponsorName => ContextData.SponsorName;

        public string TrustNameInLatestFinance => LatestYearFinancialData?.TrustName;

        public int? CompanyNo => ContextData.CompanyNumber;
        
        public int? CompanyNoInLatestFinance => LatestYearFinancialData?.CompanyNo;
        
        public int? UID => ContextData.UID;

        public string PhoneNumber => ContextData.TelephoneNum;

        public string OfstedRating => ContextData.OfstedRating;

        public string OfstedInspectionDate
        {
            get
            {
                try
                {
                    return DateTime.Parse(ContextData.OfstedLastInsp, CultureInfo.CurrentCulture, DateTimeStyles.None).ToLongDateString();
                }
                catch
                {
                    return null;
                }
            }
        }

        public string OpenDate
        {
            get
            {
                try
                {
                    return DateTime.Parse(ContextData.OpenDate, CultureInfo.CurrentCulture, DateTimeStyles.None).ToLongDateString();
                }
                catch
                {
                    return null;
                }
            }
        }

        public string CloseDate
        {
            get
            {
                try
                {
                    return DateTime.Parse(ContextData.CloseDate, CultureInfo.CurrentCulture, DateTimeStyles.None).ToLongDateString();
                }
                catch
                {
                    return null;
                }
            }
        }

        public string OfstedRatingText
        {
            get
            {
                switch (OfstedRating)
                {
                    case OfstedRatings.Rating.GOOD:
                        return OfstedRatings.Description.GOOD;
                    case OfstedRatings.Rating.OUTSTANDING:
                        return OfstedRatings.Description.OUTSTANDING;
                    case OfstedRatings.Rating.INADEQUATE:
                        return OfstedRatings.Description.INADEQUATE;
                    case OfstedRatings.Rating.REQUIRES_IMPROVEMENT:
                        return OfstedRatings.Description.REQUIRES_IMPROVEMENT;
                    default:
                        return OfstedRatings.Description.NOT_RATED;
                }
            }
        }

        public float TotalPupils => ContextData.NumberOfPupils.GetValueOrDefault();

        public string IsPost16 => ContextData.OfficialSixthForm == "Has a sixth form" ? "Yes" : "No";

        public string HasNursery => ContextData.NurseryProvision == "Has Nursery Classes" ? "Yes" : "No";

        public string Address => ContextData.Address;

        public override string Type => ContextData.TypeOfEstablishment;

        public override EstablishmentType EstablishmentType => (EstablishmentType)Enum.Parse(typeof(EstablishmentType), ContextData.FinanceType);

        public bool IsSAT => ContextData.MatSat == "SAT";
        
        public bool IsMAT => ContextData.MatSat == "MAT";

        public bool IsSATinLatestFinance => LatestYearFinancialData.IsSAT;

        public bool IsMATinLatestFinance => LatestYearFinancialData.IsMAT;

        public decimal? FSM => LatestYearFinancialData?.PercentageOfEligibleFreeSchoolMeals;

        public decimal? SEN => LatestYearFinancialData?.PercentageOfPupilsWithSen;

        public string UrbanRural => LatestYearFinancialData?.UrbanRural;

        public decimal? ExpenditurePerPupil => LatestYearFinancialData?.PerPupilTotalExpenditure;

        public string PhaseInFinancialSubmission => LatestYearFinancialData?.SchoolPhase;

        public string OverallPhaseInFinancialSubmission => LatestYearFinancialData?.SchoolOverallPhase;

        public List<SENCriteriaModel> TopSenCharacteristics => LatestYearFinancialData?.TopSenCharacteristics;

        public List<SENCriteriaModel> SenCharacteristics => LatestYearFinancialData?.SenCharacteristics;

        public BicProgressScoreType BicProgressScoreType
        {
            get {
                switch (OverallPhaseInFinancialSubmission)
                {
                    case "Primary":
                        return BicProgressScoreType.KS2;
                    case "Secondary":
                    case "All-through":
                    default:
                        return BicProgressScoreType.P8;
                }
            }
        }

        public decimal? ProgressScore => LatestYearFinancialData?.ProgressScore;

        public decimal? KS2ProgressScore => LatestYearFinancialData?.Ks2Progress;

        public decimal? P8ProgressScore => LatestYearFinancialData.P8Mea;

        public decimal? P8Banding => LatestYearFinancialData?.P8Banding;

        //public bool IsFederation => ContextData.IsFederation;
        //public int? FederationUID => ContextData.FederationUid;
        //public int[] FederationMembers => ContextData.FederationMembers;
        public int? FederationsCode => ContextData.FederationsCode;
        public string FederationName => ContextData.FederationName;        
        public string Federation => ContextData.Federation;        
        public bool IsPartOfFederation => FederationsCode.HasValue;

        public bool HasCoordinates
        {
            get
            {
                try
                {
                    var lat = GetCoordinates(1);
                    var lng = GetCoordinates(0);

                    if (lat == string.Empty || lng == string.Empty)
                    {
                        return false;
                    }

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool InsideSearchArea { get; set; }

        public string Lat => GetCoordinates(1);
        public string Lng => GetCoordinates(0);

        private string GetCoordinates(int index)
        {
            var location = GetLocation();
            if (location != null)
            {
                switch (index)
                {
                    case 0:
                        return location.coordinates[0];
                    case 1:
                        return location.coordinates[1];
                }
            }
            return string.Empty;
        }
    }
}