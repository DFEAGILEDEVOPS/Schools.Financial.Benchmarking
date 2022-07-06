using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public interface IAzureMapsService
    {
        Task<PlaceDto[]> SearchAsync(string text, bool isTypeahead);
    }
}