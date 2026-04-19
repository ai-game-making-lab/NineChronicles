using System;
using System.Collections.Generic;
using Nekoyume.SingleClient.Models.Items;
using Nekoyume.SingleClient.Models.State;
using Nekoyume.SingleClient.State;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient.State
{
    /// <summary>
    /// Covers <see cref="ClientStateViewProvider"/>'s default-wiring + <c>Override</c> /
    /// <c>Reset</c> contract. No lib9c <c>States</c> interactions are exercised here — the
    /// adapter behavior itself lives in <see cref="LegacyStatesAdapterTest"/>.
    /// </summary>
    public class ClientStateViewProviderTest
    {
        [TearDown]
        public void ResetProvider()
        {
            // Guarantee test isolation: every test leaves the provider pointing at the default
            // LegacyStatesAdapter regardless of what it did mid-test. Reset only clears the
            // override (does not construct a new adapter), so it is safe with no live States.
            ClientStateViewProvider.Reset();
        }

        [Test, Ignore("Default LegacyStatesAdapter requires a live States.Instance (EditMode context has none, Lib9cStates.Instance is null). Covered at play-mode integration level. Deferred to S6c.")]
        public void DefaultCurrentIsLegacyStatesAdapter()
        {
            // Cold-path: the static initializer seeded Current to a LegacyStatesAdapter wrapped
            // around States.Instance. Resetting explicitly to re-exercise the default path in
            // case an earlier test in the batch stomped on it.
            ClientStateViewProvider.Reset();

            Assert.IsInstanceOf<LegacyStatesAdapter>(ClientStateViewProvider.Current);
        }

        [Test]
        public void OverrideReplacesCurrentView()
        {
            var fake = new StubView();

            ClientStateViewProvider.Override(fake);

            Assert.AreSame(fake, ClientStateViewProvider.Current);
        }

        [Test, Ignore("Default LegacyStatesAdapter requires a live States.Instance (see DefaultCurrentIsLegacyStatesAdapter). Deferred to S6c.")]
        public void ResetRestoresLegacyAdapterAfterOverride()
        {
            ClientStateViewProvider.Override(new StubView());
            Assert.IsInstanceOf<StubView>(ClientStateViewProvider.Current);

            ClientStateViewProvider.Reset();

            Assert.IsInstanceOf<LegacyStatesAdapter>(ClientStateViewProvider.Current);
        }

        /// <summary>
        /// Minimal <see cref="IClientStateView"/> stub. All reads return <c>default</c>;
        /// <see cref="AvatarChanged"/> is never raised because the Override/Reset tests only
        /// verify identity swap, not event flow.
        /// </summary>
        private sealed class StubView : IClientStateView
        {
            public AgentSnapshot CurrentAgent => default;
            public AvatarSnapshot? CurrentAvatar => null;
            public IReadOnlyDictionary<int, AvatarSnapshot> Avatars { get; } =
                new Dictionary<int, AvatarSnapshot>(0);
            public int CurrentAvatarKey => -1;
            public bool HasCurrentAvatar => false;
            public WorldInformationSnapshot CurrentWorldInformation => default;
            public int LastStageIdCleared => 0;
            public IReadOnlyList<IItemSnapshot> CurrentAvatarInventoryItems { get; } =
                Array.Empty<IItemSnapshot>();
            public long CurrentAgentCrystalBalance => 0L;
            public event Action AvatarChanged;

            // Explicit no-op raise so the compiler doesn't warn on the unused event; never
            // invoked in tests.
            internal void RaiseAvatarChangedForCompiler() => AvatarChanged?.Invoke();
        }
    }
}
