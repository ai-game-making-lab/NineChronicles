using System;
using System.Collections.Generic;
using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.SingleClient.Models.Skills;
using Nekoyume.SingleClient.Models.Stats;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Immutable snapshot of a lib9c <c>Consumable</c>. Captures the <see cref="Stats"/> list
    /// (the primary driver of consumable tooltips and food-sorting UI) plus the inherited
    /// <c>ItemUsable</c> skill lists and block-gate.
    /// </summary>
    public readonly struct ConsumableSnapshot : IItemSnapshot, IEquatable<ConsumableSnapshot>
    {
        public ItemType ItemType { get; }
        public ItemSubType ItemSubType { get; }
        public int Id { get; }
        public int Grade { get; }
        public ElementalType ElementalType { get; }
        public Guid? NonFungibleId { get; }
        public string TradableId { get; }

        public long? RequiredBlockIndex { get; }
        public IReadOnlyList<StatView> Stats { get; }
        public IReadOnlyList<SkillSnapshot> Skills { get; }

        public ConsumableSnapshot(
            ItemType itemType,
            ItemSubType itemSubType,
            int id,
            int grade,
            ElementalType elementalType,
            Guid? nonFungibleId,
            string tradableId,
            long? requiredBlockIndex,
            IReadOnlyList<StatView> stats,
            IReadOnlyList<SkillSnapshot> skills)
        {
            ItemType = itemType;
            ItemSubType = itemSubType;
            Id = id;
            Grade = grade;
            ElementalType = elementalType;
            NonFungibleId = nonFungibleId;
            TradableId = tradableId ?? string.Empty;
            RequiredBlockIndex = requiredBlockIndex;
            Stats = stats ?? Array.Empty<StatView>();
            Skills = skills ?? Array.Empty<SkillSnapshot>();
        }

        public bool Equals(ConsumableSnapshot other) =>
            Id == other.Id &&
            NonFungibleId == other.NonFungibleId;

        public override bool Equals(object obj) => obj is ConsumableSnapshot other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Id;
                hash = (hash * 397) ^ (NonFungibleId?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public static bool operator ==(ConsumableSnapshot left, ConsumableSnapshot right) => left.Equals(right);
        public static bool operator !=(ConsumableSnapshot left, ConsumableSnapshot right) => !left.Equals(right);
    }
}
