using System;
using System.Collections.Generic;
using System.Linq;

namespace Nekoyume.SingleClient
{
    public sealed class ClientRuntimeState
    {
        public int Version { get; }
        public string PlayerId { get; }
        public string AvatarId { get; }
        public string AvatarName { get; }
        public int AvatarSlotIndex { get; }
        public int AvatarLevel { get; }
        public long ActionPoint { get; }
        public IReadOnlyList<ClientAvatarState> Avatars { get; }
        public IReadOnlyList<ClientInventoryItemState> InventoryItems { get; }
        public IReadOnlyList<int> ClearedStageIds { get; }
        public int HighestClearedStageId { get; }
        public long BlockIndex { get; }
        public long UpdatedAtUnixSeconds { get; }

        public ClientRuntimeState(SingleClientState state)
        {
            Version = state.version;
            PlayerId = state.playerId;
            AvatarId = state.avatarId;
            AvatarName = state.avatar?.name;
            AvatarSlotIndex = state.avatar?.slotIndex ?? 0;
            AvatarLevel = state.avatar?.level ?? 0;
            ActionPoint = state.avatar?.actionPoint ?? 0;
            Avatars = state.avatars?
                .Where(avatar => avatar is not null)
                .Select(avatar => new ClientAvatarState(
                    avatar.slotIndex,
                    avatar.id,
                    avatar.name,
                    avatar.level,
                    avatar.actionPoint))
                .ToArray() ?? new ClientAvatarState[] { };
            InventoryItems = state.inventory?.items?
                .Where(item => item is not null)
                .Select(item => new ClientInventoryItemState(item.itemId, item.count))
                .ToArray() ?? new ClientInventoryItemState[] { };
            ClearedStageIds = state.stage?.clearedStageIds?.ToArray() ?? new int[] { };
            HighestClearedStageId = state.stage?.highestClearedStageId ?? 0;
            BlockIndex = state.blockIndex;
            UpdatedAtUnixSeconds = state.updatedAtUnixSeconds;
        }
    }

    public sealed class ClientAvatarState
    {
        public int SlotIndex { get; }
        public string AvatarId { get; }
        public string AvatarName { get; }
        public int Level { get; }
        public long ActionPoint { get; }

        public ClientAvatarState(
            int slotIndex,
            string avatarId,
            string avatarName,
            int level,
            long actionPoint)
        {
            SlotIndex = slotIndex;
            AvatarId = avatarId;
            AvatarName = avatarName;
            Level = level;
            ActionPoint = actionPoint;
        }
    }

    public sealed class ClientInventoryItemState
    {
        public string ItemId { get; }
        public long Count { get; }

        public ClientInventoryItemState(string itemId, long count)
        {
            ItemId = itemId;
            Count = count;
        }
    }

    public sealed class ClientItemCost
    {
        public string ItemId { get; }
        public long Count { get; }

        public ClientItemCost(string itemId, long count)
        {
            ItemId = itemId;
            Count = count;
        }
    }

    public sealed class ClientItemCostPreview
    {
        public string ItemId { get; }
        public long Count { get; }
        public long Balance { get; }
        public long Shortfall { get; }
        public bool HasEnough => Shortfall == 0;

        public ClientItemCostPreview(SingleClientItemCostPreview preview)
        {
            ItemId = preview.ItemId;
            Count = preview.Count;
            Balance = preview.Balance;
            Shortfall = preview.Shortfall;
        }
    }

    public sealed class ClientItemCostDelta
    {
        public string ItemId { get; }
        public long Count { get; }
        public long CountBefore { get; }
        public long CountAfter { get; }

        public ClientItemCostDelta(SingleClientItemCostDelta delta)
        {
            ItemId = delta.ItemId;
            Count = delta.Count;
            CountBefore = delta.CountBefore;
            CountAfter = delta.CountAfter;
        }
    }

    public sealed class ClientStagePlayResult
    {
        public ClientRuntimeState State { get; }
        public int StageId { get; }
        public bool WasFirstClear { get; }
        public long ActionPointCost { get; }
        public long ActionPointBefore { get; }
        public long ActionPointAfter { get; }
        public string EntryCostItemId { get; }
        public long EntryCostItemCount { get; }
        public long EntryCostItemCountBefore { get; }
        public long EntryCostItemCountAfter { get; }
        public IReadOnlyList<ClientItemCostDelta> ItemCostDeltas { get; }

        public ClientStagePlayResult(SingleClientStagePlayResult result)
        {
            State = new ClientRuntimeState(result.State);
            StageId = result.StageId;
            WasFirstClear = result.WasFirstClear;
            ActionPointCost = result.ActionPointCost;
            ActionPointBefore = result.ActionPointBefore;
            ActionPointAfter = result.ActionPointAfter;
            EntryCostItemId = result.EntryCostItemId;
            EntryCostItemCount = result.EntryCostItemCount;
            EntryCostItemCountBefore = result.EntryCostItemCountBefore;
            EntryCostItemCountAfter = result.EntryCostItemCountAfter;
            ItemCostDeltas = result.ItemCostDeltas?
                .Select(delta => new ClientItemCostDelta(delta))
                .ToArray() ?? new ClientItemCostDelta[] { };
        }
    }

    public sealed class ClientSweepResult
    {
        public ClientRuntimeState State { get; }
        public int StageId { get; }
        public bool WasFirstClear { get; }
        public long ActionPointCost { get; }
        public long ActionPointBefore { get; }
        public long ActionPointAfter { get; }
        public IReadOnlyList<ClientItemCostDelta> ItemCostDeltas { get; }

        public ClientSweepResult(SingleClientSweepResult result)
        {
            State = new ClientRuntimeState(result.State);
            StageId = result.StageId;
            WasFirstClear = result.WasFirstClear;
            ActionPointCost = result.ActionPointCost;
            ActionPointBefore = result.ActionPointBefore;
            ActionPointAfter = result.ActionPointAfter;
            ItemCostDeltas = result.ItemCostDeltas?
                .Select(delta => new ClientItemCostDelta(delta))
                .ToArray() ?? new ClientItemCostDelta[] { };
        }
    }

    public sealed class ClientStagePlayPreview
    {
        public ClientRuntimeState State { get; }
        public int StageId { get; }
        public bool WasFirstClear { get; }
        public long ActionPointCost { get; }
        public long ActionPointBalance { get; }
        public long ActionPointShortfall { get; }
        public bool HasEnoughActionPoint { get; }
        public bool HasEntryCost { get; }
        public string EntryCostItemId { get; }
        public long EntryCostItemCount { get; }
        public long EntryCostItemBalance { get; }
        public long EntryCostItemShortfall { get; }
        public bool HasEnoughEntryCostItem { get; }
        public bool CanPlay { get; }
        public IReadOnlyList<ClientItemCostPreview> ItemCosts { get; }

        public ClientStagePlayPreview(SingleClientStagePlayPreview preview)
        {
            State = new ClientRuntimeState(preview.State);
            StageId = preview.StageId;
            WasFirstClear = preview.WasFirstClear;
            ActionPointCost = preview.ActionPointCost;
            ActionPointBalance = preview.ActionPointBalance;
            ActionPointShortfall = preview.ActionPointShortfall;
            HasEnoughActionPoint = preview.HasEnoughActionPoint;
            HasEntryCost = preview.HasEntryCost;
            EntryCostItemId = preview.EntryCostItemId;
            EntryCostItemCount = preview.EntryCostItemCount;
            EntryCostItemBalance = preview.EntryCostItemBalance;
            EntryCostItemShortfall = preview.EntryCostItemShortfall;
            HasEnoughEntryCostItem = preview.HasEnoughEntryCostItem;
            CanPlay = preview.CanPlay;
            ItemCosts = preview.ItemCosts?
                .Select(cost => new ClientItemCostPreview(cost))
                .ToArray() ?? new ClientItemCostPreview[] { };
        }
    }

    public sealed class SingleClientItemCostPreview
    {
        public string ItemId { get; }
        public long Count { get; }
        public long Balance { get; }
        public long Shortfall { get; }
        public bool HasEnough => Shortfall == 0;

        public SingleClientItemCostPreview(string itemId, long count, long balance)
        {
            ItemId = itemId;
            Count = count;
            Balance = balance;
            Shortfall = Math.Max(0L, count - balance);
        }
    }

    public sealed class SingleClientStagePlayPreview
    {
        public SingleClientState State { get; }
        public int StageId { get; }
        public bool WasFirstClear { get; }
        public long ActionPointCost { get; }
        public long ActionPointBalance { get; }
        public long ActionPointShortfall { get; }
        public bool HasEnoughActionPoint => ActionPointShortfall == 0;
        public bool HasEntryCost => !string.IsNullOrWhiteSpace(EntryCostItemId);
        public string EntryCostItemId => ItemCosts.FirstOrDefault()?.ItemId;
        public long EntryCostItemCount => ItemCosts.FirstOrDefault()?.Count ?? 0L;
        public long EntryCostItemBalance => ItemCosts.FirstOrDefault()?.Balance ?? 0L;
        public long EntryCostItemShortfall => ItemCosts.FirstOrDefault()?.Shortfall ?? 0L;
        public bool HasEnoughEntryCostItem => ItemCosts.All(cost => cost.HasEnough);
        public bool CanPlay => HasEnoughActionPoint && HasEnoughEntryCostItem;
        public IReadOnlyList<SingleClientItemCostPreview> ItemCosts { get; }

        public SingleClientStagePlayPreview(
            SingleClientState state,
            int stageId,
            bool wasFirstClear,
            long actionPointCost,
            long actionPointBalance,
            string entryCostItemId,
            long entryCostItemCount,
            long entryCostItemBalance)
            : this(
                state,
                stageId,
                wasFirstClear,
                actionPointCost,
                actionPointBalance,
                CreateItemCostPreviews(entryCostItemId, entryCostItemCount, entryCostItemBalance))
        {
        }

        public SingleClientStagePlayPreview(
            SingleClientState state,
            int stageId,
            bool wasFirstClear,
            long actionPointCost,
            long actionPointBalance,
            IEnumerable<SingleClientItemCostPreview> itemCosts)
        {
            State = state;
            StageId = stageId;
            WasFirstClear = wasFirstClear;
            ActionPointCost = actionPointCost;
            ActionPointBalance = actionPointBalance;
            ActionPointShortfall = Math.Max(0L, actionPointCost - actionPointBalance);
            ItemCosts = itemCosts?
                .Where(cost => cost is not null)
                .ToArray() ?? new SingleClientItemCostPreview[] { };
        }

        private static IEnumerable<SingleClientItemCostPreview> CreateItemCostPreviews(
            string itemId,
            long count,
            long balance)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return Array.Empty<SingleClientItemCostPreview>();
            }

            return new[]
            {
                new SingleClientItemCostPreview(itemId, count, balance)
            };
        }
    }

    public sealed class SingleClientItemCostDelta
    {
        public string ItemId { get; }
        public long Count { get; }
        public long CountBefore { get; }
        public long CountAfter { get; }

        public SingleClientItemCostDelta(
            string itemId,
            long count,
            long countBefore,
            long countAfter)
        {
            ItemId = itemId;
            Count = count;
            CountBefore = countBefore;
            CountAfter = countAfter;
        }
    }

    public sealed class SingleClientStagePlayResult
    {
        public SingleClientState State { get; }
        public int StageId { get; }
        public bool WasFirstClear { get; }
        public long ActionPointCost { get; }
        public long ActionPointBefore { get; }
        public long ActionPointAfter { get; }
        public string EntryCostItemId { get; }
        public long EntryCostItemCount { get; }
        public long EntryCostItemCountBefore { get; }
        public long EntryCostItemCountAfter { get; }
        public IReadOnlyList<SingleClientItemCostDelta> ItemCostDeltas { get; }

        public SingleClientStagePlayResult(
            SingleClientState state,
            int stageId,
            bool wasFirstClear,
            long actionPointCost,
            long actionPointBefore,
            long actionPointAfter,
            string entryCostItemId,
            long entryCostItemCount,
            long entryCostItemCountBefore,
            long entryCostItemCountAfter)
            : this(
                state,
                stageId,
                wasFirstClear,
                actionPointCost,
                actionPointBefore,
                actionPointAfter,
                CreateItemCostDeltas(
                    entryCostItemId,
                    entryCostItemCount,
                    entryCostItemCountBefore,
                    entryCostItemCountAfter))
        {
        }

        public SingleClientStagePlayResult(
            SingleClientState state,
            int stageId,
            bool wasFirstClear,
            long actionPointCost,
            long actionPointBefore,
            long actionPointAfter,
            IEnumerable<SingleClientItemCostDelta> itemCostDeltas)
        {
            State = state;
            StageId = stageId;
            WasFirstClear = wasFirstClear;
            ActionPointCost = actionPointCost;
            ActionPointBefore = actionPointBefore;
            ActionPointAfter = actionPointAfter;
            ItemCostDeltas = itemCostDeltas?
                .Where(delta => delta is not null)
                .ToArray() ?? new SingleClientItemCostDelta[] { };
            var firstDelta = ItemCostDeltas.FirstOrDefault();
            EntryCostItemId = firstDelta?.ItemId;
            EntryCostItemCount = firstDelta?.Count ?? 0L;
            EntryCostItemCountBefore = firstDelta?.CountBefore ?? 0L;
            EntryCostItemCountAfter = firstDelta?.CountAfter ?? 0L;
        }

        private static IEnumerable<SingleClientItemCostDelta> CreateItemCostDeltas(
            string itemId,
            long count,
            long countBefore,
            long countAfter)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return Array.Empty<SingleClientItemCostDelta>();
            }

            return new[]
            {
                new SingleClientItemCostDelta(itemId, count, countBefore, countAfter)
            };
        }
    }

    public sealed class SingleClientGrindResult
    {
        public SingleClientState State { get; }
        public IReadOnlyList<string> EquipmentIds { get; }
        public string CurrencyTicker { get; }
        public System.Numerics.BigInteger CrystalGained { get; }
        public System.Numerics.BigInteger BalanceBefore { get; }
        public System.Numerics.BigInteger BalanceAfter { get; }

        public SingleClientGrindResult(
            SingleClientState state,
            IEnumerable<string> equipmentIds,
            string currencyTicker,
            System.Numerics.BigInteger crystalGained,
            System.Numerics.BigInteger balanceBefore,
            System.Numerics.BigInteger balanceAfter)
        {
            State = state;
            EquipmentIds = equipmentIds?
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToArray() ?? Array.Empty<string>();
            CurrencyTicker = currencyTicker;
            CrystalGained = crystalGained;
            BalanceBefore = balanceBefore;
            BalanceAfter = balanceAfter;
        }
    }

    public sealed class ClientGrindResult
    {
        public ClientRuntimeState State { get; }
        public IReadOnlyList<string> EquipmentIds { get; }
        public string CurrencyTicker { get; }
        public System.Numerics.BigInteger CrystalGained { get; }
        public System.Numerics.BigInteger BalanceBefore { get; }
        public System.Numerics.BigInteger BalanceAfter { get; }

        public ClientGrindResult(SingleClientGrindResult result)
        {
            State = new ClientRuntimeState(result.State);
            EquipmentIds = result.EquipmentIds?.ToArray() ?? Array.Empty<string>();
            CurrencyTicker = result.CurrencyTicker;
            CrystalGained = result.CrystalGained;
            BalanceBefore = result.BalanceBefore;
            BalanceAfter = result.BalanceAfter;
        }
    }

    public sealed class SingleClientEnhanceResult
    {
        public SingleClientState State { get; }
        public string BaseEquipmentId { get; }
        public int LevelBefore { get; }
        public int LevelAfter { get; }
        public IReadOnlyList<string> MaterialEquipmentIds { get; }

        public SingleClientEnhanceResult(
            SingleClientState state,
            string baseEquipmentId,
            int levelBefore,
            int levelAfter,
            IEnumerable<string> materialEquipmentIds)
        {
            State = state;
            BaseEquipmentId = baseEquipmentId;
            LevelBefore = levelBefore;
            LevelAfter = levelAfter;
            MaterialEquipmentIds = materialEquipmentIds?
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToArray() ?? Array.Empty<string>();
        }
    }

    public sealed class ClientEnhanceResult
    {
        public ClientRuntimeState State { get; }
        public string BaseEquipmentId { get; }
        public int LevelBefore { get; }
        public int LevelAfter { get; }
        public IReadOnlyList<string> MaterialEquipmentIds { get; }

        public ClientEnhanceResult(SingleClientEnhanceResult result)
        {
            State = new ClientRuntimeState(result.State);
            BaseEquipmentId = result.BaseEquipmentId;
            LevelBefore = result.LevelBefore;
            LevelAfter = result.LevelAfter;
            MaterialEquipmentIds = result.MaterialEquipmentIds?.ToArray()
                ?? Array.Empty<string>();
        }
    }

    public sealed class SingleClientSweepResult
    {
        public SingleClientState State { get; }
        public int StageId { get; }
        public bool WasFirstClear { get; }
        public long ActionPointCost { get; }
        public long ActionPointBefore { get; }
        public long ActionPointAfter { get; }
        public IReadOnlyList<SingleClientItemCostDelta> ItemCostDeltas { get; }

        public SingleClientSweepResult(
            SingleClientState state,
            int stageId,
            bool wasFirstClear,
            long actionPointCost,
            long actionPointBefore,
            long actionPointAfter,
            IEnumerable<SingleClientItemCostDelta> itemCostDeltas)
        {
            State = state;
            StageId = stageId;
            WasFirstClear = wasFirstClear;
            ActionPointCost = actionPointCost;
            ActionPointBefore = actionPointBefore;
            ActionPointAfter = actionPointAfter;
            ItemCostDeltas = itemCostDeltas?
                .Where(delta => delta is not null)
                .ToArray() ?? new SingleClientItemCostDelta[] { };
        }
    }
}
