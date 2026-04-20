#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Generic;

namespace Nekoyume.SingleClient.Models.State
{
    /// <summary>
    /// Immutable projection of lib9c's <see cref="Nekoyume.Model.WorldInformation"/>. Flattens
    /// the opaque internal <c>Dictionary&lt;int, World&gt;</c> into an ordered
    /// <see cref="IReadOnlyList{WorldSnapshot}"/> (sorted by world id) plus a pre-computed
    /// <see cref="LastStageIdCleared"/> so the world map UI doesn't have to scan the world
    /// list to find the highest cleared stage on every frame.
    /// </summary>
    /// <remarks>
    /// The list is snapshotted at mapping time; callers can release the underlying lib9c
    /// <c>WorldInformation</c> without invalidating the DTO. Mimisbrunnr worlds are included
    /// in <see cref="Worlds"/> but excluded from <see cref="LastStageIdCleared"/>, mirroring
    /// <c>WorldInformation.TryGetLastClearedStageId</c>'s main-chain-only semantics.
    /// </remarks>
    public readonly struct WorldInformationSnapshot : IEquatable<WorldInformationSnapshot>
    {
        public IReadOnlyList<WorldSnapshot> Worlds { get; }

        /// <summary>
        /// Highest cleared stage id across all non-Mimisbrunnr worlds, or <c>0</c> when the
        /// avatar hasn't cleared any stage yet. Matches the semantics of
        /// <c>WorldInformation.TryGetLastClearedStageId</c> (returns <c>default(int)</c> = 0
        /// when nothing is cleared).
        /// </summary>
        public int LastStageIdCleared { get; }

        public WorldInformationSnapshot(
            IReadOnlyList<WorldSnapshot> worlds,
            int lastStageIdCleared)
        {
            Worlds = worlds ?? Array.Empty<WorldSnapshot>();
            LastStageIdCleared = lastStageIdCleared;
        }

        public bool Equals(WorldInformationSnapshot other)
        {
            if (LastStageIdCleared != other.LastStageIdCleared)
            {
                return false;
            }

            var a = Worlds;
            var b = other.Worlds;
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            if (a.Count != b.Count)
            {
                return false;
            }

            for (var i = 0; i < a.Count; i++)
            {
                if (!a[i].Equals(b[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj) =>
            obj is WorldInformationSnapshot other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = LastStageIdCleared;
                if (Worlds != null)
                {
                    hash = (hash * 397) ^ Worlds.Count;
                }
                return hash;
            }
        }

        public static bool operator ==(WorldInformationSnapshot left, WorldInformationSnapshot right) =>
            left.Equals(right);

        public static bool operator !=(WorldInformationSnapshot left, WorldInformationSnapshot right) =>
            !left.Equals(right);
    }
}

#endif
