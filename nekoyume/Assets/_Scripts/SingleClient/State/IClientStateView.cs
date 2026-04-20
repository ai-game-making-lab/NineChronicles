#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Generic;
using Nekoyume.SingleClient.Models.Items;
using Nekoyume.SingleClient.Models.State;

namespace Nekoyume.SingleClient.State
{
    /// <summary>
    /// Libplanet-free read-only facade over <c>Nekoyume.State.States.Instance</c>. Exposes only
    /// client-owned DTO types (<see cref="AgentSnapshot"/>, <see cref="AvatarSnapshot"/>,
    /// <see cref="WorldInformationSnapshot"/>, <see cref="IItemSnapshot"/>) so the 148
    /// <c>States.Instance</c> consumers can migrate off lib9c/Libplanet/Bencodex types in
    /// follow-up slices without touching their call sites again.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the reader-side half of the Single-Client runtime boundary: it plays the same
    /// role for state reads that <c>IClientRuntime</c> plays for state mutations. Callers that
    /// currently read <c>States.Instance.CurrentAvatarState</c>, <c>.AgentState</c>,
    /// <c>.AvatarStates</c>, <c>.CurrentAvatarKey</c>, <c>.CurrentAvatarState.inventory</c>, or
    /// the avatar's <c>worldInformation</c> should migrate to the equivalent members here.
    /// </para>
    /// <para>
    /// No caching in this slice — every accessor re-projects on read via the appropriate
    /// <c>*SnapshotMapper.ToView()</c>. S6c will introduce an invalidation-aware cache once the
    /// consumer migration has flushed out the read-pattern survey.
    /// </para>
    /// <para>
    /// Mutation continues to flow through the legacy <c>States</c> setters and
    /// <c>IClientRuntime</c>; this interface is read-only. State change notifications are
    /// surfaced through <see cref="AvatarChanged"/>.
    /// </para>
    /// </remarks>
    public interface IClientStateView
    {
        /// <summary>
        /// Projection of <c>States.Instance.AgentState</c>. When no agent is set, returns
        /// <c>default(AgentSnapshot)</c> — callers should treat <see cref="AgentSnapshot.Address"/>
        /// equal to <c>default</c> as "not yet signed in".
        /// </summary>
        AgentSnapshot CurrentAgent { get; }

        /// <summary>
        /// Projection of <c>States.Instance.CurrentAvatarState</c>. <see langword="null"/> when
        /// no avatar has been selected yet (matches <c>States.CurrentAvatarKey == -1</c>).
        /// </summary>
        AvatarSnapshot? CurrentAvatar { get; }

        /// <summary>
        /// Slot index (0/1/2 in the default config) → avatar snapshot map. Only populated slots
        /// appear; an empty dictionary means the agent has no avatars yet.
        /// </summary>
        IReadOnlyDictionary<int, AvatarSnapshot> Avatars { get; }

        /// <summary>
        /// Currently selected avatar slot, or <c>-1</c> when no avatar is selected. Mirrors
        /// <c>States.Instance.CurrentAvatarKey</c>.
        /// </summary>
        int CurrentAvatarKey { get; }

        /// <summary>
        /// <see langword="true"/> iff a current avatar is selected and its snapshot projects
        /// cleanly. Equivalent to <c><see cref="CurrentAvatar"/> is not null</c>.
        /// </summary>
        bool HasCurrentAvatar { get; }

        /// <summary>
        /// Projection of <c>States.Instance.CurrentAvatarState.worldInformation</c>. Returns
        /// <c>default(WorldInformationSnapshot)</c> when no avatar is selected.
        /// </summary>
        WorldInformationSnapshot CurrentWorldInformation { get; }

        /// <summary>
        /// Highest cleared stage id on the main (non-Mimisbrunnr) worlds. Mirrors
        /// <c>CurrentWorldInformation.LastStageIdCleared</c> and returns <c>0</c> when no
        /// avatar is selected.
        /// </summary>
        int LastStageIdCleared { get; }

        /// <summary>
        /// Polymorphic list of the current avatar's inventory slots. Empty (non-null) list when
        /// no avatar is selected. Each entry is an <see cref="IItemSnapshot"/> — callers can
        /// pattern-match on the concrete snapshot kind (Equipment/Costume/Consumable/Material).
        /// </summary>
        IReadOnlyList<IItemSnapshot> CurrentAvatarInventoryItems { get; }

        /// <summary>
        /// Current agent's CRYSTAL balance truncated to <see cref="long"/> for convenience.
        /// Returns <c>0</c> when no agent is set. Follow-up slices will thicken this into a
        /// dedicated currency DTO (to carry decimals + ticker + the full <see cref="System.Numerics.BigInteger"/>
        /// payload) as the stake/market/combination accessors land.
        /// </summary>
        long CurrentAgentCrystalBalance { get; }

        /// <summary>
        /// Current agent's CRYSTAL balance as <see cref="System.Numerics.BigInteger"/>, matching
        /// lib9c <c>FungibleAssetValue.MajorUnit</c>. Avoids the <see cref="long"/> truncation of
        /// <see cref="CurrentAgentCrystalBalance"/> for accumulator balances that can exceed
        /// <see cref="long.MaxValue"/>. Returns <c>0</c> when no agent is set.
        /// </summary>
        System.Numerics.BigInteger CurrentAgentCrystalBalanceMajorUnit { get; }

        /// <summary>
        /// Current staking level (0 = no stake, 1+ = staked tier). Mirrors
        /// <c>States.Instance.StakingLevel</c>. Used by WorldMap / MobileShop / ArenaBoard /
        /// ShopSell UI to gate stake-dependent features.
        /// </summary>
        int StakingLevel { get; }

        /// <summary>
        /// Current agent's NCG (gold) balance MajorUnit as <see cref="System.Numerics.BigInteger"/>.
        /// Mirrors <c>States.Instance.GoldBalanceState.Gold.MajorUnit</c>. Used across
        /// Shop/Craft/Arena/AdventureBoss UI for NCG affordability checks. Returns <c>0</c> when
        /// no agent is set.
        /// </summary>
        System.Numerics.BigInteger CurrentAgentGoldBalanceMajorUnit { get; }

        /// <summary>
        /// Current agent's NCG (gold) balance as full <see cref="Libplanet.Types.Assets.FungibleAssetValue"/>.
        /// Pragmatic shim that re-exposes the Libplanet value so Payment / ItemTooltipBuy /
        /// TicketPurchase UI can compare FAVs directly without a facade-level FAV DTO. Will
        /// be narrowed to a client currency DTO in a follow-up slice.
        /// </summary>
        Libplanet.Types.Assets.FungibleAssetValue CurrentAgentGoldBalanceFav { get; }

        // ---- S6c pragmatic shims (mass-migration accelerator) ----
        // The following shims re-expose lib9c types as-is so the ~100 remaining consumers can
        // consolidate reads onto the facade without waiting on per-type DTO design. This
        // breaks the facade's "client-owned DTO only" invariant intentionally; once the rest
        // of the audit (S9c/S9d) lands and States.Instance is fully shadowed, each shim can be
        // narrowed to a DTO in a dedicated follow-up slice.

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>AvatarState</c> of the current avatar, or
        /// <see langword="null"/> if none selected. Bridges the 100+ consumers that read
        /// <c>inventory</c>/<c>worldInformation</c>/<c>questList</c>/<c>mailBox</c> directly.
        /// </summary>
        Nekoyume.Model.State.AvatarState CurrentAvatarStateRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>AgentState</c> of the current agent, or
        /// <see langword="null"/>. Bridges consumers that read agent-scoped data the snapshot
        /// does not yet surface.
        /// </summary>
        Nekoyume.Model.State.AgentState CurrentAgentStateRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>GoldBalanceState</c> of the current agent.
        /// Kept to satisfy payment/staking call sites that pass the whole state object.
        /// </summary>
        Nekoyume.Model.State.GoldBalanceState CurrentGoldBalanceStateRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>GameConfigState</c>.
        /// </summary>
        Nekoyume.Model.State.GameConfigState CurrentGameConfigStateRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>AvatarStates</c> dictionary. Mirrors
        /// <c>States.Instance.AvatarStates</c> directly for consumers that need the full lib9c
        /// <c>AvatarState</c> object rather than the snapshot projection.
        /// </summary>
        System.Collections.Generic.IReadOnlyDictionary<int, Nekoyume.Model.State.AvatarState>
            AvatarStatesRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>CurrentItemSlotStates</c> dict (battleType → ItemSlotState).
        /// </summary>
        System.Collections.Generic.IReadOnlyDictionary<Nekoyume.Model.EnumType.BattleType, Nekoyume.Model.State.ItemSlotState>
            CurrentItemSlotStatesRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>CurrentRuneSlotStates</c> dict.
        /// </summary>
        System.Collections.Generic.IReadOnlyDictionary<Nekoyume.Model.EnumType.BattleType, Nekoyume.Model.State.RuneSlotState>
            CurrentRuneSlotStatesRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>AllRuneState</c>.
        /// </summary>
        Nekoyume.Model.State.AllRuneState AllRuneStateRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Client-owned <c>PetStates</c> (Nekoyume.State namespace).
        /// </summary>
        Nekoyume.State.PetStates PetStatesRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>StakeState</c> nullable (aliased as StakeStateV2 in States.cs).
        /// </summary>
        Nekoyume.Model.Stake.StakeState? StakeStateV2Raw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>CrystalRandomSkillState</c>.
        /// </summary>
        Nekoyume.Model.State.CrystalRandomSkillState CrystalRandomSkillStateRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>CurrentAvatarBalances</c> dict (ticker → FAV).
        /// </summary>
        System.Collections.Generic.IReadOnlyDictionary<string, Libplanet.Types.Assets.FungibleAssetValue>
            CurrentAvatarBalancesRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>StakedBalance</c> FAV.
        /// </summary>
        Libplanet.Types.Assets.FungibleAssetValue StakedBalanceRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>CrystalBalance</c> FAV.
        /// </summary>
        Libplanet.Types.Assets.FungibleAssetValue CrystalBalanceRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>CollectionState</c>.
        /// </summary>
        Nekoyume.Model.State.CollectionState CollectionStateRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>HammerPointStates</c> dict.
        /// </summary>
        System.Collections.Generic.IReadOnlyDictionary<int, Nekoyume.Model.State.HammerPointState> HammerPointStatesRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Raw lib9c <c>StakeRegularRewardSheet</c>.
        /// </summary>
        Nekoyume.TableData.StakeRegularRewardSheet StakeRegularRewardSheetRaw { get; }

        /// <summary>
        /// <b>Pragmatic shim.</b> Proxies <c>States.Instance.GetEquippedItems(battleType)</c>.
        /// </summary>
        (System.Collections.Generic.List<Nekoyume.Model.Item.Equipment>, System.Collections.Generic.List<Nekoyume.Model.Item.Costume>)
            GetEquippedItems(Nekoyume.Model.EnumType.BattleType battleType);

        /// <summary>
        /// <b>Pragmatic shim.</b> Proxies <c>States.Instance.GetEquippedRuneStates(battleType)</c>.
        /// </summary>
        System.Collections.Generic.List<Nekoyume.Model.State.RuneState>
            GetEquippedRuneStates(Nekoyume.Model.EnumType.BattleType battleType);

        /// <summary>
        /// <b>Pragmatic shim.</b> 2-arg overload of <c>GetEquippedRuneStates</c>.
        /// </summary>
        System.Collections.Generic.List<Nekoyume.Model.State.RuneState>
            GetEquippedRuneStates(Nekoyume.Model.State.AllRuneState allRuneState, Nekoyume.Model.EnumType.BattleType battleType);

        /// <summary>
        /// <b>Pragmatic shim.</b> Proxies <c>States.Instance.GetUsedCombinationSlotState()</c>.
        /// </summary>
        System.Collections.Generic.Dictionary<int, Nekoyume.Model.State.CombinationSlotState>
            GetUsedCombinationSlotState();

        /// <summary>
        /// <b>Pragmatic shim.</b> Proxies the 2-arg <c>GetUsedCombinationSlotState(AvatarState, long)</c>.
        /// </summary>
        System.Collections.Generic.Dictionary<int, Nekoyume.Model.State.CombinationSlotState>
            GetUsedCombinationSlotState(Nekoyume.Model.State.AvatarState avatarState, long currentBlockIndex);

        /// <summary>
        /// <b>Pragmatic shim.</b> Proxies <c>States.Instance.GetCombinationSlotState(avatarState)</c>.
        /// </summary>
        System.Collections.Generic.IDictionary<int, Nekoyume.Model.State.CombinationSlotState>
            GetCombinationSlotState(Nekoyume.Model.State.AvatarState avatarState);

        /// <summary>
        /// <b>Pragmatic shim.</b> Proxies <c>States.Instance.UpdateRuneSlotState()</c>.
        /// </summary>
        void UpdateRuneSlotState();

        /// <summary>
        /// <b>Pragmatic shim.</b> Proxies <c>States.Instance.UpdateHammerPointStates(IEnumerable&lt;int&gt;)</c>.
        /// </summary>
        void UpdateHammerPointStates(System.Collections.Generic.IEnumerable<int> recipeIds);

        /// <summary>
        /// <b>Pragmatic shim.</b> Proxies <c>States.Instance.SetCurrentAvatarBalance(FungibleAssetValue)</c>.
        /// </summary>
        void SetCurrentAvatarBalance(Libplanet.Types.Assets.FungibleAssetValue fav);

        /// <summary>
        /// <b>Pragmatic shim.</b> Proxies <c>States.Instance.SelectAvatarAsync(int, HashDigest&lt;SHA256&gt;)</c>.
        /// </summary>
        Cysharp.Threading.Tasks.UniTask<Nekoyume.Model.State.AvatarState>
            SelectAvatarAsync(int index, Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> stateRootHash);

        /// <summary>
        /// <b>Pragmatic shim.</b> 3-arg variant with forceNewSelection.
        /// </summary>
        Cysharp.Threading.Tasks.UniTask<Nekoyume.Model.State.AvatarState>
            SelectAvatarAsync(int index, Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> stateRootHash, bool forceNewSelection);

        /// <summary>
        /// <b>Pragmatic shim.</b> 4-arg variant with provided AvatarState.
        /// </summary>
        Cysharp.Threading.Tasks.UniTask<Nekoyume.Model.State.AvatarState>
            SelectAvatarAsync(int index, Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> stateRootHash, Nekoyume.Model.State.AvatarState avatarState, bool forceNewSelection);

        /// <summary>
        /// <b>Pragmatic shim.</b> Proxies <c>States.Instance.InitAvatarBalancesAsync()</c>.
        /// </summary>
        Cysharp.Threading.Tasks.UniTask InitAvatarBalancesAsync();

        /// <summary>
        /// <b>Pragmatic shim.</b> Proxies <c>States.Instance.InitItemSlotStates()</c>.
        /// </summary>
        Cysharp.Threading.Tasks.UniTask InitItemSlotStates();

        /// <summary>
        /// Raised after the underlying <c>States</c> instance completes an avatar
        /// selection/update. The new avatar is accessible via <see cref="CurrentAvatar"/> from
        /// the handler; no snapshot payload is passed because handlers overwhelmingly want the
        /// full current state rather than a diff.
        /// </summary>
        event System.Action AvatarChanged;
    }
}

#endif
