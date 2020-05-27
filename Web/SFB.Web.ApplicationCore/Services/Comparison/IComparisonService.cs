using System.Threading.Tasks;
using SFB.Web.ApplicationCore;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Helpers.Enums;

namespace SFB.Web.ApplicationCore.Services.Comparison
{
    public interface IComparisonService
    {
        Task<ComparisonResult> GenerateBenchmarkListWithAdvancedComparisonAsync(BenchmarkCriteria criteria,
            EstablishmentType estType, int basketSize = 30);

        Task<ComparisonResult> GenerateBenchmarkListWithAdvancedComparisonAsync(BenchmarkCriteria criteria,
            EstablishmentType estType, bool excludePartial, int basketSize = 30);

        Task<ComparisonResult> GenerateBenchmarkListWithSimpleComparisonAsync(BenchmarkCriteria benchmarkCriteria,
            EstablishmentType estType, int basketSize,
            SimpleCriteria simpleCriteria, FinancialDataModel defaultSchoolFinancialDataModel);

        Task<ComparisonResult> GenerateBenchmarkListWithOneClickComparisonAsync(BenchmarkCriteria benchmarkCriteria,
            EstablishmentType estType, int basketSize, FinancialDataModel defaultSchoolFinancialDataModel);

        Task<ComparisonResult> GenerateBenchmarkListWithBestInClassComparisonAsync(EstablishmentType establishmentType, BenchmarkCriteria benchmarkCriteria,
            BestInClassCriteria bicCriteria, FinancialDataModel defaultSchoolFinancialDataModel);
    }
}
