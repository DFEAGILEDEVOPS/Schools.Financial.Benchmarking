using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SFB.Web.UI.Helpers
{
    public static class HtmlHelperExtensions
    {
        private const string AssetsPath = "public/build/";
        /// <summary>
        /// Outputs the path to the requested js by name (ignoring file hash)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="path"></param>
        /// <returns>path to js file</returns>
        public static string GetWebpackScriptUrl(this HtmlHelper helper, string expression, string path = null)
        {
            if (path == null)
            {
                path = HttpRuntime.AppDomainAppPath;
            }

            var files = Directory.GetFiles(Path.Combine(path, AssetsPath), expression).Select(Path.GetFileName).ToList();

            return files.Count == 1 ? $"/{AssetsPath}/{files[0]}" : "";
        }
    }
}