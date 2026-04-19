using Lib9cWorld = Nekoyume.Model.WorldInformation.World;

namespace Nekoyume.SingleClient.Models.State
{
    /// <summary>
    /// Projects a lib9c <see cref="Lib9cWorld"/> struct into a <see cref="WorldSnapshot"/>.
    /// Kept extension-style so migrating callers only append <c>.ToView()</c> to their existing
    /// world-map calls.
    /// </summary>
    public static class WorldSnapshotMapper
    {
        public static WorldSnapshot ToView(this Lib9cWorld source)
        {
            // Lib9cWorld is a value-type struct; a zero-id entry is treated as the default
            // (not the uninitialized state the caller should project). We still project every
            // field, letting IsUnlocked / IsStageCleared do their real work from the sentinel
            // block-index values rather than from the id being non-zero.
            return new WorldSnapshot(
                id: source.Id,
                nameOrResourceKey: source.Name,
                stageBegin: source.StageBegin,
                stageEnd: source.StageEnd,
                stageClearedId: source.StageClearedId,
                unlockedBlockIndex: source.UnlockedBlockIndex,
                stageClearedBlockIndex: source.StageClearedBlockIndex,
                isUnlocked: source.IsUnlocked,
                isStageCleared: source.IsStageCleared);
        }
    }
}
