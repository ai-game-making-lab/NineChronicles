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

            SkillSheetRowView? skill = null;
            if (skillRow != null)
            {
                skill = skillRow.ToView();
            }

            return source.ToView(skill);
        }

        /// <summary>
        /// Overload accepting an already-projected <see cref="SkillSheetRowView"/> so migrated
        /// call sites never need to hold on to a lib9c <see cref="Lib9cSkillSheet.Row"/> just
        /// to satisfy the mapper signature. Passing <see langword="null"/> is equivalent to
        /// <see cref="ToView(Lib9cRuneOptionInfo, Lib9cSkillSheet.Row)"/> with no companion
        /// skill.
        /// </summary>
        public static RuneOptionInfoView ToView(
            this Lib9cRuneOptionInfo source,
            SkillSheetRowView? skill)
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
