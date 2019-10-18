using Newtonsoft.Json;

namespace SFB.Web.Common.Entities
{
    public class AcademiesContextualDataObject
    {
        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.URN)]
        public int URN;

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.ESTAB_NAME)]
        public string EstablishmentName;

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME)]
        public string TrustName;

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PERIOD_COVERED_BY_RETURN)]
        public int PeriodCoveredByReturn;

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE)]
        public string OverallPhase;

        public bool HasIncompleteFinancialData => PeriodCoveredByReturn != 12;
    }
}
