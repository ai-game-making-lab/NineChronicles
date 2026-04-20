#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.SingleClient.Models.Buffs;
using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.SingleClient.Models.Items;
using Nekoyume.SingleClient.Models.Stats;
using Lib9cEquipmentItemSheet = Nekoyume.TableData.EquipmentItemSheet;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Projects a lib9c <see cref="Lib9cEquipmentItemSheet.Row"/> into the client-owned
    /// <see cref="EquipmentItemSheetRowView"/>. The equipment main stat is modeled as a
    /// <see cref="StatView"/> with <c>AdditionalValue = 0</c> since the sheet row only carries
    /// the base magnitude — the per-instance enhancement lives on <c>Equipment.StatsMap</c>.
    /// </summary>
    public static class EquipmentItemSheetRowMapper
    {
        public static EquipmentItemSheetRowView ToView(this Lib9cEquipmentItemSheet.Row source)
        {
            if (source is null)
            {
                return default;
            }

            var stat = new StatView(
                statType: BuffViewMapper.MapStatType(source.Stat.StatType),
                baseValue: source.Stat.BaseValue,
                additionalValue: 0m);

            return new EquipmentItemSheetRowView(
                id: source.Id,
                itemSubType: source.ItemSubType.ToView(),
                grade: source.Grade,
                elementalType: source.ElementalType.ToView(),
                setId: source.SetId,
                stat: stat,
                attackRange: source.AttackRange,
                spineResourcePath: source.SpineResourcePath,
                exp: source.Exp);
        }
    }
}

#endif
