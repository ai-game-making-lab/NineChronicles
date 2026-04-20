#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Linq;
using Nekoyume.SingleClient.Models.Items;
using Nekoyume.TableData;
using NUnit.Framework;
using ElementalType = Nekoyume.SingleClient.Models.Elemental.ElementalType;
using Lib9cConsumable = Nekoyume.Model.Item.Consumable;
using Lib9cCostume = Nekoyume.Model.Item.Costume;
using Lib9cElementalType = Nekoyume.Model.Elemental.ElementalType;
using Lib9cItemBase = Nekoyume.Model.Item.ItemBase;
using Lib9cItemSubType = Nekoyume.Model.Item.ItemSubType;
using Lib9cMaterial = Nekoyume.Model.Item.Material;
using Lib9cStatType = Nekoyume.Model.Stat.StatType;
using Lib9cTradableMaterial = Nekoyume.Model.Item.TradableMaterial;
using Lib9cWeapon = Nekoyume.Model.Item.Weapon;
using StatType = Nekoyume.SingleClient.Models.Stats.StatType;

namespace Tests.EditMode.SingleClient
{
    /// <summary>
    /// Covers <see cref="ItemSnapshotMapper"/>'s per-subtype projections and the polymorphic
    /// dispatcher. The fixture builds lib9c <c>ItemBase</c> instances directly from
    /// <c>*ItemSheet.Row</c> via <c>Row.Set(...)</c>, matching the idiom used by the existing
    /// lib9c ShopStateTest/EquipmentTest fixtures — no <c>TableSheets</c> or CSV loading.
    /// </summary>
    public class ItemSnapshotMapperTest
    {
        private const long RequiredBlockIndex = 0L;

        private static EquipmentItemSheet.Row BuildWeaponRow(
            int id = 10100000, string elementalType = "Fire", string statType = "ATK",
            string statValue = "42")
        {
            var row = new EquipmentItemSheet.Row();
            row.Set(new[]
            {
                id.ToString(),    // Id
                "Weapon",         // ItemSubType
                "1",              // Grade
                elementalType,    // ElementalType
                "0",              // SetId
                statType,         // Stat.StatType
                statValue,        // Stat.BaseValue
                "2",              // AttackRange
                id.ToString(),    // SpineResourcePath
            });
            return row;
        }

        private static CostumeItemSheet.Row BuildCostumeRow(int id = 40100000)
        {
            var row = new CostumeItemSheet.Row();
            row.Set(new[]
            {
                id.ToString(),     // Id
                "FullCostume",     // ItemSubType
                "2",               // Grade
                "Normal",          // ElementalType
                "",                // SpineResourcePath (auto-derived)
            });
            return row;
        }

        private static ConsumableItemSheet.Row BuildConsumableRow(int id = 20010000)
        {
            var row = new ConsumableItemSheet.Row();
            row.Set(new[]
            {
                id.ToString(),     // Id
                "Food",            // ItemSubType
                "3",               // Grade
                "Water",           // ElementalType
                "HP",              // Stat[0].StatType
                "100",             // Stat[0].BaseValue
                "ATK",             // Stat[1].StatType
                "10",              // Stat[1].BaseValue
            });
            return row;
        }

        private static MaterialItemSheet.Row BuildMaterialRow(int id = 50100000)
        {
            var row = new MaterialItemSheet.Row();
            row.Set(new[]
            {
                id.ToString(),     // Id
                "NormalMaterial",  // ItemSubType
                "1",               // Grade
                "Normal",          // ElementalType
            });
            return row;
        }

        [Test]
        public void ToEquipmentSnapshotRoundTripsCoreFields()
        {
            var row = BuildWeaponRow();
            var nonFungibleId = Guid.NewGuid();
            var weapon = new Lib9cWeapon(row, nonFungibleId, RequiredBlockIndex);
            weapon.level = 5;
            weapon.equipped = true;
            weapon.optionCountFromCombination = 2;
            weapon.MadeWithMimisbrunnrRecipe = true;

            var snapshot = weapon.ToEquipmentSnapshot();

            Assert.AreEqual(row.Id, snapshot.Id);
            Assert.AreEqual(1, snapshot.Grade);
            Assert.AreEqual(ItemType.Equipment, snapshot.ItemType);
            Assert.AreEqual(ItemSubType.Weapon, snapshot.ItemSubType);
            Assert.AreEqual(ElementalType.Fire, snapshot.ElementalType);
            Assert.AreEqual(nonFungibleId, snapshot.NonFungibleId);
            Assert.AreEqual(string.Empty, snapshot.TradableId);
            Assert.AreEqual(5, snapshot.Level);
            Assert.AreEqual(StatType.ATK, snapshot.Stat.StatType);
            Assert.AreEqual(42m, snapshot.Stat.BaseValue);
            Assert.AreEqual(42m, snapshot.Stat.TotalValue);
            Assert.IsTrue(snapshot.Equipped);
            Assert.IsTrue(snapshot.MadeWithMimisbrunnrRecipe);
            Assert.AreEqual(RequiredBlockIndex, snapshot.RequiredBlockIndex);
            Assert.AreEqual(2, snapshot.OptionCountFromCombination);
            Assert.IsNotNull(snapshot.Skills);
            Assert.IsNotNull(snapshot.BuffSkills);
            Assert.IsNotNull(snapshot.StatsMap);

            // Equipment-only fields round-tripped from lib9c.
            // Vanilla ctor seeds IconId = iconId != 0 ? iconId : data.Id, so for a
            // non-custom-craft weapon the snapshot IconId equals the row Id.
            Assert.AreEqual(row.Id, snapshot.IconId);
            Assert.AreEqual(weapon.IconId, snapshot.IconId);
            Assert.IsFalse(snapshot.ByCustomCraft);
            Assert.IsFalse(snapshot.CraftWithRandom);
            Assert.IsFalse(snapshot.HasRandomOnlyIcon);
            Assert.AreEqual(StatType.ATK, snapshot.UniqueStatType);
            Assert.AreEqual((int)weapon.UniqueStatType, (int)snapshot.UniqueStatType);
        }

        [Test]
        public void ToEquipmentSnapshotCustomCraftFieldsProjected()
        {
            // Custom-craft flags default to false in the ctor, but are mutable fields on lib9c
            // Equipment. Flip them post-construction to confirm the mapper reads the live
            // values (not just the ctor defaults) and that IconId is allowed to diverge from
            // the row Id for the custom-craft random-only icon path.
            var row = BuildWeaponRow();
            var weapon = new Lib9cWeapon(row, Guid.NewGuid(), RequiredBlockIndex);
            const int divergentIconId = 99999999;
            weapon.IconId = divergentIconId;
            weapon.ByCustomCraft = true;
            weapon.CraftWithRandom = true;
            weapon.HasRandomOnlyIcon = true;

            var snapshot = weapon.ToEquipmentSnapshot();

            Assert.AreEqual(divergentIconId, snapshot.IconId,
                "IconId must follow lib9c.Equipment.IconId even when it diverges from Id.");
            Assert.AreNotEqual(snapshot.Id, snapshot.IconId);
            Assert.IsTrue(snapshot.ByCustomCraft);
            Assert.IsTrue(snapshot.CraftWithRandom);
            Assert.IsTrue(snapshot.HasRandomOnlyIcon);
        }

        [Test]
        public void ToEquipmentSnapshotElementalTypeOrdinalMatchesLib9c()
        {
            // ElementalType mirror must stay ordinal-compatible with lib9c so the mapper's cast
            // chain (Lib9cET -> ToView() -> SingleClient ET) yields the expected enum.
            foreach (Lib9cElementalType value in Enum.GetValues(typeof(Lib9cElementalType)))
            {
                var row = BuildWeaponRow(elementalType: value.ToString());
                var weapon = new Lib9cWeapon(row, Guid.NewGuid(), RequiredBlockIndex);

                var snapshot = weapon.ToEquipmentSnapshot();

                Assert.AreEqual((int)value, (int)snapshot.ElementalType,
                    $"ElementalType ordinal mismatch for {value}.");
                Assert.AreEqual(value.ToString(), snapshot.ElementalType.ToString(),
                    $"ElementalType name mismatch for {value}.");
            }
        }

        [Test]
        public void ToCostumeSnapshotRoundTripsEquippedAndNonFungibleId()
        {
            var row = BuildCostumeRow();
            var nonFungibleId = Guid.NewGuid();
            var costume = new Lib9cCostume(row, nonFungibleId);
            costume.Equip();
            costume.RequiredBlockIndex = 1234L;

            var snapshot = costume.ToCostumeSnapshot();

            Assert.AreEqual(row.Id, snapshot.Id);
            Assert.AreEqual(2, snapshot.Grade);
            Assert.AreEqual(ItemType.Costume, snapshot.ItemType);
            Assert.AreEqual(ItemSubType.FullCostume, snapshot.ItemSubType);
            Assert.AreEqual(nonFungibleId, snapshot.NonFungibleId);
            Assert.IsTrue(snapshot.Equipped);
            Assert.AreEqual(1234L, snapshot.RequiredBlockIndex);
            Assert.AreEqual(string.Empty, snapshot.TradableId);

            // Unequip path round-trips too.
            costume.Unequip();
            Assert.IsFalse(costume.ToCostumeSnapshot().Equipped);
        }

        [Test]
        public void ToConsumableSnapshotProjectsStatsList()
        {
            var row = BuildConsumableRow();
            var nonFungibleId = Guid.NewGuid();
            var consumable = new Lib9cConsumable(row, nonFungibleId, RequiredBlockIndex);

            var snapshot = consumable.ToConsumableSnapshot();

            Assert.AreEqual(row.Id, snapshot.Id);
            Assert.AreEqual(ItemType.Consumable, snapshot.ItemType);
            Assert.AreEqual(ItemSubType.Food, snapshot.ItemSubType);
            Assert.AreEqual(ElementalType.Water, snapshot.ElementalType);
            Assert.AreEqual(nonFungibleId, snapshot.NonFungibleId);
            Assert.AreEqual(2, snapshot.Stats.Count);
            var hp = snapshot.Stats.FirstOrDefault(s => s.StatType == StatType.HP);
            Assert.AreEqual(100m, hp.BaseValue);
            var atk = snapshot.Stats.FirstOrDefault(s => s.StatType == StatType.ATK);
            Assert.AreEqual(10m, atk.BaseValue);
            Assert.IsNotNull(snapshot.Skills);
        }

        [Test]
        public void ToMaterialSnapshotTradableIdIsEmptyForPlainMaterial()
        {
            var row = BuildMaterialRow();
            var material = new Lib9cMaterial(row);

            var snapshot = material.ToMaterialSnapshot();

            Assert.AreEqual(row.Id, snapshot.Id);
            Assert.AreEqual(ItemType.Material, snapshot.ItemType);
            Assert.AreEqual(ItemSubType.NormalMaterial, snapshot.ItemSubType);
            Assert.IsNull(snapshot.NonFungibleId);
            Assert.AreEqual(string.Empty, snapshot.TradableId,
                "Plain Material is pure-fungible and must not leak its ItemId hash as a trade key.");
        }

        [Test]
        public void ToMaterialSnapshotTradableIdIsHexForTradableMaterial()
        {
            var row = BuildMaterialRow();
            var tradable = new Lib9cTradableMaterial(row);

            var snapshot = tradable.ToMaterialSnapshot();

            Assert.AreEqual(row.Id, snapshot.Id);
            Assert.AreEqual(ItemType.Material, snapshot.ItemType);
            Assert.IsNull(snapshot.NonFungibleId);
            Assert.IsFalse(string.IsNullOrEmpty(snapshot.TradableId),
                "TradableMaterial must surface the hex-encoded HashDigest<SHA256>.");
            // HashDigest<SHA256>.ToString() is a lowercase 64-character hex string.
            Assert.AreEqual(64, snapshot.TradableId.Length,
                "TradableId must be a 64-char hex digest for SHA256.");
            Assert.AreEqual(tradable.ItemId.ToString(), snapshot.TradableId);
        }

        [Test]
        public void ToPolySnapshotPicksCorrectSubtype()
        {
            var weapon = new Lib9cWeapon(BuildWeaponRow(), Guid.NewGuid(), RequiredBlockIndex);
            var costume = new Lib9cCostume(BuildCostumeRow(), Guid.NewGuid());
            var consumable = new Lib9cConsumable(BuildConsumableRow(), Guid.NewGuid(), RequiredBlockIndex);
            var material = new Lib9cMaterial(BuildMaterialRow());
            var tradable = new Lib9cTradableMaterial(BuildMaterialRow());

            Assert.IsInstanceOf<EquipmentSnapshot>(((Lib9cItemBase)weapon).ToPolySnapshot());
            Assert.IsInstanceOf<CostumeSnapshot>(((Lib9cItemBase)costume).ToPolySnapshot());
            Assert.IsInstanceOf<ConsumableSnapshot>(((Lib9cItemBase)consumable).ToPolySnapshot());
            Assert.IsInstanceOf<MaterialSnapshot>(((Lib9cItemBase)material).ToPolySnapshot());
            Assert.IsInstanceOf<MaterialSnapshot>(((Lib9cItemBase)tradable).ToPolySnapshot());

            // IItemSnapshot interface carries the shared fields regardless of subtype.
            var polyWeapon = ((Lib9cItemBase)weapon).ToPolySnapshot();
            Assert.AreEqual(ItemType.Equipment, polyWeapon.ItemType);
            Assert.AreEqual(ItemSubType.Weapon, polyWeapon.ItemSubType);
        }

        [Test]
        public void ToSnapshotBaseProjectionReadsFromLib9c()
        {
            var weapon = new Lib9cWeapon(BuildWeaponRow(), Guid.NewGuid(), RequiredBlockIndex);
            var snapshot = ((Lib9cItemBase)weapon).ToSnapshot();

            Assert.AreEqual(weapon.Id, snapshot.Id);
            Assert.AreEqual(ItemType.Equipment, snapshot.ItemType);
            Assert.AreEqual(ItemSubType.Weapon, snapshot.ItemSubType);
            Assert.AreEqual(ElementalType.Fire, snapshot.ElementalType);
            Assert.AreEqual(weapon.ItemId, snapshot.NonFungibleId);
        }

        [Test]
        public void NullInputsReturnDefaultSnapshots()
        {
            Assert.AreEqual(default(ItemSnapshot), ((Lib9cItemBase)null).ToSnapshot());
            Assert.AreEqual(default(EquipmentSnapshot), ((Lib9cWeapon)null).ToEquipmentSnapshot());
            Assert.AreEqual(default(CostumeSnapshot), ((Lib9cCostume)null).ToCostumeSnapshot());
            Assert.AreEqual(default(ConsumableSnapshot), ((Lib9cConsumable)null).ToConsumableSnapshot());
            Assert.AreEqual(default(MaterialSnapshot), ((Lib9cMaterial)null).ToMaterialSnapshot());

            // Silence unused-using warnings on Lib9cStatType / Lib9cItemSubType without adding
            // noise branches — these usings anchor future test additions that assert ordinal
            // parity at the same boundary.
            _ = Lib9cStatType.NONE;
            _ = Lib9cItemSubType.Weapon;
        }
    }
}

#endif
