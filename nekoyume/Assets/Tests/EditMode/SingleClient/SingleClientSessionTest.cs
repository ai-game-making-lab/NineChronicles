using System;
using System.IO;
using Nekoyume.SingleClient;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient
{
    public class SingleClientSessionTest
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
        public void StartLoadsState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var store = new FileSingleClientStateStore(path);
            var session = new SingleClientSession(store);

            var state = session.Start();

            Assert.IsTrue(session.IsStarted);
            Assert.AreSame(state, session.State);
        }

        [Test]
        public void AdvanceBlockPersistsState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));

            session.Start();
            var blockIndex = session.AdvanceBlock(3);
            var reloaded = new FileSingleClientStateStore(path).LoadOrCreate();

            Assert.AreEqual(3, blockIndex);
            Assert.AreEqual(3, reloaded.blockIndex);
        }

        [Test]
        public void SweepStageConsumesApAndItemsAndMarksCleared()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));

            session.Start();
            session.CreateOrSelectAvatar(0, "sweeper");
            session.FillActionPoint(120);
            session.GrantInventoryItem("apStone", 2);
            session.GrantInventoryItem("entry", 10);

            var result = session.SweepStage(
                stageId: 5,
                actionPointCost: 40,
                apStoneItemId: "apStone",
                apStoneCount: 2,
                entryCostItemId: "entry",
                entryCostItemCount: 4);

            Assert.AreEqual(5, result.StageId);
            Assert.IsTrue(result.WasFirstClear);
            Assert.AreEqual(120, result.ActionPointBefore);
            Assert.AreEqual(80, result.ActionPointAfter);
            Assert.AreEqual(0, session.State.inventory.GetCount("apStone"));
            Assert.AreEqual(6, session.State.inventory.GetCount("entry"));
            Assert.IsTrue(session.State.stage.IsCleared(5));
        }

        [Test]
        public void SweepStageThrowsWhenActionPointInsufficient()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));

            session.Start();
            session.CreateOrSelectAvatar(0, "sweeper");
            session.FillActionPoint(10);

            Assert.Throws<InvalidOperationException>(
                () => session.SweepStage(stageId: 1, actionPointCost: 40));
        }

        [Test]
        public void SweepStageThrowsWhenItemInsufficient()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));

            session.Start();
            session.CreateOrSelectAvatar(0, "sweeper");
            session.FillActionPoint(100);
            session.GrantInventoryItem("entry", 2);

            Assert.Throws<InvalidOperationException>(
                () => session.SweepStage(
                    stageId: 1,
                    actionPointCost: 10,
                    entryCostItemId: "entry",
                    entryCostItemCount: 5));
        }

        [Test]
        public void EnhanceEquipmentLevelsUpBaseAndRemovesMaterials()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));

            session.Start();
            session.CreateOrSelectAvatar(0, "enhancer");
            session.GrantEquipment("eq-base", itemSheetId: 10110000);
            session.GrantEquipment("eq-mat-1", itemSheetId: 10110000);
            session.GrantEquipment("eq-mat-2", itemSheetId: 10110000);

            var result = session.EnhanceEquipment(
                baseEquipmentId: "eq-base",
                materialEquipmentIds: new[] { "eq-mat-1", "eq-mat-2" },
                levelDelta: 2);

            Assert.AreEqual("eq-base", result.BaseEquipmentId);
            Assert.AreEqual(0, result.LevelBefore);
            Assert.AreEqual(2, result.LevelAfter);
            Assert.AreEqual(2, result.MaterialEquipmentIds.Count);
            Assert.IsTrue(session.State.inventory.HasEquipment("eq-base"));
            Assert.IsFalse(session.State.inventory.HasEquipment("eq-mat-1"));
            Assert.IsFalse(session.State.inventory.HasEquipment("eq-mat-2"));
        }

        [Test]
        public void EnhanceEquipmentThrowsWhenBaseMissing()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));

            session.Start();
            session.CreateOrSelectAvatar(0, "enhancer");
            session.GrantEquipment("eq-mat", itemSheetId: 10110000);

            Assert.Throws<InvalidOperationException>(
                () => session.EnhanceEquipment("eq-ghost", new[] { "eq-mat" }));
        }

        [Test]
        public void EnhanceEquipmentThrowsWhenMaterialMissing()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));

            session.Start();
            session.CreateOrSelectAvatar(0, "enhancer");
            session.GrantEquipment("eq-base", itemSheetId: 10110000);

            Assert.Throws<InvalidOperationException>(
                () => session.EnhanceEquipment("eq-base", new[] { "eq-absent" }));
        }

        [Test]
        public void EnhanceEquipmentRejectsBaseAsMaterial()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));

            session.Start();
            session.CreateOrSelectAvatar(0, "enhancer");
            session.GrantEquipment("eq-base", itemSheetId: 10110000);

            Assert.Throws<InvalidOperationException>(
                () => session.EnhanceEquipment("eq-base", new[] { "eq-base" }));
        }
    }
}
