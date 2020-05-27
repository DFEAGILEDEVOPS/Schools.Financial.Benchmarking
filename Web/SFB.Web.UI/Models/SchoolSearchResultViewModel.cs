using System;
using System.Linq;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.ApplicationCore.Models;
using System.Globalization;

namespace SFB.Web.UI.Models
{
    public class SchoolSearchResultViewModel :  ViewModelBase
    {
        private SchoolSearchResult ContextDataModel { get; set; }
        public SchoolSearchResultViewModel(SchoolSearchResult contextDataModel)
        {
            ContextDataModel = contextDataModel;
        }

        public SchoolSearchResultViewModel(SchoolSearchResult schoolContextDataModel, SchoolComparisonListModel comparisonList)
        {
            ContextDataModel = schoolContextDataModel;
            base.ComparisonList = comparisonList;
        }

        public bool IsInComparisonList
        {
            get
            {
                if (ComparisonList == null)
                {
                    return false;
                }
                return base.ComparisonList.BenchmarkSchools.Any(s => s.Urn == ContextDataModel.URN);
            }
        }

        public bool IsDefaultBenchmark => base.ComparisonList.HomeSchoolUrn == ContextDataModel.URN;

        public string Id => ContextDataModel.URN.ToString();

        public string LaEstab => $"{ContextDataModel.LACode} {ContextDataModel.EstablishmentNumber}";

        public string Name => ContextDataModel.EstablishmentName;        

        public int La => int.Parse(ContextDataModel.LACode);

        public int Estab => int.Parse(ContextDataModel.EstablishmentNumber);

        public string OverallPhase => ContextDataModel.OverallPhase;

        public string Phase => ContextDataModel.PhaseOfEducation;

        public string Status => ContextDataModel.EstablishmentStatus;

        public string StatusInYear => ContextDataModel.EstablishmentStatusInLatestAcademicYear;

        public string SchoolWebSite
        {
            get
            {
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

        public float TotalPupils => ContextDataModel.NumberOfPupils == null ? 0 : float.Parse(ContextDataModel.NumberOfPupils);

        public string IsPost16 => ContextDataModel.OfficialSixthForm == "Has a sixth form" ? "Yes" : "No";

        public string HasNursery => ContextDataModel.NurseryProvisionName == "Has Nursery Classes" ? "Yes" : "No";

        public string OpenDate
        {
            get
            {
                var openDate = ContextDataModel == null ? null : (DateTime?) DateTime.ParseExact(ContextDataModel.OpenDate, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (openDate.HasValue && openDate >= new DateTime(2011, 1, 1))
                {
                    return openDate.Value.ToLongDateString();
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
                var closeDate = ContextDataModel == null ? null : (DateTime?)DateTime.ParseExact(ContextDataModel.CloseDate, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (closeDate.HasValue && closeDate >= new DateTime(2011, 1, 1))
                {
                    return closeDate.Value.ToLongDateString();
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string Address => $"{ContextDataModel.Street}, {ContextDataModel.Town}, {ContextDataModel.Postcode}";

        public string Type => ContextDataModel.TypeOfEstablishment;

        //public bool HasIncompleteFinancialData => GetInt(EdubaseDataFieldNames.PERIOD_COVERED_BY_RETURN) != 12;

        public EstablishmentType EstablishmentType => (EstablishmentType)Enum.Parse(typeof(EstablishmentType), ContextDataModel.FinanceType);

        public bool HasCoordinates
        {
            get
            {
                try
                {
                    if (Lat == string.Empty || Lng == string.Empty)
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

        public string Lat => ContextDataModel.Location.Latitude.ToString();
        public string Lng => ContextDataModel.Location.Longitude.ToString();
    }
}