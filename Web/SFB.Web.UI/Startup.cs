using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SFB.Web.UI.Startup))]
namespace SFB.Web.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
