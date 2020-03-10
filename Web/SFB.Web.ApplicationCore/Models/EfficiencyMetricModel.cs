using SFB.Web.ApplicationCore.Entities;

namespace SFB.Web.ApplicationCore.Models
{
    public class EfficiencyMetricModel
    {
        public EfficiencyMetricDataObject EfficiencyMetricData { get; set; }
        public EdubaseDataObject ContextData { get; set; }
        public SchoolTrustFinancialDataObject FinancialData { get; set; }
    }
}
