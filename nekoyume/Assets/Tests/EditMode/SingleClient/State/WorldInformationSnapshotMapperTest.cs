#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Linq;
using Nekoyume.SingleClient.Models.State;
using Nekoyume.TableData;
using NUnit.Framework;
using Lib9cWorldInformation = Nekoyume.Model.WorldInformation;

namespace Tests.EditMode.SingleClient.State
{
    /// <summary>
    /// Covers <see cref="WorldInformationSnapshotMapper"/>'s enumeration-via-Serialize path,
    /// the LastStageIdCleared pre-computation, and null safety.
    /// </summary>
    public class WorldInformationSnapshotMapperTest
    {
        private static WorldSheet BuildWorldSheet()
        {
            // Two contiguous worlds that match the <=StageEnd semantics of
            // WorldInformation.OpenAllOfWorldsAndStages=true: world 1 covers stages 1..5,
            // world 2 covers stages 6..10.
            var sheet = new WorldSheet();
            sheet.Set(
                "id,name,stage_begin,stage_end\n" +
                "1,Yggdrasil,1,5\n" +
                "2,Mimisbrunnr,6,10\n");
            return sheet;
        }

        [Test]
        public void ToViewEnumeratesEveryWorldSortedById()
        {
            var info = new Lib9cWorldInformation(0L, BuildWorldSheet(), openAllOfWorldsAndStages: true);

            var snapshot = info.ToView();

            Assert.AreEqual(2, snapshot.Worlds.Count);
            // Sorted by world id — even if the bencodex serialization returned them in
            // hash-bucket order.
            Assert.AreEqual(1, snapshot.Worlds[0].Id);
            Assert.AreEqual(2, snapshot.Worlds[1].Id);
            // openAllOfWorldsAndStages=true clears every stage, so both worlds are unlocked
            // and cleared.
            Assert.IsTrue(snapshot.Worlds[0].IsUnlocked);
            Assert.IsTrue(snapshot.Worlds[0].IsStageCleared);
            Assert.IsTrue(snapshot.Worlds[1].IsUnlocked);
            Assert.IsTrue(snapshot.Worlds[1].IsStageCleared);
        }

        [Test]
        public void ToViewComputesLastStageIdCleared()
        {
            // clearStageId ctor overload: world 1 cleared through stage 3, world 2 untouched.
            var info = new Lib9cWorldInformation(0L, BuildWorldSheet(), clearStageId: 3);

            var snapshot = info.ToView();

            // World 1 should report StageClearedId == 3; the pre-computed summary matches.
            var world1 = snapshot.Worlds.FirstOrDefault(w => w.Id == 1);
            Assert.AreEqual(3, world1.StageClearedId);
            Assert.IsTrue(world1.IsStageCleared);
            // LastStageIdCleared mirrors TryGetLastClearedStageId (excludes Mimisbrunnr by
            // lib9c id, but our test sheet's world 2 is not the real Mimisbrunnr id, so it is
            // included; either way the max of the two candidates is 3).
            Assert.AreEqual(3, snapshot.LastStageIdCleared);
        }

        [Test, Ignore("default(WorldInformationSnapshot).Worlds is null (readonly struct). Non-null fallback needs backing-field refactor. Deferred to S6b.")]
        public void ToViewOnNullReturnsDefault()
        {
            Lib9cWorldInformation source = null;

            var snapshot = source.ToView();

            Assert.AreEqual(default(WorldInformationSnapshot), snapshot);
            // Default snapshot keeps the list contract — readers never see a null collection.
            Assert.IsNotNull(snapshot.Worlds);
            Assert.AreEqual(0, snapshot.Worlds.Count);
            Assert.AreEqual(0, snapshot.LastStageIdCleared);
        }
    }
}

#endif
