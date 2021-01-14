using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public interface IBenchmarkBasketService
    {
        void AddDefaultSchoolToBenchmarkList(SchoolViewModel bmSchool);
        void AddSchoolsToBenchmarkListFromComparisonResult(ComparisonResult comparisonResult);
        Task AddSchoolsToBenchmarkListFromURNsAsync(ComparisonType comparison, List<int> urnList);
        void EmptyBenchmarkList();
        Task SetSchoolAsDefaultFromUrnAsync(int urn);
        void SetSchoolAsDefaultFromViewModel(SchoolViewModel benchmarkSchool);
        SchoolComparisonListModel ExtractSchoolComparisonListFromCookie();         
        SchoolComparisonListModel ExtractManualComparisonListFromCookie();
        void UpdateSchoolComparisonListCookie(CookieActions withAction, BenchmarkSchoolModel benchmarkSchool);
        void UpdateManualComparisonListCookie(CookieActions withAction, BenchmarkSchoolModel benchmarkSchool);
        TrustComparisonListModel ExtractTrustComparisonListFromCookie();
        TrustComparisonListModel UpdateTrustComparisonListCookie(CookieActions withAction, int? companyNo = null, string matName = null);

    }
}
