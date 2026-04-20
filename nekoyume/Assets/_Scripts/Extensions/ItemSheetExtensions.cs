#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Linq;
using System.Security.Cryptography;
using Libplanet.Common;
using Nekoyume.L10n;
using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.TableData;

namespace Nekoyume
{
    public static class ItemSheetExtensions
    {
        public static string GetLocalizedName(
            this ItemSheet.Row value,
            bool hasColor = true,
            bool useElementalIcon = true)
        {
            if (value is EquipmentItemSheet.Row equipmentRow)
            {
                if (hasColor)
                {
                    return equipmentRow.GetLocalizedName(0, useElementalIcon);
                }

                return LocalizationExtensions.GetLocalizedNonColoredName(
                    equipmentRow.ElementalType.ToView(),
                    equipmentRow.Id,
                    useElementalIcon);
            }

            if (value is ConsumableItemSheet.Row consumableRow)
            {
                return consumableRow.GetLocalizedName(hasColor);
            }

            return LocalizationExtensions.GetLocalizedNonColoredName(
                value.ElementalType.ToView(), value.Id, useElementalIcon);
        }

        public static string GetLocalizedDescription(this ItemSheet.Row value)
        {
            return L10nManager.Localize($"ITEM_DESCRIPTION_{value.Id}");
        }

        public static bool TryGetLocalizedName(
            this MaterialItemSheet sheet,
            HashDigest<SHA256> fungibleId,
            out string name)
        {
            if (sheet is null)
            {
                name = null;
                return false;
            }

            var row = sheet.OrderedList!.FirstOrDefault(row => row.ItemId.Equals(fungibleId));
            if (row is null)
            {
                name = null;
                return false;
            }

            name = row.GetLocalizedName();
            return true;
        }
    }
}

#endif
