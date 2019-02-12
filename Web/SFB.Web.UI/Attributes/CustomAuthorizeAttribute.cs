using System.Configuration;
using System.Web.Mvc;

namespace SFB.Web.UI.Attributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["AuthenticationEnabled"] ?? "false"))
            {
                base.OnAuthorization(filterContext);
            }
        }
    }
}