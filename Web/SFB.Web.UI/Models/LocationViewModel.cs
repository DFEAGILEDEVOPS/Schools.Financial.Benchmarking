
namespace SFB.Web.UI.Models
{
    public class LocationViewModel : ViewModelBase
    {
        public string LocationName { get; set; }
        public string LatLon { get; set; }

        public LocationViewModel(string locationName, string latLon)
        {
            LocationName = locationName;
            LatLon = latLon;
        }
    }
}