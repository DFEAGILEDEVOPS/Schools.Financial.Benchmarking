using SFB.Web.UI.Models;

namespace SFB.Web.UI.Helpers
{
    public interface IFilterBuilder
    {
        void AddStatusFilters(dynamic facets, dynamic queryParams);

        void AddReligiousCharacterFilters(dynamic facets, dynamic queryParams);

        void AddSchoolLevelFilters(dynamic facets, dynamic queryParams);

        void AddSchoolTypeFilters(dynamic facets, dynamic queryParams);

        void AddGenderFilters(dynamic facets, dynamic queryParams);

        void AddOfstedRatingFilters(dynamic facets, dynamic queryParams);

        Filter[] GetResult();

    }
}