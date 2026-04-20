#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.IO;
using System.Numerics;
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

        [Test]
        public void AddCurrencyAccumulatesBalance()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));

            session.Start();
            session.CreateOrSelectAvatar(0, "wallet");

            var afterFirst = session.AddCurrency("CRYSTAL", new BigInteger(500));
            var afterSecond = session.AddCurrency("CRYSTAL", new BigInteger(250));

            Assert.AreEqual(new BigInteger(500), afterFirst);
            Assert.AreEqual(new BigInteger(750), afterSecond);
            Assert.AreEqual(new BigInteger(750), session.GetCurrency("CRYSTAL"));
        }

        [Test]
        public void ConsumeCurrencyReducesBalanceAndRejectsOverspend()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));

            session.Start();
            session.CreateOrSelectAvatar(0, "wallet");
            session.AddCurrency("CRYSTAL", new BigInteger(100));

            var remaining = session.ConsumeCurrency("CRYSTAL", new BigInteger(30));
            Assert.AreEqual(new BigInteger(70), remaining);

            Assert.Throws<InvalidOperationException>(
                () => session.ConsumeCurrency("CRYSTAL", new BigInteger(200)));
        }

        [Test]
        public void GrindEquipmentRemovesEquipmentsAndGrantsCrystal()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));

            session.Start();
            session.CreateOrSelectAvatar(0, "grinder");
            session.GrantEquipment("eq-1", itemSheetId: 10110000);
            session.GrantEquipment("eq-2", itemSheetId: 10110000);
            session.GrantEquipment("eq-keep", itemSheetId: 10110000);

            var result = session.GrindEquipment(
                new[] { "eq-1", "eq-2" },
                new BigInteger(1500));

            Assert.AreEqual(2, result.EquipmentIds.Count);
            Assert.AreEqual(new BigInteger(0), result.BalanceBefore);
            Assert.AreEqual(new BigInteger(1500), result.BalanceAfter);
            Assert.AreEqual("CRYSTAL", result.CurrencyTicker);
            Assert.IsFalse(session.State.inventory.HasEquipment("eq-1"));
            Assert.IsFalse(session.State.inventory.HasEquipment("eq-2"));
            Assert.IsTrue(session.State.inventory.HasEquipment("eq-keep"));
            Assert.AreEqual(new BigInteger(1500), session.GetCurrency("CRYSTAL"));
        }

        [Test]
        public void GrindEquipmentThrowsWhenEquipmentMissing()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));

            session.Start();
            session.CreateOrSelectAvatar(0, "grinder");
            session.GrantEquipment("eq-1", itemSheetId: 10110000);

            Assert.Throws<InvalidOperationException>(
                () => session.GrindEquipment(
                    new[] { "eq-1", "eq-ghost" },
                    new BigInteger(100)));
        }
    }
}

#endif
