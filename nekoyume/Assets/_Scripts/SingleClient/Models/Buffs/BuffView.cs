using System;
using Nekoyume.SingleClient.Models.Stats;

namespace Nekoyume.SingleClient.Models.Buffs
{
    public enum BuffViewKind
    {
        Stat,
        Action,
    }

    /// <summary>
    /// Immutable snapshot of a live battle buff, shaped for the UI layer. Capturing just the
    /// fields the HUD/tooltip actually reads lets the UI migrate off <c>Nekoyume.Model.Buff.Buff</c>
    /// without dragging in the rest of lib9c.
    /// </summary>
    public readonly struct BuffView : IEquatable<BuffView>
    {
        public int Id { get; }
        public int GroupId { get; }
        public int OriginalDuration { get; }
        public int RemainedDuration { get; }
        public bool IsBuff { get; }
        public bool IsDebuff { get; }
        public BuffViewKind Kind { get; }

        public StatType StatType { get; }
        public long StatValue { get; }

        public BuffView(
            int id,
            int groupId,
            int originalDuration,
            int remainedDuration,
            bool isBuff,
            bool isDebuff,
            BuffViewKind kind,
            StatType statType,
            long statValue)
        {
            Id = id;
            GroupId = groupId;
            OriginalDuration = originalDuration;
            RemainedDuration = remainedDuration;
            IsBuff = isBuff;
            IsDebuff = isDebuff;
            Kind = kind;
            StatType = statType;
            StatValue = statValue;
        }

        public bool IsStatBuff => Kind == BuffViewKind.Stat;
        public bool IsActionBuff => Kind == BuffViewKind.Action;

        public bool Equals(BuffView other) => Id == other.Id;

        public override bool Equals(object obj) => obj is BuffView other && Equals(other);

        public override int GetHashCode() => Id;

        public static bool operator ==(BuffView left, BuffView right) => left.Equals(right);

        public static bool operator !=(BuffView left, BuffView right) => !left.Equals(right);
    }
}
