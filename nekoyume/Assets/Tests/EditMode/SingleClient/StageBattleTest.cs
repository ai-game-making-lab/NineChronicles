using System;
using System.IO;
using Nekoyume.SingleClient;
using Nekoyume.SingleClient.Combat;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient
{
    /// <summary>
    /// EditMode tests for the BattleStage loop. Does not depend on lib9c —
    /// deliberately omits the <c>#if LIB9C_RESTORED</c> guard that wraps the
    /// older EditMode SingleClient tests in this folder.
    /// </summary>
    public class StageBattleTest
    {
        private string _tempDirectory;
        private string _statePath;

        [SetUp]
        public void SetUp()
        {
            _tempDirectory = Path.Combine(
                Path.GetTempPath(),
                "NineChroniclesStageBattleTests",
                Guid.NewGuid().ToString("N"));
            _statePath = Path.Combine(_tempDirectory, "state.json");
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        private SingleClientSession StartSessionWithAp(long actionPoint)
        {
            var session = new SingleClientSession(new FileSingleClientStateStore(_statePath));
            session.Start();
            session.CreateOrSelectAvatar(0, "tester");
            session.FillActionPoint(actionPoint);
            return session;
        }

        [Test]
        public void AvatarStatsScaleWithLevel()
        {
            Assert.AreEqual(100L, SingleClientAvatarStats.HpAtLevel(1));
            Assert.AreEqual(120L, SingleClientAvatarStats.HpAtLevel(2));
            Assert.AreEqual(10L, SingleClientAvatarStats.AtkAtLevel(1));
            Assert.AreEqual(13L, SingleClientAvatarStats.AtkAtLevel(2));
            Assert.AreEqual(5L, SingleClientAvatarStats.DefAtLevel(1));
        }

        [Test]
        public void GrantExpCarriesOverAndLevelsUpMultipleTimes()
        {
            var avatar = SingleClientAvatarState.CreateDefault("x");
            avatar.level = 1;
            avatar.exp = 0;

            var levelsGained = SingleClientAvatarStats.GrantExp(avatar, 250);

            // level 1→2 at 100 exp, 2→3 at 200 exp total cumulative threshold.
            // Thresholds: L1(100), L2(200). 250 - 100 = 150; 150 - 200 needed → no, stays at L2 with 150 exp.
            Assert.AreEqual(1, levelsGained);
            Assert.AreEqual(2, avatar.level);
            Assert.AreEqual(150L, avatar.exp);
        }

        [Test]
        public void Stage1IsWinnableAtLevel1WithDeterministicSeed()
        {
            var session = StartSessionWithAp(20);

            var result = session.BattleStage(stageId: 1, seed: 12345);

            Assert.IsTrue(result.PlayerWon, "Level-1 avatar should defeat Stage 1 (Slime).");
            Assert.IsTrue(result.WasFirstClear);
            Assert.AreEqual(1, result.StageId);
            Assert.AreEqual(5L, result.ActionPointCost);
            Assert.AreEqual(20L, result.ActionPointBefore);
            Assert.AreEqual(15L, result.ActionPointAfter);
            Assert.AreEqual(30L, result.ExpGained);
            Assert.AreEqual(1, result.State.stage.clearedStageIds.Count);
            Assert.AreEqual(1, result.State.stage.highestClearedStageId);
        }

        [Test]
        public void Stage5LosesAtLevel1AndDoesNotMarkCleared()
        {
            var session = StartSessionWithAp(20);

            var result = session.BattleStage(stageId: 5, seed: 1);

            Assert.IsFalse(result.PlayerWon, "Level-1 avatar cannot defeat Stage 5 (Dragon).");
            Assert.IsFalse(result.WasFirstClear);
            Assert.AreEqual(0L, result.ExpGained);
            Assert.AreEqual(0, result.State.stage.highestClearedStageId);
            // AP is still consumed (entry fee).
            Assert.AreEqual(10L, result.ActionPointCost);
            Assert.AreEqual(10L, result.ActionPointAfter);
        }

        [Test]
        public void DeterministicSeedProducesIdenticalOutcome()
        {
            var stage = SingleClientStageSheet.Get(3);
            var avatar = SingleClientAvatarState.CreateDefault("x");
            avatar.level = 5;

            var a = SingleClientStageBattle.Execute(stage, avatar, seed: 777);
            var b = SingleClientStageBattle.Execute(stage, avatar, seed: 777);

            Assert.AreEqual(a.PlayerWon, b.PlayerWon);
            Assert.AreEqual(a.Turns, b.Turns);
            Assert.AreEqual(a.Result.AttackerRemainingHp, b.Result.AttackerRemainingHp);
            Assert.AreEqual(a.Result.DefenderRemainingHp, b.Result.DefenderRemainingHp);
        }

        [Test]
        public void InsufficientActionPointThrows()
        {
            var session = StartSessionWithAp(2);

            Assert.Throws<InvalidOperationException>(
                () => session.BattleStage(stageId: 1, seed: 1));
        }

        [Test]
        public void InvalidStageIdRejected()
        {
            var session = StartSessionWithAp(20);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => session.BattleStage(stageId: 0, seed: 1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => session.BattleStage(stageId: 99, seed: 1));
        }

        [Test]
        public void WinGrantsExpAndMayLevelUp()
        {
            var session = StartSessionWithAp(100);

            // Stage 1 gives 30 exp. Level 1 → 2 threshold is 100 exp.
            // 4 wins = 120 exp → reaches L2 with 20 exp carryover.
            for (var i = 0; i < 4; i++)
            {
                session.BattleStage(stageId: 1, seed: 12345 + i);
            }

            var final = session.State.avatar;
            Assert.GreaterOrEqual(final.level, 2, $"Expected level-up, got L{final.level} (exp {final.exp}).");
            Assert.AreEqual(1, session.State.stage.highestClearedStageId);
        }

        [Test]
        public void RuntimeFacadeExposesClientBattleStageResult()
        {
            var store = new FileSingleClientStateStore(_statePath);
            var session = new SingleClientSession(store);
            session.Start();
            session.CreateOrSelectAvatar(0, "runtime-user");
            session.FillActionPoint(50);
            var runtime = new SingleClientRuntime(session);

            var result = runtime.BattleStage(stageId: 1, seed: 12345);

            Assert.IsInstanceOf<ClientBattleStageResult>(result);
            Assert.IsTrue(result.PlayerWon);
            Assert.AreEqual(1, result.StageId);
            Assert.AreEqual(30L, result.ExpGained);
            Assert.IsNotNull(result.State);
            Assert.AreEqual(1, result.State.HighestClearedStageId);
            // AvatarExp property wired through ClientRuntimeState.
            Assert.AreEqual(30L, result.State.AvatarExp);
        }
    }
}
