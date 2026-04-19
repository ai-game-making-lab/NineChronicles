using System.Collections.Generic;
using Nekoyume.SingleClient.Models.Stats;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Client-owned mirror of <c>Nekoyume.TableData.RuneOptionSheet.Row.RuneOptionInfo</c>.
    /// The lib9c record pairs a combat-power value with a stat-option list and an optional
    /// skill block (id + chance/cooldown/duration/magnitude/reference-type). This view flattens
    /// that into the subset the rune tooltip reads, plus an optional <see cref="Skill"/>
    /// mirror when callers pass the resolved <c>SkillSheet.Row</c> alongside the option info.
    /// Simulator-only fields (<c>StatReferenceType</c>, <c>SkillValueType</c>) are intentionally
    /// omitted — UI never branches on them.
    /// </summary>
    public readonly struct RuneOptionInfoView
    {
        public int Cp { get; }

        /// <summary>
        /// Ordered stat options. Each entry pairs the lib9c <c>DecimalStat</c>'s type with its
        /// (rounded) base value. <see cref="decimal"/> is preserved here because rune stat
        /// rows can carry fractional percentage magnitudes.
        /// </summary>
        public IReadOnlyList<RuneStatOption> Stats { get; }

        public int SkillId { get; }
        public int SkillCooldown { get; }
        public int SkillChance { get; }
        public decimal SkillValue { get; }
        public StatType SkillStatType { get; }
        public int BuffDuration { get; }

        /// <summary>
        /// Optional companion snapshot of the skill row the rune invokes. Callers that have
        /// the sheet handy should pass it so tooltip consumers avoid a second lookup; leaving
        /// it <see langword="null"/> (default) keeps the mapper usable from sites that only
        /// have the <c>RuneOptionInfo</c>.
        /// </summary>
        public SkillSheetRowView? Skill { get; }

        public RuneOptionInfoView(
            int cp,
            IReadOnlyList<RuneStatOption> stats,
            int skillId,
            int skillCooldown,
            int skillChance,
            decimal skillValue,
            StatType skillStatType,
            int buffDuration,
            SkillSheetRowView? skill)
        {
            Cp = cp;
            Stats = stats ?? System.Array.Empty<RuneStatOption>();
            SkillId = skillId;
            SkillCooldown = skillCooldown;
            SkillChance = skillChance;
            SkillValue = skillValue;
            SkillStatType = skillStatType;
            BuffDuration = buffDuration;
            Skill = skill;
        }

        public bool HasSkill => SkillId != 0;
    }

    /// <summary>
    /// Paired tuple (<see cref="StatType"/>, <see cref="decimal"/> magnitude) that makes up a
    /// single rune stat line. Declared as a struct instead of <c>ValueTuple</c> so consumers
    /// can refer to the fields by name without ambiguity.
    /// </summary>
    public readonly struct RuneStatOption
    {
        public StatType StatType { get; }
        public decimal Value { get; }

        public RuneStatOption(StatType statType, decimal value)
        {
            StatType = statType;
            Value = value;
        }
    }
}
