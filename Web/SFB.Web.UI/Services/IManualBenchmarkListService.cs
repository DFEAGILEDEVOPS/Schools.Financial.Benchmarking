using SFB.Web.UI.Models;
using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public interface IManualBenchmarkListService
    {
        Task AddSchoolToManualBenchmarkListAsync(long urn);
        void AddSchoolToManualBenchmarkList(SchoolViewModel benchmarkSchool);
        void TryAddSchoolToManualBenchmarkList(SchoolViewModel benchmarkSchool);

        void SetSchoolAsDefaultInManualBenchmarkList(SchoolComparisonListModel schoolComparisonList); 
        Task SetSchoolAsDefaultInManualBenchmarkList(long urn);

        void UnsetDefaultSchoolInManualBenchmarkList();

        SchoolComparisonListModel GetManualBenchmarkList();

        Task RemoveSchoolFromManualBenchmarkListAsync(long urn);

        void ClearManualBenchmarkList();
    }

}
