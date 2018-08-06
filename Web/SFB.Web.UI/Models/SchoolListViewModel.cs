using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class SchoolListViewModel : ViewModelListBase<SchoolViewModel>
    {
        public SchoolListViewModel(List<SchoolViewModel> modelList, SchoolComparisonListModel comparisonList, string orderBy = "")
             : base(modelList, comparisonList, orderBy)
        {}
    }
}