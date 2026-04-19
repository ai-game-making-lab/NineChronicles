using System.Collections.Generic;
using Nekoyume.SingleClient.Blockchain;
using Nekoyume.SingleClient.Models.Items;
using Lib9cAvatarState = Nekoyume.Model.State.AvatarState;
using Lib9cInventory = Nekoyume.Model.Item.Inventory;
using LibplanetAddress = Libplanet.Crypto.Address;

namespace Nekoyume.SingleClient.Models.State
{
    /// <summary>
    /// Projects lib9c's <see cref="Lib9cAvatarState"/> into <see cref="AvatarSnapshot"/>.
    /// Kept extension-style so migrating callers only append <c>.ToView()</c> to their
    /// <c>States.Instance.CurrentAvatarState</c> accesses.
    /// </summary>
    /// <remarks>
    /// Address projection goes through <see cref="ToClientAddress"/>, which feeds the 20-byte
    /// payload from lib9c's <see cref="LibplanetAddress"/> into the client-owned
    /// <see cref="Address"/> shim. This is the only place in the mapper chain that touches
    /// Libplanet types directly — the resulting DTOs carry the client <see cref="Address"/>
    /// everywhere else.
    /// </remarks>
    public static class AvatarSnapshotMapper
    {
        public static AvatarSnapshot ToView(this Lib9cAvatarState source)
        {
            if (source is null)
            {
                return default;
            }

            var inventoryItems = ProjectInventory(source.inventory);
            var worldInformation = source.worldInformation.ToView();
            var itemSubTypeCounts = ComputeItemSubTypeCounts(source.inventory);
            var rewardMapStageId = ComputeMaxStageCleared(source);

            return new AvatarSnapshot(
                address: ToClientAddress(source.address),
                name: source.name,
                level: source.level,
                exp: source.exp,
                ear: source.ear,
                lens: source.lens,
                tail: source.tail,
                hair: source.hair,
                hairIndex: source.hair,
                tailIndex: source.tail,
                updatedAt: source.updatedAt,
                blockIndex: source.blockIndex,
                characterId: source.characterId,
                rewardMapStageId: rewardMapStageId,
                dailyRewardReceivedIndex: source.dailyRewardReceivedIndex,
                actionPoint: source.actionPoint,
                inventoryItems: inventoryItems,
                worldInformation: worldInformation,
                itemSubTypeCounts: itemSubTypeCounts);
        }

        /// <summary>
        /// Bridges a lib9c <see cref="LibplanetAddress"/> into the client-owned
        /// <see cref="Address"/> shim by round-tripping the 20-byte payload. lib9c's
        /// <see cref="LibplanetAddress.ToByteArray"/> returns a defensive copy, and the client
        /// <c>Address(byte[])</c> ctor clones the array again, so the resulting DTO shares no
        /// memory with the lib9c instance.
        /// </summary>
        internal static Address ToClientAddress(LibplanetAddress source) =>
            new Address(source.ToByteArray());

        private static IReadOnlyList<IItemSnapshot> ProjectInventory(Lib9cInventory inventory)
        {
            if (inventory is null)
            {
                return System.Array.Empty<IItemSnapshot>();
            }

            var items = inventory.Items;
            if (items == null || items.Count == 0)
            {
                return System.Array.Empty<IItemSnapshot>();
            }

            var list = new List<IItemSnapshot>(items.Count);
            foreach (var slot in items)
            {
                if (slot?.item is null)
                {
                    continue;
                }

                list.Add(slot.item.ToPolySnapshot());
            }

            return list;
        }

        private static IReadOnlyDictionary<ItemSubType, int> ComputeItemSubTypeCounts(
            Lib9cInventory inventory)
        {
            if (inventory is null)
            {
                return EmptyCounts;
            }

            var items = inventory.Items;
            if (items == null || items.Count == 0)
            {
                return EmptyCounts;
            }

            var counts = new Dictionary<ItemSubType, int>();
            foreach (var slot in items)
            {
                if (slot?.item is null)
                {
                    continue;
                }

                var subType = slot.item.ItemSubType.ToView();
                // Equipment / Costume / Consumable instances always have count == 1 per slot;
                // fungible Materials carry their stack count. Add whichever the slot reports
                // so the aggregate count matches the inventory-filter badge numbers the UI
                // already renders.
                counts.TryGetValue(subType, out var current);
                counts[subType] = current + (slot.count == 0 ? 1 : slot.count);
            }

            return counts;
        }

        private static int ComputeMaxStageCleared(Lib9cAvatarState source)
        {
            // stageMap is a CollectionMap<int, int>; its keys are stage ids, values a 1/0
            // flag for clears. The highest key is the max stage id the avatar has cleared.
            var stageMap = source.stageMap;
            if (stageMap is null || stageMap.Count == 0)
            {
                return 0;
            }

            var max = 0;
            foreach (var key in stageMap.Keys)
            {
                if (key > max)
                {
                    max = key;
                }
            }

            return max;
        }

        private static readonly IReadOnlyDictionary<ItemSubType, int> EmptyCounts =
            new Dictionary<ItemSubType, int>(0);
    }
}
