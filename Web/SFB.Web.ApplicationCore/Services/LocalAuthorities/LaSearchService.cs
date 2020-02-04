using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Services.LocalAuthorities;

namespace SFB.Web.UI.Services
{
    public class LaSearchService : ILaSearchService
    {
        private readonly ILocalAuthoritiesService _laService;

        public LaSearchService(ILocalAuthoritiesService laService)
        {
            _laService = laService;
        }

        public List<LaModel> SearchContains(string name)
        {
            var localAuthorities =
                (List<dynamic>)JsonConvert.DeserializeObject<List<dynamic>>(_laService.GetLocalAuthorities());

            var filteredResults = localAuthorities
                .Where(la => la.LANAME.ToString().ToLowerInvariant().Contains(name.ToLowerInvariant()))
                .Select(la => new LaModel() { Id = la.id, LaName = la.LANAME })
                .ToList();
            return filteredResults;
        }

        public LaModel SearchExactMatch(string name)
        {
            var localAuthorities =
                (List<dynamic>)JsonConvert.DeserializeObject<List<dynamic>>(_laService.GetLocalAuthorities());

            var filteredResult = localAuthorities
                .Where(la => la.LANAME.ToString().ToLowerInvariant().Equals(name.ToLowerInvariant()))
                .Select(la => new LaModel() { Id = la.id, LaName = la.LANAME })
                .FirstOrDefault();
            return filteredResult;
        }

        public bool LaCodesContain(int laCode)
        {
            var localAuthorities =
                (List<dynamic>)JsonConvert.DeserializeObject<List<dynamic>>(_laService.GetLocalAuthorities());
                        
            return localAuthorities.Any(la => la.id == laCode);
        }
    }
}