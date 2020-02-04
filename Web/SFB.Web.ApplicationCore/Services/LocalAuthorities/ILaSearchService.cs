using SFB.Web.ApplicationCore.Models;
using System.Collections.Generic;

namespace SFB.Web.ApplicationCore.Services.LocalAuthorities
{
    public interface ILaSearchService
    {
        List<LaModel> SearchContains(string name);
        LaModel SearchExactMatch(string name);
        bool LaCodesContain(int laCode);
    }
}