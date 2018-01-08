using System.Web;
using System.Web.Routing;

namespace SFB.Web.UI.Utils
{
    public class RequestContextWrapper : IRequestContext
    {
        public RequestContext GetContext()
        {
            return HttpContext.Current.Request.RequestContext;
        }
    }
}