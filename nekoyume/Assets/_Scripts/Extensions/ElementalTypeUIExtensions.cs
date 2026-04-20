#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;
using Nekoyume.L10n;
using Nekoyume.Model.Stat;
using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.UI;

namespace Nekoyume
{
    internal static class ElementalTypeUIExtensions
    {
        private static readonly Dictionary<ElementalType, Dictionary<StatType, List<string>>> GetOptionsCache = new();


        public static IEnumerable<string> GetOptions(this ElementalType from, StatType statType)
        {
            if (statType != StatType.ATK &&
                statType != StatType.DEF)
            {
                return new List<string>();
            }

            if (GetOptionsCache.ContainsKey(from) &&
                GetOptionsCache[from].ContainsKey(statType))
            {
                return GetOptionsCache[from][statType];
            }

            if (!GetOptionsCache.ContainsKey(from))
            {
                GetOptionsCache[from] = new Dictionary<StatType, List<string>>(StatTypeComparer.Instance);
            }

            var dict = GetOptionsCache[from];

            if (!dict.ContainsKey(statType))
            {
                dict[statType] = new List<string>();
            }

            var list = dict[statType];

            if (from == ElementalType.Normal)
            {
                return list;
            }

            if (statType == StatType.ATK)
            {
                if (from.TryGetWinCase(out var lose))
                {
                    var format = L10nManager.Localize("ELEMENTAL_TYPE_OPTION_ATK_WIN_FORMAT");
                    list.Add(string.Format(format, lose.GetLocalizedString(), ElementalRules.WinMultiplier - 1));
                }
            }
            else if (statType == StatType.DEF)
            {
                if (from.TryGetLoseCase(out var win))
                {
                    var format = L10nManager.Localize("ELEMENTAL_TYPE_OPTION_DEF_LOSE_FORMAT");
                    list.Add(string.Format(format, win.GetLocalizedString(), ElementalRules.WinMultiplier - 1));
                }
            }

            return list;
        }
    }
}

#endif
