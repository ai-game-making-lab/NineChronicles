#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.IO;
using System.Numerics;
using Nekoyume.SingleClient;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient
{
    public class SingleClientRuntimeTest
    {
        private string _temporaryDirectory;

        [SetUp]
        public void SetUp()
        {
            _temporaryDirectory = Path.Combine(
                Path.GetTempPath(),
                "NineChroniclesSingleClientTests",
                Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_temporaryDirectory))
            {
                Directory.Delete(_temporaryDirectory, true);
            }
        }

        [Test]
        public void StartExposesLibplanetFreeRuntimeState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(
                path,
                () => DateTimeOffset.FromUnixTimeSeconds(100),
                () => "2222222222222222222222222222222222222222222222222222222222222222"));
            var runtime = new SingleClientRuntime(session);

            var state = runtime.Start();

            Assert.IsTrue(runtime.IsStarted);
            Assert.AreEqual(SingleClientState.CurrentVersion, state.Version);
            Assert.AreEqual(SingleClientState.DefaultPlayerId, state.PlayerId);
            Assert.AreEqual(SingleClientState.DefaultAvatarId, state.AvatarId);
            Assert.AreEqual(SingleClientState.DefaultAvatarName, state.AvatarName);
            Assert.AreEqual(0, state.AvatarSlotIndex);
            Assert.AreEqual(1, state.AvatarLevel);
            Assert.AreEqual(0, state.ActionPoint);
            Assert.AreEqual(1, state.Avatars.Count);
            Assert.IsEmpty(state.InventoryItems);
            Assert.IsEmpty(state.ClearedStageIds);
            Assert.AreEqual(0, state.HighestClearedStageId);
            Assert.AreEqual(0, state.BlockIndex);
            Assert.AreEqual(100, state.UpdatedAtUnixSeconds);
        }

        [Test]
        public void AdvanceBlockUpdatesRuntimeState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.AdvanceBlock(2);

            Assert.AreEqual(2, runtime.State.BlockIndex);
        }

        [Test]
        public void CreateOrSelectAvatarPersistsLocalState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            var state = runtime.CreateOrSelectAvatar("avatar-2", "Second Avatar");
            var reloaded = new FileSingleClientStateStore(path).LoadOrCreate();

            Assert.AreEqual("avatar-2", state.AvatarId);
            Assert.AreEqual("Second Avatar", state.AvatarName);
            Assert.AreEqual("avatar-2", reloaded.avatarId);
            Assert.AreEqual("avatar-2", reloaded.avatar.id);
            Assert.AreEqual("Second Avatar", reloaded.avatar.name);
        }

        [Test]
        public void CreateOrSelectAvatarSlotPersistsLocalState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            var state = runtime.CreateOrSelectAvatar(2, "Slot Two");
            var reloaded = new FileSingleClientStateStore(path).LoadOrCreate();

            Assert.AreEqual("local-avatar-2", state.AvatarId);
            Assert.AreEqual(2, state.AvatarSlotIndex);
            Assert.AreEqual("Slot Two", state.AvatarName);
            Assert.AreEqual(2, state.Avatars.Count);
            Assert.AreEqual(0, state.Avatars[0].SlotIndex);
            Assert.AreEqual(2, state.Avatars[1].SlotIndex);
            Assert.AreEqual("local-avatar-2", reloaded.avatarId);
            Assert.AreEqual(2, reloaded.avatar.slotIndex);
            Assert.AreEqual(2, reloaded.avatars[1].slotIndex);
            Assert.AreEqual("Slot Two", reloaded.avatars[1].name);
        }

        [Test]
        public void SelectAvatarSlotPersistsSelectedLocalState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.CreateOrSelectAvatar(2, "Slot Two");
            runtime.CreateOrSelectAvatar(1, "Slot One");
            var state = runtime.SelectAvatar(2);
            var reloaded = new FileSingleClientStateStore(path).LoadOrCreate();

            Assert.AreEqual("local-avatar-2", state.AvatarId);
            Assert.AreEqual(2, state.AvatarSlotIndex);
            Assert.AreEqual("Slot Two", state.AvatarName);
            Assert.AreEqual(3, state.Avatars.Count);
            Assert.AreEqual("local-avatar-2", reloaded.avatarId);
            Assert.AreEqual(2, reloaded.avatar.slotIndex);
            Assert.AreEqual("Slot Two", reloaded.avatar.name);
        }

        [Test]
        public void SelectAvatarSlotRejectsMissingSlot()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();

            Assert.Throws<InvalidOperationException>(() => runtime.SelectAvatar(3));
        }

        [Test]
        public void ChargeAndConsumeActionPointPersistLocalState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.ChargeActionPoint(10);
            var state = runtime.ConsumeActionPoint(3);
            var reloaded = new FileSingleClientStateStore(path).LoadOrCreate();

            Assert.AreEqual(7, state.ActionPoint);
            Assert.AreEqual(7, reloaded.avatar.actionPoint);
        }

        [Test]
        public void ConsumeActionPointRejectsInsufficientBalance()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();

            Assert.Throws<InvalidOperationException>(() => runtime.ConsumeActionPoint(1));
        }

        [Test]
        public void FillActionPointPersistsLocalState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.ChargeActionPoint(3);
            var state = runtime.FillActionPoint(120);
            var reloaded = new FileSingleClientStateStore(path).LoadOrCreate();

            Assert.AreEqual(120, state.ActionPoint);
            Assert.AreEqual(120, reloaded.avatar.actionPoint);
        }

        [Test]
        public void GrantAndConsumeInventoryItemPersistLocalState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.GrantInventoryItem("item-2", 4);
            runtime.GrantInventoryItem("item-1", 3);
            var state = runtime.ConsumeInventoryItem("item-2", 1);
            var reloaded = new FileSingleClientStateStore(path).LoadOrCreate();

            Assert.AreEqual(2, state.InventoryItems.Count);
            Assert.AreEqual("item-1", state.InventoryItems[0].ItemId);
            Assert.AreEqual(3, state.InventoryItems[0].Count);
            Assert.AreEqual("item-2", state.InventoryItems[1].ItemId);
            Assert.AreEqual(3, state.InventoryItems[1].Count);
            Assert.AreEqual(2, reloaded.inventory.items.Count);
            Assert.AreEqual("item-2", reloaded.inventory.items[1].itemId);
            Assert.AreEqual(3, reloaded.inventory.items[1].count);
        }

        [Test]
        public void ConsumeInventoryItemRejectsInsufficientBalance()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.GrantInventoryItem("item-1", 1);

            Assert.Throws<InvalidOperationException>(
                () => runtime.ConsumeInventoryItem("item-1", 2));
        }

        [Test]
        public void PlayStageConsumesCostsAndPersistsClear()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.FillActionPoint(10);
            runtime.GrantInventoryItem("101", 3);
            var result = runtime.PlayStage(7, 4, "101", 2);
            var state = result.State;
            var reloaded = new FileSingleClientStateStore(path).LoadOrCreate();

            Assert.AreEqual(7, result.StageId);
            Assert.IsTrue(result.WasFirstClear);
            Assert.AreEqual(4, result.ActionPointCost);
            Assert.AreEqual(10, result.ActionPointBefore);
            Assert.AreEqual(6, result.ActionPointAfter);
            Assert.AreEqual("101", result.EntryCostItemId);
            Assert.AreEqual(2, result.EntryCostItemCount);
            Assert.AreEqual(3, result.EntryCostItemCountBefore);
            Assert.AreEqual(1, result.EntryCostItemCountAfter);
            Assert.AreEqual(6, state.ActionPoint);
            Assert.AreEqual(1, state.InventoryItems.Count);
            Assert.AreEqual("101", state.InventoryItems[0].ItemId);
            Assert.AreEqual(1, state.InventoryItems[0].Count);
            CollectionAssert.AreEqual(new[] { 7 }, state.ClearedStageIds);
            Assert.AreEqual(7, state.HighestClearedStageId);
            Assert.AreEqual(6, reloaded.avatar.actionPoint);
            Assert.AreEqual(1, reloaded.inventory.items[0].count);
            CollectionAssert.AreEqual(new[] { 7 }, reloaded.stage.clearedStageIds);
        }

        [Test]
        public void PlayStageConsumesMultipleItemCostsAtomically()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.FillActionPoint(10);
            runtime.GrantInventoryItem("101", 3);
            runtime.GrantInventoryItem("500000", 2);
            var result = runtime.PlayStage(
                7,
                4,
                new[]
                {
                    new ClientItemCost("101", 2),
                    new ClientItemCost("500000", 1)
                });
            var reloaded = new FileSingleClientStateStore(path).LoadOrCreate();

            Assert.AreEqual(2, result.ItemCostDeltas.Count);
            Assert.AreEqual("101", result.ItemCostDeltas[0].ItemId);
            Assert.AreEqual(2, result.ItemCostDeltas[0].Count);
            Assert.AreEqual(3, result.ItemCostDeltas[0].CountBefore);
            Assert.AreEqual(1, result.ItemCostDeltas[0].CountAfter);
            Assert.AreEqual("500000", result.ItemCostDeltas[1].ItemId);
            Assert.AreEqual(1, result.ItemCostDeltas[1].Count);
            Assert.AreEqual(2, result.ItemCostDeltas[1].CountBefore);
            Assert.AreEqual(1, result.ItemCostDeltas[1].CountAfter);
            Assert.AreEqual(6, result.State.ActionPoint);
            CollectionAssert.AreEqual(new[] { 7 }, result.State.ClearedStageIds);
            Assert.AreEqual(2, reloaded.inventory.items.Count);
            Assert.AreEqual("101", reloaded.inventory.items[0].itemId);
            Assert.AreEqual(1, reloaded.inventory.items[0].count);
            Assert.AreEqual("500000", reloaded.inventory.items[1].itemId);
            Assert.AreEqual(1, reloaded.inventory.items[1].count);
        }

        [Test]
        public void PlayStageRejectsInsufficientAdditionalItemWithoutPartialMutation()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.FillActionPoint(10);
            runtime.GrantInventoryItem("101", 3);

            Assert.Throws<InvalidOperationException>(() => runtime.PlayStage(
                7,
                4,
                new[]
                {
                    new ClientItemCost("101", 2),
                    new ClientItemCost("500000", 1)
                }));
            Assert.AreEqual(10, runtime.State.ActionPoint);
            Assert.AreEqual(1, runtime.State.InventoryItems.Count);
            Assert.AreEqual("101", runtime.State.InventoryItems[0].ItemId);
            Assert.AreEqual(3, runtime.State.InventoryItems[0].Count);
            Assert.IsEmpty(runtime.State.ClearedStageIds);
        }

        [Test]
        public void PreviewStagePlayReportsBalancesWithoutMutation()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.FillActionPoint(3);
            runtime.GrantInventoryItem("101", 1);
            var preview = runtime.PreviewStagePlay(7, 5, "101", 2);

            Assert.IsFalse(preview.CanPlay);
            Assert.IsTrue(preview.WasFirstClear);
            Assert.AreEqual(7, preview.StageId);
            Assert.AreEqual(5, preview.ActionPointCost);
            Assert.AreEqual(3, preview.ActionPointBalance);
            Assert.AreEqual(2, preview.ActionPointShortfall);
            Assert.IsFalse(preview.HasEnoughActionPoint);
            Assert.IsTrue(preview.HasEntryCost);
            Assert.AreEqual("101", preview.EntryCostItemId);
            Assert.AreEqual(2, preview.EntryCostItemCount);
            Assert.AreEqual(1, preview.EntryCostItemBalance);
            Assert.AreEqual(1, preview.EntryCostItemShortfall);
            Assert.IsFalse(preview.HasEnoughEntryCostItem);
            Assert.AreEqual(3, runtime.State.ActionPoint);
            Assert.AreEqual(1, runtime.State.InventoryItems[0].Count);
            Assert.IsEmpty(runtime.State.ClearedStageIds);
        }

        [Test]
        public void PlayStageReportsRepeatClear()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.FillActionPoint(10);
            runtime.PlayStage(7, 1);
            var result = runtime.PlayStage(7, 2);

            Assert.IsFalse(result.WasFirstClear);
            Assert.AreEqual(9, result.ActionPointBefore);
            Assert.AreEqual(7, result.ActionPointAfter);
            Assert.AreEqual(0, result.EntryCostItemCount);
            Assert.IsNull(result.EntryCostItemId);
            CollectionAssert.AreEqual(new[] { 7 }, result.State.ClearedStageIds);
        }

        [Test]
        public void PlayStageRejectsInsufficientActionPointWithoutConsumingEntryItem()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.FillActionPoint(1);
            runtime.GrantInventoryItem("101", 3);

            Assert.Throws<InvalidOperationException>(() => runtime.PlayStage(7, 2, "101", 2));
            Assert.AreEqual(1, runtime.State.ActionPoint);
            Assert.AreEqual(3, runtime.State.InventoryItems[0].Count);
            Assert.IsEmpty(runtime.State.ClearedStageIds);
        }

        [Test]
        public void PlayStageRejectsInsufficientEntryItemWithoutConsumingActionPoint()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.FillActionPoint(10);
            runtime.GrantInventoryItem("101", 1);

            Assert.Throws<InvalidOperationException>(() => runtime.PlayStage(7, 4, "101", 2));
            Assert.AreEqual(10, runtime.State.ActionPoint);
            Assert.AreEqual(1, runtime.State.InventoryItems[0].Count);
            Assert.IsEmpty(runtime.State.ClearedStageIds);
        }

        [Test]
        public void ClearStagePersistsLocalState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.ClearStage(3);
            runtime.ClearStage(1);
            var state = runtime.ClearStage(3);
            var reloaded = new FileSingleClientStateStore(path).LoadOrCreate();

            CollectionAssert.AreEqual(new[] { 1, 3 }, state.ClearedStageIds);
            Assert.AreEqual(3, state.HighestClearedStageId);
            CollectionAssert.AreEqual(new[] { 1, 3 }, reloaded.stage.clearedStageIds);
            Assert.AreEqual(3, reloaded.stage.highestClearedStageId);
        }

        [Test]
        public void SweepStageConsumesCostsAndMarksCleared()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.FillActionPoint(80);
            runtime.GrantInventoryItem("apStone", 1);
            runtime.GrantInventoryItem("entry", 6);

            var result = runtime.SweepStage(
                stageId: 3,
                actionPointCost: 20,
                apStoneItemId: "apStone",
                apStoneCount: 1,
                entryCostItemId: "entry",
                entryCostItemCount: 4);

            Assert.AreEqual(3, result.StageId);
            Assert.IsTrue(result.WasFirstClear);
            Assert.AreEqual(80, result.ActionPointBefore);
            Assert.AreEqual(60, result.ActionPointAfter);
            Assert.AreEqual(2, result.ItemCostDeltas.Count);
            CollectionAssert.AreEqual(new[] { 3 }, result.State.ClearedStageIds);
        }

        [Test]
        public void GrantAndEnhanceEquipmentUpdatesBaseLevel()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.GrantEquipment("eq-base", itemSheetId: 10110000);
            runtime.GrantEquipment("eq-mat", itemSheetId: 10110000);

            var result = runtime.EnhanceEquipment(
                baseEquipmentId: "eq-base",
                materialEquipmentIds: new[] { "eq-mat" },
                levelDelta: 3);

            Assert.AreEqual("eq-base", result.BaseEquipmentId);
            Assert.AreEqual(0, result.LevelBefore);
            Assert.AreEqual(3, result.LevelAfter);
            Assert.AreEqual(1, result.MaterialEquipmentIds.Count);
        }

        [Test]
        public void GrindEquipmentRemovesItemsAndReturnsBalanceDelta()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var runtime = new SingleClientRuntime(new SingleClientSession(
                new FileSingleClientStateStore(path)));

            runtime.Start();
            runtime.GrantEquipment("eq-1", itemSheetId: 10110000);
            runtime.GrantEquipment("eq-2", itemSheetId: 10110000);

            var result = runtime.GrindEquipment(
                equipmentIds: new[] { "eq-1", "eq-2" },
                crystalGained: new BigInteger(2500));

            Assert.AreEqual(2, result.EquipmentIds.Count);
            Assert.AreEqual(new BigInteger(0), result.BalanceBefore);
            Assert.AreEqual(new BigInteger(2500), result.BalanceAfter);
            Assert.AreEqual(new BigInteger(2500), runtime.GetCurrency("CRYSTAL"));
        }
    }
}

#endif
