using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nekoyume.SingleClient.Models.Quest
{
    /// Projection of lib9c <c>QuestList</c>. Enumerable container the UI filters / counts; also
    /// carries the <see cref="CompletedQuestIds"/> list that <c>QuestPopup</c> reads to toggle
    /// toggle-badge state without re-scanning the quest array.
    public sealed class QuestListSnapshot : IEnumerable<QuestSnapshot>
    {
        private readonly List<QuestSnapshot> _quests;

        public QuestListSnapshot()
        {
            _quests = new List<QuestSnapshot>();
            CompletedQuestIds = new List<int>();
        }

        public QuestListSnapshot(
            IEnumerable<QuestSnapshot> quests,
            IReadOnlyList<int> completedQuestIds = null,
            int listVersion = 1)
        {
            _quests = quests?.ToList() ?? new List<QuestSnapshot>();
            CompletedQuestIds = completedQuestIds ?? new List<int>();
            ListVersion = listVersion;
        }

        public int ListVersion { get; set; } = 1;

        public IReadOnlyList<int> CompletedQuestIds { get; set; }

        public int Count => _quests.Count;

        public QuestSnapshot this[int index] => _quests[index];

        public List<QuestSnapshot> FindAll(System.Predicate<QuestSnapshot> match) =>
            _quests.FindAll(match);

        public IEnumerator<QuestSnapshot> GetEnumerator() => _quests.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
