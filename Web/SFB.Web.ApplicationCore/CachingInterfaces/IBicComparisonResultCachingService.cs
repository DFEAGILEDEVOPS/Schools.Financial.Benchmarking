using SFB.Web.ApplicationCore.Models;

namespace SFB.Web.ApplicationCore.Services
{
    public interface IBicComparisonResultCachingService
    {
        ComparisonResult GetBicComparisonResultByUrn(int urn);
        void StoreBicComparisonResultByUrn(int urn, ComparisonResult comparisonResult);
    }
}
