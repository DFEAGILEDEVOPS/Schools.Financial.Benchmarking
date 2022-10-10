using Newtonsoft.Json;

namespace SFB.Web.UI.Services
{
    public class PlaceDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("value")]
        public string Value { get; set; }
        
        [JsonProperty("coords")]
        public LatLon Coords { get; set; }
        
        public PlaceDto() {}
        public PlaceDto(string id, string name, LatLon coords)
        {
            Id = id;
            Name = name;
            Value = name;
            Coords = coords;
        }
    }
}