using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SFB.Web.UI.Helpers
{
    public static class ValidationHelper
    {
        public static MvcHtmlString GovUKValidationSummary(this HtmlHelper helper)
        {
            var modelState = helper.ViewData.ModelState;

            if (modelState.IsValid) return MvcHtmlString.Empty;

            var errors = modelState.Keys.SelectMany(k => modelState[k].Errors.Select(e => new { Key = k, Error = e }));
            var sb = new StringBuilder();
            
            sb.Append("<div class=\"error-summary\" role=\"group\" aria-labelledby=\"error-summary-heading\" tabindex=\"-1\">");
            sb.Append("<h1 class=\"heading-medium error-summary-heading\" id=\"error-summary-heading\">");
            sb.Append("Please review the following:");
            sb.Append("</h1>");
            sb.Append("<p>");
            sb.Append("One option must be selected for each question.");
            sb.Append("</p>");
            sb.Append("<ul class=\"error-summary-list\">");
            foreach(var error in errors)
            {
                sb.Append("<li>");
                sb.AppendFormat("<a href = \"#"+ error.Key.ToLower() +"\" >" + error.Error.ErrorMessage + "</a>");
                sb.Append("</li>");
            }
            sb.Append("</ul>");
            sb.Append("</div>");

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}