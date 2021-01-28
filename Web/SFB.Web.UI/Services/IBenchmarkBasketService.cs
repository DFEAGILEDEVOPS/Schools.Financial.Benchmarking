using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public interface IBenchmarkBasketService
    {
        Task AddSchoolToBenchmarkListAsync(int urn);
        void AddSchoolToBenchmarkList(SchoolViewModel bmSchool);
        void AddSchoolToBenchmarkList(BenchmarkSchoolModel bmSchool);
        void AddSchoolToManualBenchmarkList(SchoolViewModel benchmarkSchool);
        Task AddSchoolToManualBenchmarkListAsync(int urn);
        void AddSchoolsToBenchmarkList(ComparisonResult comparisonResult);
        Task AddSchoolsToBenchmarkListAsync(ComparisonType comparison, List<int> urnList);
        TrustComparisonListModel AddTrustToBenchmarkList(int companyNumber, string trustOrCompanyName);
        
        Task SetSchoolAsDefaultAsync(int urn);
        void SetSchoolAsDefault(SchoolViewModel benchmarkSchool);        
        void SetSchoolAsDefault(BenchmarkSchoolModel benchmarkSchool);
        void SetSchoolAsDefaultInManualComparisonList(SchoolComparisonListModel schoolComparisonList);
        Task SetTrustAsDefaultAsync(int companyNo);
        Task SetSchoolAsDefaultInManualComparisonList(int urn);

        void UnsetDefaultSchool();
        void UnsetDefaultSchoolInManualBenchmarkList();

        SchoolComparisonListModel GetSchoolBenchmarkList();         
        SchoolComparisonListModel GetManualBenchmarkList();
        TrustComparisonListModel GetTrustBenchmarkList();

        TrustComparisonListModel ClearTrustBenchmarkList();
        void ClearSchoolBenchmarkList();
        void ClearManualBenchmarkList();

        Task RemoveSchoolFromBenchmarkListAsync(int urn);
        Task RemoveSchoolFromManualBenchmarkListAsync(int urn);
        TrustComparisonListModel RemoveTrustFromBenchmarkList(int companyNo);

        void AddDefaultTrustToBenchmarkList();
    }
}
