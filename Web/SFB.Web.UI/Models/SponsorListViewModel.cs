using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class SponsorListViewModel : ViewModelListBase<SponsorViewModel>
    {
        public SponsorListViewModel(List<SponsorViewModel> modelList, ComparisonListModel comparisonList, string orderBy = "")
        {
            base.SchoolComparisonList = comparisonList;
            base.ModelList = modelList;
            base.OrderBy = orderBy;
        }
    }
}