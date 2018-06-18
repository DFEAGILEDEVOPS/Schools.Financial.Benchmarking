using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFB.Web.Common.DataObjects
{
    public class AcademiesContextualDataObject
    {
        //$"SELECT c['URN'], c['School Name'] as EstablishmentName, c['Period covered by return'] FROM c WHERE c['MATNumber']=@MatNo"

        [JsonProperty(PropertyName = SchoolFinanceDBFieldNames.URN)]
        public int URN;

        [JsonProperty(PropertyName = SchoolFinanceDBFieldNames.ESTAB_NAME)]
        public string EstablishmentName;

        [JsonProperty(PropertyName = SchoolFinanceDBFieldNames.PERIOD_COVERED_BY_RETURN)]
        public int PeriodCoveredByReturn;
        
        public bool HasIncompleteFinancialData => PeriodCoveredByReturn != 12;
    }
}
