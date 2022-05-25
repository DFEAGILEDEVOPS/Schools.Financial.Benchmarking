using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public interface IGiasLookupService
    {
        Task<bool> GiasHasPage(int urn, bool isMat);
    }
}