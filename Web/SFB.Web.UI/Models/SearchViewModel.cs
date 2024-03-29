﻿namespace SFB.Web.UI.Models
{
    public class SearchViewModel : ViewModelBase
    {
        public SearchViewModel()
        {
            
        }
        public SearchViewModel(SchoolComparisonListModel comparisonList, string queryKey)
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
            get { return "/trustsearch/suggest"; }
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
                return "govuk-form-group--error";

            return string.Empty;
        }

        public string GetInputErrorClass(string testQueryKey)
        {
            if (SearchTypeHasError(testQueryKey))
                return "govuk-input--error";

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

        public string GetOpenOnlyDisabledAttr(string testQueryKey)
        {
            if (!this.IsSearchType(testQueryKey))
                return "disabled";

            return string.Empty;
        }

        public bool IsSearchType(string testSearchType)
        {
            return this.QueryKey == testSearchType;
        }
    }
}