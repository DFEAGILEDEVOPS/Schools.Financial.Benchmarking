using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SFB.Web.UI.Controllers
{
    public class LocationController : Controller
    {
        private readonly ISchoolBenchmarkListService _benchmarkBasketService;
        private readonly ILocationSearchService _locationSearchService;

        public LocationController(ISchoolBenchmarkListService benchmarkBasketService, ILocationSearchService locationSearchService)
        {
            _benchmarkBasketService = benchmarkBasketService;
            _locationSearchService = locationSearchService;
        }
        
        public ActionResult Suggest(string locationOrPostcode, bool openOnly = false)
        {
            var suggestions = TempData["LocationResults"] as SuggestionQueryResult;
            var searchMethod = TempData["SearchMethod"] as string;
            if(suggestions == null)
            {
                suggestions = _locationSearchService.SuggestLocationName(locationOrPostcode);
            }

            var result = new List<LocationViewModel>();

            suggestions.Matches.ForEach(item => result.Add(new LocationViewModel(item.Text, item.LatLon)));

            var vm = new LocationListViewModel(result, _benchmarkBasketService.GetSchoolBenchmarkList(), locationOrPostcode, string.Empty, openOnly, searchMethod);

            return View(vm);
        }
    }
}