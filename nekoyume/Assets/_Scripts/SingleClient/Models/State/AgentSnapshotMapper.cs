#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;
using System.Numerics;
using Nekoyume.SingleClient.Blockchain;
using Lib9cAgentState = Nekoyume.Model.State.AgentState;
using LibplanetAddress = Libplanet.Crypto.Address;

namespace Nekoyume.SingleClient.Models.State
{
    /// <summary>
    /// Projects lib9c's <see cref="Lib9cAgentState"/> into <see cref="AgentSnapshot"/>. Because
    /// the adjacent "agent" data <c>States.Instance</c> exposes (crystal balance, refill AP
    /// config) is not stored on <c>AgentState</c> itself but scattered across sibling fields,
    /// this mapper accepts optional overrides so the state-sync layer can pass them in at the
    /// same call site. Defaults to zero / empty when the caller has nothing to supply.
    /// </summary>
    public static class AgentSnapshotMapper
    {
        /// <summary>
        /// Base projection — captures only the fields lib9c's <see cref="Lib9cAgentState"/>
        /// actually carries. Call the overload with <see cref="BigInteger"/> / refill-AP
        /// overrides when the state-sync layer has those values on hand.
        /// </summary>
        public static AgentSnapshot ToView(this Lib9cAgentState source)
        {
            return source.ToView(crystalBalance: BigInteger.Zero, refillActionPoint: string.Empty);
        }

        /// <summary>
        /// Extended projection. <paramref name="crystalBalance"/> typically comes from the
        /// sibling <c>FungibleAssetValue</c> the state-sync layer fetches alongside the agent
        /// state (<c>States.Instance.CrystalBalance.RawValue</c>).
        /// <paramref name="refillActionPoint"/> is the stringified value the game config
        /// currently surfaces (ticker or count) — forwarded verbatim.
        /// </summary>
        public static AgentSnapshot ToView(
            this Lib9cAgentState source,
            BigInteger crystalBalance,
            string refillActionPoint)
        {
            if (source is null)
            {
                return default;
            }

            return new AgentSnapshot(
                address: AvatarSnapshotMapper.ToClientAddress(source.address),
                monsterCollectionRound: source.MonsterCollectionRound,
                avatarAddresses: ProjectAvatarAddresses(source.avatarAddresses),
                crystalBalance: crystalBalance,
                refillActionPoint: refillActionPoint);
        }

        private static IReadOnlyDictionary<int, Address> ProjectAvatarAddresses(
            IReadOnlyDictionary<int, LibplanetAddress> source)
        {
            if (source is null || source.Count == 0)
            {
                return EmptyAvatarAddresses;
            }

            var result = new Dictionary<int, Address>(source.Count);
            foreach (var kv in source)
            {
                result[kv.Key] = AvatarSnapshotMapper.ToClientAddress(kv.Value);
            }

            return result;
        }

        private static readonly IReadOnlyDictionary<int, Address> EmptyAvatarAddresses =
            new Dictionary<int, Address>(0);
    }
}

#endif
