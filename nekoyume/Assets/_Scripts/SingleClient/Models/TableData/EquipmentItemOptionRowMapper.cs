#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.SingleClient.Models.Buffs;
using Lib9cEquipmentItemOptionSheet = Nekoyume.TableData.EquipmentItemOptionSheet;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Projects a lib9c <see cref="Lib9cEquipmentItemOptionSheet.Row"/> into the client-owned
    /// <see cref="EquipmentItemOptionRowView"/>.
    /// </summary>
    public static class EquipmentItemOptionRowMapper
    {
        public static EquipmentItemOptionRowView ToView(this Lib9cEquipmentItemOptionSheet.Row source)
        {
            if (source is null)
            {
                return default;
            }

            return new EquipmentItemOptionRowView(
                id: source.Id,
                statType: BuffViewMapper.MapStatType(source.StatType),
                statMin: source.StatMin,
                statMax: source.StatMax,
                skillId: source.SkillId,
                skillDamageMin: source.SkillDamageMin,
                skillDamageMax: source.SkillDamageMax,
                skillChanceMin: source.SkillChanceMin,
                skillChanceMax: source.SkillChanceMax,
                statPowerRatioMin: source.StatDamageRatioMin,
                statPowerRatioMax: source.StatDamageRatioMax,
                referencedStatType: BuffViewMapper.MapStatType(source.ReferencedStatType));
        }
    }
}

#endif
