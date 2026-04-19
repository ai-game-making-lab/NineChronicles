using System;
using System.Collections.Generic;

namespace Nekoyume.SingleClient.Models.State
{
    /// <summary>
    /// Immutable projection of the subset of <c>StageSheet.Row</c> fields the UI reads per
    /// stage (AP cost, turn limit, monster roster). Richer fields on the lib9c row (reward
    /// tables, background / BGM keys, enemy stat modifiers) stay out of the DTO for now;
    /// the scaffolding slice tracks only what the <c>States.Instance</c> migration's 148
    /// consumers touch. Extend the ctor when a new UI field joins the consumer surface —
    /// don't repurpose existing members.
    /// </summary>
    /// <remarks>
    /// <see cref="MonsterIds"/> is a flat list of the monster-character ids the stage will
    /// spawn; the exact ordering matches however the mapper surfaces them (currently a passthrough
    /// of whatever list the caller supplies, because lib9c's <c>StageSheet.Row</c> does not
    /// carry a monster roster directly — it's assembled elsewhere from
    /// <c>StageWaveSheet</c>). The DTO exists so future consumers can flatten the
    /// wave/enemy data into a ready-to-render list without re-reading the sheet.
    /// </remarks>
    public readonly struct StageSnapshot : IEquatable<StageSnapshot>
    {
        public int Id { get; }
        public int CostAp { get; }
        public int TurnLimit { get; }
        public IReadOnlyList<int> MonsterIds { get; }

        public StageSnapshot(int id, int costAp, int turnLimit, IReadOnlyList<int> monsterIds)
        {
            Id = id;
            CostAp = costAp;
            TurnLimit = turnLimit;
            MonsterIds = monsterIds ?? Array.Empty<int>();
        }

        public bool Equals(StageSnapshot other) =>
            Id == other.Id &&
            CostAp == other.CostAp &&
            TurnLimit == other.TurnLimit &&
            ReferenceEquals(MonsterIds, other.MonsterIds);

        public override bool Equals(object obj) => obj is StageSnapshot other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Id;
                hash = (hash * 397) ^ CostAp;
                hash = (hash * 397) ^ TurnLimit;
                return hash;
            }
        }

        public static bool operator ==(StageSnapshot left, StageSnapshot right) => left.Equals(right);
        public static bool operator !=(StageSnapshot left, StageSnapshot right) => !left.Equals(right);
    }
}
