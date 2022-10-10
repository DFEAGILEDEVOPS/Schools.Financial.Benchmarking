using System.Linq;
using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public class PlacesLookupService : IPlacesLookupService
    {
        private readonly IAzureMapsService _azureMapsService;

        public PlacesLookupService(IAzureMapsService azureMapsService)
        {
            _azureMapsService = azureMapsService;
        }
        
        public async Task<PlaceDto[]> SearchAsync(string text, bool isTypeahead)
        {
            return (await _azureMapsService.SearchAsync(text, isTypeahead)).ToArray();
        }
    }
}