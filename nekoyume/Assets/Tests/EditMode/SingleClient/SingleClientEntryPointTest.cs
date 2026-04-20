using System;
using NUnit.Framework;
using Nekoyume.SingleClient;
using Nekoyume.SingleClient.Combat;

namespace Nekoyume.Tests.EditMode.SingleClient
{
    /// <summary>
    /// Smoke tests for the Lib9c-free Phase A/C scaffolding. Does NOT require
    /// scene load — everything exercises pure C# API surfaces.
    /// </summary>
    [TestFixture]
    public class SingleClientEntryPointTest
    {
        [Test]
        public void DefaultState_HasValidAvatarAndZeroActionPoint()
        {
            var state = SingleClientState.CreateDefault(DateTimeOffset.UtcNow, privateKeyHex: null);
            Assert.NotNull(state);
            Assert.NotNull(state.avatar);
            Assert.AreEqual(SingleClientState.DefaultAvatarId, state.avatar.id);
            Assert.AreEqual(0L, state.avatar.actionPoint);
        }

        [Test]
        public void Runtime_FillActionPoint_IsObservableInState()
        {
            var session = new SingleClientSession(new InMemoryStateStore());
            var runtime = new SingleClientRuntime(session);
            runtime.Start();

            runtime.FillActionPoint(120);

            Assert.AreEqual(120L, runtime.State.ActionPoint);
        }

        [Test]
        public void Runtime_ConsumeActionPoint_ThrowsOnInsufficientBalance()
        {
            var session = new SingleClientSession(new InMemoryStateStore());
            var runtime = new SingleClientRuntime(session);
            runtime.Start();
            runtime.FillActionPoint(10);

            Assert.Throws<System.InvalidOperationException>(() => runtime.ConsumeActionPoint(50));
            Assert.AreEqual(10L, runtime.State.ActionPoint);
        }

        [Test]
        public void Runtime_ConsumeActionPoint_WithinBalance_Decrements()
        {
            var session = new SingleClientSession(new InMemoryStateStore());
            var runtime = new SingleClientRuntime(session);
            runtime.Start();
            runtime.FillActionPoint(30);

            runtime.ConsumeActionPoint(10);

            Assert.AreEqual(20L, runtime.State.ActionPoint);
        }

        [Test]
        public void Battle_DeterministicWithSeed_MatchesOnReplay()
        {
            var atk = new SingleClientBattle.Combatant("Hero", 100, 25, 5);
            var def = new SingleClientBattle.Combatant("Slime", 60, 10, 2);

            var r1 = SingleClientBattle.Simulate(atk, def, seed: 42);
            var r2 = SingleClientBattle.Simulate(atk, def, seed: 42);

            Assert.AreEqual(r1.AttackerWins, r2.AttackerWins);
            Assert.AreEqual(r1.Turns, r2.Turns);
            Assert.AreEqual(r1.AttackerRemainingHp, r2.AttackerRemainingHp);
            Assert.AreEqual(r1.DefenderRemainingHp, r2.DefenderRemainingHp);
        }

        [Test]
        public void Battle_StrongVsWeak_AttackerWins()
        {
            var atk = new SingleClientBattle.Combatant("Hero", 200, 50, 10);
            var def = new SingleClientBattle.Combatant("Slime", 20, 5, 1);

            var result = SingleClientBattle.Simulate(atk, def, seed: 1);

            Assert.IsTrue(result.AttackerWins, $"Expected attacker to win, got {result.AttackerRemainingHp}/{result.DefenderRemainingHp}");
            Assert.IsFalse(result.DefenderWins);
        }

        private sealed class InMemoryStateStore : ISingleClientStateStore
        {
            private SingleClientState _state;

            public bool Exists => _state != null;
            public SingleClientState LoadOrCreate() => _state ??= SingleClientState.CreateDefault(DateTimeOffset.UtcNow, privateKeyHex: null);
            public void Save(SingleClientState state) { _state = state; }
        }
    }
}
