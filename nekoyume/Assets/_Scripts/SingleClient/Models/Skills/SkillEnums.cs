#if LIB9C_RESTORED // stubbed out after lib9c deletion
namespace Nekoyume.SingleClient.Models.Skills
{
    /// <summary>
    /// Client-owned mirror of <c>Nekoyume.Model.Skill.SkillType</c>. Enum ordering is kept
    /// identical to lib9c so direct (int) casts stay lossless during migration.
    /// </summary>
    public enum SkillType
    {
        Attack = 0,
        Heal = 1,
        Buff = 2,
        Debuff = 3,
    }

    /// <summary>
    /// Client-owned mirror of <c>Nekoyume.Model.Skill.SkillCategory</c>. Enum ordering and
    /// membership matches lib9c 1:1 for ordinal-compatible casts.
    /// </summary>
    public enum SkillCategory
    {
        NormalAttack = 0,
        BlowAttack = 1,
        DoubleAttack = 2,
        AreaAttack = 3,
        BuffRemovalAttack = 4,
        ShatterStrike = 5,
        Heal = 6,
        HPBuff = 7,
        AttackBuff = 8,
        DefenseBuff = 9,
        CriticalBuff = 10,
        HitBuff = 11,
        SpeedBuff = 12,
        DamageReductionBuff = 13,
        CriticalDamageBuff = 14,
        Buff = 15,
        Debuff = 16,
        TickDamage = 17,
        Focus = 18,
        Dispel = 19,
        FullBuffRemovalAttack = 20,
    }

    /// <summary>
    /// Client-owned mirror of <c>Nekoyume.Model.Skill.SkillTargetType</c>. The lib9c
    /// <c>SkillTargetTypeExtension.GetTarget</c> is intentionally NOT ported because it depends
    /// on the simulator's <c>CharacterBase</c>; UI callers never need target resolution.
    /// </summary>
    public enum SkillTargetType
    {
        Enemy = 0,
        Enemies = 1,
        Self = 2,
        Ally = 3,
    }
}

#endif
