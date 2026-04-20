#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Nekoyume.SingleClient.Models.Elemental;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Discriminator interface implemented by every per-subtype item snapshot
    /// (<see cref="EquipmentSnapshot"/>, <see cref="CostumeSnapshot"/>,
    /// <see cref="ConsumableSnapshot"/>, <see cref="MaterialSnapshot"/>). Lets UI helpers
    /// switch-pattern on the runtime snapshot kind without the allocation cost of returning
    /// <see cref="object"/> from the polymorphic mapper.
    /// </summary>
    public interface IItemSnapshot
    {
        ItemType ItemType { get; }
        ItemSubType ItemSubType { get; }
        int Id { get; }
        int Grade { get; }
        ElementalType ElementalType { get; }

        /// <summary>
        /// Non-fungible instance id. Set for Equipment/Costume/Consumable, <see langword="null"/>
        /// for the fungible <c>Material</c>/<c>TradableMaterial</c> families.
        /// </summary>
        Guid? NonFungibleId { get; }

        /// <summary>
        /// Hex-encoded <c>HashDigest&lt;SHA256&gt;</c> for Material/TradableMaterial instances.
        /// Empty string for non-material subtypes (whose trade affinity is captured by
        /// <see cref="NonFungibleId"/>).
        /// </summary>
        string TradableId { get; }
    }

    /// <summary>
    /// Immutable base projection of a lib9c <c>ItemBase</c>. Carries the six fields every UI
    /// consumer reads regardless of the runtime subtype; per-subtype snapshots layer the
    /// specialized fields on top. All values are captured at <c>ToSnapshot()</c> time so the
    /// original lib9c instance can be released without invalidating the DTO.
    /// </summary>
    public readonly struct ItemSnapshot : IItemSnapshot, IEquatable<ItemSnapshot>
    {
        public ItemType ItemType { get; }
        public ItemSubType ItemSubType { get; }
        public int Id { get; }
        public int Grade { get; }
        public ElementalType ElementalType { get; }
        public Guid? NonFungibleId { get; }
        public string TradableId { get; }

        public ItemSnapshot(
            ItemType itemType,
            ItemSubType itemSubType,
            int id,
            int grade,
            ElementalType elementalType,
            Guid? nonFungibleId,
            string tradableId)
        {
            ItemType = itemType;
            ItemSubType = itemSubType;
            Id = id;
            Grade = grade;
            ElementalType = elementalType;
            NonFungibleId = nonFungibleId;
            TradableId = tradableId ?? string.Empty;
        }

        public bool Equals(ItemSnapshot other) =>
            Id == other.Id &&
            NonFungibleId == other.NonFungibleId &&
            string.Equals(TradableId, other.TradableId, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is ItemSnapshot other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Id;
                hash = (hash * 397) ^ (NonFungibleId?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ (TradableId?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public static bool operator ==(ItemSnapshot left, ItemSnapshot right) => left.Equals(right);
        public static bool operator !=(ItemSnapshot left, ItemSnapshot right) => !left.Equals(right);
    }
}

#endif
