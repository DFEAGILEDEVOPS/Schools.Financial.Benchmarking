namespace SFB.Web.UI.Models
{
    public class TrustSelectionViewModel : ViewModelBase
    {
        public TrustViewModel BenchmarkTrust { get; set; }
        public TrustComparisonListModel TrustComparisonList { get; set; }

        public TrustSelectionViewModel(TrustViewModel trust, TrustComparisonListModel trustComparisonList)
        {
            this.TrustComparisonList = trustComparisonList;
            this.BenchmarkTrust = trust;
        }
    }
}
