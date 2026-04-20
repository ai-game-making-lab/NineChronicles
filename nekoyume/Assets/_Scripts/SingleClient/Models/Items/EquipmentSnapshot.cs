#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Generic;
using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.SingleClient.Models.Skills;
using Nekoyume.SingleClient.Models.Stats;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Immutable snapshot of a lib9c <c>Equipment</c>. Extends the <see cref="ItemSnapshot"/>
    /// base with the fields the Inventory / Shop / Grind / Enhancement UI reads off
    /// <c>ItemUsable</c>/<c>Equipment</c>: enhancement level, the primary <c>DecimalStat</c>,
    /// the attached skill/buff lists, and the <c>StatsMap</c> rollup used by tooltips.
    /// </summary>
    public readonly struct EquipmentSnapshot : IItemSnapshot, IEquatable<EquipmentSnapshot>
    {
        public ItemType ItemType { get; }
        public ItemSubType ItemSubType { get; }
        public int Id { get; }
        public int Grade { get; }
        public ElementalType ElementalType { get; }
        public Guid? NonFungibleId { get; }
        public string TradableId { get; }

        public int Level { get; }
        public StatView Stat { get; }
        public IReadOnlyList<SkillSnapshot> Skills { get; }
        public IReadOnlyList<SkillSnapshot> BuffSkills { get; }

        /// <summary>
        /// Rollup view of <c>Equipment.StatsMap.GetDecimalStats(true)</c>. Tooltips use this to
        /// render primary + additional stat rows together.
        /// </summary>
        public IReadOnlyList<StatView> StatsMap { get; }

        public int SetId { get; }
        public bool Equipped { get; }
        public bool MadeWithMimisbrunnrRecipe { get; }

        /// <summary>
        /// <see langword="null"/> when the underlying lib9c instance had no block-gate (e.g. a
        /// sheet-only projection). Concrete equipment always exposes a non-null value.
        /// </summary>
        public long? RequiredBlockIndex { get; }

        /// <summary>
        /// lib9c exposes <c>optionCountFromCombination</c> as a plain <see cref="int"/> field
        /// defaulting to zero. Kept nullable here so UI call sites that distinguish
        /// "never combined" vs "combined with zero options" can drop an overload when they
        /// migrate; mapper populates it unconditionally.
        /// </summary>
        public int? OptionCountFromCombination { get; }

        /// <summary>
        /// Mirrors lib9c <c>Equipment.IconId</c>. Diverges from <see cref="Id"/> for
        /// custom-crafted equipment, so icon resolution MUST prefer this field over
        /// <see cref="Id"/>. For vanilla equipment, lib9c initializes <c>IconId</c> to
        /// <c>data.Id</c>, so this equals <see cref="Id"/> by default.
        /// </summary>
        public int IconId { get; }

        /// <summary>
        /// Mirrors lib9c <c>Equipment.ByCustomCraft</c>. Drives the custom-craft decoration area
        /// in the inventory item view.
        /// </summary>
        public bool ByCustomCraft { get; }

        /// <summary>
        /// Mirrors lib9c <c>Equipment.CraftWithRandom</c>. Indicates random options were rolled
        /// during custom-craft combination.
        /// </summary>
        public bool CraftWithRandom { get; }

        /// <summary>
        /// Mirrors lib9c <c>Equipment.HasRandomOnlyIcon</c>. When <see langword="true"/>, the
        /// random-only icon variant should be used regardless of <see cref="IconId"/>.
        /// </summary>
        public bool HasRandomOnlyIcon { get; }

        /// <summary>
        /// Mirrors lib9c <c>Equipment.UniqueStatType</c> (the primary <c>Stat.StatType</c>).
        /// Exposed alongside <see cref="Stat"/> so option views can read the unique stat without
        /// touching the <see cref="StatView"/> struct.
        /// </summary>
        public StatType UniqueStatType { get; }

        /// <summary>
        /// Mirrors lib9c <c>Equipment.Exp</c>. Experience accumulated via enhancement. Used by
        /// Enhancement UI together with <c>GetRealExp</c> to project enhancement targets. Equals
        /// zero for vanilla equipment whose Exp has not been set on-chain.
        /// </summary>
        public long Exp { get; }

        public EquipmentSnapshot(
            ItemType itemType,
            ItemSubType itemSubType,
            int id,
            int grade,
            ElementalType elementalType,
            Guid? nonFungibleId,
            string tradableId,
            int level,
            StatView stat,
            IReadOnlyList<SkillSnapshot> skills,
            IReadOnlyList<SkillSnapshot> buffSkills,
            IReadOnlyList<StatView> statsMap,
            int setId,
            bool equipped,
            bool madeWithMimisbrunnrRecipe,
            long? requiredBlockIndex,
            int? optionCountFromCombination,
            int iconId,
            bool byCustomCraft,
            bool craftWithRandom,
            bool hasRandomOnlyIcon,
            StatType uniqueStatType,
            long exp = 0L)
        {
            ItemType = itemType;
            ItemSubType = itemSubType;
            Id = id;
            Grade = grade;
            ElementalType = elementalType;
            NonFungibleId = nonFungibleId;
            TradableId = tradableId ?? string.Empty;
            Level = level;
            Stat = stat;
            Skills = skills ?? Array.Empty<SkillSnapshot>();
            BuffSkills = buffSkills ?? Array.Empty<SkillSnapshot>();
            StatsMap = statsMap ?? Array.Empty<StatView>();
            SetId = setId;
            Equipped = equipped;
            MadeWithMimisbrunnrRecipe = madeWithMimisbrunnrRecipe;
            RequiredBlockIndex = requiredBlockIndex;
            OptionCountFromCombination = optionCountFromCombination;
            IconId = iconId;
            ByCustomCraft = byCustomCraft;
            CraftWithRandom = craftWithRandom;
            HasRandomOnlyIcon = hasRandomOnlyIcon;
            UniqueStatType = uniqueStatType;
            Exp = exp;
        }

        public bool Equals(EquipmentSnapshot other) =>
            Id == other.Id &&
            NonFungibleId == other.NonFungibleId &&
            Level == other.Level;

        public override bool Equals(object obj) => obj is EquipmentSnapshot other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Id;
                hash = (hash * 397) ^ (NonFungibleId?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ Level;
                return hash;
            }
        }

        public static bool operator ==(EquipmentSnapshot left, EquipmentSnapshot right) => left.Equals(right);
        public static bool operator !=(EquipmentSnapshot left, EquipmentSnapshot right) => !left.Equals(right);
    }
}

#endif
