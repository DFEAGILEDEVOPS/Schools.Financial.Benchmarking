using System;
using Newtonsoft.Json;

namespace SFB.Web.Common.DataObjects
{
    public class EdubaseDataObject
    {
        //        "SELECT c['URN'], c['EstablishmentName'], c['OverallPhase'], c['PhaseOfEducation'], c['TypeOfEstablishment'], c['Street'], c['Town'], c['Location'], c['Postcode'], c['Trusts'], " +
        //" c['LAName'], c['LACode'], c['EstablishmentNumber'], c['TelephoneNum'], c['NumberOfPupils'], c['StatutoryLowAge'], c['StatutoryHighAge'], c['HeadFirstName'], " +
        //$"c['HeadLastName'], c['OfficialSixthForm'], c['SchoolWebsite'], c['OfstedRating'], c['OfstedLastInsp'], udf.PARSE_FINANCIAL_TYPE_CODE(c['FinanceType']) AS FinanceType, c['OpenDate'], c['CloseDate'] FROM c WHERE {where}";

        [JsonProperty(PropertyName = DBFieldNames.URN)]
        public int URN;

        [JsonProperty(PropertyName = DBFieldNames.ESTAB_NAME)]
        public string EstablishmentName;

        [JsonProperty(PropertyName = DBFieldNames.OVERALL_PHASE)]
        public string OverallPhase;

        public string PhaseOfEducation;
        public string TypeOfEstablishment;
        public string Street;
        public string Town;
        public LocationDataObject Location;
        public string Postcode;
        public string Trusts;
        public int LACode;
        public int EstablishmentNumber;
        public string TelephoneNum;
        public int NumberOfPupils;
        public int StatutoryLowAge;
        public int StatutoryHighAge;
        public string HeadFirstName;
        public string HeadLastName;
        public string OfficialSixthForm;
        public string SchoolWebsite;
        public string OfstedRating;
        public string OfstedLastInsp;//TODO: change to date
        public string FinanceType;
        public DateTime? OpenDate;
        public DateTime? CloseDate;
    }
}
