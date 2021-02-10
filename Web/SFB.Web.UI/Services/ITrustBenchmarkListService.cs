using SFB.Web.UI.Models;
using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public interface ITrustBenchmarkListService
    {
        TrustComparisonListModel TryAddTrustToBenchmarkList(int companyNumber, string trustOrCompanyName);
        TrustComparisonListModel AddTrustToBenchmarkList(int companyNumber, string trustOrCompanyName);
        void AddDefaultTrustToBenchmarkList();

        Task SetTrustAsDefaultAsync(int companyNo);

        TrustComparisonListModel GetTrustBenchmarkList();

        TrustComparisonListModel RemoveTrustFromBenchmarkList(int companyNo);

        TrustComparisonListModel ClearTrustBenchmarkList();
    }

}
