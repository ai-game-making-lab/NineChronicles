#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;
using System.Linq;
using Nekoyume.SingleClient.Models.Quest;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient.Quest
{
    /// Scaffold-level smoke coverage for the Quest mirror DTOs.
    public class QuestSnapshotSmokeTest
    {
        [Test]
        public void QuestSnapshotFieldsAreSetAndReadable()
        {
            var reward = new Dictionary<int, int> { { 303000, 5 } };
            var snapshot = new GenericQuestSnapshot(QuestType.Adventure)
            {
                Id = 1001,
                Goal = 10,
                Current = 3,
                Complete = false,
                IsReceivable = false,
                RewardItemMap = reward,
                ProgressText = "(3/10)",
            };

            Assert.AreEqual(1001, snapshot.Id);
            Assert.AreEqual(QuestType.Adventure, snapshot.QuestType);
            Assert.AreEqual(10, snapshot.Goal);
            Assert.AreEqual(3, snapshot.Current);
            Assert.IsFalse(snapshot.Complete);
            Assert.AreEqual("(3/10)", snapshot.ProgressText);
            Assert.AreEqual(0.3f, snapshot.Progress, 0.0001f);
            Assert.AreEqual(5, snapshot.RewardItemMap[303000]);
        }

        [Test]
        public void ProgressHandlesZeroGoal()
        {
            var snapshot = new GenericQuestSnapshot(QuestType.Obtain) { Goal = 0, Current = 0 };
            Assert.AreEqual(0f, snapshot.Progress);
        }

        [Test]
        public void QuestListEnumeratesAndFindsAll()
        {
            var a = new GenericQuestSnapshot(QuestType.Adventure) { Id = 1, Complete = true };
            var b = new GenericQuestSnapshot(QuestType.Craft) { Id = 2, Complete = false };
            var c = new GenericQuestSnapshot(QuestType.Adventure) { Id = 3, Complete = false };
            var list = new QuestListSnapshot(new[] { a, b, c }, new List<int> { 1 });

            Assert.AreEqual(3, list.Count);
            var adventureOnly = list.FindAll(q => q.QuestType == QuestType.Adventure);
            Assert.AreEqual(2, adventureOnly.Count);
            CollectionAssert.AreEquivalent(new[] { 1, 3 }, adventureOnly.Select(q => q.Id));
            Assert.AreEqual(1, list.CompletedQuestIds.Count);
            Assert.AreEqual(1, list.ListVersion);
        }
    }
}

#endif
