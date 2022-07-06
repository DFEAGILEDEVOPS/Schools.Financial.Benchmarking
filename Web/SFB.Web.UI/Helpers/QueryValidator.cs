using System;
using System.Collections.Generic;
using System.Linq;

namespace SFB.Web.UI.Helpers
{
 public static class QueryValidator
    {
        private static readonly char[] ALLOWED_SYMBOLS = new char[5]
        {
            '\'',
            '.',
            '-',
            '!',
            ','
        };

        public static bool ValidatePlaceSuggestionQuery(string query) => !string.IsNullOrWhiteSpace(query) && query.Length <= 50 && query.All<char>((Func<char, bool>) (character => char.IsLetterOrDigit(character) || char.IsWhiteSpace(character) || ((IEnumerable<char>) QueryValidator.ALLOWED_SYMBOLS).Contains<char>(character)));
    }
}