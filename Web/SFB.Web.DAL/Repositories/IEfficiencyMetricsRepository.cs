using SFB.Web.Common.DataObjects;

namespace SFB.Web.DAL.Repositories
{
    public interface IEfficiencyMetricsRepository
    {
        BestInBreedDataObject GetBestInBreedDataObjectByUrn(int urn);
    }
}
