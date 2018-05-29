using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class SearchedSchoolListViewModel : SchoolListViewModel
    {
        public string SearchType { get; }
        public string NameKeyword { get; }
        public string LocationKeyword { get; }
        public string LaKeyword { get; }

        public SearchedSchoolListViewModel(List<SchoolViewModel> modelList, SchoolComparisonListModel comparisonList, string searchType, string nameKeyword, string locationKeyword, string laKeyword, string orderBy = "") 
            : base(modelList, comparisonList, orderBy)
        {
            SearchType = searchType;
            NameKeyword = nameKeyword;
            LocationKeyword = locationKeyword;
            LaKeyword = laKeyword;
        }
    }
}