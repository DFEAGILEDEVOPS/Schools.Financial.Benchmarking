using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System;
using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public class ManualBenchmarkListService : BenchmarkBasketCookieManager, IManualBenchmarkListService
    {
        private readonly IContextDataService _contextDataService;

        public ManualBenchmarkListService(IContextDataService contextDataService)
        {
            _contextDataService = contextDataService;
        }

        public void TryAddSchoolToManualBenchmarkList(SchoolViewModel bmSchool)
        {
            try
            {
                UpdateManualComparisonListCookie(CookieActions.Add, new BenchmarkSchoolModel(bmSchool));
            }
            catch (ApplicationException) { } //Ignore duplicate school additions
        }
        public void AddSchoolToManualBenchmarkList(SchoolViewModel bmSchool)
        {
            UpdateManualComparisonListCookie(CookieActions.Add, new BenchmarkSchoolModel(bmSchool));
        }

        public async Task AddSchoolToManualBenchmarkListAsync(long urn)
        {
            var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), null);
            AddSchoolToManualBenchmarkList(benchmarkSchool);
        }

        public async Task RemoveSchoolFromManualBenchmarkListAsync(long urn)
        {
            var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), null);

            UpdateManualComparisonListCookie(CookieActions.Remove, new BenchmarkSchoolModel(benchmarkSchool));
        }

        public void UnsetDefaultSchoolInManualBenchmarkList()
        {
            UpdateManualComparisonListCookie(CookieActions.UnsetDefault);
        }

        public void ClearManualBenchmarkList()
        {
            UpdateManualComparisonListCookie(CookieActions.RemoveAll);
        }

        public SchoolComparisonListModel GetManualBenchmarkList()
        {
            return ExtractManualComparisonListFromCookie();
        }

        public void SetSchoolAsDefaultInManualBenchmarkList(SchoolComparisonListModel schoolComparisonList)
        {
            UpdateManualComparisonListCookie(CookieActions.SetDefault, new BenchmarkSchoolModel(schoolComparisonList));
        }

        public async Task SetSchoolAsDefaultInManualBenchmarkList(long urn)
        {
            var bmSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), null);
            UpdateManualComparisonListCookie(CookieActions.SetDefault, new BenchmarkSchoolModel(bmSchool));
        }
    }
 }
