using System;
using System.Collections.Generic;
using Nekoyume.SingleClient.Blockchain;
using Nekoyume.SingleClient.Models.Items;

namespace Nekoyume.SingleClient.Models.State
{
    /// <summary>
    /// Immutable projection of lib9c's <see cref="Nekoyume.Model.State.AvatarState"/> with
    /// only the fields the <c>States.Instance</c> migration's 148 consumers read. Captures
    /// identity (<see cref="Address"/>, <see cref="Name"/>), progression (<see cref="Level"/>,
    /// <see cref="Exp"/>, <see cref="BlockIndex"/>), customization (hair / lens / ear / tail),
    /// the flattened <see cref="WorldInformation"/>, and a frozen <see cref="InventoryItems"/>
    /// list so callers can release the lib9c instance without invalidating the DTO.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Customization: lib9c's <c>AvatarState</c> stores only <c>hair</c>, <c>lens</c>,
    /// <c>ear</c>, <c>tail</c> integers (no explicit <c>*Index</c> sister fields). The
    /// <see cref="HairIndex"/> / <see cref="TailIndex"/> mirrors alias those same values so
    /// UI code that expects the <c>*Index</c> name (common in the NineChronicles UI codebase)
    /// keeps working without the mapper needing to split them.
    /// </para>
    /// <para>
    /// <see cref="RewardMap_StageId"/> is the highest cleared stage id derived from
    /// <c>AvatarState.stageMap</c> (the per-stage 1/0 clear bitmap; max key = last cleared
    /// stage). Kept as a single int because that's what the map / reward UI consumes — the
    /// full <c>stageMap</c>, <c>monsterMap</c>, <c>itemMap</c>, <c>eventMap</c> CollectionMaps
    /// are out of scope for this slice (S6a) and tracked for S6b.
    /// </para>
    /// <para>
    /// Mail and Quest subnamespaces are deferred — <c>mailBox</c> / <c>questList</c> stay
    /// accessed off the lib9c <c>AvatarState</c> until the follow-up slice adds their own
    /// snapshot types.
    /// </para>
    /// </remarks>
    public readonly struct AvatarSnapshot : IEquatable<AvatarSnapshot>
    {
        public Address Address { get; }
        public string Name { get; }

        public int Level { get; }
        public long Exp { get; }

        public int Ear { get; }
        public int Lens { get; }
        public int Tail { get; }
        public int Hair { get; }

        /// <summary>Alias of <see cref="Hair"/> for UI code that uses the <c>*Index</c> name.</summary>
        public int HairIndex { get; }

        /// <summary>Alias of <see cref="Tail"/> for UI code that uses the <c>*Index</c> name.</summary>
        public int TailIndex { get; }

        public long UpdatedAt { get; }
        public long BlockIndex { get; }
        public int CharacterId { get; }

        /// <summary>
        /// Highest stage id recorded in <c>AvatarState.stageMap</c>, or <c>0</c> when the
        /// avatar has no stage clears yet.
        /// </summary>
        public int RewardMap_StageId { get; }

        public long DailyRewardReceivedIndex { get; }
        public long ActionPoint { get; }

        public IReadOnlyList<IItemSnapshot> InventoryItems { get; }
        public WorldInformationSnapshot WorldInformation { get; }

        /// <summary>
        /// Count of inventory items grouped by <see cref="ItemSubType"/>. Equipment / costume
        /// slots count by instance; fungible materials sum their stack counts. Pre-computed so
        /// the equipment-slot / inventory-filter UI can query counts in O(1).
        /// </summary>
        public IReadOnlyDictionary<ItemSubType, int> ItemSubTypeCounts { get; }

        public AvatarSnapshot(
            Address address,
            string name,
            int level,
            long exp,
            int ear,
            int lens,
            int tail,
            int hair,
            int hairIndex,
            int tailIndex,
            long updatedAt,
            long blockIndex,
            int characterId,
            int rewardMapStageId,
            long dailyRewardReceivedIndex,
            long actionPoint,
            IReadOnlyList<IItemSnapshot> inventoryItems,
            WorldInformationSnapshot worldInformation,
            IReadOnlyDictionary<ItemSubType, int> itemSubTypeCounts)
        {
            Address = address;
            Name = name ?? string.Empty;
            Level = level;
            Exp = exp;
            Ear = ear;
            Lens = lens;
            Tail = tail;
            Hair = hair;
            HairIndex = hairIndex;
            TailIndex = tailIndex;
            UpdatedAt = updatedAt;
            BlockIndex = blockIndex;
            CharacterId = characterId;
            RewardMap_StageId = rewardMapStageId;
            DailyRewardReceivedIndex = dailyRewardReceivedIndex;
            ActionPoint = actionPoint;
            InventoryItems = inventoryItems ?? Array.Empty<IItemSnapshot>();
            WorldInformation = worldInformation;
            ItemSubTypeCounts = itemSubTypeCounts ?? EmptyCounts;
        }

        private static readonly IReadOnlyDictionary<ItemSubType, int> EmptyCounts =
            new Dictionary<ItemSubType, int>(0);

        public bool Equals(AvatarSnapshot other) =>
            Address.Equals(other.Address) &&
            BlockIndex == other.BlockIndex &&
            UpdatedAt == other.UpdatedAt &&
            Level == other.Level;

        public override bool Equals(object obj) => obj is AvatarSnapshot other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Address.GetHashCode();
                hash = (hash * 397) ^ BlockIndex.GetHashCode();
                hash = (hash * 397) ^ UpdatedAt.GetHashCode();
                hash = (hash * 397) ^ Level;
                return hash;
            }
        }

        public static bool operator ==(AvatarSnapshot left, AvatarSnapshot right) => left.Equals(right);
        public static bool operator !=(AvatarSnapshot left, AvatarSnapshot right) => !left.Equals(right);
    }
}
