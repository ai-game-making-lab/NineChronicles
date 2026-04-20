#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.SingleClient.Models.Stats;
using Nekoyume.SingleClient.Models.TableData;
using Nekoyume.TableData;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient.TableData
{
    public class EquipmentItemOptionRowViewMapperTest
    {
        private static EquipmentItemOptionSheet.Row BuildRow()
        {
            var row = new EquipmentItemOptionSheet.Row();
            row.Set(new[]
            {
                "700000", // Id
                "ATK",    // StatType
                "10",     // StatMin
                "25",     // StatMax
                "100001", // SkillId
                "40",     // SkillDamageMin
                "80",     // SkillDamageMax
                "50",     // SkillChanceMin
                "75",     // SkillChanceMax
                "1500",   // StatDamageRatioMin
                "3000",   // StatDamageRatioMax
                "HP",     // ReferencedStatType
            });
            return row;
        }

        [Test]
        public void ProjectsEveryMirroredField()
        {
            var row = BuildRow();

            var view = row.ToView();

            Assert.AreEqual(700000, view.Id);
            Assert.AreEqual(StatType.ATK, view.StatType);
            Assert.AreEqual(10, view.StatMin);
            Assert.AreEqual(25, view.StatMax);
            Assert.AreEqual(100001, view.SkillId);
            Assert.AreEqual(40, view.SkillDamageMin);
            Assert.AreEqual(80, view.SkillDamageMax);
            Assert.AreEqual(50, view.SkillChanceMin);
            Assert.AreEqual(75, view.SkillChanceMax);
            Assert.AreEqual(1500m, view.StatPowerRatioMin);
            Assert.AreEqual(3000m, view.StatPowerRatioMax);
            Assert.AreEqual(StatType.HP, view.ReferencedStatType);
            Assert.IsTrue(view.HasSkill);
        }

        [Test]
        public void OmittedOptionalFieldsDefaultToZero()
        {
            // The short-form csv row (9 columns) is still valid in lib9c — StatDamageRatio* and
            // ReferencedStatType stay at their default values.
            var row = new EquipmentItemOptionSheet.Row();
            row.Set(new[]
            {
                "700001", "DEF", "5", "5", "0", "0", "0", "0", "0",
            });

            var view = row.ToView();

            Assert.AreEqual(0, view.SkillId);
            Assert.AreEqual(0m, view.StatPowerRatioMin);
            Assert.AreEqual(0m, view.StatPowerRatioMax);
            Assert.AreEqual(StatType.NONE, view.ReferencedStatType);
            Assert.IsFalse(view.HasSkill);
        }

        [Test]
        public void ToViewOnNullReturnsDefault()
        {
            EquipmentItemOptionSheet.Row row = null;
            Assert.AreEqual(default(EquipmentItemOptionRowView), row.ToView());
        }
    }
}

#endif
