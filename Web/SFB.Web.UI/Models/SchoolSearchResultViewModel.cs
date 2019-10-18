using System;
using System.Linq;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore;
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
                return base.ComparisonList.BenchmarkSchools.Any(s => s.Urn == GetString(EdubaseDataFieldNames.URN));
            }
        }

        public bool IsDefaultBenchmark => base.ComparisonList.HomeSchoolUrn == GetString(EdubaseDataFieldNames.URN);

        public string Id => GetInt(EdubaseDataFieldNames.URN).ToString();

        public string LaEstab => $"{GetString(EdubaseDataFieldNames.LA_CODE)} {GetString(EdubaseDataFieldNames.ESTAB_NO)}";

        public string Name
        {
            get
            {
                return GetString(EdubaseDataFieldNames.ESTAB_NAME);
            }

            set { }
        }

        public int La => GetInt(EdubaseDataFieldNames.LA_CODE);

        public int Estab => GetInt(EdubaseDataFieldNames.ESTAB_NO);

        public string OverallPhase => GetString(EdubaseDataFieldNames.OVERALL_PHASE);

        public string Phase => GetString(EdubaseDataFieldNames.PHASE_OF_EDUCATION);

        public string Status => GetString(EdubaseDataFieldNames.ESTAB_STATUS);

        public string SchoolWebSite
        {
            get
            {
                var url = GetString(EdubaseDataFieldNames.SCHOOL_WEB_SITE);
                if (url != null && url.ToLower().StartsWith("www"))
                {
                    url = "http://" + url;
                }

                return url;
            }
        }

        public string AgeRange => $"{GetInt(EdubaseDataFieldNames.STAT_LOW)} to {GetInt(EdubaseDataFieldNames.STAT_HIGH)}";

        public string HeadTeachFullName => $"{GetString(EdubaseDataFieldNames.HEAD_FIRST_NAME)} {GetString(EdubaseDataFieldNames.HEAD_LAST_NAME)}";

        public string TrustName => GetString(EdubaseDataFieldNames.TRUSTS);

        public string PhoneNumber => GetString(EdubaseDataFieldNames.TEL_NO);

        public string OfstedRating => GetString(EdubaseDataFieldNames.OFSTED_RATING);

        public DateTime OfstedInspectionDate => GetDate(EdubaseDataFieldNames.OFSTE_LAST_INSP).GetValueOrDefault();

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

        public float TotalPupils => GetFloat(EdubaseDataFieldNames.NO_PUPIL);

        public string IsPost16 => GetString(EdubaseDataFieldNames.OFFICIAL_6_FORM) == "Has a sixth form" ? "Yes" : "No";

        public string HasNursery => GetInt(EdubaseDataFieldNames.STAT_LOW) <= 3 ? "Yes" : "No";

        public string OpenDate
        {
            get
            {
                var openDate = GetDateBinary(EdubaseDataFieldNames.OPEN_DATE);
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
                var closeDate = GetDateBinary(EdubaseDataFieldNames.CLOSE_DATE);
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

        public string Address => $"{GetString(EdubaseDataFieldNames.STREET)}, {GetString(EdubaseDataFieldNames.TOWN)}, {GetString(EdubaseDataFieldNames.POSTCODE)}";

        public string Type => GetString(EdubaseDataFieldNames.TYPE_OF_ESTAB);

        public bool HasIncompleteFinancialData => GetInt(EdubaseDataFieldNames.PERIOD_COVERED_BY_RETURN) != 12;

        public EstablishmentType EstablishmentType => (EstablishmentType)Enum.Parse(typeof(EstablishmentType), GetString(EdubaseDataFieldNames.FINANCE_TYPE));

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