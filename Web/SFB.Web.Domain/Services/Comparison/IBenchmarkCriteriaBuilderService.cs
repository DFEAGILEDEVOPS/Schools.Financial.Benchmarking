using SFB.Web.Common;
using SFB.Web.Domain.Models;

namespace SFB.Web.Domain.Services.Comparison
{
    public interface IBenchmarkCriteriaBuilderService
    {
        BenchmarkCriteria BuildFromSimpleComparisonCriteria(FinancialDataModel benchmarkSchoolData, SimpleCriteria simpleCriteria, int percentageMargin = 0);
        BenchmarkCriteria BuildFromSimpleComparisonCriteria(FinancialDataModel benchmarkSchoolData, bool includeFsm, bool includeSen, bool includeEal, bool includeLa, int percentageMargin = 0);
        BenchmarkCriteria BuildFromBicComparisonCriteria(FinancialDataModel benchmarkSchoolData, BestInClassCriteria bicCriteria, int percentageMargin = 0);
    }
}
