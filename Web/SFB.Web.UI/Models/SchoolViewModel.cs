using System;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.Common.DataObjects;
using SFB.Web.Domain.Models;
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

        public string Id => ContextDataModel.URN.ToString();

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
                if (url != null && url.ToLower().StartsWith("www"))
                {
                    url = "http://" + url;
                }

                return url;
            }
        }

        public string AgeRange => $"{ContextDataModel.StatutoryLowAge} to {ContextDataModel.StatutoryHighAge}";

        public string HeadTeachFullName => $"{ContextDataModel.HeadFirstName} {ContextDataModel.HeadLastName}";

        public string TrustName => ContextDataModel.Trusts;
        
        public string PhoneNumber => ContextDataModel.TelephoneNum;

        public string OfstedRating => ContextDataModel.OfstedRating;

        public DateTime OfstedInspectionDate => DateTime.Parse(ContextDataModel.OfstedLastInsp, CultureInfo.CurrentCulture, DateTimeStyles.None);

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

        public int TotalPupils => ContextDataModel.NumberOfPupils;

        public string IsPost16 => ContextDataModel.OfficialSixthForm == "Has a sixth form" ? "Yes" : "No";

        public string HasNursery => ContextDataModel.StatutoryLowAge <= 3  ?  "Yes" : "No";

        //TODO: check conversion is correct
        public string OpenDate
        {
            get
            {
                var openDate = ContextDataModel.OpenDate;
                if (openDate >= new DateTime(2011,1,1))
                {
                    return openDate?.ToLongDateString();
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string CloseDate
        {
            get
            {
                var closeDate = ContextDataModel.CloseDate;
                if (closeDate >= new DateTime(2011, 1, 1))
                {
                    return closeDate?.ToLongDateString();
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string Address => $"{ContextDataModel.Street}, {ContextDataModel.Town}, {ContextDataModel.Postcode}";

        public override string Type => ContextDataModel.TypeOfEstablishment;

        //TODO: check can convert to enum
        public override EstablishmentType EstablishmentType => (EstablishmentType)Enum.Parse(typeof(EstablishmentType), ContextDataModel.FinanceType);

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