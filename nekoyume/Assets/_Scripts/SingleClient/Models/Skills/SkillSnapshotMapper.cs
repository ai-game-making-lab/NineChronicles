#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.SingleClient.Models.Buffs;
using Nekoyume.SingleClient.Models.Elemental;
using Lib9cSkill = Nekoyume.Model.Skill.Skill;

namespace Nekoyume.SingleClient.Models.Skills
{
    /// <summary>
    /// Projects the lib9c <see cref="Lib9cSkill"/> runtime object into <see cref="SkillSnapshot"/>.
    /// Kept as an extension-style helper so migrating call sites stays a single `.ToSnapshot()`
    /// insertion at each UI boundary.
    /// </summary>
    public static class SkillSnapshotMapper
    {
        public static SkillSnapshot ToSnapshot(this Lib9cSkill source)
        {
            if (source is null)
            {
                return default;
            }

            var row = source.SkillRow;
            return new SkillSnapshot(
                id: row.Id,
                skillType: (SkillType)(int)row.SkillType,
                skillCategory: (SkillCategory)(int)row.SkillCategory,
                skillTargetType: (SkillTargetType)(int)row.SkillTargetType,
                elementalType: row.ElementalType.ToView(),
                hitCount: row.HitCount,
                cooldown: row.Cooldown,
                power: source.Power,
                chance: source.Chance,
                statPowerRatio: source.StatPowerRatio,
                referencedStatType: BuffViewMapper.MapStatType(source.ReferencedStatType));
        }
    }
}

#endif
