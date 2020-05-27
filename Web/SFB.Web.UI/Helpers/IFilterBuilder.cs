using SFB.Web.UI.Models;

namespace SFB.Web.UI.Helpers
{
    public interface IFilterBuilder
    {
        Filter[] ConstructSchoolSearchFilters(dynamic parameters, dynamic facets);
        Filter[] ConstructTrustSchoolSearchFilters(dynamic parameters, dynamic facets);
    }
}