#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.SingleClient.Models.Skills;
using Lib9cSkillSheet = Nekoyume.TableData.SkillSheet;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Projects a lib9c <see cref="Lib9cSkillSheet.Row"/> into <see cref="SkillSheetRowView"/>.
    /// Kept extension-style so migrating callers only need to append <c>.ToView()</c> at the
    /// sheet-access boundary.
    /// </summary>
    public static class SkillSheetRowMapper
    {
        public static SkillSheetRowView ToView(this Lib9cSkillSheet.Row source)
        {
            if (source is null)
            {
                return default;
            }

            return new SkillSheetRowView(
                source: source,
                id: source.Id,
                elementalType: source.ElementalType.ToView(),
                skillType: (SkillType)(int)source.SkillType,
                skillCategory: (SkillCategory)(int)source.SkillCategory,
                skillTargetType: (SkillTargetType)(int)source.SkillTargetType,
                hitCount: source.HitCount,
                cooldown: source.Cooldown);
        }
    }
}

#endif
