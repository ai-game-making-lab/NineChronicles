using NUnit.Framework;
using ItemEnumMapper = Nekoyume.SingleClient.Models.Items.ItemEnumMapper;
using ItemSubType = Nekoyume.SingleClient.Models.Items.ItemSubType;
using ItemType = Nekoyume.SingleClient.Models.Items.ItemType;
using Lib9cItemSubType = Nekoyume.Model.Item.ItemSubType;
using Lib9cItemType = Nekoyume.Model.Item.ItemType;
using Lib9cLockType = Nekoyume.Model.Item.LockType;
using LockType = Nekoyume.SingleClient.Models.Items.LockType;

namespace Tests.EditMode.SingleClient
{
    public class ItemEnumMapperTest
    {
        [Test]
        public void ItemTypeRoundTripsAcrossAllLib9cValues()
        {
            foreach (Lib9cItemType value in System.Enum.GetValues(typeof(Lib9cItemType)))
            {
                var view = ItemEnumMapper.ToView(value);
                Assert.AreEqual(value.ToString(), view.ToString(),
                    $"ItemType name mismatch for {value}.");
                Assert.AreEqual(value, ItemEnumMapper.ToLib9c(view),
                    $"ItemType round-trip failed for {value}.");
            }
        }

        [Test]
        public void ItemSubTypeRoundTripsAcrossAllLib9cValues()
        {
            foreach (Lib9cItemSubType value in System.Enum.GetValues(typeof(Lib9cItemSubType)))
            {
                var view = ItemEnumMapper.ToView(value);
                Assert.AreEqual(value.ToString(), view.ToString(),
                    $"ItemSubType name mismatch for {value}.");
                Assert.AreEqual(value, ItemEnumMapper.ToLib9c(view),
                    $"ItemSubType round-trip failed for {value}.");
            }
        }

        [Test]
        public void LockTypeRoundTripsAcrossAllLib9cValues()
        {
            foreach (Lib9cLockType value in System.Enum.GetValues(typeof(Lib9cLockType)))
            {
                var view = ItemEnumMapper.ToView(value);
                Assert.AreEqual(value.ToString(), view.ToString(),
                    $"LockType name mismatch for {value}.");
                Assert.AreEqual(value, ItemEnumMapper.ToLib9c(view),
                    $"LockType round-trip failed for {value}.");
            }
        }

        [Test]
        public void ItemSubTypeCoversExpectedClusters()
        {
            // Cover the obvious clusters so the count stays in sync if lib9c adds a new value.
            Assert.AreEqual(ItemType.Equipment, ItemEnumMapper.ToLib9c(ItemType.Equipment) switch
            {
                Lib9cItemType.Equipment => ItemType.Equipment,
                _ => ItemType.Consumable,
            });

            Assert.IsTrue((int)ItemSubType.Weapon >= 6 && (int)ItemSubType.Ring <= 10,
                "Equipment subtypes must occupy ordinals 6..10 for ordinal-compatible casts.");
            Assert.IsTrue((int)ItemSubType.FullCostume >= 1 && (int)ItemSubType.TailCostume <= 5,
                "Costume subtypes must occupy ordinals 1..5.");
        }
    }
}
