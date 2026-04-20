#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.IO;
using Nekoyume.SingleClient;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient
{
    public class FileSingleClientStateStoreTest
    {
        private const string PrivateKeyHex =
            "1111111111111111111111111111111111111111111111111111111111111111";

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
        public void LoadOrCreateCreatesDefaultState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var now = DateTimeOffset.FromUnixTimeSeconds(1234);
            var store = new FileSingleClientStateStore(path, () => now, () => PrivateKeyHex);

            var state = store.LoadOrCreate();

            Assert.IsTrue(store.Exists);
            Assert.AreEqual(SingleClientState.CurrentVersion, state.version);
            Assert.AreEqual(SingleClientState.DefaultPlayerId, state.playerId);
            Assert.AreEqual(SingleClientState.DefaultAvatarId, state.avatarId);
            Assert.AreEqual(PrivateKeyHex, state.privateKeyHex);
            Assert.IsNotNull(state.avatar);
            Assert.AreEqual(SingleClientState.DefaultAvatarName, state.avatar.name);
            Assert.AreEqual(1, state.avatar.level);
            Assert.AreEqual(1, state.avatars.Count);
            Assert.AreEqual(0, state.avatars[0].slotIndex);
            Assert.AreEqual(SingleClientState.DefaultAvatarId, state.avatars[0].id);
            Assert.AreEqual(0, state.blockIndex);
            Assert.AreEqual(1234, state.updatedAtUnixSeconds);
        }

        [Test]
        public void SavePersistsState()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var savedAt = DateTimeOffset.FromUnixTimeSeconds(5678);
            var store = new FileSingleClientStateStore(path, () => savedAt, () => PrivateKeyHex);
            var state = SingleClientState.CreateDefault(DateTimeOffset.FromUnixTimeSeconds(0));
            state.playerId = "player-a";
            state.avatarId = "avatar-a";
            state.blockIndex = 42;

            store.Save(state);
            var loaded = store.LoadOrCreate();

            Assert.AreEqual("player-a", loaded.playerId);
            Assert.AreEqual("avatar-a", loaded.avatarId);
            Assert.AreEqual(PrivateKeyHex, loaded.privateKeyHex);
            Assert.AreEqual(42, loaded.blockIndex);
            Assert.AreEqual(5678, loaded.updatedAtUnixSeconds);
        }

        [Test]
        public void LoadOrCreateBackfillsMissingDefaults()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            Directory.CreateDirectory(_temporaryDirectory);
            File.WriteAllText(path, "{}");
            var store = new FileSingleClientStateStore(
                path,
                () => DateTimeOffset.FromUnixTimeSeconds(9999),
                () => PrivateKeyHex);

            var state = store.LoadOrCreate();

            Assert.AreEqual(SingleClientState.CurrentVersion, state.version);
            Assert.AreEqual(SingleClientState.DefaultPlayerId, state.playerId);
            Assert.AreEqual(SingleClientState.DefaultAvatarId, state.avatarId);
            Assert.AreEqual(PrivateKeyHex, state.privateKeyHex);
            Assert.IsNotNull(state.avatar);
            Assert.AreEqual(SingleClientState.DefaultAvatarId, state.avatar.id);
            Assert.AreEqual(SingleClientState.DefaultAvatarName, state.avatar.name);
            Assert.AreEqual(1, state.avatar.level);
            Assert.AreEqual(1, state.avatars.Count);
            Assert.AreEqual(SingleClientState.DefaultAvatarId, state.avatars[0].id);
            Assert.IsNotNull(state.inventory);
            Assert.IsNotNull(state.inventory.items);
            Assert.IsNotNull(state.inventory.equipments);
            Assert.IsNotNull(state.balances);
        }

        [Test]
        public void EquipmentInventoryRoundTrips()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));
            session.Start();
            session.CreateOrSelectAvatar(0, "keeper");
            session.GrantEquipment("eq-1", itemSheetId: 10110000, level: 2);
            session.GrantEquipment("eq-2", itemSheetId: 10110100, level: 0, requiredBlockIndex: 7);

            var reloaded = new FileSingleClientStateStore(path).LoadOrCreate();

            Assert.AreEqual(2, reloaded.inventory.equipments.Count);
            Assert.AreEqual("eq-1", reloaded.inventory.equipments[0].nonFungibleId);
            Assert.AreEqual(10110000, reloaded.inventory.equipments[0].itemSheetId);
            Assert.AreEqual(2, reloaded.inventory.equipments[0].level);
            Assert.AreEqual("eq-2", reloaded.inventory.equipments[1].nonFungibleId);
            Assert.AreEqual(7, reloaded.inventory.equipments[1].requiredBlockIndex);
        }

        [Test]
        public void CurrencyBalancesRoundTripAcrossReload()
        {
            var path = Path.Combine(_temporaryDirectory, "state.json");
            var session = new SingleClientSession(new FileSingleClientStateStore(path));
            session.Start();
            session.CreateOrSelectAvatar(0, "wallet");
            session.AddCurrency("CRYSTAL", new System.Numerics.BigInteger(123456789));
            session.AddCurrency("Mead", new System.Numerics.BigInteger(42));

            var reloaded = new FileSingleClientStateStore(path).LoadOrCreate();

            Assert.AreEqual(2, reloaded.balances.Count);
            var crystal = reloaded.balances.Find(b => b.ticker == "CRYSTAL");
            var mead = reloaded.balances.Find(b => b.ticker == "Mead");
            Assert.IsNotNull(crystal);
            Assert.AreEqual("123456789", crystal.rawValue);
            Assert.IsNotNull(mead);
            Assert.AreEqual("42", mead.rawValue);
        }
    }
}

#endif
