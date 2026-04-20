#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;
using Lib9cStageSheetRow = Nekoyume.TableData.StageSheet.Row;

namespace Nekoyume.SingleClient.Models.State
{
    /// <summary>
    /// Projects lib9c's <see cref="Lib9cStageSheetRow"/> into <see cref="StageSnapshot"/>.
    /// Only captures the fields the 148 <c>States.Instance</c> consumers currently read
    /// from a stage row (<c>Id</c>, <c>CostAP</c>, <c>TurnLimit</c>). The optional
    /// <c>monsterIds</c> parameter is a passthrough for whichever upstream layer has
    /// assembled the spawn roster (typically the <c>StageWaveSheet</c> joiner) — lib9c's
    /// <c>StageSheet.Row</c> does not carry the monster list directly.
    /// </summary>
    public static class StageSnapshotMapper
    {
        public static StageSnapshot ToView(this Lib9cStageSheetRow source)
        {
            return source.ToView(monsterIds: null);
        }

        public static StageSnapshot ToView(
            this Lib9cStageSheetRow source,
            IReadOnlyList<int> monsterIds)
        {
            if (source is null)
            {
                return default;
            }

            return new StageSnapshot(
                id: source.Id,
                costAp: source.CostAP,
                turnLimit: source.TurnLimit,
                monsterIds: monsterIds);
        }
    }
}

#endif
