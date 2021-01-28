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
        void AddSchoolsToBenchmarkList(ComparisonResult comparisonResult);
        Task AddSchoolsToBenchmarkListAsync(ComparisonType comparison, List<int> urnList);
        void AddTrustToBenchmarkList(int companyNumber, string trustOrCompanyName);
        Task SetSchoolAsDefaultAsync(int urn);
        void SetSchoolAsDefault(SchoolViewModel benchmarkSchool);        
        void UnsetDefaultSchool();
        Task SetTrustAsDefaultAsync(int companyNo);
        SchoolComparisonListModel GetSchoolBenchmarkList();         
        SchoolComparisonListModel GetManualBenchmarkList();
        TrustComparisonListModel GetTrustBenchmarkList();
        void ClearTrustBenchmarkList();
        void ClearSchoolBenchmarkList();
        void ClearManualBenchmarkList();
        void RemoveSchoolFromBenchmarkList(int urn);
    }
}
