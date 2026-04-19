using Nekoyume.SingleClient.Models.Stats;
using Nekoyume.SingleClient.Models.TableData;
using Nekoyume.TableData;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient.TableData
{
    public class StatBuffRowViewMapperTest
    {
        private static StatBuffSheet.Row BuildRow(
            string id = "600001",
            string groupId = "600001",
            string chance = "50",
            string duration = "5",
            string targetType = "Self",
            string statType = "ATK",
            string op = "Add",
            string value = "250",
            string enhanceable = "true",
            string maxStack = "3")
        {
            var row = new StatBuffSheet.Row();
            row.Set(new[] { id, groupId, chance, duration, targetType, statType, op, value, enhanceable, maxStack });
            return row;
        }

        [Test]
        public void ProjectsEveryMirroredField()
        {
            var row = BuildRow();

            var view = row.ToView();

            Assert.AreEqual(600001, view.Id);
            Assert.AreEqual(600001, view.GroupId);
            Assert.AreEqual(StatType.ATK, view.StatType);
            Assert.AreEqual(5, view.Duration);
            Assert.AreEqual(250m, view.Value);
            Assert.AreEqual(50, view.Chance);
        }

        [Test]
        public void DebuffRowMapsNegativeStatType()
        {
            var row = BuildRow(statType: "SPD", op: "Percentage", value: "-20", chance: "100");

            var view = row.ToView();

            Assert.AreEqual(StatType.SPD, view.StatType);
            Assert.AreEqual(-20m, view.Value);
            Assert.AreEqual(100, view.Chance);
        }

        [Test]
        public void ToViewOnNullReturnsDefault()
        {
            StatBuffSheet.Row row = null;
            Assert.AreEqual(default(StatBuffRowView), row.ToView());
        }
    }
}
