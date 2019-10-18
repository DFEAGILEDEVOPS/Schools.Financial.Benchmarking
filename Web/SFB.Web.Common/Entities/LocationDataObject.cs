using Newtonsoft.Json;

namespace SFB.Web.Common.Entities
{
    public class LocationDataObject
    {
        [JsonProperty(PropertyName = EdubaseDataFieldNames.LOCATION_TYPE)]
        public string type { get; set; }

        [JsonProperty(PropertyName = EdubaseDataFieldNames.LOCATION_COORDINATES)]
        public string[] coordinates { get; set; }
    }
}
