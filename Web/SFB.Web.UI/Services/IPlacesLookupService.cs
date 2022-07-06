using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public interface IPlacesLookupService
    {
        Task<PlaceDto[]> SearchAsync(string text, bool isTypeahead);
    }
}