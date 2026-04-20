using System;
using System.Collections.Generic;

namespace Nekoyume.SingleClient.Models.Quest
{
    /// Projection of lib9c <c>Quest</c>. Only the identity + progress + completion fields that
    /// the quest-scroll, celebrate-popup, and guided-quest cells read. Concrete subtypes
    /// (WorldQuest / CombinationEquipmentQuest / etc.) will be layered in follow-up slices once
    /// the UI ingestion paths have been traced exhaustively.
    public abstract class QuestSnapshot
    {
        public int Id { get; set; }
        public int Goal { get; set; }
        public int Current { get; set; }
        public bool Complete { get; set; }
        public bool IsReceivable { get; set; }
        public bool IsPaidInAction { get; set; }
        public abstract QuestType QuestType { get; }

        /// Flattened reward item map (item id -> count). UI enumerates this for the reward
        /// strip. Mirror of lib9c's <c>QuestReward.ItemMap</c>.
        public IReadOnlyDictionary<int, int> RewardItemMap { get; set; }

        /// Pre-formatted progress text (e.g. "(2/5)"). Mapper renders once so UI doesn't need
        /// to import the lib9c formatter.
        public string ProgressText { get; set; }

        public float Progress => Goal == 0 ? 0f : (float)Current / Goal;
    }

    /// Non-abstract concrete snapshot used for the majority of UI display paths that don't care
    /// about the lib9c subtype. Subtype-specific snapshots (WorldQuest / CombinationEquipmentQuest)
    /// will extend this in a follow-up slice.
    public sealed class GenericQuestSnapshot : QuestSnapshot
    {
        public override QuestType QuestType { get; }

        public GenericQuestSnapshot(QuestType questType)
        {
            QuestType = questType;
        }
    }
}
