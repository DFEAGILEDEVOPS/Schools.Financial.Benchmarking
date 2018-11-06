using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class LaListViewModel : ViewModelListBase<LaViewModel>
    {
        public bool OpenOnly { get; }

        public LaListViewModel(List<LaViewModel> modelList, SchoolComparisonListModel comparisonList, string orderBy = "", bool openOnly = false)
            : base(modelList, comparisonList, orderBy)
        {
            OpenOnly = openOnly;
        }
    }
}