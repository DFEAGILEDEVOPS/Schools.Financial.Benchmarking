using SFB.Web.Common.DataObjects;

namespace SFB.Web.Domain.Services.DataAccess
{
    public interface IBestInBreedDataService
    {
        BestInBreedDataObject GetBestInClassDataObjectByUrnAndPhase(int urn, string phase = null);
    }
}
