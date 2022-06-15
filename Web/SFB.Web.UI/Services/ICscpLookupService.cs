using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public interface ICscpLookupService
    {
        Task<bool> CscpHasPage(int urn, bool isMat);
    }
}