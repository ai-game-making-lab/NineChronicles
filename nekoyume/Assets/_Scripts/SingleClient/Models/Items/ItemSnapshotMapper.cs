#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Generic;
using Nekoyume.SingleClient.Models.Buffs;
using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.SingleClient.Models.Skills;
using Nekoyume.SingleClient.Models.Stats;
using Lib9cConsumable = Nekoyume.Model.Item.Consumable;
using Lib9cCostume = Nekoyume.Model.Item.Costume;
using Lib9cDecimalStat = Nekoyume.Model.Stat.DecimalStat;
using Lib9cEquipment = Nekoyume.Model.Item.Equipment;
using Lib9cItemBase = Nekoyume.Model.Item.ItemBase;
using Lib9cMaterial = Nekoyume.Model.Item.Material;
using Lib9cTradableMaterial = Nekoyume.Model.Item.TradableMaterial;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Projects lib9c <c>ItemBase</c> subtypes into their client-owned snapshot counterparts.
    /// Kept extension-style so each migrating call site only adds a trailing <c>.ToSnapshot()</c>
    /// or <c>.ToEquipmentSnapshot()</c>. The polymorphic <see cref="ToPolySnapshot"/> dispatches
    /// to the right per-subtype helper and returns the common <see cref="IItemSnapshot"/>
    /// interface so callers can switch-pattern without boxing.
    /// </summary>
    public static class ItemSnapshotMapper
    {
        /// <summary>
        /// Base projection; returns just the fields lib9c <c>ItemBase</c> defines. Use the
        /// per-subtype helpers (or <see cref="ToPolySnapshot"/>) when the caller needs the
        /// Equipment/Costume/Consumable/Material-specific payload.
        /// </summary>
        public static ItemSnapshot ToSnapshot(this Lib9cItemBase item)
        {
            if (item is null)
            {
                return default;
            }

            return new ItemSnapshot(
                itemType: item.ItemType.ToView(),
                itemSubType: item.ItemSubType.ToView(),
                id: item.Id,
                grade: item.Grade,
                elementalType: item.ElementalType.ToView(),
                nonFungibleId: ResolveNonFungibleId(item),
                tradableId: ResolveTradableIdHex(item));
        }

        public static EquipmentSnapshot ToEquipmentSnapshot(this Lib9cEquipment source)
        {
            if (source is null)
            {
                return default;
            }

            var skills = ProjectSkills(source.Skills);
            var buffSkills = ProjectBuffSkills(source.BuffSkills);
            var statsMap = source.StatsMap.ToView();

            return new EquipmentSnapshot(
                itemType: source.ItemType.ToView(),
                itemSubType: source.ItemSubType.ToView(),
                id: source.Id,
                grade: source.Grade,
                elementalType: source.ElementalType.ToView(),
                nonFungibleId: source.ItemId,
                tradableId: string.Empty,
                level: source.level,
                stat: source.Stat.ToView(),
                skills: skills,
                buffSkills: buffSkills,
                statsMap: statsMap,
                setId: source.SetId,
                equipped: source.equipped,
                madeWithMimisbrunnrRecipe: source.MadeWithMimisbrunnrRecipe,
                requiredBlockIndex: source.RequiredBlockIndex,
                optionCountFromCombination: source.optionCountFromCombination,
                iconId: source.IconId,
                byCustomCraft: source.ByCustomCraft,
                craftWithRandom: source.CraftWithRandom,
                hasRandomOnlyIcon: source.HasRandomOnlyIcon,
                uniqueStatType: BuffViewMapper.MapStatType(source.UniqueStatType),
                exp: source.Exp);
        }

        public static CostumeSnapshot ToCostumeSnapshot(this Lib9cCostume source)
        {
            if (source is null)
            {
                return default;
            }

            return new CostumeSnapshot(
                itemType: source.ItemType.ToView(),
                itemSubType: source.ItemSubType.ToView(),
                id: source.Id,
                grade: source.Grade,
                elementalType: source.ElementalType.ToView(),
                nonFungibleId: source.ItemId,
                tradableId: string.Empty,
                equipped: source.equipped,
                requiredBlockIndex: source.RequiredBlockIndex);
        }

        public static ConsumableSnapshot ToConsumableSnapshot(this Lib9cConsumable source)
        {
            if (source is null)
            {
                return default;
            }

            var stats = ProjectDecimalStats(source.Stats);
            var skills = ProjectSkills(source.Skills);

            return new ConsumableSnapshot(
                itemType: source.ItemType.ToView(),
                itemSubType: source.ItemSubType.ToView(),
                id: source.Id,
                grade: source.Grade,
                elementalType: source.ElementalType.ToView(),
                nonFungibleId: source.ItemId,
                tradableId: string.Empty,
                requiredBlockIndex: source.RequiredBlockIndex,
                stats: stats,
                skills: skills);
        }

        public static MaterialSnapshot ToMaterialSnapshot(this Lib9cMaterial source)
        {
            if (source is null)
            {
                return default;
            }

            // Only TradableMaterial carries a trade affinity; plain Material is pure-fungible
            // inventory and has no on-chain trade key. The hex-encoded HashDigest<SHA256> is
            // lib9c's <c>TradableMaterial.ItemId</c>, shared with the base Material but only
            // surfaced here for the TradableMaterial case so UI callers can't accidentally key
            // off a plain-material hash as if it were a tradable id.
            var tradableId = source is Lib9cTradableMaterial tradable
                ? tradable.ItemId.ToString()
                : string.Empty;

            return new MaterialSnapshot(
                itemType: source.ItemType.ToView(),
                itemSubType: source.ItemSubType.ToView(),
                id: source.Id,
                grade: source.Grade,
                elementalType: source.ElementalType.ToView(),
                tradableId: tradableId);
        }

        /// <summary>
        /// Polymorphic dispatcher. Returns the narrow per-subtype snapshot boxed as the common
        /// <see cref="IItemSnapshot"/> interface. Callers can <c>switch</c> on the runtime type
        /// to reach into Equipment/Costume/Consumable/Material-specific fields, or stay on
        /// <see cref="IItemSnapshot"/> when only the shared fields matter.
        /// </summary>
        /// <remarks>
        /// Order matches lib9c's subtype specificity: Equipment before ItemUsable, Consumable
        /// before ItemUsable, TradableMaterial before Material (since TradableMaterial IS-A
        /// Material but should still route to the Material mapper — the mapper reads the
        /// <c>ItemId</c> hash either way).
        /// </remarks>
        public static IItemSnapshot ToPolySnapshot(this Lib9cItemBase item)
        {
            switch (item)
            {
                case null:
                    return default(ItemSnapshot);
                case Lib9cEquipment equipment:
                    return equipment.ToEquipmentSnapshot();
                case Lib9cCostume costume:
                    return costume.ToCostumeSnapshot();
                case Lib9cConsumable consumable:
                    return consumable.ToConsumableSnapshot();
                case Lib9cMaterial material:
                    // TradableMaterial extends Material; the Material mapper already hex-encodes
                    // the shared ItemId hash so no extra branch is needed for now.
                    return material.ToMaterialSnapshot();
                default:
                    return item.ToSnapshot();
            }
        }

        private static Guid? ResolveNonFungibleId(Lib9cItemBase item)
        {
            switch (item)
            {
                case Lib9cEquipment equipment:
                    return equipment.ItemId;
                case Lib9cCostume costume:
                    return costume.ItemId;
                case Lib9cConsumable consumable:
                    return consumable.ItemId;
                default:
                    return null;
            }
        }

        private static string ResolveTradableIdHex(Lib9cItemBase item)
        {
            // Only TradableMaterial carries a trade affinity; plain Material is pure fungible
            // inventory. See ToMaterialSnapshot for the same rule.
            if (item is Lib9cTradableMaterial tradable)
            {
                return tradable.ItemId.ToString();
            }

            return string.Empty;
        }

        private static IReadOnlyList<SkillSnapshot> ProjectSkills(
            IReadOnlyList<Nekoyume.Model.Skill.Skill> source)
        {
            if (source is null || source.Count == 0)
            {
                return Array.Empty<SkillSnapshot>();
            }

            var result = new List<SkillSnapshot>(source.Count);
            foreach (var skill in source)
            {
                result.Add(skill.ToSnapshot());
            }

            return result;
        }

        private static IReadOnlyList<SkillSnapshot> ProjectBuffSkills(
            IReadOnlyList<Nekoyume.Model.Skill.BuffSkill> source)
        {
            if (source is null || source.Count == 0)
            {
                return Array.Empty<SkillSnapshot>();
            }

            var result = new List<SkillSnapshot>(source.Count);
            foreach (var skill in source)
            {
                // BuffSkill inherits Skill; SkillSnapshotMapper.ToSnapshot takes the base type.
                result.Add(skill.ToSnapshot());
            }

            return result;
        }

        private static IReadOnlyList<StatView> ProjectDecimalStats(IReadOnlyList<Lib9cDecimalStat> source)
        {
            if (source is null || source.Count == 0)
            {
                return Array.Empty<StatView>();
            }

            var result = new List<StatView>(source.Count);
            foreach (var stat in source)
            {
                result.Add(stat.ToView());
            }

            return result;
        }
    }
}

#endif
