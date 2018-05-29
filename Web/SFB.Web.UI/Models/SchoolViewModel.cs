using System;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.UI.Helpers.Constants;

namespace SFB.Web.UI.Models
{
    public class SchoolViewModel : EstablishmentViewModelBase
    {

        public SchoolViewModel(dynamic model)
        {
            base.DataModel = model;
        }

        public SchoolViewModel(dynamic SchoolFinancialDataModel, SchoolComparisonListModel comparisonList)
        {
            base.DataModel = SchoolFinancialDataModel;
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
                return base.ComparisonList.BenchmarkSchools.Any(s => s.Urn == GetString("URN"));
            }
        }

        public bool IsDefaultBenchmark => base.ComparisonList.HomeSchoolUrn == GetString("URN");

        public string Id => GetString("URN");

        public string LaEstab => $"{GetString("LACode")} {GetString("EstablishmentNumber")}";

        public override string Name
        {
            get
            {
                return GetString("EstablishmentName");
            }

            set { }
        }

        public int La => GetInt("LACode");

        public int Estab => GetInt("EstablishmentNumber");

        public string OverallPhase => GetString("OverallPhase");

        public string Phase => GetString("PhaseOfEducation");

        public bool IsSixthForm => this.Phase == "16 plus";

        public string Status => GetString("EstablishmentStatus");

        public string SchoolWebSite
        {
            get {
                var url = GetString("SchoolWebsite");
                if (url != null && url.ToLower().StartsWith("www"))
                {
                    url = "http://" + url;
                }

                return url;
            }
        }

        public string AgeRange => $"{GetInt("StatutoryLowAge")} to {GetInt("StatutoryHighAge")}";

        public string HeadTeachFullName => $"{GetString("HeadFirstName")} {GetString("HeadLastName")}";

        public string TrustName => GetString("Trusts");
        
        public string PhoneNumber => GetString("TelephoneNum");

        public string OfstedRating => GetString("OfstedRating");

        public DateTime OfstedInspectionDate => GetDate("OfstedLastInsp").GetValueOrDefault();

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

        public int TotalPupils => GetInt("NumberOfPupils");

        public string IsPost16 => GetString("OfficialSixthForm") == "Has a sixth form" ? "Yes" : "No";

        public string HasNursery => GetInt("StatutoryLowAge") <= 3  ?  "Yes" : "No";

        public string OpenDate
        {
            get
            {
                var openDate = GetDate("OpenDate");
                if (openDate.HasValue && openDate >= new DateTime(2011,1,1))
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
                var closeDate = GetDate("CloseDate");
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

        public string Address => $"{GetString("Street")}, {GetString("Town")}, {GetString("Postcode")}";

        public override string Type => GetString("TypeOfEstablishment");

        public bool HasIncompleteFinancialData => GetInt("Period covered by return") != 12;

        public override EstabType EstabType => GetString("FinanceType") == "A" ? EstabType.Academies : EstabType.Maintained;

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