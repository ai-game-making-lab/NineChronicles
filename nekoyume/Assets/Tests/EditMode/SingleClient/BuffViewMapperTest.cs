using System.Collections.Generic;
using Nekoyume.Model.Buff;
using Nekoyume.SingleClient.Models.Buffs;
using Nekoyume.SingleClient.Models.Stats;
using Nekoyume.TableData;
using NUnit.Framework;
using Lib9cBuff = Nekoyume.Model.Buff.Buff;
using Lib9cStatType = Nekoyume.Model.Stat.StatType;

namespace Tests.EditMode.SingleClient
{
    public class BuffViewMapperTest
    {
        [Test]
        public void StatBuffProjectsStatTypeAndValue()
        {
            var row = new StatBuffSheet.Row();
            row.Set(new[]
            {
                "100001",   // Id
                "100001",   // GroupId
                "100",      // Chance
                "3",        // Duration
                "Enemy",    // TargetType (SkillTargetType)
                "HP",       // StatType
                "Add",      // OperationType
                "100",      // Value
                "true",     // IsEnhanceable
                "0",        // MaxStack
            });

            var buff = new StatBuff(row);

            var view = buff.ToView();

            Assert.AreEqual(row.Id, view.Id);
            Assert.AreEqual(row.GroupId, view.GroupId);
            Assert.AreEqual(row.Duration, view.OriginalDuration);
            Assert.AreEqual(row.Duration, view.RemainedDuration);
            Assert.AreEqual(BuffViewKind.Stat, view.Kind);
            Assert.IsTrue(view.IsStatBuff);
            Assert.AreEqual(StatType.HP, view.StatType);
            Assert.AreEqual(row.Value, view.StatValue);
        }

        [Test]
        public void ToViewMapSkipsNullInput()
        {
            IReadOnlyDictionary<int, Lib9cBuff> source = null;
            var result = source.ToViewMap();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void StatTypeCastIsOrdinalCompatible()
        {
            foreach (Lib9cStatType value in System.Enum.GetValues(typeof(Lib9cStatType)))
            {
                var mapped = BuffViewMapper.MapStatType(value);
                var roundTrip = BuffViewMapper.MapStatType(mapped);
                Assert.AreEqual(value, roundTrip,
                    $"StatType ordinal mismatch between lib9c and SingleClient for {value}.");
                Assert.AreEqual(value.ToString(), mapped.ToString(),
                    $"StatType name mismatch between lib9c and SingleClient for {value}.");
            }
        }
    }
}
