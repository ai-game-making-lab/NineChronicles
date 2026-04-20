#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.TableData;
using NUnit.Framework;
using BuffSkill = Nekoyume.Model.Skill.BuffSkill;
using Lib9cSkillCategory = Nekoyume.Model.Skill.SkillCategory;
using Lib9cSkillTargetType = Nekoyume.Model.Skill.SkillTargetType;
using Lib9cSkillType = Nekoyume.Model.Skill.SkillType;
using Lib9cStatType = Nekoyume.Model.Stat.StatType;
using NormalAttack = Nekoyume.Model.Skill.NormalAttack;
using SkillCategory = Nekoyume.SingleClient.Models.Skills.SkillCategory;
using SkillSnapshotMapper = Nekoyume.SingleClient.Models.Skills.SkillSnapshotMapper;
using SkillTargetType = Nekoyume.SingleClient.Models.Skills.SkillTargetType;
using SkillType = Nekoyume.SingleClient.Models.Skills.SkillType;
using StatType = Nekoyume.SingleClient.Models.Stats.StatType;

namespace Tests.EditMode.SingleClient
{
    public class SkillSnapshotMapperTest
    {
        [Test]
        public void NormalAttackProjectsAllRowAndRuntimeFields()
        {
            var row = new SkillSheet.Row();
            row.Set(new[]
            {
                "100000",        // Id
                "Fire",          // ElementalType
                "Attack",        // SkillType
                "NormalAttack",  // SkillCategory
                "Enemy",         // SkillTargetType
                "1",             // HitCount
                "0",             // Cooldown
            });

            var skill = new NormalAttack(row, power: 350, chance: 100, statPowerRatio: 0,
                referencedStatType: Lib9cStatType.NONE);

            var snapshot = SkillSnapshotMapper.ToSnapshot(skill);

            Assert.AreEqual(row.Id, snapshot.Id);
            Assert.AreEqual(SkillType.Attack, snapshot.SkillType);
            Assert.AreEqual(SkillCategory.NormalAttack, snapshot.SkillCategory);
            Assert.AreEqual(SkillTargetType.Enemy, snapshot.SkillTargetType);
            Assert.AreEqual(row.HitCount, snapshot.HitCount);
            Assert.AreEqual(row.Cooldown, snapshot.Cooldown);
            Assert.AreEqual(350, snapshot.Power);
            Assert.AreEqual(100, snapshot.Chance);
            Assert.AreEqual(0, snapshot.StatPowerRatio);
            Assert.AreEqual(StatType.NONE, snapshot.ReferencedStatType);
            Assert.IsFalse(snapshot.IsBuffSkill);
        }

        [Test]
        public void BuffSkillFlagSetFromLib9cSkillType()
        {
            var row = new SkillSheet.Row();
            row.Set(new[]
            {
                "100010",        // Id
                "Normal",        // ElementalType
                "Buff",          // SkillType
                "HPBuff",        // SkillCategory
                "Self",          // SkillTargetType
                "1",             // HitCount
                "2",             // Cooldown
            });

            var skill = new BuffSkill(row, power: 0, chance: 100, statPowerRatio: 0,
                referencedStatType: Lib9cStatType.NONE);

            var snapshot = SkillSnapshotMapper.ToSnapshot(skill);

            Assert.AreEqual(SkillType.Buff, snapshot.SkillType);
            Assert.AreEqual(SkillCategory.HPBuff, snapshot.SkillCategory);
            Assert.AreEqual(SkillTargetType.Self, snapshot.SkillTargetType);
            Assert.IsTrue(snapshot.IsBuffSkill);
        }

        [Test]
        public void EnumCastsAreOrdinalCompatibleWithLib9c()
        {
            foreach (Lib9cSkillType value in System.Enum.GetValues(typeof(Lib9cSkillType)))
            {
                var mapped = (SkillType)(int)value;
                Assert.AreEqual(value.ToString(), mapped.ToString(),
                    $"SkillType ordinal mismatch for {value}.");
            }

            foreach (Lib9cSkillCategory value in System.Enum.GetValues(typeof(Lib9cSkillCategory)))
            {
                var mapped = (SkillCategory)(int)value;
                Assert.AreEqual(value.ToString(), mapped.ToString(),
                    $"SkillCategory ordinal mismatch for {value}.");
            }

            foreach (Lib9cSkillTargetType value in System.Enum.GetValues(typeof(Lib9cSkillTargetType)))
            {
                var mapped = (SkillTargetType)(int)value;
                Assert.AreEqual(value.ToString(), mapped.ToString(),
                    $"SkillTargetType ordinal mismatch for {value}.");
            }
        }
    }
}

#endif
