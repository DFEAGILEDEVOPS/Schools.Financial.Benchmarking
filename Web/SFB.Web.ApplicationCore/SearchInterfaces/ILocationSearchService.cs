using SFB.Web.ApplicationCore.Models;

namespace SFB.Web.ApplicationCore.Services
{
    public interface ILocationSearchService
    {
        SuggestionQueryResult SuggestLocationName(string query);
    }
}
