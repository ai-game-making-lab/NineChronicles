using System;

namespace Nekoyume.SingleClient.Models.Stats
{
    /// <summary>
    /// Immutable snapshot of a lib9c <c>DecimalStat</c>/<c>StatMap</c> entry, shaped for the UI
    /// layer. Carries <see cref="BaseValue"/>, <see cref="AdditionalValue"/>, and the derived
    /// <see cref="TotalValue"/> so tooltips and option rows stop importing
    /// <c>Nekoyume.Model.Stat.DecimalStat</c>.
    /// </summary>
    /// <remarks>
    /// lib9c stores stat magnitudes as <see cref="decimal"/>; keeping the same numeric type here
    /// avoids the precision loss that would come from flattening to <see cref="double"/> or
    /// <see cref="long"/>. Consumers that only need integer display values can still read
    /// <c>(long)stat.TotalValue</c> at the call site.
    /// </remarks>
    public readonly struct StatView : IEquatable<StatView>
    {
        public StatType StatType { get; }
        public decimal BaseValue { get; }
        public decimal AdditionalValue { get; }

        public decimal TotalValue => BaseValue + AdditionalValue;

        public StatView(StatType statType, decimal baseValue, decimal additionalValue)
        {
            StatType = statType;
            BaseValue = baseValue;
            AdditionalValue = additionalValue;
        }

        public bool HasBaseValue => BaseValue > 0m;
        public bool HasAdditionalValue => AdditionalValue > 0m;

        public bool Equals(StatView other) =>
            StatType == other.StatType &&
            BaseValue == other.BaseValue &&
            AdditionalValue == other.AdditionalValue;

        public override bool Equals(object obj) => obj is StatView other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int)StatType;
                hash = (hash * 397) ^ BaseValue.GetHashCode();
                hash = (hash * 397) ^ AdditionalValue.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(StatView left, StatView right) => left.Equals(right);
        public static bool operator !=(StatView left, StatView right) => !left.Equals(right);
    }
}
