using System.Collections.Generic;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Services
{
    public interface ILaSearchService
    {
        List<LaViewModel> SearchContains(string name);
        LaViewModel SearchExactMatch(string name);
    }
}