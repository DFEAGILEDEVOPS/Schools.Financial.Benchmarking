using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFB.Web.Common.DataObjects
{
    public class BestInBreedDataObject
    {
        [JsonProperty(PropertyName = EfficiencyMetricDBFieldNames.URN)]
        public int URN;

        [JsonProperty(PropertyName = EfficiencyMetricDBFieldNames.RANK)]
        public int Rank;

        [JsonProperty(PropertyName = EfficiencyMetricDBFieldNames.SCORE)]
        public int Score;

        [JsonProperty(PropertyName = EfficiencyMetricDBFieldNames.NEIGHBOURS)]
        public List<BestInBreedDataObject> Neighbours;
    }
}
