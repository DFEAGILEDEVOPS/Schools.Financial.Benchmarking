using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Helpers.Constants;

namespace SFB.Web.ApplicationCore.Entities
{
    public class EdubaseDataObject
    {
        [JsonProperty(PropertyName = EdubaseDataFieldNames.URN)]
        public int URN;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.ESTAB_NAME)]
        public string EstablishmentName;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.OVERALL_PHASE)]
        public string OverallPhase;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.PHASE_OF_EDUCATION)]
        public string PhaseOfEducation;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.TYPE_OF_ESTAB)]
        public string TypeOfEstablishment;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.STREET)]
        public string Street;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.TOWN)]
        public string Town;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.LOCATION)]
        public LocationDataObject Location;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.POSTCODE)]
        public string Postcode;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.TRUSTS)]
        public string Trusts;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.MAT_NUMBER)]
        public string MATNumber;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.COMPANY_NUMBER)]
        public int? CompanyNumber;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.LA_CODE)]
        public int LACode;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.ESTAB_NO)]
        public int EstablishmentNumber;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.LA_ESTAB)]
        public int LAEstab;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.TEL_NO)]
        public string TelephoneNum;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.NO_PUPIL)]
        public float? NumberOfPupils;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.STAT_LOW)]
        public int StatutoryLowAge;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.STAT_HIGH)]
        public int StatutoryHighAge;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.HEAD_FIRST_NAME)]
        public string HeadFirstName;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.HAS_NURSERY)]
        public string HasNursery;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.HEAD_LAST_NAME)]
        public string HeadLastName;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.OFFICIAL_6_FORM)]
        public string OfficialSixthForm;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.SCHOOL_WEB_SITE)]
        public string SchoolWebsite;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.OFSTED_RATING)]
        public string OfstedRating;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.OFSTE_LAST_INSP)]
        public string OfstedLastInsp;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.FINANCE_TYPE)]
        public string FinanceType;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.OPEN_DATE)]
        public string OpenDate;

        [JsonProperty(PropertyName = EdubaseDataFieldNames.CLOSE_DATE)]
        public string CloseDate;
    }
}
