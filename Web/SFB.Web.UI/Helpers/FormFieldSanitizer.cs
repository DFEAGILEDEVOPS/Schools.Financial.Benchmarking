using System.Web;

namespace SFB.Web.UI.Helpers
{
    public static class FormFieldSanitizer
    {
        public static string SanitizeFormField(string text)
        {
            if (text.Contains("=") || text.Contains(";"))
            {
                throw new HttpRequestValidationException("Possible SQL injection attack!");
            }

            return text;
        }
    }
}
