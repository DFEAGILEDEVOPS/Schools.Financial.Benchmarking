using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class TrustListViewModel : ViewModelListBase<AcademyTrustViewModel>
    {
        public string SearchType { get; }
        public string NameKeyword { get; }
        public string LocationKeyword { get; }
        public string LaKeyword { get; }

        public TrustListViewModel(List<AcademyTrustViewModel> modelList, SchoolComparisonListModel comparisonList, string searchType, string nameKeyword, string locationKeyword, string laKeyword, string orderBy = "")
                        : base(modelList, null, orderBy)
        {
            SearchType = searchType;
            NameKeyword = nameKeyword;
            LocationKeyword = locationKeyword;
            LaKeyword = laKeyword;
        }
    }
}