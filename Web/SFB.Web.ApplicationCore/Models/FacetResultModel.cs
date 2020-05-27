using Newtonsoft.Json;

namespace SFB.Web.ApplicationCore.Models
{
    public class FacetResultModel
    {
        public FacetResultModel()
        {}

        public FacetResultModel(string value, long? from, long? to, long count)
        {
            Value = value;
            From = from;
            To = to;
            Count = count;
        }

        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("from")]
        public long? From { get; set; }
        [JsonProperty("to")]
        public long? To { get; set; }
        [JsonProperty("count")]
        public long Count { get; set; }
    }
}