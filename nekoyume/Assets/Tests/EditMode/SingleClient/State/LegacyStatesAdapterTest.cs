using Nekoyume.SingleClient.State;
using NUnit.Framework;
using Lib9cStates = Nekoyume.State.States;

namespace Tests.EditMode.SingleClient.State
{
    /// <summary>
    /// Covers <see cref="LegacyStatesAdapter"/>'s read-through behavior against a freshly
    /// constructed <see cref="Lib9cStates"/> instance (no avatar selected, no agent set).
    /// </summary>
    /// <remarks>
    /// <see cref="Lib9cStates"/> has a parameterless ctor that simply calls
    /// <c>DeselectAvatar()</c>, so constructing one in isolation (without
    /// <c>Game.Game.instance</c>) is safe for the empty-state assertions. Assertions that
    /// require a populated <c>AvatarState</c> / <c>AgentState</c> are marked
    /// <c>[Ignore]</c> because the lib9c constructors for those types pull in <c>QuestList</c>
    /// + <c>WorldSheet</c> fixtures that the existing
    /// <c>AvatarSnapshotMapperTest</c> already flagged as fragile (see the Ignore rationale
    /// on that test) — they land in S6b alongside the consumer migration.
    /// </remarks>
    public class LegacyStatesAdapterTest
    {
        private Lib9cStates _states;
        private LegacyStatesAdapter _adapter;

        [SetUp]
        public void SetUp()
        {
            _states = new Lib9cStates();
            _adapter = new LegacyStatesAdapter(_states);
        }

        [TearDown]
        public void TearDown()
        {
            _adapter?.Dispose();
        }

        [Test]
        public void EmptyStatesYieldsNoCurrentAvatar()
        {
            Assert.IsFalse(_adapter.HasCurrentAvatar);
            Assert.IsNull(_adapter.CurrentAvatar);
            // States() ctor calls DeselectAvatar() which sets CurrentAvatarKey = -1.
            Assert.AreEqual(-1, _adapter.CurrentAvatarKey);
        }

        [Test]
        public void EmptyStatesYieldsEmptyAvatarsMap()
        {
            Assert.IsNotNull(_adapter.Avatars);
            Assert.AreEqual(0, _adapter.Avatars.Count);
            // Inventory list must be empty-non-null to match the IClientStateView contract.
            Assert.IsNotNull(_adapter.CurrentAvatarInventoryItems);
            Assert.AreEqual(0, _adapter.CurrentAvatarInventoryItems.Count);
            Assert.AreEqual(0, _adapter.LastStageIdCleared);
        }

        [Test]
        public void EmptyStatesYieldsDefaultAgentSnapshotAndZeroCrystal()
        {
            // AgentState is null on a fresh States instance — the adapter must project to
            // default(AgentSnapshot) and zero crystal rather than NRE.
            Assert.AreEqual(default(Nekoyume.SingleClient.Models.State.AgentSnapshot),
                _adapter.CurrentAgent);
            Assert.AreEqual(0L, _adapter.CurrentAgentCrystalBalance);
        }

        [Test, Ignore("Populating States.CurrentAvatarState requires a non-null WorldSheet fixture that the existing AvatarSnapshotMapperTest already flagged as fragile. Deferred to S6b with the consumer migration.")]
        public void ReadsThroughToCurrentAvatarStateProjection()
        {
            // Placeholder for the S6b slice: construct a minimal AvatarState through the
            // same BuildAvatar helper AvatarSnapshotMapperTest uses, push it through
            // States.SetAgentStateAsync + AddOrReplaceAvatarStateAsync, then assert that
            // _adapter.CurrentAvatar.Value.Name matches the lib9c AvatarState.name. The full
            // round-trip is covered in AvatarSnapshotMapperTest once the fixture issue clears.
        }
    }
}
