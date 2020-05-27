using SFB.Web.UI.Models;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Helpers
{
    public interface IBenchmarkBasketCookieManager
    {
        SchoolComparisonListModel ExtractSchoolComparisonListFromCookie();
        SchoolComparisonListModel ExtractManualComparisonListFromCookie();
        void UpdateSchoolComparisonListCookie(CookieActions withAction, BenchmarkSchoolModel benchmarkSchool);
        void UpdateManualComparisonListCookie(CookieActions withAction, BenchmarkSchoolModel benchmarkSchool);
        TrustComparisonListModel ExtractTrustComparisonListFromCookie();
        TrustComparisonListModel UpdateTrustComparisonListCookie(CookieActions withAction, int? companyNo = null, string matName = null);
    }
}