using System;
using System.Web;

namespace SFB.Web.UI.Models
{
    public class Pagination
    {
        HttpContext _currentContext = HttpContext.Current;

        public int Start { get; set; }
        public int Total { get; set; }
        public int MaxResultsPerPage { get; set; }
        public int PageLinksPerPage { get; set; }

        public int End
        {
            get
            {
                return Math.Min(this.Start + this.MaxResultsPerPage - 1, this.Total);
            }
        }

        public int CurrentPage
        {
            get
            {
                return (int)Math.Ceiling((double)End / MaxResultsPerPage);
            }
        }

        public int PageCount
        {
            get
            {
                return (int)Math.Ceiling((double)Total / MaxResultsPerPage);
            }
        }

        public int FirstShownPageNumber
        {
            get
            {
                if (CurrentPage < this.PageLinksPerPage)
                    return 1;

                if (CurrentPage > this.PageCount - this.PageLinksPerPage)
                    return this.PageCount - this.PageLinksPerPage + 1;

                return CurrentPage - 2;
            }
        }

        public int LastShownPageNumber
        {
            get
            {
                int estimatedLastPage = FirstShownPageNumber + this.PageLinksPerPage - 1;
                return Math.Min(estimatedLastPage, this.PageCount);
            }
        }

        public bool HasPreviousPage()
        {
            return this.CurrentPage > 1;
        }

        public bool HasMorePages()
        {
            return this.CurrentPage < this.PageCount;
        }

        public string GetSummary()
        {
            return Total > 50 ? $"Showing {Start} to {End} of {Total}" : string.Empty;
        }

        private string ContructQueryParams(int pageIndex)
        {
            var query = _currentContext.Request.Url.Query;
            if (query.StartsWith("?"))
            {
                query = query.Substring(1);
            }

            if (query.Contains("page"))
            {
                var collection = HttpUtility.ParseQueryString(query);
                collection.Set("page", pageIndex.ToString());
                return collection.ToString();
            }

            return query + "&page=" + pageIndex;
        }

        public string ConstructPageUrl(string baseUrl, int pageIndex)
        {
            return baseUrl + "?" + this.ContructQueryParams(pageIndex);
        }

        public string ConstructPageUrl(int pageIndex)
        {
            var requestPath = _currentContext.Request.Url.AbsolutePath;
            if (requestPath.EndsWith("-js"))
                requestPath = requestPath.Substring(0, requestPath.LastIndexOf("-js", StringComparison.Ordinal));
            return requestPath + "?" + this.ContructQueryParams(pageIndex);
        }

    }
}