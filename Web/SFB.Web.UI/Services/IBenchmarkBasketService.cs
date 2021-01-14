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
        Task AddSchoolsToBenchmarkListFromUrnsAsync(ComparisonType comparison, List<int> urnList);
        void EmptyBenchmarkList();
        Task SetSchoolAsDefaultFromUrnAsync(int urn);
        Task SetTrustAsDefaultFromCompanyNoAsync(int companyNo);
        void SetSchoolAsDefaultFromViewModel(SchoolViewModel benchmarkSchool);
        SchoolComparisonListModel GetSchoolComparisonList();         
        SchoolComparisonListModel GetManualComparisonList();
        TrustComparisonListModel GetTrustComparisonList();

        //TODO: Inherit from BenchmarkBasketCookieManager and leave these methods in BenchmarkBasketCookieManager. 
        //Hide cookie implementation details in BenchmarkBasketCookieManager
        void UpdateSchoolComparisonListCookie(CookieActions withAction, BenchmarkSchoolModel benchmarkSchool);
        void UpdateManualComparisonListCookie(CookieActions withAction, BenchmarkSchoolModel benchmarkSchool);
        TrustComparisonListModel UpdateTrustComparisonListCookie(CookieActions withAction, int? companyNo = null, string matName = null);

    }
}
