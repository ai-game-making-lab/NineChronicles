using System.Collections.Generic;
using Nekoyume.Model.Stat;
using Nekoyume.SingleClient.Models.Stats;
using Nekoyume.SingleClient.Models.TableData;
using Nekoyume.TableData;
using NUnit.Framework;
using Lib9cDecimalStat = Nekoyume.Model.Stat.DecimalStat;
using Lib9cOperationType = Nekoyume.Model.Stat.StatModifier.OperationType;
using Lib9cStatReferenceType = Nekoyume.Model.EnumType.StatReferenceType;
using Lib9cStatType = Nekoyume.Model.Stat.StatType;
using Lib9cRuneOptionInfo = Nekoyume.TableData.RuneOptionSheet.Row.RuneOptionInfo;

namespace Tests.EditMode.SingleClient.TableData
{
    public class RuneOptionInfoViewMapperTest
    {
        [Test]
        public void ProjectsEveryMirroredField()
        {
            var stats = new List<(Lib9cDecimalStat, Lib9cOperationType)>
            {
                (new Lib9cDecimalStat(Lib9cStatType.ATK, 150m), Lib9cOperationType.Add),
                (new Lib9cDecimalStat(Lib9cStatType.CRI, 7m), Lib9cOperationType.Percentage),
            };

            var info = new Lib9cRuneOptionInfo(
                cp: 9999,
                stats: stats,
                skillId: 100001,
                skillCooldown: 4,
                skillChance: 80,
                skillValue: 500m,
                skillValueType: Lib9cOperationType.Add,
                skillStatType: Lib9cStatType.HP,
                statReferenceType: Lib9cStatReferenceType.Caster,
                buffDuration: 3);

            var view = info.ToView();

            Assert.AreEqual(9999, view.Cp);
            Assert.AreEqual(100001, view.SkillId);
            Assert.AreEqual(4, view.SkillCooldown);
            Assert.AreEqual(80, view.SkillChance);
            Assert.AreEqual(500m, view.SkillValue);
            Assert.AreEqual(StatType.HP, view.SkillStatType);
            Assert.AreEqual(3, view.BuffDuration);
            Assert.IsTrue(view.HasSkill);
            Assert.IsNull(view.Skill);

            Assert.AreEqual(2, view.Stats.Count);
            Assert.AreEqual(StatType.ATK, view.Stats[0].StatType);
            Assert.AreEqual(150m, view.Stats[0].Value);
            Assert.AreEqual(StatType.CRI, view.Stats[1].StatType);
            Assert.AreEqual(7m, view.Stats[1].Value);
        }

        [Test]
        public void CompanionSkillRowIsMirrored()
        {
            var stats = new List<(Lib9cDecimalStat, Lib9cOperationType)>();
            var info = new Lib9cRuneOptionInfo(cp: 100, stats: stats);

            var skillRow = new SkillSheet.Row();
            skillRow.Set(new[] { "100001", "Fire", "Attack", "NormalAttack", "Enemy", "1", "0" });

            var view = info.ToView(skillRow);

            Assert.IsNotNull(view.Skill);
            Assert.AreEqual(100001, view.Skill.Value.Id);
            Assert.IsFalse(view.HasSkill);
        }

        [Test]
        public void SkipsNullStatEntries()
        {
            var stats = new List<(Lib9cDecimalStat, Lib9cOperationType)>
            {
                (null, Lib9cOperationType.Add),
                (new Lib9cDecimalStat(Lib9cStatType.DEF, 50m), Lib9cOperationType.Add),
            };

            var info = new Lib9cRuneOptionInfo(cp: 0, stats: stats);

            var view = info.ToView();

            Assert.AreEqual(1, view.Stats.Count);
            Assert.AreEqual(StatType.DEF, view.Stats[0].StatType);
        }

        [Test]
        public void ToViewOnNullReturnsDefault()
        {
            Lib9cRuneOptionInfo info = null;

            var view = info.ToView();

            Assert.AreEqual(default(RuneOptionInfoView), view);
        }
    }
}
