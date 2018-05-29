namespace SFB.Web.UI.Models
{
    public class SchoolSearchViewModel : DynamicViewModelBase
    {
        public SchoolSearchViewModel()
        {
            
        }
        public SchoolSearchViewModel(SchoolComparisonListModel comparisonList, string queryKey)
        {
            base.ComparisonList = comparisonList;
            this.QueryKey = queryKey;
        }

        public string QueryKey { get; set; }

        public string SchoolSuggestionUrl {
            get { return "/schoolsearch/suggest"; }
        }

        public string TrustSuggestionUrl
        {
            get { return "/schoolsearch/suggesttrust"; }
        }

        public dynamic Authorities { get; internal set; }

        public string LaSuggestionUrl
        {
            get { return "/api/laregion"; }
        }

        public string SearchType { get; set; }
        public string LaCodeByCode { get; set; }
        public string LaCodeByName { get; set; }
        public string LaName { get; set; }

        public bool SearchTypeHasError(string testQueryKey)
        {
            return (this.SearchType == testQueryKey) && this.HasError();
        }

        public string GetSearchTypeErrorClass(string testQueryKey)
        {
            if (SearchTypeHasError(testQueryKey))
                return "error";

            return string.Empty;
        }

        public string GetRadioButtonSelectedClass(string testQueryKey)
        {
            if (this.IsSearchType(testQueryKey))
                return "selected";

            return string.Empty;
        }

        public string GetRadioButtonCheckedAttr(string testQueryKey)
        {
            if (this.IsSearchType(testQueryKey))
                return "checked";

            return string.Empty;
        }

        public bool IsSearchType(string testSearchType)
        {
            return this.QueryKey == testSearchType;
        }
    }
}