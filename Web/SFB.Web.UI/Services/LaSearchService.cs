using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Services
{
    public class LaSearchService : ILaSearchService
    {
        private readonly ILocalAuthoritiesService _laService;

        public LaSearchService(ILocalAuthoritiesService laService)
        {
            _laService = laService;
        }

        public List<LaViewModel> SearchContains(string name)
        {
            var localAuthorities =
                (List<dynamic>)JsonConvert.DeserializeObject<List<dynamic>>(_laService.GetLocalAuthorities());

            var filteredResults = localAuthorities
                .Where(la => la.LANAME.ToString().ToLowerInvariant().Contains(name.ToLowerInvariant()))
                .Select(la => new LaViewModel() { id = la.id, LaName = la.LANAME })
                .ToList();
            return filteredResults;
        }

        public LaViewModel SearchExactMatch(string name)
        {
            var localAuthorities =
                (List<dynamic>)JsonConvert.DeserializeObject<List<dynamic>>(_laService.GetLocalAuthorities());

            var filteredResult = localAuthorities
                .Where(la => la.LANAME.ToString().ToLowerInvariant().Equals(name.ToLowerInvariant()))
                .Select(la => new LaViewModel() { id = la.id, LaName = la.LANAME })
                .FirstOrDefault();
            return filteredResult;
        }
    }
}