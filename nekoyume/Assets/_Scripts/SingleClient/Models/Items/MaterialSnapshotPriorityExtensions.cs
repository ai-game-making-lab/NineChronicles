using System.Linq;
using Nekoyume.Action;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Client mirror of lib9c <c>InventoryExtensions.GetMaterialPriority(Material)</c>. UI sites
    /// (<c>Inventory.cs</c>, <c>GrindModule.cs</c>) sort material inventory by this priority;
    /// exposing the same ordering on <see cref="MaterialSnapshot"/> lets consumers migrate off the
    /// <c>ItemBase as Material</c> downcast pattern.
    /// </summary>
    public static class MaterialSnapshotPriorityExtensions
    {
        // Dust material IDs from CostType enum. Inlined here instead of reading the private
        // InventoryExtensions.DustIds so the SingleClient surface stays independent of the lib9c
        // file's internals; update in lock-step if CostType changes.
        private static readonly int[] DustIds =
        {
            800201, // SilverDust
            600201, // GoldDust
            600202, // RubyDust
            600203, // EmeraldDust
            600206, // SapphireDust
        };

        public static int GetMaterialPriority(this MaterialSnapshot material)
        {
            if (DustIds.Contains(material.Id))
            {
                return 0;
            }

            if (material.Id == 500001)
            {
                return 1;
            }

            if (material.ItemSubType == ItemSubType.ApStone ||
                material.ItemSubType == ItemSubType.Hourglass)
            {
                return 2;
            }

            if (material.ItemSubType == ItemSubType.Circle ||
                material.ItemSubType == ItemSubType.Scroll)
            {
                return 3;
            }

            if (ItemEnhancement.HammerIds.Contains(material.Id))
            {
                return 4;
            }

            return int.MaxValue;
        }

        /// <summary>
        /// Compares the snapshot's hex-encoded <see cref="MaterialSnapshot.TradableId"/> against a
        /// lib9c <see cref="Nekoyume.Model.Item.Material.ItemId"/> hash. Plain <c>Material</c>
        /// (non-tradable) snapshots always return <see langword="false"/> (empty TradableId).
        /// </summary>
        public static bool MatchesItemId(
            this MaterialSnapshot material,
            Nekoyume.Model.Item.Material lib9cMaterial)
        {
            if (lib9cMaterial is null || string.IsNullOrEmpty(material.TradableId))
            {
                return false;
            }

            return string.Equals(material.TradableId, lib9cMaterial.ItemId.ToString(), System.StringComparison.Ordinal);
        }
    }
}
