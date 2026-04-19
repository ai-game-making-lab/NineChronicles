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
        /// Raised after the underlying <c>States</c> instance completes an avatar
        /// selection/update. The new avatar is accessible via <see cref="CurrentAvatar"/> from
        /// the handler; no snapshot payload is passed because handlers overwhelmingly want the
        /// full current state rather than a diff.
        /// </summary>
        event System.Action AvatarChanged;
    }
}
