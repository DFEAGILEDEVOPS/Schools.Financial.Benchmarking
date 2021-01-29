using SFB.Web.UI.Models;
using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public interface IManualBenchmarkListService
    {
        Task AddSchoolToManualBenchmarkListAsync(int urn);
        void AddSchoolToManualBenchmarkList(SchoolViewModel benchmarkSchool);
        void TryAddSchoolToManualBenchmarkList(SchoolViewModel benchmarkSchool);

        void SetSchoolAsDefaultInManualBenchmarkList(SchoolComparisonListModel schoolComparisonList); 
        Task SetSchoolAsDefaultInManualBenchmarkList(int urn);

        void UnsetDefaultSchoolInManualBenchmarkList();

        SchoolComparisonListModel GetManualBenchmarkList();

        Task RemoveSchoolFromManualBenchmarkListAsync(int urn);

        void ClearManualBenchmarkList();
    }

}
