using Nekoyume.SingleClient.Models.Elemental;
using NUnit.Framework;
using Lib9cElementalType = Nekoyume.Model.Elemental.ElementalType;
using Lib9cElementalRules = Nekoyume.Model.Elemental.ElementalTypeExtension;

namespace Tests.EditMode.SingleClient
{
    public class ElementalRulesTest
    {
        [Test]
        public void ElementalTypeOrdinalsMatchLib9c()
        {
            foreach (Lib9cElementalType value in System.Enum.GetValues(typeof(Lib9cElementalType)))
            {
                var view = ElementalTypeMapper.ToView(value);
                Assert.AreEqual(value.ToString(), view.ToString(),
                    $"ElementalType name mismatch between lib9c and SingleClient for {value}.");
                Assert.AreEqual(value, ElementalTypeMapper.ToLib9c(view),
                    $"ElementalType round-trip failed for {value}.");
            }
        }

        [Test]
        public void BattleResultMatchesLib9cAcrossAllMatchups()
        {
            foreach (Lib9cElementalType from in System.Enum.GetValues(typeof(Lib9cElementalType)))
            {
                foreach (Lib9cElementalType to in System.Enum.GetValues(typeof(Lib9cElementalType)))
                {
                    var lib9c = Lib9cElementalRules.GetBattleResult(from, to);
                    var single = from.ToView().GetBattleResult(to.ToView());
                    Assert.AreEqual(lib9c.ToString(), single.ToString(),
                        $"Battle result diverged for {from} vs {to}.");
                }
            }
        }

        [Test]
        public void MultiplierMatchesLib9cAcrossAllMatchups()
        {
            foreach (Lib9cElementalType from in System.Enum.GetValues(typeof(Lib9cElementalType)))
            {
                foreach (Lib9cElementalType to in System.Enum.GetValues(typeof(Lib9cElementalType)))
                {
                    var lib9c = Lib9cElementalRules.GetMultiplier(from, to);
                    var single = from.ToView().GetMultiplier(to.ToView());
                    Assert.AreEqual(lib9c, single,
                        $"Multiplier diverged for {from} vs {to}.");
                }
            }
        }
    }
}
