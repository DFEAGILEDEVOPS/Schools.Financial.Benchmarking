using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class SchoolListViewModel : ViewModelListBase<SchoolViewModel>
    {
        public SchoolListViewModel(List<SchoolViewModel> modelList, ComparisonListModel comparisonList, string orderBy = "")
        {
            base.SchoolComparisonList = comparisonList;
            base.ModelList = modelList;
            base.OrderBy = orderBy;            
        }
    }
}