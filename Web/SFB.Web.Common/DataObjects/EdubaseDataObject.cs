using System;
using Newtonsoft.Json;

namespace SFB.Web.Common.DataObjects
{
    public class EdubaseDataObject
    {
        [JsonProperty(PropertyName = EdubaseDBFieldNames.URN)]
        public int URN;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.ESTAB_NAME)]
        public string EstablishmentName;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.OVERALL_PHASE)]
        public string OverallPhase;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.PHASE_OF_EDUCATION)]
        public string PhaseOfEducation;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.TYPE_OF_ESTAB)]
        public string TypeOfEstablishment;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.STREET)]
        public string Street;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.TOWN)]
        public string Town;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.LOCATION)]
        public LocationDataObject Location;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.POSTCODE)]
        public string Postcode;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.TRUSTS)]
        public string Trusts;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.MAT_NUMBER)]
        public string MATNumber;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.LA_CODE)]
        public int LACode;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.ESTAB_NO)]
        public int EstablishmentNumber;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.TEL_NO)]
        public string TelephoneNum;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.NO_PUPIL)]
        public float? NumberOfPupils;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.STAT_LOW)]
        public int StatutoryLowAge;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.STAT_HIGH)]
        public int StatutoryHighAge;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.HEAD_FIRST_NAME)]
        public string HeadFirstName;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.HEAD_LAST_NAME)]
        public string HeadLastName;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.OFFICIAL_6_FORM)]
        public string OfficialSixthForm;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.SCHOOL_WEB_SITE)]
        public string SchoolWebsite;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.OFSTED_RATING)]
        public string OfstedRating;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.OFSTE_LAST_INSP)]
        public string OfstedLastInsp;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.FINANCE_TYPE)]
        public string FinanceType;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.OPEN_DATE)]
        public string OpenDate;

        [JsonProperty(PropertyName = EdubaseDBFieldNames.CLOSE_DATE)]
        public string CloseDate;
    }
}
