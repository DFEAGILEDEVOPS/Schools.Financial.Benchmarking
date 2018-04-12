using SFB.Web.Common;
using SFB.Web.Domain.Models;

namespace SFB.Web.Domain.Services.Comparison
{
    public interface IBenchmarkCriteriaBuilderService
    {
        BenchmarkCriteria BuildFromSimpleComparisonCriteria(SchoolDataModel benchmarkSchoolData, SimpleCriteria simpleCriteria, int percentageMargin = 0);
        BenchmarkCriteria BuildFromSimpleComparisonCriteria(SchoolDataModel benchmarkSchoolData, bool includeFsm, bool includeSen, bool includeEal, bool includeLa, int percentageMargin = 0);
    }
}
