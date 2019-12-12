using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SFB.Web.UI.Helpers
{
    public static class ExtensionMethods
    {
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }

            return null;
        }

        public static string FormatMoney(this decimal amount)
        {
            if (amount >= 1000000)
            {
                return $"£{(amount / 1000000).ToString("0.##")}m";
            }

            if (amount <= -1000000)
            {
                return $"-£{Math.Abs(amount / 1000000).ToString("0.##")}m";
            }

            if (amount >= 10000)
            {
                return $"£{(amount / 1000).ToString("0.#")}k";
            }

            if (amount <= -10000)
            {
                return $"-£{Math.Abs(amount / 1000).ToString("0.#")}k";
            }

            return amount.ToString("C0");
        }


        public static KeyValuePair<string, object> WithValue(this string key, object value)
        {
            return new KeyValuePair<string, object>(key, value);
        }

        public static List<string> Tokens(this string s, params string[] knownTokens)
        {
            try
            {
                var tokens = new List<string>();
                var rx = new Regex(@"(\$\([a-zA-Z0-9]+\))+");
                var rxTokenName = new Regex("[0-9a-zA-Z]+");
                foreach (var m in rx.Matches(s))
                {
                    var tokenValue = rxTokenName.Match(m.ToString()).Value;
                    if (knownTokens.Length == 0)
                    {
                        tokens.Add(tokenValue);
                        continue;
                    }

                    if (knownTokens.Contains(tokenValue))
                    {
                        tokens.Add(tokenValue);
                        continue;
                    }

                    throw new Exception($"Unknown token '{tokenValue}'");
                }

                return tokens;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static string ReplaceToken(this string s, string token, string replacementValue)
        {
            try
            {
                var rxSub = new Regex($"(\\$\\({token}\\))+");
                return rxSub.Replace(s, replacementValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static bool IsAllPropertiesNull(this object myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                    var value = pi.GetValue(myObject);
                    if (value != null)
                    {
                        return false;
                    }
            }

            return true;
        }
    }
}