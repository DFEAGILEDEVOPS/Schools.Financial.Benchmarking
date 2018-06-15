using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFB.Web.UI.Models
{
    public class SchoolSearchListViewModel : ViewModelListBase<SchoolSearchResultViewModel>
    {
        public SchoolSearchListViewModel(List<SchoolSearchResultViewModel> modelList, SchoolComparisonListModel comparisonList, string orderBy = "")
        {
            base.SchoolComparisonList = comparisonList;
            base.ModelList = modelList;
            base.OrderBy = orderBy;
        }
    }
}