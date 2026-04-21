using System.Collections.Generic;

namespace Nekoyume.SingleClient.Combat
{
    /// <summary>
    /// Lib9c-free stage definition table. Five hand-authored entries covering
    /// the minimum playable progression for <see cref="SingleClientStageBattle"/>.
    /// </summary>
    public sealed class SingleClientStageRow
    {
        public int StageId { get; }
        public string DefenderName { get; }
        public long DefenderHp { get; }
        public long DefenderAtk { get; }
        public long DefenderDef { get; }
        public long ActionPointCost { get; }
        public long ExpReward { get; }

        public SingleClientStageRow(
            int stageId,
            string defenderName,
            long defenderHp,
            long defenderAtk,
            long defenderDef,
            long actionPointCost,
            long expReward)
        {
            StageId = stageId;
            DefenderName = defenderName;
            DefenderHp = defenderHp;
            DefenderAtk = defenderAtk;
            DefenderDef = defenderDef;
            ActionPointCost = actionPointCost;
            ExpReward = expReward;
        }
    }

    public static class SingleClientStageSheet
    {
        private static readonly IReadOnlyDictionary<int, SingleClientStageRow> Rows =
            new Dictionary<int, SingleClientStageRow>
            {
                [1] = new(1, "Slime",     50,  8,  3,  5,  30),
                [2] = new(2, "Goblin",    80, 12,  5,  5,  50),
                [3] = new(3, "Orc",      130, 18,  8,  5,  80),
                [4] = new(4, "Troll",    200, 25, 12, 10, 120),
                [5] = new(5, "Dragon",   300, 35, 18, 10, 200),
            };

        public static int MaxStageId => 5;

        public static bool TryGet(int stageId, out SingleClientStageRow row)
        {
            return Rows.TryGetValue(stageId, out row);
        }

        public static SingleClientStageRow Get(int stageId)
        {
            if (!Rows.TryGetValue(stageId, out var row))
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(stageId),
                    stageId,
                    $"Stage {stageId} is not defined. Available: 1..{MaxStageId}.");
            }

            return row;
        }

        public static IReadOnlyCollection<int> StageIds => Rows.Keys as IReadOnlyCollection<int>
            ?? new List<int>(Rows.Keys);
    }
}
