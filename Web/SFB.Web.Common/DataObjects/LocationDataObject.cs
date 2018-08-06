using Newtonsoft.Json;

namespace SFB.Web.Common.DataObjects
{
    public class LocationDataObject
    {
        [JsonProperty(PropertyName = EdubaseDBFieldNames.LOCATION_TYPE)]
        public string type { get; set; }

        [JsonProperty(PropertyName = EdubaseDBFieldNames.LOCATION_COORDINATES)]
        public string[] coordinates { get; set; }
    }
}
