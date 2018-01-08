using System.Web.Routing;

namespace SFB.Web.UI.Utils
{
    public interface IRequestContext
    {
        RequestContext GetContext();
    }
}