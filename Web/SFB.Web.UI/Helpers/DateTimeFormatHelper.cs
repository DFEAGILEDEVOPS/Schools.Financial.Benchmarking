using System;

namespace SFB.Web.UI.Helpers
{
    public static class DateTimeFormatHelper
    {
        public static string Format(dynamic item, string format = "yyyyMMdd")
        {
            DateTime temp;
            string dateTime = item?.ToString();
            if (!string.IsNullOrWhiteSpace(dateTime) && DateTime.TryParseExact(dateTime, format, null, System.Globalization.DateTimeStyles.None, out temp))
            {
                return temp.ToString("dd/MM/yyyy");
            }
            return string.Empty;
        }
    }
}
