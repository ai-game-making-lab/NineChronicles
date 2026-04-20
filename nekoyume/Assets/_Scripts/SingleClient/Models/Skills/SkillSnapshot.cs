#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.SingleClient.Models.Stats;

namespace Nekoyume.SingleClient.Models.Skills
{
    /// <summary>
    /// Immutable snapshot of a live skill, shaped for the UI layer. Captures only the fields the
    /// tooltip/row views actually read so the UI can stop importing <c>Nekoyume.Model.Skill.Skill</c>
    /// and its <c>SkillSheet.Row</c>.
    /// </summary>
    public readonly struct SkillSnapshot
    {
        public int Id { get; }
        public SkillType SkillType { get; }
        public SkillCategory SkillCategory { get; }
        public SkillTargetType SkillTargetType { get; }
        public ElementalType ElementalType { get; }
        public int HitCount { get; }
        public int Cooldown { get; }
        public long Power { get; }
        public int Chance { get; }
        public int StatPowerRatio { get; }
        public StatType ReferencedStatType { get; }

        public SkillSnapshot(
            int id,
            SkillType skillType,
            SkillCategory skillCategory,
            SkillTargetType skillTargetType,
            ElementalType elementalType,
            int hitCount,
            int cooldown,
            long power,
            int chance,
            int statPowerRatio,
            StatType referencedStatType)
        {
            Id = id;
            SkillType = skillType;
            SkillCategory = skillCategory;
            SkillTargetType = skillTargetType;
            ElementalType = elementalType;
            HitCount = hitCount;
            Cooldown = cooldown;
            Power = power;
            Chance = chance;
            StatPowerRatio = statPowerRatio;
            ReferencedStatType = referencedStatType;
        }

        public bool IsBuffSkill =>
            SkillType == SkillType.Buff ||
            SkillType == SkillType.Debuff;
    }
}

#endif
