using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class LocationListViewModel : ViewModelListBase<LocationViewModel>
    {
        public bool OpenOnly { get; }
        public string LocationOrPostcode { get; }

        public LocationListViewModel(List<LocationViewModel> modelList, SchoolComparisonListModel comparisonList, string locationOrPostcode, string orderBy = "", bool openOnly = false)
            : base(modelList, comparisonList, orderBy)
        {
            OpenOnly = openOnly;
            LocationOrPostcode = locationOrPostcode;
        }
    }
}