using System;
using System.Collections.Generic;
using System.Numerics;
using Nekoyume.SingleClient.Blockchain;

namespace Nekoyume.SingleClient.Models.State
{
    /// <summary>
    /// Immutable projection of lib9c's <see cref="Nekoyume.Model.State.AgentState"/> plus the
    /// adjacent account-level balances / counters that <c>States.Instance</c> currently
    /// surfaces (crystal balance, monster-collection round, configured refill AP amount).
    /// Captures the avatar-slot → avatar-address map as an <see cref="IReadOnlyDictionary{int, Address}"/>
    /// so the avatar-select UI can render the three slots without reaching into lib9c types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="CrystalBalance"/> is typed as <see cref="BigInteger"/> because lib9c's
    /// <c>FungibleAssetValue.RawValue</c> is <see cref="BigInteger"/> (not <see cref="long"/>).
    /// A typical crystal balance comfortably fits in <see cref="long"/>, but large whales'
    /// on-chain accumulators have already exceeded <see cref="long.MaxValue"/>, so downgrading
    /// would silently truncate. Callers that only need a display approximation can call
    /// <c>(long)CrystalBalance</c>.
    /// </para>
    /// <para>
    /// <see cref="RefillActionPoint"/> is intentionally a <see cref="string"/>: the mapper
    /// passes the configured currency ticker (or the stringified default value) verbatim so
    /// the UI can format it without depending on <c>FungibleAssetValue</c>.
    /// </para>
    /// </remarks>
    public readonly struct AgentSnapshot : IEquatable<AgentSnapshot>
    {
        public Address Address { get; }
        public int MonsterCollectionRound { get; }

        /// <summary>
        /// Map of avatar slot index (0 / 1 / 2 in the default config) to the avatar address
        /// bound to that slot. Only populated slots are present — missing slots are absent
        /// from the dictionary rather than mapped to a zero-address sentinel.
        /// </summary>
        public IReadOnlyDictionary<int, Address> AvatarAddresses { get; }

        public BigInteger CrystalBalance { get; }
        public string RefillActionPoint { get; }

        public AgentSnapshot(
            Address address,
            int monsterCollectionRound,
            IReadOnlyDictionary<int, Address> avatarAddresses,
            BigInteger crystalBalance,
            string refillActionPoint)
        {
            Address = address;
            MonsterCollectionRound = monsterCollectionRound;
            AvatarAddresses = avatarAddresses ?? EmptyAvatarAddresses;
            CrystalBalance = crystalBalance;
            RefillActionPoint = refillActionPoint ?? string.Empty;
        }

        private static readonly IReadOnlyDictionary<int, Address> EmptyAvatarAddresses =
            new Dictionary<int, Address>(0);

        public bool Equals(AgentSnapshot other) =>
            Address.Equals(other.Address) &&
            MonsterCollectionRound == other.MonsterCollectionRound &&
            CrystalBalance == other.CrystalBalance &&
            string.Equals(RefillActionPoint, other.RefillActionPoint, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is AgentSnapshot other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Address.GetHashCode();
                hash = (hash * 397) ^ MonsterCollectionRound;
                hash = (hash * 397) ^ CrystalBalance.GetHashCode();
                hash = (hash * 397) ^ (RefillActionPoint?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public static bool operator ==(AgentSnapshot left, AgentSnapshot right) => left.Equals(right);
        public static bool operator !=(AgentSnapshot left, AgentSnapshot right) => !left.Equals(right);
    }
}
