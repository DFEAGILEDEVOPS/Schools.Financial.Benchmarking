using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFB.Web.Common;
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.Domain.Models;

namespace SFB.Web.Domain.Services.Comparison
{
    public interface IComparisonService
    {
        Task<ComparisonResult> GenerateBenchmarkListWithAdvancedComparisonAsync(BenchmarkCriteria criteria,
            EstablishmentType estType, int basketSize = 30);

        Task<ComparisonResult> GenerateBenchmarkListWithSimpleComparisonAsync(BenchmarkCriteria benchmarkCriteria,
            EstablishmentType estType, int basketSize,
            SimpleCriteria simpleCriteria, SchoolFinancialDataModel defaultSchoolFinancialDataModel);
    }
}
