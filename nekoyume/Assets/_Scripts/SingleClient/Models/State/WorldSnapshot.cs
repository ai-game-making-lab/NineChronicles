#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;

namespace Nekoyume.SingleClient.Models.State
{
    /// <summary>
    /// Immutable projection of a single entry inside lib9c's
    /// <see cref="Nekoyume.Model.WorldInformation.World"/>. Captures the fields UI consumers
    /// read (id / display key / stage range / unlock + clear progress) so callers can render
    /// world-map state without retaining a reference to the live lib9c object.
    /// </summary>
    /// <remarks>
    /// Two predicate helpers — <see cref="IsUnlocked"/> and <see cref="IsStageCleared"/> —
    /// mirror lib9c's <c>UnlockedBlockIndex != -1</c> / <c>StageClearedBlockIndex != -1</c>
    /// convention and are eagerly captured at mapping time so callers never need to recompute
    /// them from the raw block indices.
    /// </remarks>
    public readonly struct WorldSnapshot : IEquatable<WorldSnapshot>
    {
        public int Id { get; }

        /// <summary>
        /// World display name as stored in the lib9c <c>WorldSheet.Row</c>. Typically acts as
        /// the localization resource key on the UI side (hence the spec name
        /// <c>Name_or_ResourceKey</c>); passed through verbatim so the UI layer can decide
        /// whether to render it directly or feed it to the localization table.
        /// </summary>
        public string NameOrResourceKey { get; }

        public int StageBegin { get; }
        public int StageEnd { get; }
        public int StageClearedId { get; }
        public long UnlockedBlockIndex { get; }
        public long StageClearedBlockIndex { get; }
        public bool IsUnlocked { get; }
        public bool IsStageCleared { get; }

        public WorldSnapshot(
            int id,
            string nameOrResourceKey,
            int stageBegin,
            int stageEnd,
            int stageClearedId,
            long unlockedBlockIndex,
            long stageClearedBlockIndex,
            bool isUnlocked,
            bool isStageCleared)
        {
            Id = id;
            NameOrResourceKey = nameOrResourceKey ?? string.Empty;
            StageBegin = stageBegin;
            StageEnd = stageEnd;
            StageClearedId = stageClearedId;
            UnlockedBlockIndex = unlockedBlockIndex;
            StageClearedBlockIndex = stageClearedBlockIndex;
            IsUnlocked = isUnlocked;
            IsStageCleared = isStageCleared;
        }

        public bool Equals(WorldSnapshot other) =>
            Id == other.Id &&
            StageClearedId == other.StageClearedId &&
            UnlockedBlockIndex == other.UnlockedBlockIndex &&
            StageClearedBlockIndex == other.StageClearedBlockIndex;

        public override bool Equals(object obj) => obj is WorldSnapshot other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Id;
                hash = (hash * 397) ^ StageClearedId;
                hash = (hash * 397) ^ UnlockedBlockIndex.GetHashCode();
                hash = (hash * 397) ^ StageClearedBlockIndex.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(WorldSnapshot left, WorldSnapshot right) => left.Equals(right);
        public static bool operator !=(WorldSnapshot left, WorldSnapshot right) => !left.Equals(right);
    }
}

#endif
