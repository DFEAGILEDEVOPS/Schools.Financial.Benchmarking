using SFB.Web.Domain.Models;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Services
{
    public interface IStatisticalCriteriaBuilderService
    {
        BenchmarkCriteria Build(SchoolViewModel benchmarkSchool, bool includeFsm, bool includeSen, bool includeEal, bool includeLa, int percentageMargin);
    }
}
