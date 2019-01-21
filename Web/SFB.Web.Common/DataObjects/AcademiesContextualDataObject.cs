using Newtonsoft.Json;

namespace SFB.Web.Common.DataObjects
{
    public class AcademiesContextualDataObject
    {
        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.URN)]
        public int URN;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ESTAB_NAME)]
        public string EstablishmentName;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TRUST_COMPANY_NAME)]
        public string TrustName;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERIOD_COVERED_BY_RETURN)]
        public int PeriodCoveredByReturn;
        
        public bool HasIncompleteFinancialData => PeriodCoveredByReturn != 12;
    }
}
