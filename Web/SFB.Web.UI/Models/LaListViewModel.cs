using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class LaListViewModel : ViewModelListBase<LaViewModel>
    {
        public LaListViewModel(List<LaViewModel> modelList, ComparisonListModel comparisonList, string orderBy = "")
        {
            base.SchoolComparisonList = comparisonList;
            base.ModelList = modelList;
            base.OrderBy = orderBy;
        }
    }
}