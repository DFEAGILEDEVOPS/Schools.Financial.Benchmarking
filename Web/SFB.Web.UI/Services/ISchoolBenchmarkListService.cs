using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public interface ISchoolBenchmarkListService
    {
        Task TryAddSchoolToBenchmarkListAsync(long urn);
        Task AddSchoolToBenchmarkListAsync(long urn);
        void AddSchoolToBenchmarkList(SchoolViewModel bmSchool);        
        void AddFederationToBenchmarkList(FederationViewModel bmFederation);        
        void TryAddSchoolToBenchmarkList(SchoolViewModel bmSchool);        
        void TryAddFederationToBenchmarkList(FederationViewModel bmFederation);
        void AddSchoolToBenchmarkList(BenchmarkSchoolModel bmSchool);
        void TryAddSchoolToBenchmarkList(BenchmarkSchoolModel bmSchool);
        void AddSchoolsToBenchmarkList(ComparisonResult comparisonResult);
        Task AddSchoolsToBenchmarkListAsync(ComparisonType comparison, List<long> urnList);
        
        Task SetSchoolAsDefaultAsync(long urn);
        void SetSchoolAsDefault(SchoolViewModel benchmarkSchool);        
        void SetFederationAsDefault(FederationViewModel benchmarkSchool);        
        void SetSchoolAsDefault(BenchmarkSchoolModel benchmarkSchool);
      

        void UnsetDefaultSchool(); 

        SchoolComparisonListModel GetSchoolBenchmarkList();     

        void ClearSchoolBenchmarkList();

        Task RemoveSchoolFromBenchmarkListAsync(long urn);
    }

}
