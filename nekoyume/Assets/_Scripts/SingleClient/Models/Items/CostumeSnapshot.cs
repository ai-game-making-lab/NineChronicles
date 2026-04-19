using System;
using Nekoyume.SingleClient.Models.Elemental;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Immutable snapshot of a lib9c <c>Costume</c>. Costumes are non-fungible but do not carry
    /// the <c>ItemUsable</c> stat/skill payload, so the snapshot stays flat: base fields plus
    /// the <c>Equipped</c> flag and block-gate.
    /// </summary>
    public readonly struct CostumeSnapshot : IItemSnapshot, IEquatable<CostumeSnapshot>
    {
        public ItemType ItemType { get; }
        public ItemSubType ItemSubType { get; }
        public int Id { get; }
        public int Grade { get; }
        public ElementalType ElementalType { get; }

        /// <summary>
        /// Always non-null for costumes (lib9c <c>Costume.ItemId</c> is required). The
        /// <see cref="IItemSnapshot.NonFungibleId"/> surface returns <see cref="Nullable{Guid}"/>
        /// for polymorphic call sites; consumers that know the concrete <see cref="CostumeSnapshot"/>
        /// can read this property to skip the null-check.
        /// </summary>
        public Guid NonFungibleId { get; }

        public string TradableId { get; }

        public bool Equipped { get; }
        public long? RequiredBlockIndex { get; }

        Guid? IItemSnapshot.NonFungibleId => NonFungibleId;

        public CostumeSnapshot(
            ItemType itemType,
            ItemSubType itemSubType,
            int id,
            int grade,
            ElementalType elementalType,
            Guid nonFungibleId,
            string tradableId,
            bool equipped,
            long? requiredBlockIndex)
        {
            ItemType = itemType;
            ItemSubType = itemSubType;
            Id = id;
            Grade = grade;
            ElementalType = elementalType;
            NonFungibleId = nonFungibleId;
            TradableId = tradableId ?? string.Empty;
            Equipped = equipped;
            RequiredBlockIndex = requiredBlockIndex;
        }

        public bool Equals(CostumeSnapshot other) =>
            Id == other.Id &&
            NonFungibleId.Equals(other.NonFungibleId) &&
            Equipped == other.Equipped;

        public override bool Equals(object obj) => obj is CostumeSnapshot other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Id;
                hash = (hash * 397) ^ NonFungibleId.GetHashCode();
                hash = (hash * 397) ^ Equipped.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(CostumeSnapshot left, CostumeSnapshot right) => left.Equals(right);
        public static bool operator !=(CostumeSnapshot left, CostumeSnapshot right) => !left.Equals(right);
    }
}
