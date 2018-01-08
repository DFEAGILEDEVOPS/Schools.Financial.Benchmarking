using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.Domain.Services;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Controllers
{
    public class LaController : BaseController
    {
        private readonly ISchoolApiService _schoolApiService;

        public LaController(ISchoolApiService schoolApiService)
        {
            _schoolApiService = schoolApiService;
        }

        public ActionResult Search(string name, string orderby = "", int page = 1)
        {
            var localAuthorities = (List<dynamic>)_schoolApiService.GetLocalAuthorities();

            var filteredResults = localAuthorities
                .Where(la => la.LANAME.ToLowerInvariant().Contains(name.ToLowerInvariant()))
                .Select(la => new LaViewModel(){id = la.id, LaName = la.LANAME}).ToList();
            
            var vm = new LaListViewModel(filteredResults, base.ExtractSchoolComparisonListFromCookie(), orderby);
            
            vm.Pagination = new Pagination
                {
                    Start = (SearchDefaults.RESULTS_PER_PAGE * (page - 1)) + 1,
                    Total = filteredResults.Count,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.RESULTS_PER_PAGE
                };

            return View(vm);
        }
    }
}