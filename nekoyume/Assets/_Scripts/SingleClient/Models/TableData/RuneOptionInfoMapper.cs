using System.Collections.Generic;
using Nekoyume.SingleClient.Models.Buffs;
using Lib9cRuneOptionInfo = Nekoyume.TableData.RuneOptionSheet.Row.RuneOptionInfo;
using Lib9cSkillSheet = Nekoyume.TableData.SkillSheet;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Projects a lib9c <see cref="Lib9cRuneOptionInfo"/> (plus an optional companion
    /// <c>SkillSheet.Row</c>) into the client-owned <see cref="RuneOptionInfoView"/>.
    /// </summary>
    public static class RuneOptionInfoMapper
    {
        public static RuneOptionInfoView ToView(
            this Lib9cRuneOptionInfo source,
            Lib9cSkillSheet.Row skillRow = null)
        {
            if (source is null)
            {
                return default;
            }

            var stats = new List<RuneStatOption>(source.Stats?.Count ?? 0);
            if (source.Stats != null)
            {
                foreach (var (stat, _) in source.Stats)
                {
                    if (stat is null)
                    {
                        continue;
                    }

                    stats.Add(new RuneStatOption(
                        statType: BuffViewMapper.MapStatType(stat.StatType),
                        value: stat.BaseValue));
                }
            }

            SkillSheetRowView? skill = null;
            if (skillRow != null)
            {
                skill = skillRow.ToView();
            }

            return new RuneOptionInfoView(
                cp: (int)source.Cp,
                stats: stats,
                skillId: source.SkillId,
                skillCooldown: source.SkillCooldown,
                skillChance: source.SkillChance,
                skillValue: source.SkillValue,
                skillStatType: BuffViewMapper.MapStatType(source.SkillStatType),
                buffDuration: source.BuffDuration,
                skill: skill);
        }
    }
}
