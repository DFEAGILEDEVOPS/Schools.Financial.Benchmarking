using System.Web.Mvc;
using System.Web.Routing;

namespace SFB.Web.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "DownloadDataSet",
                url: "Download/{estab}/DataSet",
                defaults: new { controller = "Download", action = "DataSet" }
            );

            routes.MapRoute(
                name: "Download",
                url: "Download/{estab}",
                defaults: new { controller = "Download", action = "AcademicYear" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index"}
            );

        }
    }
}
