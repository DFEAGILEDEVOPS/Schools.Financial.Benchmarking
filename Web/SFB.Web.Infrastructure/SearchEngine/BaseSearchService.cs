using Microsoft.Azure.Search.Models;
using SFB.Web.ApplicationCore.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFB.Web.Infrastructure.SearchEngine
{
    public abstract class BaseSearchService<T>
    {
        protected Dictionary<string, FacetResultModel[]> MapResponseFacetsToFacetsModel(DocumentSearchResult<T> response)
        {
            var facetsModel = new Dictionary<string, FacetResultModel[]>();
            if (response.Facets != null)
            {
                foreach (var facet in response.Facets)
                {
                    facetsModel.Add(facet.Key, facet.Value.Select(fv => new FacetResultModel(fv.Value.ToString(), (long?)fv.From, (long?)fv.To, fv.Count.GetValueOrDefault())).ToArray());
                }
            }

            return facetsModel;
        }
    }
}
