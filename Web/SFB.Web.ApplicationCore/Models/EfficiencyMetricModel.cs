using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Entities;

namespace SFB.Web.ApplicationCore.Models
{
    public class EfficiencyMetricModel
    {
        [JsonProperty]
        public EfficiencyMetricDataObject EfficiencyMetricData { get; set; }
        [JsonProperty]
        public EdubaseDataObject ContextData { get; set; }
        [JsonProperty]
        public SchoolTrustFinancialDataObject FinancialData { get; set; }
    }
}
