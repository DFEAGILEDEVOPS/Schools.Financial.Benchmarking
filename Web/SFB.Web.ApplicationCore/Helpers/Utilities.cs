using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SFB.Web.ApplicationCore.Helpers
{
    public static class Utilities
    {
        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Finds text within a string (case-insensitive, unlike Contains())
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static bool ContainsString(this string text, string search) => text?.IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1;

        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.
        /// </summary>
        /// <param name="input">n/a</param>
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string to replace all occurrences of oldValue.</param>
        /// <param name="caseInsensitive">Whether to search in a case-insensitive way</param>
        /// <returns>
        ///     A string that is equivalent to the current string except that all instances of
        ///     oldValue are replaced with newValue. If oldValue is not found in the current
        ///     instance, the method returns the current instance unchanged.
        /// </returns>
        public static string Replace(this string input, string oldValue, string newValue, bool caseInsensitive)
        {
            if (!caseInsensitive) return input?.Replace(oldValue, newValue);
            else return Regex.Replace(input, Regex.Escape(oldValue), newValue.Replace("$", "$$"), RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// Returns the value from a dictionary or NULL (as opposed to throwing KeyNotFound exception)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T2 Get<T1, T2>(this IDictionary<T1, T2> data, params T1[] keys)
        {
            var k = keys.Where(x => data.ContainsKey(x)).FirstOrDefault();
            if (k != null) return data[k];
            else return default(T2);
        }

    }
}
