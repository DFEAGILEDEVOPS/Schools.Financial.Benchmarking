using System.Collections.Generic;

namespace SFB.Web.ApplicationCore.Models
{
    public class SearchResultsModel<T>
    {
        public SearchResultsModel(int numberOfResults, Dictionary<string, FacetResultModel[]> facets, IEnumerable<T> results,
            int taken,
            int skipped)
        {
            Facets = facets;
            NumberOfResults = numberOfResults;
            Taken = taken;
            Skipped = skipped;
            Results = results;
        }

        public string ErrorMessage { get; set; }
        public int NumberOfResults { get; set; }
        public IEnumerable<T> Results { get; set; }
        public int Taken { get; private set; }
        public int Skipped { get; private set; }
        public Dictionary<string, FacetResultModel[]> Facets { get; set; }
        public string QueryLat { get; set; }
        public string QueryLong { get; set; }
        public bool Disambiguate { get; set; }
    }

    public class TrustSearchResult
    {
        public string TrustOrCompanyName { get; set; }

        public string CompanyNumber { get; set; }

        public string Trusts { get; set; }
    }
}
