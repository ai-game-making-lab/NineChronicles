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
        }
    }
}
