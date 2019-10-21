namespace SFB.Web.UI.Models
{
    public class SaveOverwriteViewModel : ViewModelBase
    {
        public string SavedUrns { get; set; }
        public string SavedCompanyNos { get; set; }
        public TrustComparisonListModel TrustComparisonList { get; internal set; }
    }
}