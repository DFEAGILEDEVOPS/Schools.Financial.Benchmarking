using System.Web;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Helpers
{
    public interface IBenchmarkBasketCookieManager
    {
        SchoolComparisonListModel ExtractSchoolComparisonListFromCookie();
        void UpdateSchoolComparisonListCookie(string withAction, BenchmarkSchoolModel benchmarkSchool);
        TrustComparisonListModel ExtractTrustComparisonListFromCookie();
        //TODO: Add the update method for trusts
    }
}