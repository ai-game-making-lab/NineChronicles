#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Nekoyume.SingleClient.Models.Elemental;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Immutable snapshot of a lib9c <c>Material</c> / <c>TradableMaterial</c>. Materials are
    /// fungible, so <see cref="IItemSnapshot.NonFungibleId"/> is <see langword="null"/> and the
    /// hex-encoded <see cref="IItemSnapshot.TradableId"/> (derived from the lib9c
    /// <c>HashDigest&lt;SHA256&gt;</c>) is the only stable handle UI code should key off.
    /// </summary>
    public readonly struct MaterialSnapshot : IItemSnapshot, IEquatable<MaterialSnapshot>
    {
        public ItemType ItemType { get; }
        public ItemSubType ItemSubType { get; }
        public int Id { get; }
        public int Grade { get; }
        public ElementalType ElementalType { get; }
        public Guid? NonFungibleId { get; }
        public string TradableId { get; }

        public MaterialSnapshot(
            ItemType itemType,
            ItemSubType itemSubType,
            int id,
            int grade,
            ElementalType elementalType,
            string tradableId)
        {
            ItemType = itemType;
            ItemSubType = itemSubType;
            Id = id;
            Grade = grade;
            ElementalType = elementalType;
            NonFungibleId = null;
            TradableId = tradableId ?? string.Empty;
        }

        public bool Equals(MaterialSnapshot other) =>
            Id == other.Id &&
            string.Equals(TradableId, other.TradableId, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is MaterialSnapshot other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Id;
                hash = (hash * 397) ^ (TradableId?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public static bool operator ==(MaterialSnapshot left, MaterialSnapshot right) => left.Equals(right);
        public static bool operator !=(MaterialSnapshot left, MaterialSnapshot right) => !left.Equals(right);
    }
}

#endif
