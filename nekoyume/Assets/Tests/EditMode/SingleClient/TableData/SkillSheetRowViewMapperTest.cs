using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.SingleClient.Models.Skills;
using Nekoyume.SingleClient.Models.TableData;
using Nekoyume.TableData;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient.TableData
{
    public class SkillSheetRowViewMapperTest
    {
        private static SkillSheet.Row BuildRow(
            string id = "100000",
            string elemental = "Fire",
            string skillType = "Attack",
            string skillCategory = "NormalAttack",
            string skillTargetType = "Enemy",
            string hitCount = "1",
            string cooldown = "0")
        {
            var row = new SkillSheet.Row();
            row.Set(new[] { id, elemental, skillType, skillCategory, skillTargetType, hitCount, cooldown });
            return row;
        }

        [Test]
        public void ProjectsEveryMirroredField()
        {
            var row = BuildRow(
                id: "100001",
                elemental: "Water",
                skillType: "Heal",
                skillCategory: "Heal",
                skillTargetType: "Self",
                hitCount: "3",
                cooldown: "7");

            var view = row.ToView();

            Assert.AreEqual(100001, view.Id);
            Assert.AreEqual(ElementalType.Water, view.ElementalType);
            Assert.AreEqual(SkillType.Heal, view.SkillType);
            Assert.AreEqual(SkillCategory.Heal, view.SkillCategory);
            Assert.AreEqual(SkillTargetType.Self, view.SkillTargetType);
            Assert.AreEqual(3, view.HitCount);
            Assert.AreEqual(7, view.Cooldown);
            Assert.IsFalse(view.IsBuffSkill);
        }

        [Test]
        public void BuffAndDebuffSkillsFlagIsBuffSkill()
        {
            var buffRow = BuildRow(skillType: "Buff", skillCategory: "HPBuff", skillTargetType: "Self");
            var debuffRow = BuildRow(skillType: "Debuff", skillCategory: "Debuff", skillTargetType: "Enemy");

            Assert.IsTrue(buffRow.ToView().IsBuffSkill);
            Assert.IsTrue(debuffRow.ToView().IsBuffSkill);
        }

        [Test]
        public void ToViewOnNullReturnsDefault()
        {
            SkillSheet.Row row = null;

            var view = row.ToView();

            Assert.AreEqual(default(SkillSheetRowView), view);
            // GetLocalizedName on default falls back to the raw key (no L10n init required).
            Assert.AreEqual("SKILL_NAME_0", view.GetLocalizedName());
        }

        [Test]
        public void EqualityIsBasedOnId()
        {
            var a = BuildRow(id: "42").ToView();
            var b = BuildRow(id: "42", hitCount: "9").ToView();
            var c = BuildRow(id: "99").ToView();

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a, c);
        }
    }
}
