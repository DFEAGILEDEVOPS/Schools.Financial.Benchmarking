using SFB.Web.Common.DataObjects;

namespace SFB.Web.Domain.Services.DataAccess
{
    public interface IBestInBreedDataService
    {
        BestInBreedDataObject GetBestInBreedDataObjectByUrn(int urn);
    }
}
