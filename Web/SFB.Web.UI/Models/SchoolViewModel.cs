using System;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.Common.DataObjects;
using System.Globalization;

namespace SFB.Web.UI.Models
{
    public class SchoolViewModel : EstablishmentViewModelBase
    {
        private EdubaseDataObject ContextDataModel { get; set; }

        public SchoolViewModel(EdubaseDataObject contextDataModel)
        {
            this.ContextDataModel = contextDataModel;
        }

        public SchoolViewModel(EdubaseDataObject schoolContextDataModel, SchoolComparisonListModel comparisonList)
        {
            this.ContextDataModel = schoolContextDataModel;
            base.ComparisonList = comparisonList;
        }

        public LocationDataObject GetLocation()
        {
            return ContextDataModel.Location;
        }

        public bool IsInComparisonList
        {
            get
            {
                if (ComparisonList == null)
                {
                    return false;
                }
                return base.ComparisonList.BenchmarkSchools.Any(s => s.Urn == ContextDataModel.URN.ToString());
            }
        }

        public bool IsDefaultBenchmark => base.ComparisonList.HomeSchoolUrn == ContextDataModel.URN.ToString();

        public int Id => ContextDataModel.URN;

        public string LaEstab => $"{ContextDataModel.LACode} {ContextDataModel.EstablishmentNumber}";

        public override string Name
        {
            get
            {
                return ContextDataModel.EstablishmentName;
            }

            set { }
        }

        public int La => ContextDataModel.LACode;

        public int Estab => ContextDataModel.EstablishmentNumber;

        public string OverallPhase => ContextDataModel.OverallPhase;

        public string Phase => ContextDataModel.PhaseOfEducation;

        public bool IsSixthForm => this.Phase == "16 plus";

        public string SchoolWebSite
        {
            get {
                var url = ContextDataModel.SchoolWebsite;
                if (url != null && !url.ToLower().StartsWith("http"))
                {
                    url = "http://" + url;
                }

                return url;
            }
        }

        public string AgeRange => $"{ContextDataModel.StatutoryLowAge} to {ContextDataModel.StatutoryHighAge}";

        public string HeadTeachFullName => $"{ContextDataModel.HeadFirstName} {ContextDataModel.HeadLastName}";

        public string TrustName => ContextDataModel.Trusts;

        public int? CompanyNo => ContextDataModel.CompanyNumber;

        public string PhoneNumber => ContextDataModel.TelephoneNum;

        public string OfstedRating => ContextDataModel.OfstedRating;

        public string OfstedInspectionDate 
        {
            get
            {
                try
                {
                    return DateTime.Parse(ContextDataModel.OfstedLastInsp, CultureInfo.CurrentCulture, DateTimeStyles.None).ToLongDateString();                    
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
                    return DateTime.Parse(ContextDataModel.OpenDate, CultureInfo.CurrentCulture, DateTimeStyles.None).ToLongDateString();
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
                    return DateTime.Parse(ContextDataModel.CloseDate, CultureInfo.CurrentCulture, DateTimeStyles.None).ToLongDateString();
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
                        return null;
                }
            }
        }

        public float TotalPupils => ContextDataModel.NumberOfPupils.GetValueOrDefault();

        public string IsPost16 => ContextDataModel.OfficialSixthForm == "Has a sixth form" ? "Yes" : "No";

        public string HasNursery => ContextDataModel.StatutoryLowAge <= 3  ?  "Yes" : "No";

        public string Address => $"{ContextDataModel.Street}, {ContextDataModel.Town}, {ContextDataModel.Postcode}";

        public override string Type => ContextDataModel.TypeOfEstablishment;

        public override EstablishmentType EstablishmentType => (EstablishmentType)Enum.Parse(typeof(EstablishmentType), ContextDataModel.FinanceType);

        public bool IsSAT => LatestYearFinancialData.IsSAT;

        public bool IsMAT => LatestYearFinancialData.IsMAT;

        public bool HasProgressScore => LatestYearFinancialData.Ks2Progress.HasValue || LatestYearFinancialData.P8Mea.HasValue;

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