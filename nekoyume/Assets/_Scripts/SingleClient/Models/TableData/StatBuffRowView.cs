#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Nekoyume.SingleClient.Models.Stats;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Immutable client-owned mirror of <c>Nekoyume.TableData.StatBuffSheet.Row</c>. Captures
    /// the fields the skill tooltip / buff icon row reads: identity (Id/GroupId), the target
    /// stat, the duration/chance and the magnitude. <see cref="Value"/> is stored as a
    /// <see cref="decimal"/> so callers that format partial stacks (e.g. tick damage) stay
    /// precision-correct; the lib9c source keeps a <see cref="long"/> but the UI already
    /// performs decimal arithmetic against ratios.
    /// </summary>
    public readonly struct StatBuffRowView : IEquatable<StatBuffRowView>
    {
        public int Id { get; }
        public int GroupId { get; }
        public StatType StatType { get; }
        public int Duration { get; }
        public decimal Value { get; }
        public int Chance { get; }

        public StatBuffRowView(
            int id,
            int groupId,
            StatType statType,
            int duration,
            decimal value,
            int chance)
        {
            Id = id;
            GroupId = groupId;
            StatType = statType;
            Duration = duration;
            Value = value;
            Chance = chance;
        }

        public bool Equals(StatBuffRowView other) => Id == other.Id;

        public override bool Equals(object obj) => obj is StatBuffRowView other && Equals(other);

        public override int GetHashCode() => Id;

        public static bool operator ==(StatBuffRowView left, StatBuffRowView right) => left.Equals(right);

        public static bool operator !=(StatBuffRowView left, StatBuffRowView right) => !left.Equals(right);
    }
}

#endif
