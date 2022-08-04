using SFB.Web.ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SFB.Web.UI.Helpers
{
    public static class FormatHelpers
    {
        public class LinkItem
        {
            public string Url { get; set; }
            public string Name { get; set; }
            public bool IsExternal { get; set; }
        }

        public static string GrammarCase(string[] stringList)
        {
            string result = "";
            int index = 0;

            foreach (string entry in stringList)
            {
                result += entry;
                if (index < stringList.Length - 2)
                    result += ", ";
                else if (index == stringList.Length - 2)
                    result += " and ";

                index++;
            }

            return result;
        }

        public static double ConvertBytesToMegabytes(long bytes, int decimalPlaces = 2, double minimumValue = 0)
        {
            var mb = Math.Round((double)bytes / 1024 / 1024, decimalPlaces);
            return mb > minimumValue ? mb : minimumValue;
        }

        public static string ConcatNonEmpties(string separator, params dynamic[] items)
        {
            return string.Join(separator, items.Where(x => x != null && !string.IsNullOrWhiteSpace(x.Value)));
        }

        public static string ConcatNonEmpties(string separator, params string[] items)
        {
            return string.Join(separator, items.Where(x => x != null && !string.IsNullOrWhiteSpace(x)));
        }

        public static string BooleanFormat(bool value)
        {
            if (value) return "Yes";
            return "No";
        }

        public static string BooleanFormat(bool? value)
        {
            if (!value.HasValue) return "Not Specified";
            if (value.Value) return "Yes";
            return "No";
        }

        public static string CreateAnalyticsPath(params string[] items)
        {
            items = items?.Where(x => !string.IsNullOrWhiteSpace(x) && x != "/").Select(x => new string(x.ToCharArray().Select(c => char.IsLetterOrDigit(c) || c == '/' ? c : '-').ToArray())).ToArray();
            if (items != null && items.Length > 0)
            {
                items = items.Select(x => x.TrimEnd('/').TrimStart('/')).ToArray();
                var retVal = string.Join("/", items).ToLower();
                if (!retVal.StartsWith("/")) retVal = "/" + retVal;
                return retVal;
            }
            else return "/";
        }
        
        public static bool IsNullOrEmpty(this string text) => string.IsNullOrWhiteSpace(text);
        public static double? ToDouble(this string text)
        {
            if (!text.IsNullOrEmpty() && double.TryParse(text, out double temp)) return temp;
            else return null;
        }
        
        public static string Clean(this string text)
        {
            text = text?.Trim();
            if (text.IsNullOrEmpty()) return null;
            else return text;
        }
        
        public static string Remove(this string data, params string[] stringsToRemove)
        {
            if (data == null || string.IsNullOrWhiteSpace(data)) return null;
            foreach (var item in stringsToRemove) data = data.Replace(item, string.Empty);
            return data;
        }
        
        public static string GetPart(this string data, string separator, int index = 0)
        {
            data = data.Clean();
            if (data == null)
            {
                return null;
            }

            var bits = data.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            if(index <= bits.GetUpperBound(0))
            {
                return bits[index];
            }

            return null;
        }
        
        public static bool IsUkPostCode(this string text)
        {
            return Regex.IsMatch(text,
                @"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})",
                RegexOptions.IgnoreCase);
        }
    }
}