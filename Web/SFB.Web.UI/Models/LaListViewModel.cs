using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class LaListViewModel : ViewModelListBase<LaViewModel>
    {
        public LaListViewModel(List<LaViewModel> modelList, SchoolComparisonListModel comparisonList, string orderBy = "")
            : base(modelList, comparisonList, orderBy)
        {}
    }
}