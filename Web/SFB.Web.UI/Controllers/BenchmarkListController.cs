using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFB.Web.Domain.Services.DataAccess;
using System;

namespace SFB.Web.UI.Controllers
{
    public class BenchmarkListController : BaseController
    {
        private readonly IContextDataService _contextDataService;

        public BenchmarkListController(IContextDataService contextDataService)
        {
            _contextDataService = contextDataService;
        }

        public ActionResult Index()
        {
            var comparisonList = base.ExtractSchoolComparisonListFromCookie();

            if (comparisonList.BenchmarkSchools.Count > 1)
            {
                dynamic dynamicBenchmarkSchools = _contextDataService.GetMultipleSchoolsByUrns(comparisonList.BenchmarkSchools.Select(b => Int32.Parse(b.Urn)).ToList());

                comparisonList.BenchmarkSchools = new List<BenchmarkSchoolModel>();

                foreach (var dynamicBenchmarkSchool in dynamicBenchmarkSchools)
                {
                    var school = new SchoolViewModel(dynamicBenchmarkSchool);
                    var benchmarkSchool = new BenchmarkSchoolModel()
                    {
                        Address = school.Address,
                        Name = school.Name,
                        Phase = school.OverallPhase,
                        Type = school.Type,
                        EstabType = school.EstablishmentType.ToString(),
                        Urn = school.Id
                    };

                    comparisonList.BenchmarkSchools.Add(benchmarkSchool);
                }
            }else if (comparisonList.BenchmarkSchools.Count == 1)
            {
                var dynamicBenchmarkSchool = _contextDataService.GetSchoolByUrn(Int32.Parse(comparisonList.BenchmarkSchools[0].Urn));

                comparisonList.BenchmarkSchools = new List<BenchmarkSchoolModel>();

                var school = new SchoolViewModel(dynamicBenchmarkSchool);
                var benchmarkSchool = new BenchmarkSchoolModel()
                {
                    Address = school.Address,
                    Name = school.Name,
                    Phase = school.OverallPhase,
                    Type = school.Type,
                    EstabType = school.EstablishmentType.ToString(),
                    Urn = school.Id
                };

                comparisonList.BenchmarkSchools.Add(benchmarkSchool);
            }

            return View(comparisonList);
        }

        public PartialViewResult UpdateBenchmarkBasket(int? urn, string withAction)
        {
            HttpCookie cookie;

            if (urn.HasValue)
            {
                var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolByUrn(urn.GetValueOrDefault()), null);

                cookie = base.UpdateSchoolComparisonListCookie(withAction,
                    new BenchmarkSchoolModel()
                    {
                        Name = benchmarkSchool.Name,
                        Urn = benchmarkSchool.Id,
                        Type = benchmarkSchool.Type,
                        EstabType = benchmarkSchool.EstablishmentType.ToString()
                    });
            }
            else
            {
                cookie = base.UpdateSchoolComparisonListCookie(withAction, null);
            }

            if (cookie != null)
            {
                Response.Cookies.Add(cookie);
            }

            return PartialView("Partials/BenchmarkBasketControls", base.ExtractSchoolComparisonListFromCookie());
        }

    }
}