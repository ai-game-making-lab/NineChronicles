using System;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Client-owned mirror of <c>Nekoyume.Model.Item.ItemType</c>. Enum ordering matches lib9c
    /// 1:1 so direct (int) casts stay lossless while callers migrate off the lib9c namespace.
    /// </summary>
    public enum ItemType
    {
        Consumable = 0,
        Costume = 1,
        Equipment = 2,
        Material = 3,
    }

    /// <summary>
    /// Client-owned mirror of <c>Nekoyume.Model.Item.ItemSubType</c>. Enum ordering and
    /// membership matches lib9c; <c>Chest</c> is retained because lib9c still references it even
    /// though it is annotated <see cref="ObsoleteAttribute"/>.
    /// </summary>
    public enum ItemSubType
    {
        // Consumable
        Food = 0,

        // Costume
        FullCostume = 1,
        HairCostume = 2,
        EarCostume = 3,
        EyeCostume = 4,
        TailCostume = 5,

        // Equipment
        Weapon = 6,
        Armor = 7,
        Belt = 8,
        Necklace = 9,
        Ring = 10,

        // Material
        EquipmentMaterial = 11,
        FoodMaterial = 12,
        MonsterPart = 13,
        NormalMaterial = 14,
        Hourglass = 15,
        ApStone = 16,

        [Obsolete("Mirror of lib9c's obsolete Chest ItemSubType; kept only for ordinal parity.")]
        Chest = 17,

        // Costume
        Title = 18,

        // Aura
        Aura = 19,

        // Grimoire
        Grimoire = 20,

        // Custom Craft
        Scroll = 21,
        Circle = 22,
    }

    /// <summary>
    /// Client-owned mirror of <c>Nekoyume.Model.Item.LockType</c>. The only case lib9c ships is
    /// <see cref="Order"/>, preserved here so UI locks can drop the lib9c import once call sites
    /// migrate.
    /// </summary>
    public enum LockType
    {
        Order,
    }
}
