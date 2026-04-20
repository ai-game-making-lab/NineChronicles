#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;

namespace Nekoyume.Helper
{
    public class MarkupHelper
    {
        private static readonly Dictionary<string, string> Markups = new()
        {
            { "[comma]", "," },
            { "[newline]", "\n" }
        };

        public static void ReplaceMarkups(ref string value)
        {
            foreach (var markup in Markups)
            {
                if (value.Contains(markup.Key))
                {
                    value = value.Replace(markup.Key, markup.Value);
                }
            }
        }
    }
}

#endif
