using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Helpers.Constants;

namespace SFB.Web.ApplicationCore.Entities
{
    public class LocationDataObject
    {
        [JsonProperty(PropertyName = EdubaseDataFieldNames.LOCATION_TYPE)]
        public string type { get; set; }

        [JsonProperty(PropertyName = EdubaseDataFieldNames.LOCATION_COORDINATES)]
        public string[] coordinates { get; set; }
    }
}
