using System;
using System.Collections.Generic;
using Nekoyume.SingleClient.Models.Items;
using Nekoyume.SingleClient.Models.State;
using Lib9cStates = Nekoyume.State.States;
using ReactiveAvatarState = Nekoyume.State.ReactiveAvatarState;
using UniRx;

namespace Nekoyume.SingleClient.State
{
    /// <summary>
    /// <see cref="IClientStateView"/> implementation that wraps the legacy
    /// <see cref="Lib9cStates"/> singleton. Every read projects on demand through the
    /// <c>Nekoyume.SingleClient.Models.State.*SnapshotMapper.ToView()</c> extensions — no
    /// caching in this slice.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Zero changes to <c>States</c> itself. This adapter attaches no subscriptions to the
    /// <c>States</c> instance's internal setters; instead it rides the existing
    /// <see cref="ReactiveAvatarState.Address"/> observable, which <c>States</c> already fires
    /// from <c>UpdateCurrentAvatarState</c> (the single funnel through which every avatar
    /// select / replace / deselect flows). That lets us project <see cref="AvatarChanged"/>
    /// without adding a new subject to <c>States</c>.
    /// </para>
    /// <para>
    /// Subscription lifetime: the <see cref="IDisposable"/> returned by UniRx is retained for
    /// the lifetime of the adapter and torn down on <see cref="Dispose"/>. Production callers
    /// route through <see cref="ClientStateViewProvider"/>, which creates the adapter once at
    /// boot and never swaps it out except via <see cref="ClientStateViewProvider.Reset"/>, so
    /// this is effectively a process-lifetime subscription.
    /// </para>
    /// </remarks>
    public sealed class LegacyStatesAdapter : IClientStateView, IDisposable
    {
        private readonly Lib9cStates _states;
        private readonly IDisposable _avatarAddressSubscription;
        private bool _disposed;

        /// <summary>
        /// Raised after the underlying <see cref="Lib9cStates"/> completes an avatar
        /// select/update. Wired to <see cref="ReactiveAvatarState.Address"/>; see class-level
        /// remarks for the rationale.
        /// </summary>
        public event System.Action AvatarChanged;

        public LegacyStatesAdapter(Lib9cStates states)
        {
            _states = states ?? throw new ArgumentNullException(nameof(states));

            // ReactiveAvatarState.Address is driven from ReactiveAvatarState.Initialize(), which
            // States.UpdateCurrentAvatarState() calls on every avatar select/replace. The
            // observable is static, so we don't hold a reference to the subject itself — the
            // IDisposable handle is enough to unsubscribe on Dispose.
            //
            // .Skip(1) would drop the initial replay value, but we actually want handlers to
            // learn about the current avatar the first time they subscribe via .Subscribe()
            // after construction. Leaving it un-skipped keeps parity with direct
            // States.CurrentAvatarState reads.
            _avatarAddressSubscription = UniRx.ObservableExtensions.Subscribe(
                ReactiveAvatarState.Address,
                _ => AvatarChanged?.Invoke());
        }

        public AgentSnapshot CurrentAgent
        {
            get
            {
                var agentState = _states.AgentState;
                if (agentState is null)
                {
                    return default;
                }

                // Crystal balance lives on the States singleton itself (ConcurrentDictionary-
                // backed, not on AgentState). Forward it through the extended mapper overload
                // so the snapshot carries both.
                var crystalRaw = _states.CrystalBalance.RawValue;
                return agentState.ToView(crystalRaw, refillActionPoint: string.Empty);
            }
        }

        public AvatarSnapshot? CurrentAvatar
        {
            get
            {
                var current = _states.CurrentAvatarState;
                if (current is null)
                {
                    return null;
                }

                return current.ToView();
            }
        }

        public IReadOnlyDictionary<int, AvatarSnapshot> Avatars
        {
            get
            {
                var src = _states.AvatarStates;
                if (src is null || src.Count == 0)
                {
                    return EmptyAvatars;
                }

                var result = new Dictionary<int, AvatarSnapshot>(src.Count);
                foreach (var pair in src)
                {
                    if (pair.Value is null)
                    {
                        continue;
                    }

                    result[pair.Key] = pair.Value.ToView();
                }

                return result;
            }
        }

        public int CurrentAvatarKey => _states.CurrentAvatarKey;

        public bool HasCurrentAvatar => _states.CurrentAvatarState is not null;

        public WorldInformationSnapshot CurrentWorldInformation
        {
            get
            {
                var avatarState = _states.CurrentAvatarState;
                if (avatarState?.worldInformation is null)
                {
                    return default;
                }

                return avatarState.worldInformation.ToView();
            }
        }

        public int LastStageIdCleared
        {
            get
            {
                var avatarState = _states.CurrentAvatarState;
                var info = avatarState?.worldInformation;
                if (info is null)
                {
                    return 0;
                }

                try
                {
                    return info.TryGetLastClearedStageId(out var stageId) ? stageId : 0;
                }
                catch (NullReferenceException)
                {
                    // Parity with WorldInformationSnapshotMapper.ToView's defensive branch for
                    // the null-WorldSheet fixture.
                    return 0;
                }
            }
        }

        public IReadOnlyList<IItemSnapshot> CurrentAvatarInventoryItems
        {
            get
            {
                var avatarState = _states.CurrentAvatarState;
                if (avatarState is null)
                {
                    return Array.Empty<IItemSnapshot>();
                }

                // Cheapest path: reuse the full AvatarSnapshot projection so the item mapper
                // logic stays in one place (AvatarSnapshotMapper.ProjectInventory). The
                // allocation is one List<IItemSnapshot> per read — acceptable given no caching
                // in this slice and the follow-up cache landing in S6c.
                return avatarState.ToView().InventoryItems;
            }
        }

        public long CurrentAgentCrystalBalance
        {
            get
            {
                var agentState = _states.AgentState;
                if (agentState is null)
                {
                    return 0L;
                }

                // Crystal balance can exceed long.MaxValue for large accumulators (see
                // AgentSnapshot docs). This property intentionally truncates via (long) cast;
                // callers that need the full BigInteger should consume CurrentAgent.CrystalBalance
                // from the snapshot directly until a dedicated currency DTO lands.
                return (long)_states.CrystalBalance.RawValue;
            }
        }

        public System.Numerics.BigInteger CurrentAgentCrystalBalanceMajorUnit
        {
            get
            {
                var agentState = _states.AgentState;
                if (agentState is null)
                {
                    return System.Numerics.BigInteger.Zero;
                }

                return _states.CrystalBalance.MajorUnit;
            }
        }

        public int StakingLevel => _states.StakingLevel;

        public System.Numerics.BigInteger CurrentAgentGoldBalanceMajorUnit
        {
            get
            {
                var agentState = _states.AgentState;
                if (agentState is null)
                {
                    return System.Numerics.BigInteger.Zero;
                }

                return _states.GoldBalanceState.Gold.MajorUnit;
            }
        }

        public Libplanet.Types.Assets.FungibleAssetValue CurrentAgentGoldBalanceFav =>
            _states.AgentState is null
                ? default
                : _states.GoldBalanceState.Gold;

        public Nekoyume.Model.State.AvatarState CurrentAvatarStateRaw => _states.CurrentAvatarState;

        public Nekoyume.Model.State.AgentState CurrentAgentStateRaw => _states.AgentState;

        public Nekoyume.Model.State.GoldBalanceState CurrentGoldBalanceStateRaw => _states.GoldBalanceState;

        public Nekoyume.Model.State.GameConfigState CurrentGameConfigStateRaw => _states.GameConfigState;

        public System.Collections.Generic.IReadOnlyDictionary<int, Nekoyume.Model.State.AvatarState>
            AvatarStatesRaw =>
            (System.Collections.Generic.IReadOnlyDictionary<int, Nekoyume.Model.State.AvatarState>)
            _states.AvatarStates;

        public System.Collections.Generic.IReadOnlyDictionary<Nekoyume.Model.EnumType.BattleType, Nekoyume.Model.State.ItemSlotState>
            CurrentItemSlotStatesRaw =>
            (System.Collections.Generic.IReadOnlyDictionary<Nekoyume.Model.EnumType.BattleType, Nekoyume.Model.State.ItemSlotState>)
            _states.CurrentItemSlotStates;

        public System.Collections.Generic.IReadOnlyDictionary<Nekoyume.Model.EnumType.BattleType, Nekoyume.Model.State.RuneSlotState>
            CurrentRuneSlotStatesRaw =>
            (System.Collections.Generic.IReadOnlyDictionary<Nekoyume.Model.EnumType.BattleType, Nekoyume.Model.State.RuneSlotState>)
            _states.CurrentRuneSlotStates;

        public Nekoyume.Model.State.AllRuneState AllRuneStateRaw => _states.AllRuneState;
        public Nekoyume.Model.State.CrystalRandomSkillState CrystalRandomSkillStateRaw => _states.CrystalRandomSkillState;
        public Nekoyume.State.PetStates PetStatesRaw => _states.PetStates;
        public Nekoyume.Model.Stake.StakeState? StakeStateV2Raw => _states.StakeStateV2;

        public System.Collections.Generic.IReadOnlyDictionary<string, Libplanet.Types.Assets.FungibleAssetValue>
            CurrentAvatarBalancesRaw =>
            (System.Collections.Generic.IReadOnlyDictionary<string, Libplanet.Types.Assets.FungibleAssetValue>)
            _states.CurrentAvatarBalances;

        public Libplanet.Types.Assets.FungibleAssetValue StakedBalanceRaw => _states.StakedBalance;
        public Libplanet.Types.Assets.FungibleAssetValue CrystalBalanceRaw => _states.CrystalBalance;

        public Nekoyume.Model.State.CollectionState CollectionStateRaw => _states.CollectionState;
        public System.Collections.Generic.IReadOnlyDictionary<int, Nekoyume.Model.State.HammerPointState> HammerPointStatesRaw =>
            _states.HammerPointStates;
        public Nekoyume.TableData.StakeRegularRewardSheet StakeRegularRewardSheetRaw => _states.StakeRegularRewardSheet;

        public (System.Collections.Generic.List<Nekoyume.Model.Item.Equipment>, System.Collections.Generic.List<Nekoyume.Model.Item.Costume>)
            GetEquippedItems(Nekoyume.Model.EnumType.BattleType battleType) =>
            _states.GetEquippedItems(battleType);

        public System.Collections.Generic.List<Nekoyume.Model.State.RuneState>
            GetEquippedRuneStates(Nekoyume.Model.EnumType.BattleType battleType) =>
            _states.GetEquippedRuneStates(battleType);

        public System.Collections.Generic.List<Nekoyume.Model.State.RuneState>
            GetEquippedRuneStates(Nekoyume.Model.State.AllRuneState allRuneState, Nekoyume.Model.EnumType.BattleType battleType) =>
            _states.GetEquippedRuneStates(allRuneState, battleType);

        public System.Collections.Generic.Dictionary<int, Nekoyume.Model.State.CombinationSlotState>
            GetUsedCombinationSlotState() =>
            _states.GetUsedCombinationSlotState();

        public System.Collections.Generic.Dictionary<int, Nekoyume.Model.State.CombinationSlotState>
            GetUsedCombinationSlotState(Nekoyume.Model.State.AvatarState avatarState, long currentBlockIndex) =>
            _states.GetUsedCombinationSlotState(avatarState, currentBlockIndex);

        public System.Collections.Generic.IDictionary<int, Nekoyume.Model.State.CombinationSlotState>
            GetCombinationSlotState(Nekoyume.Model.State.AvatarState avatarState) =>
            _states.GetCombinationSlotState(avatarState);

        public void UpdateRuneSlotState() => _states.UpdateRuneSlotState();

        public void UpdateHammerPointStates(System.Collections.Generic.IEnumerable<int> recipeIds) =>
            _states.UpdateHammerPointStates(recipeIds);

        public void SetCurrentAvatarBalance(Libplanet.Types.Assets.FungibleAssetValue fav) =>
            _states.SetCurrentAvatarBalance(fav);

        public Cysharp.Threading.Tasks.UniTask<Nekoyume.Model.State.AvatarState>
            SelectAvatarAsync(int index, Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> stateRootHash) =>
            _states.SelectAvatarAsync(index, stateRootHash);

        public Cysharp.Threading.Tasks.UniTask<Nekoyume.Model.State.AvatarState>
            SelectAvatarAsync(int index, Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> stateRootHash, bool forceNewSelection) =>
            _states.SelectAvatarAsync(index, stateRootHash, forceNewSelection: forceNewSelection);

        public Cysharp.Threading.Tasks.UniTask<Nekoyume.Model.State.AvatarState>
            SelectAvatarAsync(int index, Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> stateRootHash, Nekoyume.Model.State.AvatarState avatarState, bool forceNewSelection) =>
            _states.SelectAvatarAsync(index, stateRootHash, avatarState, forceNewSelection: forceNewSelection);

        public Cysharp.Threading.Tasks.UniTask InitAvatarBalancesAsync() =>
            _states.InitAvatarBalancesAsync();

        public Cysharp.Threading.Tasks.UniTask InitItemSlotStates() =>
            _states.InitItemSlotStates();

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _avatarAddressSubscription?.Dispose();
            AvatarChanged = null;
        }

        private static readonly IReadOnlyDictionary<int, AvatarSnapshot> EmptyAvatars =
            new Dictionary<int, AvatarSnapshot>(0);
    }
}
