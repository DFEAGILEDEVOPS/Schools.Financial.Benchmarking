using SFB.Web.Domain.Models;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SFB.Web.UI.Controllers
{
    public class LocationController : Controller
    {
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;

        public LocationController(IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
        {
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
        }
        
        public ActionResult Suggest(string locationOrPostcode, bool openOnly = true)
        {
            var suggestions = TempData["LocationResults"] as SuggestionQueryResult;
            var result = new List<LocationViewModel>();
            foreach (var item in suggestions.Matches)
            {
                result.Add(new LocationViewModel(item.Text, item.LatLon));
            }

            var vm = new LocationListViewModel(result, _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie(), locationOrPostcode, string.Empty, openOnly);

            return View(vm);
        }
    }
}