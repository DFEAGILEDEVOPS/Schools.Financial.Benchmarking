using System;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.UI.Helpers.Constants;

namespace SFB.Web.UI.Models
{
    public class SchoolSearchResultViewModel : DynamicViewModelBase
    {
        public SchoolSearchResultViewModel(dynamic contextDataModel)
        {
            base.ContextDataModel = contextDataModel;
        }

        public SchoolSearchResultViewModel(dynamic schoolContextDataModel, SchoolComparisonListModel comparisonList)
        {
            base.ContextDataModel = schoolContextDataModel;
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
                return base.ComparisonList.BenchmarkSchools.Any(s => s.Urn == GetString(EdubaseDBFieldNames.URN));
            }
        }

        public bool IsDefaultBenchmark => base.ComparisonList.HomeSchoolUrn == GetString(EdubaseDBFieldNames.URN);

        public string Id => GetInt(EdubaseDBFieldNames.URN).ToString();

        public string LaEstab => $"{GetString(EdubaseDBFieldNames.LA_CODE)} {GetString(EdubaseDBFieldNames.ESTAB_NO)}";

        public string Name
        {
            get
            {
                return GetString(EdubaseDBFieldNames.ESTAB_NAME);
            }

            set { }
        }

        public int La => GetInt(EdubaseDBFieldNames.LA_CODE);

        public int Estab => GetInt(EdubaseDBFieldNames.ESTAB_NO);

        public string OverallPhase => GetString(EdubaseDBFieldNames.OVERALL_PHASE);

        public string Phase => GetString(EdubaseDBFieldNames.PHASE_OF_EDUCATION);

        public bool IsSixthForm => this.Phase == "16 plus";

        public string Status => GetString(EdubaseDBFieldNames.ESTAB_STATUS);

        public string SchoolWebSite
        {
            get
            {
                var url = GetString(EdubaseDBFieldNames.SCHOOL_WEB_SITE);
                if (url != null && url.ToLower().StartsWith("www"))
                {
                    url = "http://" + url;
                }

                return url;
            }
        }

        public string AgeRange => $"{GetInt(EdubaseDBFieldNames.STAT_LOW)} to {GetInt(EdubaseDBFieldNames.STAT_HIGH)}";

        public string HeadTeachFullName => $"{GetString(EdubaseDBFieldNames.HEAD_FIRST_NAME)} {GetString(EdubaseDBFieldNames.HEAD_LAST_NAME)}";

        public string TrustName => GetString(EdubaseDBFieldNames.TRUSTS);

        public string PhoneNumber => GetString(EdubaseDBFieldNames.TEL_NO);

        public string OfstedRating => GetString(EdubaseDBFieldNames.OFSTED_RATING);

        public DateTime OfstedInspectionDate => GetDate(EdubaseDBFieldNames.OFSTE_LAST_INSP).GetValueOrDefault();

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

        public int TotalPupils => GetInt(EdubaseDBFieldNames.NO_PUPIL);

        public string IsPost16 => GetString(EdubaseDBFieldNames.OFFICIAL_6_FORM) == "Has a sixth form" ? "Yes" : "No";

        public string HasNursery => GetInt(EdubaseDBFieldNames.STAT_LOW) <= 3 ? "Yes" : "No";

        public string OpenDate
        {
            get
            {
                var openDate = GetDateBinary(EdubaseDBFieldNames.OPEN_DATE);
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
                var closeDate = GetDateBinary(EdubaseDBFieldNames.CLOSE_DATE);
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

        public string Address => $"{GetString(EdubaseDBFieldNames.STREET)}, {GetString(EdubaseDBFieldNames.TOWN)}, {GetString(EdubaseDBFieldNames.POSTCODE)}";

        public string Type => GetString(EdubaseDBFieldNames.TYPE_OF_ESTAB);

        public bool HasIncompleteFinancialData => GetInt(EdubaseDBFieldNames.PERIOD_COVERED_BY_RETURN) != 12;

        public EstablishmentType EstablishmentType => (EstablishmentType)Enum.Parse(typeof(EstablishmentType), GetString(EdubaseDBFieldNames.FINANCE_TYPE));

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