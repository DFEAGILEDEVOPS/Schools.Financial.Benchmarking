using Newtonsoft.Json;

namespace SFB.Web.UI.Models
{
    public class CookiePolicyModel
    {
        [JsonProperty(PropertyName = "essential")]
        public bool Essential { get; set; }

        [JsonProperty(PropertyName = "settings")]
        public bool Settings { get; set; }

        [JsonProperty(PropertyName = "usage")]
        public bool Usage { get; set; }

        [JsonProperty(PropertyName = "campaigns")]
        public bool Campaigns { get; set; }
    }
}