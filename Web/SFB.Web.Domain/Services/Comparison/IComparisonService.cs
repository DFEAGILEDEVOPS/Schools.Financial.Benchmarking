using System.Threading.Tasks;
using SFB.Web.Common;
using SFB.Web.Domain.Models;
using System.Collections.Generic;

namespace SFB.Web.Domain.Services.Comparison
{
    public interface IComparisonService
    {
        Task<ComparisonResult> GenerateBenchmarkListWithAdvancedComparisonAsync(BenchmarkCriteria criteria,
            EstablishmentType estType, int basketSize = 30);

        Task<ComparisonResult> GenerateBenchmarkListWithSimpleComparisonAsync(BenchmarkCriteria benchmarkCriteria,
            EstablishmentType estType, int basketSize,
            SimpleCriteria simpleCriteria, FinancialDataModel defaultSchoolFinancialDataModel);

        List<BestInClassResult> GenerateBenchmarkListWithBestInClassComparison(int urn, string phase = null);

        bool IsBestInClassComparisonAvailable(int urn);

        bool IsMultipleEfficienctMetricsAvailable(int urn);
    }
}
