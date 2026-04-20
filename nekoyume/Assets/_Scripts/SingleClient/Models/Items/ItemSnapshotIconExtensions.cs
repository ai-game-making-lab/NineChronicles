#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.Helper;
using UnityEngine;
using Lib9cItemSubType = Nekoyume.Model.Item.ItemSubType;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Snapshot-side mirror of <see cref="ItemExtensions.GetIconSprite(Nekoyume.Model.Item.ItemBase)"/>.
    /// Resolves the UI icon sprite for an <see cref="IItemSnapshot"/> without requiring a
    /// downcast to lib9c <c>ItemBase</c>, so migrating Item views can consume the snapshot
    /// directly.
    /// </summary>
    /// <remarks>
    /// Equipment deliberately reads <see cref="EquipmentSnapshot.IconId"/> instead of
    /// <see cref="IItemSnapshot.Id"/>: lib9c's custom-craft flow assigns a divergent
    /// <c>IconId</c> (e.g. a random-only icon id) while keeping <c>Id</c> pinned to the
    /// <c>EquipmentItemSheet</c> row id. Every other subtype keys off <c>Id</c> plus the
    /// subtype/grade fallback, matching <see cref="SpriteHelper.GetItemIcon(int, Lib9cItemSubType, int)"/>.
    /// </remarks>
    public static class ItemSnapshotIconExtensions
    {
        public static Sprite GetIconSprite(this IItemSnapshot item)
        {
            if (item is null)
            {
                return null;
            }

            if (item is EquipmentSnapshot equipment)
            {
                return SpriteHelper.GetItemIcon(equipment.IconId);
            }

            // Cast the client-owned ItemSubType back to lib9c's enum for the SpriteHelper
            // overload. The two enums are kept ordinal-compatible (see ItemEnums.cs), so the
            // (int) cast is lossless. Once SpriteHelper itself grows a client-enum overload we
            // can drop the cast.
            var lib9cSubType = (Lib9cItemSubType)(int)item.ItemSubType;
            return SpriteHelper.GetItemIcon(item.Id, lib9cSubType, item.Grade);
        }
    }
}

#endif
