#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.Game;
using Nekoyume.Helper;
using Nekoyume.L10n;
using Nekoyume.Model.Buff;
using Nekoyume.SingleClient.Models.Buffs;
using UnityEngine;

namespace Nekoyume
{
    public static class BuffSheetRowExtension
    {
        public static string GetLocalizedName(this Buff buff)
        {
            return L10nManager.Localize($"BUFF_NAME_{buff.BuffInfo.Id}");
        }

        public static string GetLocalizedDescription(this Buff buff)
        {
            var desc = L10nManager.Localize($"BUFF_DESCRIPTION_{buff.BuffInfo.Id}");
            if (buff is StatBuff stat)
            {
                return string.Format(desc, stat.RowData.Value);
            }
            else if (buff is ActionBuff action)
            {
                return desc;
            }

            return $"!{buff.BuffInfo.Id}!";
        }

        public static Sprite GetIcon(this Buff buff, TableSheets tableSheets)
        {
            return BuffHelper.GetBuffIcon(buff, tableSheets);
        }

        public static string GetLocalizedName(this BuffView view)
        {
            return L10nManager.Localize($"BUFF_NAME_{view.Id}");
        }

        public static string GetLocalizedDescription(this BuffView view)
        {
            var desc = L10nManager.Localize($"BUFF_DESCRIPTION_{view.Id}");
            if (view.IsStatBuff)
            {
                return string.Format(desc, view.StatValue);
            }

            if (view.IsActionBuff)
            {
                return desc;
            }

            return $"!{view.Id}!";
        }

        public static Sprite GetIcon(this BuffView view, TableSheets tableSheets)
        {
            return BuffHelper.GetBuffIcon(view, tableSheets);
        }
    }
}

#endif
