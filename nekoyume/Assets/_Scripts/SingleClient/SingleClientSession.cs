using System;
using System.Collections.Generic;
using System.Linq;

namespace Nekoyume.SingleClient
{
    public sealed class SingleClientSession
    {
        private readonly ISingleClientStateStore _stateStore;

        public SingleClientSession(ISingleClientStateStore stateStore)
        {
            _stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
        }

        public bool IsStarted => State is not null;

        public SingleClientState State { get; private set; }

        public SingleClientState Start()
        {
            State = _stateStore.LoadOrCreate();
            return State;
        }

        public SingleClientState CreateOrSelectAvatar(string avatarId, string avatarName)
        {
            if (string.IsNullOrWhiteSpace(avatarId))
            {
                throw new ArgumentException("Avatar id is required.", nameof(avatarId));
            }

            if (string.IsNullOrWhiteSpace(avatarName))
            {
                throw new ArgumentException("Avatar name is required.", nameof(avatarName));
            }

            EnsureStarted();
            State.avatarId = avatarId;
            State.avatar ??= SingleClientAvatarState.CreateDefault(avatarId);
            State.avatar.id = avatarId;
            State.avatar.name = avatarName.Trim();
            State.avatar.EnsureDefaults(avatarId);
            _stateStore.Save(State);
            return State;
        }

        public SingleClientState CreateOrSelectAvatar(int slotIndex, string avatarName)
        {
            if (slotIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(slotIndex),
                    slotIndex,
                    "Slot index cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(avatarName))
            {
                throw new ArgumentException("Avatar name is required.", nameof(avatarName));
            }

            EnsureStarted();
            var avatarId = SingleClientState.GetAvatarId(slotIndex);
            State.avatarId = avatarId;
            State.avatar ??= SingleClientAvatarState.CreateDefault(avatarId, slotIndex);
            State.avatar.id = avatarId;
            State.avatar.slotIndex = slotIndex;
            State.avatar.name = avatarName.Trim();
            State.avatar.EnsureDefaults(avatarId, slotIndex);
            State.EnsureDefaults(() => State.privateKeyHex);
            _stateStore.Save(State);
            return State;
        }

        public SingleClientState SelectAvatar(int slotIndex)
        {
            if (slotIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(slotIndex),
                    slotIndex,
                    "Slot index cannot be negative.");
            }

            EnsureStarted();
            State.EnsureDefaults(() => State.privateKeyHex);
            var avatar = State.avatars.FirstOrDefault(candidate => candidate.slotIndex == slotIndex);
            if (avatar is null)
            {
                throw new InvalidOperationException($"Avatar slot {slotIndex} does not exist.");
            }

            State.avatarId = avatar.id;
            State.avatar = avatar.Clone();
            State.avatar.EnsureDefaults(avatar.id, slotIndex);
            State.EnsureDefaults(() => State.privateKeyHex);
            _stateStore.Save(State);
            return State;
        }

        public SingleClientState ChargeActionPoint(long amount)
        {
            if (amount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be positive.");
            }

            EnsureStarted();
            State.avatar ??= SingleClientAvatarState.CreateDefault(State.avatarId);
            State.avatar.actionPoint += amount;
            _stateStore.Save(State);
            return State;
        }

        public SingleClientState FillActionPoint(long actionPoint)
        {
            if (actionPoint < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actionPoint),
                    actionPoint,
                    "Action point cannot be negative.");
            }

            EnsureStarted();
            State.avatar ??= SingleClientAvatarState.CreateDefault(State.avatarId);
            State.avatar.actionPoint = actionPoint;
            _stateStore.Save(State);
            return State;
        }

        public SingleClientState ConsumeActionPoint(long amount)
        {
            if (amount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be positive.");
            }

            EnsureStarted();
            State.avatar ??= SingleClientAvatarState.CreateDefault(State.avatarId);
            if (State.avatar.actionPoint < amount)
            {
                throw new InvalidOperationException("Not enough action point.");
            }

            State.avatar.actionPoint -= amount;
            _stateStore.Save(State);
            return State;
        }

        public SingleClientState GrantInventoryItem(string itemId, long count)
        {
            ValidateItemMutation(itemId, count);

            EnsureStarted();
            State.inventory ??= new SingleClientInventoryState();
            State.inventory.EnsureDefaults();
            State.inventory.Grant(itemId.Trim(), count);
            _stateStore.Save(State);
            return State;
        }

        public SingleClientEquipmentItemState GrantEquipment(
            string nonFungibleId,
            int itemSheetId,
            int level = 0,
            long requiredBlockIndex = 0)
        {
            if (string.IsNullOrWhiteSpace(nonFungibleId))
            {
                throw new ArgumentException("nonFungibleId is required.", nameof(nonFungibleId));
            }

            if (itemSheetId < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(itemSheetId), itemSheetId, "itemSheetId must be positive.");
            }

            EnsureStarted();
            State.inventory ??= new SingleClientInventoryState();
            State.inventory.EnsureDefaults();
            var entry = State.inventory.GrantEquipment(
                nonFungibleId,
                itemSheetId,
                level,
                requiredBlockIndex);
            _stateStore.Save(State);
            return entry;
        }

        public SingleClientEnhanceResult EnhanceEquipment(
            string baseEquipmentId,
            IEnumerable<string> materialEquipmentIds,
            int levelDelta = 1)
        {
            if (string.IsNullOrWhiteSpace(baseEquipmentId))
            {
                throw new ArgumentException(
                    "baseEquipmentId is required.", nameof(baseEquipmentId));
            }

            if (levelDelta < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(levelDelta), levelDelta, "levelDelta must be positive.");
            }

            var materials = (materialEquipmentIds ?? Enumerable.Empty<string>())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .ToArray();
            if (materials.Length == 0)
            {
                throw new ArgumentException(
                    "At least one material equipment is required.", nameof(materialEquipmentIds));
            }

            EnsureStarted();
            State.inventory ??= new SingleClientInventoryState();
            State.inventory.EnsureDefaults();

            var baseEquipment = State.inventory.FindEquipment(baseEquipmentId)
                ?? throw new InvalidOperationException(
                    $"Base equipment '{baseEquipmentId}' not found.");
            if (materials.Contains(baseEquipment.nonFungibleId))
            {
                throw new InvalidOperationException(
                    "Base equipment cannot be included as a material.");
            }

            foreach (var materialId in materials)
            {
                if (!State.inventory.HasEquipment(materialId))
                {
                    throw new InvalidOperationException(
                        $"Material equipment '{materialId}' not found.");
                }
            }

            var previousLevel = baseEquipment.level;
            foreach (var materialId in materials)
            {
                State.inventory.RemoveEquipment(materialId);
            }

            baseEquipment.level += levelDelta;
            baseEquipment.equipped = false;
            _stateStore.Save(State);

            return new SingleClientEnhanceResult(
                State,
                baseEquipment.nonFungibleId,
                previousLevel,
                baseEquipment.level,
                materials);
        }

        public SingleClientState ConsumeInventoryItem(string itemId, long count)
        {
            ValidateItemMutation(itemId, count);

            EnsureStarted();
            State.inventory ??= new SingleClientInventoryState();
            State.inventory.EnsureDefaults();
            if (!State.inventory.TryConsume(itemId.Trim(), count))
            {
                throw new InvalidOperationException("Not enough inventory item.");
            }

            _stateStore.Save(State);
            return State;
        }

        public SingleClientStagePlayPreview PreviewStagePlay(
            int stageId,
            long actionPointCost,
            string entryCostItemId = null,
            long entryCostItemCount = 0)
        {
            return PreviewStagePlay(
                stageId,
                actionPointCost,
                CreateItemCosts(entryCostItemId, entryCostItemCount));
        }

        public SingleClientStagePlayPreview PreviewStagePlay(
            int stageId,
            long actionPointCost,
            IEnumerable<ClientItemCost> itemCosts)
        {
            ValidateStagePlay(stageId, actionPointCost);
            var normalizedItemCosts = NormalizeItemCosts(itemCosts);
            EnsureStarted();
            EnsureStagePlayState();

            var actionPointBalance = State.avatar.actionPoint;
            var itemCostPreviews = normalizedItemCosts
                .Select(itemCost => new SingleClientItemCostPreview(
                    itemCost.ItemId,
                    itemCost.Count,
                    State.inventory.GetCount(itemCost.ItemId)))
                .ToArray();

            return new SingleClientStagePlayPreview(
                State,
                stageId,
                !State.stage.IsCleared(stageId),
                actionPointCost,
                actionPointBalance,
                itemCostPreviews);
        }

        public SingleClientStagePlayResult PlayStage(
            int stageId,
            long actionPointCost,
            string entryCostItemId = null,
            long entryCostItemCount = 0)
        {
            return PlayStage(
                stageId,
                actionPointCost,
                CreateItemCosts(entryCostItemId, entryCostItemCount));
        }

        public SingleClientStagePlayResult PlayStage(
            int stageId,
            long actionPointCost,
            IEnumerable<ClientItemCost> itemCosts)
        {
            var preview = PreviewStagePlay(
                stageId,
                actionPointCost,
                itemCosts);
            if (!preview.HasEnoughActionPoint)
            {
                throw new InvalidOperationException("Not enough action point.");
            }

            if (!preview.HasEnoughEntryCostItem)
            {
                throw new InvalidOperationException("Not enough inventory item.");
            }

            State.avatar.actionPoint -= actionPointCost;
            var itemCostDeltas = new List<SingleClientItemCostDelta>();
            foreach (var itemCost in preview.ItemCosts)
            {
                State.inventory.TryConsume(itemCost.ItemId, itemCost.Count);
                itemCostDeltas.Add(new SingleClientItemCostDelta(
                    itemCost.ItemId,
                    itemCost.Count,
                    itemCost.Balance,
                    State.inventory.GetCount(itemCost.ItemId)));
            }

            State.stage.Clear(stageId);
            _stateStore.Save(State);
            return new SingleClientStagePlayResult(
                State,
                stageId,
                preview.WasFirstClear,
                actionPointCost,
                preview.ActionPointBalance,
                State.avatar.actionPoint,
                itemCostDeltas);
        }

        public SingleClientState ClearStage(int stageId)
        {
            if (stageId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(stageId), stageId, "Stage id must be positive.");
            }

            EnsureStarted();
            State.stage ??= new SingleClientStageState();
            State.stage.EnsureDefaults();
            State.stage.Clear(stageId);
            _stateStore.Save(State);
            return State;
        }

        public SingleClientSweepResult SweepStage(
            int stageId,
            long actionPointCost,
            string apStoneItemId = null,
            long apStoneCount = 0,
            string entryCostItemId = null,
            long entryCostItemCount = 0)
        {
            return SweepStage(
                stageId,
                actionPointCost,
                CreateSweepItemCosts(apStoneItemId, apStoneCount, entryCostItemId, entryCostItemCount));
        }

        public SingleClientSweepResult SweepStage(
            int stageId,
            long actionPointCost,
            IEnumerable<ClientItemCost> itemCosts)
        {
            ValidateStagePlay(stageId, actionPointCost);
            var normalizedItemCosts = NormalizeItemCosts(itemCosts);
            EnsureStarted();
            EnsureStagePlayState();

            if (State.avatar.actionPoint < actionPointCost)
            {
                throw new InvalidOperationException("Not enough action point.");
            }

            foreach (var cost in normalizedItemCosts)
            {
                if (State.inventory.GetCount(cost.ItemId) < cost.Count)
                {
                    throw new InvalidOperationException(
                        $"Not enough inventory item '{cost.ItemId}'.");
                }
            }

            var actionPointBalanceBefore = State.avatar.actionPoint;
            State.avatar.actionPoint -= actionPointCost;

            var itemCostDeltas = new List<SingleClientItemCostDelta>();
            foreach (var cost in normalizedItemCosts)
            {
                var beforeBalance = State.inventory.GetCount(cost.ItemId);
                State.inventory.TryConsume(cost.ItemId, cost.Count);
                itemCostDeltas.Add(new SingleClientItemCostDelta(
                    cost.ItemId,
                    cost.Count,
                    beforeBalance,
                    State.inventory.GetCount(cost.ItemId)));
            }

            var wasFirstClear = !State.stage.IsCleared(stageId);
            State.stage.Clear(stageId);
            _stateStore.Save(State);

            return new SingleClientSweepResult(
                State,
                stageId,
                wasFirstClear,
                actionPointCost,
                actionPointBalanceBefore,
                State.avatar.actionPoint,
                itemCostDeltas);
        }

        public long AdvanceBlock(long count = 1)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be positive.");
            }

            EnsureStarted();
            State.blockIndex += count;
            _stateStore.Save(State);
            return State.blockIndex;
        }

        public void Save()
        {
            EnsureStarted();
            _stateStore.Save(State);
        }

        private void EnsureStarted()
        {
            if (!IsStarted)
            {
                throw new InvalidOperationException("Single-client session is not started.");
            }
        }

        private static void ValidateItemMutation(string itemId, long count)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                throw new ArgumentException("Item id is required.", nameof(itemId));
            }

            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be positive.");
            }
        }

        private void EnsureStagePlayState()
        {
            State.avatar ??= SingleClientAvatarState.CreateDefault(State.avatarId);
            State.inventory ??= new SingleClientInventoryState();
            State.inventory.EnsureDefaults();
            State.stage ??= new SingleClientStageState();
            State.stage.EnsureDefaults();
        }

        private static bool HasEntryCost(string entryCostItemId, long entryCostItemCount)
        {
            return !string.IsNullOrWhiteSpace(entryCostItemId) || entryCostItemCount > 0;
        }

        private static void ValidateStagePlay(
            int stageId,
            long actionPointCost)
        {
            if (stageId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(stageId), stageId, "Stage id must be positive.");
            }

            if (actionPointCost < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actionPointCost),
                    actionPointCost,
                    "Action point cost cannot be negative.");
            }
        }

        private static IReadOnlyList<ClientItemCost> CreateItemCosts(
            string itemId,
            long count)
        {
            if (!HasEntryCost(itemId, count))
            {
                return Array.Empty<ClientItemCost>();
            }

            return new[]
            {
                new ClientItemCost(itemId, count)
            };
        }

        private static IReadOnlyList<ClientItemCost> CreateSweepItemCosts(
            string apStoneItemId,
            long apStoneCount,
            string entryCostItemId,
            long entryCostItemCount)
        {
            var list = new List<ClientItemCost>(2);
            if (HasEntryCost(apStoneItemId, apStoneCount))
            {
                list.Add(new ClientItemCost(apStoneItemId, apStoneCount));
            }

            if (HasEntryCost(entryCostItemId, entryCostItemCount))
            {
                list.Add(new ClientItemCost(entryCostItemId, entryCostItemCount));
            }

            return list;
        }

        private static IReadOnlyList<ClientItemCost> NormalizeItemCosts(
            IEnumerable<ClientItemCost> itemCosts)
        {
            if (itemCosts is null)
            {
                return Array.Empty<ClientItemCost>();
            }

            var counts = new Dictionary<string, long>(StringComparer.Ordinal);
            foreach (var itemCost in itemCosts)
            {
                if (itemCost is null)
                {
                    continue;
                }

                ValidateItemMutation(itemCost.ItemId, itemCost.Count);
                var itemId = itemCost.ItemId.Trim();
                counts.TryGetValue(itemId, out var currentCount);
                counts[itemId] = checked(currentCount + itemCost.Count);
            }

            return counts
                .OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .Select(pair => new ClientItemCost(pair.Key, pair.Value))
                .ToArray();
        }
    }
}
