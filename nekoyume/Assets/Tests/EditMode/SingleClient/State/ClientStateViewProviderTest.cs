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
            public System.Numerics.BigInteger CurrentAgentCrystalBalanceMajorUnit => System.Numerics.BigInteger.Zero;
            public int StakingLevel => 0;
            public System.Numerics.BigInteger CurrentAgentGoldBalanceMajorUnit => System.Numerics.BigInteger.Zero;
            public Libplanet.Types.Assets.FungibleAssetValue CurrentAgentGoldBalanceFav => default;
            public Nekoyume.Model.State.AvatarState CurrentAvatarStateRaw => null;
            public Nekoyume.Model.State.AgentState CurrentAgentStateRaw => null;
            public Nekoyume.Model.State.GoldBalanceState CurrentGoldBalanceStateRaw => null;
            public Nekoyume.Model.State.GameConfigState CurrentGameConfigStateRaw => null;
            public System.Collections.Generic.IReadOnlyDictionary<int, Nekoyume.Model.State.AvatarState>
                AvatarStatesRaw { get; } =
                new System.Collections.Generic.Dictionary<int, Nekoyume.Model.State.AvatarState>(0);
            public System.Collections.Generic.IReadOnlyDictionary<Nekoyume.Model.EnumType.BattleType, Nekoyume.Model.State.ItemSlotState>
                CurrentItemSlotStatesRaw { get; } =
                new System.Collections.Generic.Dictionary<Nekoyume.Model.EnumType.BattleType, Nekoyume.Model.State.ItemSlotState>(0);
            public System.Collections.Generic.IReadOnlyDictionary<Nekoyume.Model.EnumType.BattleType, Nekoyume.Model.State.RuneSlotState>
                CurrentRuneSlotStatesRaw { get; } =
                new System.Collections.Generic.Dictionary<Nekoyume.Model.EnumType.BattleType, Nekoyume.Model.State.RuneSlotState>(0);
            public Nekoyume.Model.State.AllRuneState AllRuneStateRaw => null;
            public Nekoyume.Model.State.CrystalRandomSkillState CrystalRandomSkillStateRaw => null;
            public Nekoyume.State.PetStates PetStatesRaw => null;
            public Nekoyume.Model.Stake.StakeState? StakeStateV2Raw => null;
            public System.Collections.Generic.IReadOnlyDictionary<string, Libplanet.Types.Assets.FungibleAssetValue>
                CurrentAvatarBalancesRaw { get; } =
                new System.Collections.Generic.Dictionary<string, Libplanet.Types.Assets.FungibleAssetValue>(0);
            public Libplanet.Types.Assets.FungibleAssetValue StakedBalanceRaw => default;
            public Libplanet.Types.Assets.FungibleAssetValue CrystalBalanceRaw => default;
            public Nekoyume.Model.State.CollectionState CollectionStateRaw => null;
            public System.Collections.Generic.IReadOnlyDictionary<int, Nekoyume.Model.State.HammerPointState> HammerPointStatesRaw { get; } =
                new System.Collections.Generic.Dictionary<int, Nekoyume.Model.State.HammerPointState>(0);
            public Nekoyume.TableData.StakeRegularRewardSheet StakeRegularRewardSheetRaw => null;
            public (System.Collections.Generic.List<Nekoyume.Model.Item.Equipment>, System.Collections.Generic.List<Nekoyume.Model.Item.Costume>)
                GetEquippedItems(Nekoyume.Model.EnumType.BattleType battleType) =>
                (new System.Collections.Generic.List<Nekoyume.Model.Item.Equipment>(), new System.Collections.Generic.List<Nekoyume.Model.Item.Costume>());
            public System.Collections.Generic.List<Nekoyume.Model.State.RuneState>
                GetEquippedRuneStates(Nekoyume.Model.EnumType.BattleType battleType) =>
                new System.Collections.Generic.List<Nekoyume.Model.State.RuneState>();
            public System.Collections.Generic.List<Nekoyume.Model.State.RuneState>
                GetEquippedRuneStates(Nekoyume.Model.State.AllRuneState allRuneState, Nekoyume.Model.EnumType.BattleType battleType) =>
                new System.Collections.Generic.List<Nekoyume.Model.State.RuneState>();
            public System.Collections.Generic.Dictionary<int, Nekoyume.Model.State.CombinationSlotState>
                GetUsedCombinationSlotState() =>
                new System.Collections.Generic.Dictionary<int, Nekoyume.Model.State.CombinationSlotState>();
            public System.Collections.Generic.Dictionary<int, Nekoyume.Model.State.CombinationSlotState>
                GetUsedCombinationSlotState(Nekoyume.Model.State.AvatarState avatarState, long currentBlockIndex) =>
                new System.Collections.Generic.Dictionary<int, Nekoyume.Model.State.CombinationSlotState>();
            public System.Collections.Generic.IDictionary<int, Nekoyume.Model.State.CombinationSlotState>
                GetCombinationSlotState(Nekoyume.Model.State.AvatarState avatarState) =>
                new System.Collections.Generic.Dictionary<int, Nekoyume.Model.State.CombinationSlotState>();
            public void UpdateRuneSlotState() { }
            public void UpdateHammerPointStates(System.Collections.Generic.IEnumerable<int> recipeIds) { }
            public void SetCurrentAvatarBalance(Libplanet.Types.Assets.FungibleAssetValue fav) { }
            public Cysharp.Threading.Tasks.UniTask<Nekoyume.Model.State.AvatarState>
                SelectAvatarAsync(int index, Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> stateRootHash) =>
                Cysharp.Threading.Tasks.UniTask.FromResult<Nekoyume.Model.State.AvatarState>(null);
            public Cysharp.Threading.Tasks.UniTask<Nekoyume.Model.State.AvatarState>
                SelectAvatarAsync(int index, Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> stateRootHash, bool forceNewSelection) =>
                Cysharp.Threading.Tasks.UniTask.FromResult<Nekoyume.Model.State.AvatarState>(null);
            public Cysharp.Threading.Tasks.UniTask<Nekoyume.Model.State.AvatarState>
                SelectAvatarAsync(int index, Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> stateRootHash, Nekoyume.Model.State.AvatarState avatarState, bool forceNewSelection) =>
                Cysharp.Threading.Tasks.UniTask.FromResult<Nekoyume.Model.State.AvatarState>(null);
            public Cysharp.Threading.Tasks.UniTask InitAvatarBalancesAsync() => Cysharp.Threading.Tasks.UniTask.CompletedTask;
            public Cysharp.Threading.Tasks.UniTask InitItemSlotStates() => Cysharp.Threading.Tasks.UniTask.CompletedTask;
            public event Action AvatarChanged;

            // Explicit no-op raise so the compiler doesn't warn on the unused event; never
            // invoked in tests.
            internal void RaiseAvatarChangedForCompiler() => AvatarChanged?.Invoke();
        }
    }
}
