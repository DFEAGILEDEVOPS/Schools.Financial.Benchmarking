using SFB.Web.Common.DataObjects;

namespace SFB.Web.DAL.Repositories
{
    public interface IEfficiencyMetricsRepository
    {
        BestInBreedDataObject GetBestInClassDataObjectByUrn(int urn);
        BestInBreedDataObject GetBestInClassDataObjectByURNAndDataGroup(int urn, string dataGroup);
    }
}
