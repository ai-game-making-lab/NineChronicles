using TMPro;
using UnityEngine;
using Nekoyume.Game;
using Nekoyume.L10n;
using System.Linq;
using UnityEngine.UI;
using Nekoyume.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using Nekoyume.SingleClient.Models.Buffs;
using Nekoyume.SingleClient.Models.Skills;
using Nekoyume.SingleClient.Models.TableData;
// Client-owned enums/types the tooltip reads from row views. lib9c variants
// (Nekoyume.Model.Skill / Nekoyume.Model.Stat / Nekoyume.TableData.*) are no longer
// part of the public surface here; a few internal lookups still hit lib9c sheets via
// TableSheets.Instance while the remaining sheet mirrors land in follow-up slices.
using ClientStatType = Nekoyume.SingleClient.Models.Stats.StatType;
using ClientSkillType = Nekoyume.SingleClient.Models.Skills.SkillType;
using ClientSkillCategory = Nekoyume.SingleClient.Models.Skills.SkillCategory;

namespace Nekoyume.UI.Module.Common
{
    public class SkillPositionTooltip : PositionTooltip
    {
        private struct OptionDigest
        {
            public int SkillId;
            public int ChanceMin;
            public int ChanceMax;
            public long PowerMin;
            public long PowerMax;
            public int StatPowerRatioMin;
            public int StatPowerRatioMax;
            public ClientStatType ReferencedStatType;
        }

        [SerializeField]
        protected TextMeshProUGUI cooldownText;

        [SerializeField]
        protected GameObject buffObject;

        [SerializeField]
        protected GameObject debuffObject;

        [SerializeField]
        protected Image buffIconImage;

        [SerializeField]
        protected TextMeshProUGUI buffStatTypeText;

        [SerializeField]
        protected Image debuffIconImage;

        [SerializeField]
        protected TextMeshProUGUI debuffStatTypeText;

        private const string VariableColorTag = "<color=#f5e3c0>";

        public void Show(SkillSheetRowView skillRow, EquipmentItemOptionRowView optionRow)
        {
            titleText.text = skillRow.GetLocalizedName();

            var key = $"SKILL_DESCRIPTION_{skillRow.Id}";

            if (L10nManager.ContainsKey(key))
            {
                SetSkillDescription(key,
                    skillRow,
                    optionRow.SkillDamageMin,
                    optionRow.SkillDamageMax,
                    (int)optionRow.StatPowerRatioMin,
                    (int)optionRow.StatPowerRatioMax,
                    optionRow.SkillChanceMin,
                    TableSheets.Instance);
            }
            else
            {
                var digest = new OptionDigest()
                {
                    SkillId = optionRow.SkillId,
                    ChanceMin = optionRow.SkillChanceMin,
                    ChanceMax = optionRow.SkillChanceMax,
                    PowerMin = optionRow.SkillDamageMin,
                    PowerMax = optionRow.SkillDamageMax,
                    StatPowerRatioMin = (int)optionRow.StatPowerRatioMin,
                    StatPowerRatioMax = (int)optionRow.StatPowerRatioMax,
                    ReferencedStatType = optionRow.ReferencedStatType
                };

                switch (skillRow.SkillType)
                {
                    case ClientSkillType.Attack:
                        SetAttackSkillDescription(digest);
                        break;
                    case ClientSkillType.Heal:
                        SetHealDescription(digest);
                        break;
                    case ClientSkillType.Buff:
                        SetBuffDescription(digest, false);
                        break;
                    case ClientSkillType.Debuff:
                        SetBuffDescription(digest, true);
                        break;
                }
            }

            cooldownText.text = $"{L10nManager.Localize("UI_COOLDOWN")}: {skillRow.Cooldown}";
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Rune-option overload. <paramref name="runeValueString"/> is computed by the caller via
        /// <c>RuneFrontHelper.GetRuneValueString</c> because that helper still reads the lib9c
        /// <c>RuneOptionInfo.SkillValueType</c> field which intentionally is NOT mirrored on
        /// <see cref="RuneOptionInfoView"/> in this slice — hoisting the call keeps the tooltip
        /// free of lib9c imports until the helper itself is ported.
        /// </summary>
        public void Show(SkillSheetRowView skillRow, RuneOptionInfoView optionInfo, string runeValueString)
        {
            titleText.text = skillRow.GetLocalizedName();
            contentText.text = L10nManager.Localize($"SKILL_DESCRIPTION_{skillRow.Id}",
                optionInfo.SkillChance, optionInfo.BuffDuration, runeValueString);
            cooldownText.text = $"{L10nManager.Localize("UI_COOLDOWN")} : {optionInfo.SkillCooldown}";
            buffObject.SetActive(false);
            debuffObject.SetActive(false);
            gameObject.SetActive(true);
        }

        private void SetSkillDescription(string key, SkillSheetRowView skillRow, long skillPowerMin, long skillPowerMax, int skillRatioMin, int skillRatioMax, int skillChance, TableSheets tableSheets)
        {
            var sheets = TableSheets.Instance;
            var arg = new List<string>();
            var debuffLimitDisc = string.Empty;
            if (sheets.SkillBuffSheet.TryGetValue(skillRow.Id, out var skillBuffRow))
            {
                var buffList = skillBuffRow.BuffIds;
                if (buffList.Count == 2)
                {
                    var buff = sheets.StatBuffSheet[buffList[0]].ToView();
                    var deBuff = sheets.StatBuffSheet[buffList[1]].ToView();
                    arg.Add(skillChance.ToString());
                    arg.Add(buff.Duration.ToString());
                    arg.Add(((long)buff.Value + skillPowerMin).ToString());
                    arg.Add(deBuff.Duration.ToString());
                    arg.Add(deBuff.Value.ToString());

                    var buffIcon = BuffHelper.GetStatBuffIcon(BuffViewMapper.MapStatType(buff.StatType), false);
                    buffIconImage.overrideSprite = buffIcon;
                    buffStatTypeText.text = BuffViewMapper.MapStatType(buff.StatType).GetAcronym();

                    var deBuffIcon = BuffHelper.GetStatBuffIcon(BuffViewMapper.MapStatType(deBuff.StatType), true);
                    debuffIconImage.overrideSprite = deBuffIcon;
                    debuffStatTypeText.text = BuffViewMapper.MapStatType(deBuff.StatType).GetAcronym();

                    if (sheets.BuffLimitSheet.TryGetValue(deBuff.GroupId, out var debuffLimitRowSource))
                    {
                        var debuffLimitRow = debuffLimitRowSource.ToView();
                        debuffLimitDisc = L10nManager.Localize("SKILL_DESCRIPTION_STATDEBUFF_LIMIT", debuffStatTypeText.text, debuffLimitRow.DurationLimit);
                    }

                    buffObject.SetActive(true);
                    debuffObject.SetActive(true);
                }
                else if (buffList.Count == 1)
                {
                    var buff = sheets.StatBuffSheet[buffList[0]].ToView();
                    arg.Add(skillChance.ToString());
                    arg.Add(buff.Duration.ToString());
                    arg.Add(((long)buff.Value + skillPowerMin).ToString());
                    buffObject.SetActive(false);
                    debuffObject.SetActive(false);
                    if (skillRow.SkillType == ClientSkillType.Buff)
                    {
                        var buffIcon = BuffHelper.GetStatBuffIcon(BuffViewMapper.MapStatType(buff.StatType), false);
                        buffIconImage.overrideSprite = buffIcon;
                        buffStatTypeText.text = BuffViewMapper.MapStatType(buff.StatType).GetAcronym();
                        buffObject.SetActive(true);
                    }

                    if (skillRow.SkillType == ClientSkillType.Debuff)
                    {
                        var deBuffIcon = BuffHelper.GetStatBuffIcon(BuffViewMapper.MapStatType(buff.StatType), true);
                        debuffIconImage.overrideSprite = deBuffIcon;
                        debuffStatTypeText.text = BuffViewMapper.MapStatType(buff.StatType).GetAcronym();
                        debuffObject.SetActive(true);

                        if (sheets.BuffLimitSheet.TryGetValue(buff.GroupId, out var debuffLimitRowSource))
                        {
                            var debuffLimitRow = debuffLimitRowSource.ToView();
                            debuffLimitDisc = L10nManager.Localize("SKILL_DESCRIPTION_STATDEBUFF_LIMIT", debuffStatTypeText.text, debuffLimitRow.DurationLimit);
                        }
                    }
                }
                else
                {
                    buffObject.SetActive(false);
                    debuffObject.SetActive(false);
                }
            }
            else if (sheets.SkillActionBuffSheet.TryGetValue(skillRow.Id, out var skillActionBuffRow))
            {
                arg.Add(skillChance.ToString());
                arg.Add(skillRow.Cooldown.ToString());
                arg.Add(skillPowerMin.ToString());
                var buffIcon = BuffHelper.GetBuffOverrideIcon(skillActionBuffRow.BuffIds.First(), tableSheets);
                buffIconImage.overrideSprite = buffIcon;
                buffStatTypeText.text = skillRow.SkillCategory.ToString();
                debuffObject.SetActive(false);
            }
            else if (skillRow.SkillCategory == ClientSkillCategory.ShatterStrike)
            {
                var skilleffect = string.Empty;
                var percentageFormat = new NumberFormatInfo { PercentPositivePattern = 1, PercentNegativePattern = 1 };
                if (skillRatioMin == skillRatioMax)
                {
                    skilleffect = $"{(skillRatioMin / 10000m).ToString("P2", percentageFormat)}";
                }
                else
                {
                    skilleffect = $"{(skillRatioMin / 10000m).ToString("P2", percentageFormat)}~{(skillRatioMax / 10000m).ToString("P2", percentageFormat)}";
                }

                arg.Add(skillChance.ToString());
                arg.Add(skillRow.Cooldown.ToString());
                arg.Add(skilleffect);
                buffObject.SetActive(false);
                debuffObject.SetActive(false);
            }
            else
            {
                var skilleffect = string.Empty;
                if (skillPowerMin == skillPowerMax)
                {
                    skilleffect = skillPowerMin.ToString();
                }
                else
                {
                    skilleffect = $"{skillPowerMin}~{skillPowerMax}";
                }

                arg.Add(skillChance.ToString());
                arg.Add(skillRow.Cooldown.ToString());
                arg.Add(skilleffect);
                buffObject.SetActive(false);
                debuffObject.SetActive(false);
            }

            contentText.text = L10nManager.Localize(key, arg.ToArray()) + debuffLimitDisc;
        }

        /// <summary>
        /// Live-skill overload. Takes a <see cref="SkillSnapshot"/> so the tooltip never imports
        /// <c>Nekoyume.Model.Skill.Skill</c>; the callsite projects via <c>.ToSnapshot()</c>.
        /// </summary>
        public void Show(SkillSnapshot skill)
        {
            // Resolve the sheet row once — all downstream private helpers read SkillSheetRowView
            // fields (Id, SkillType, Cooldown, SkillCategory) and the snapshot already carries
            // Power/Chance/StatPowerRatio/ReferencedStatType as client-typed values.
            var skillRow = TableSheets.Instance.SkillSheet[skill.Id].ToView();
            Show(skillRow,
                skill.Chance, skill.Chance,
                skill.Power, skill.Power,
                skill.StatPowerRatio, skill.StatPowerRatio,
                skill.ReferencedStatType);
        }

        public void Show(
            SkillSheetRowView skillRow,
            int chanceMin,
            int chanceMax,
            long powerMin,
            long powerMax,
            int ratioMin,
            int ratioMax,
            ClientStatType referencedStatType)
        {
            titleText.text = skillRow.GetLocalizedName();

            var key = $"SKILL_DESCRIPTION_{skillRow.Id}";
            if (L10nManager.ContainsKey(key))
            {
                SetSkillDescription(key,
                    skillRow,
                    powerMin,
                    powerMax,
                    ratioMin,
                    ratioMax,
                    chanceMin,
                    TableSheets.Instance);
            }
            else
            {
                var digest = new OptionDigest()
                {
                    SkillId = skillRow.Id,
                    ChanceMin = chanceMin,
                    ChanceMax = chanceMax,
                    PowerMin = powerMin,
                    PowerMax = powerMax,
                    StatPowerRatioMin = ratioMin,
                    StatPowerRatioMax = ratioMax,
                    ReferencedStatType = referencedStatType
                };

                switch (skillRow.SkillType)
                {
                    case ClientSkillType.Attack:
                        SetAttackSkillDescription(digest);
                        break;
                    case ClientSkillType.Heal:
                        SetHealDescription(digest);
                        break;
                    case ClientSkillType.Buff:
                        SetBuffDescription(digest, false);
                        break;
                    case ClientSkillType.Debuff:
                        SetBuffDescription(digest, true);
                        break;
                }
            }

            cooldownText.text = $"{L10nManager.Localize("UI_COOLDOWN")}: {skillRow.Cooldown}";
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void SetAttackSkillDescription(OptionDigest digest)
        {
            var row = TableSheets.Instance.SkillSheet[digest.SkillId].ToView();
            SetAttackSkillDescription(row, digest);
        }

        private void SetAttackSkillDescription(SkillSheetRowView row, OptionDigest digest)
        {
            contentText.text = GetDescription("SKILL_DESCRIPTION_ATTACK", row, digest);
            buffObject.SetActive(false);
            debuffObject.SetActive(false);
        }

        private void SetHealDescription(OptionDigest digest)
        {
            var row = TableSheets.Instance.SkillSheet[digest.SkillId].ToView();
            SetHealSkillDescription(row, digest);
        }

        private void SetHealSkillDescription(SkillSheetRowView row, OptionDigest digest)
        {
            contentText.text = GetDescription("SKILL_DESCRIPTION_HEAL", row, digest);
            buffObject.SetActive(false);
            debuffObject.SetActive(false);
        }

        private void SetBuffDescription(OptionDigest digest, bool isDebuff)
        {
            var skillRow = TableSheets.Instance.SkillSheet[digest.SkillId].ToView();
            SetBuffDescription(
                skillRow,
                digest,
                isDebuff);
        }

        private void SetBuffDescription(SkillSheetRowView skillRow, OptionDigest digest, bool isDebuff)
        {
            var sheets = TableSheets.Instance;
            var buffRow = sheets.StatBuffSheet[sheets.SkillBuffSheet[skillRow.Id].BuffIds.First()].ToView();
            var chanceText = digest.ChanceMin == digest.ChanceMax ? $"{VariableColorTag}{digest.ChanceMin}%</color>" : $"{VariableColorTag}{digest.ChanceMin}-{digest.ChanceMax}%</color>";
            var lib9cStatType = BuffViewMapper.MapStatType(buffRow.StatType);
            var statType = $"{VariableColorTag}{lib9cStatType}</color>";

            string desc;
            if (digest.PowerMin == digest.PowerMax &&
                digest.StatPowerRatioMin == digest.StatPowerRatioMax)
            {
                var str = EffectToStringLib9c(
                    skillRow.Id,
                    skillRow.SkillType,
                    digest.PowerMin,
                    digest.StatPowerRatioMin,
                    digest.ReferencedStatType);
                desc = $"{VariableColorTag}{str}</color>";
            }
            else
            {
                var strMin = EffectToStringLib9c(
                    skillRow.Id,
                    skillRow.SkillType,
                    digest.PowerMin,
                    digest.StatPowerRatioMin,
                    digest.ReferencedStatType);
                var strMax = EffectToStringLib9c(
                    skillRow.Id,
                    skillRow.SkillType,
                    digest.PowerMax,
                    digest.StatPowerRatioMax,
                    digest.ReferencedStatType);
                desc = $"{VariableColorTag}{strMin}-{strMax}</color>";
            }

            var value = $"{VariableColorTag}{desc}</color>";

            var icon = BuffHelper.GetStatBuffIcon(lib9cStatType, isDebuff);
            if (isDebuff)
            {
                debuffStatTypeText.text = lib9cStatType.GetAcronym();
                debuffIconImage.overrideSprite = icon;

                var debuffLimitDisc = string.Empty;
                if (sheets.BuffLimitSheet.TryGetValue(buffRow.GroupId, out var debuffLimitRowSource))
                {
                    var debuffLimitRow = debuffLimitRowSource.ToView();
                    debuffLimitDisc = L10nManager.Localize("SKILL_DESCRIPTION_STATDEBUFF_LIMIT", statType, debuffLimitRow.DurationLimit);
                }

                contentText.text = L10nManager.Localize("SKILL_DESCRIPTION_STATDEBUFF", chanceText, statType, value) + debuffLimitDisc;
            }
            else
            {
                buffStatTypeText.text = lib9cStatType.GetAcronym();
                buffIconImage.overrideSprite = icon;
                contentText.text = L10nManager.Localize("SKILL_DESCRIPTION_STATBUFF", chanceText, statType, value);
            }

            buffObject.SetActive(!isDebuff);
            debuffObject.SetActive(isDebuff);
        }

        private string GetDescription(string format, SkillSheetRowView row, OptionDigest digest)
        {
            var chanceText = digest.ChanceMin == digest.ChanceMax ? $"{VariableColorTag}{digest.ChanceMin}%</color>" : $"{VariableColorTag}{digest.ChanceMin}-{digest.ChanceMax}%</color>";
            string desc;
            if (digest.PowerMin == digest.PowerMax &&
                digest.StatPowerRatioMin == digest.StatPowerRatioMax)
            {
                var str = EffectToStringLib9c(
                    row.Id,
                    row.SkillType,
                    digest.PowerMin,
                    digest.StatPowerRatioMin,
                    digest.ReferencedStatType);
                desc = $"{VariableColorTag}{str}</color>";
            }
            else
            {
                var strMin = EffectToStringLib9c(
                    row.Id,
                    row.SkillType,
                    digest.PowerMin,
                    digest.StatPowerRatioMin,
                    digest.ReferencedStatType);
                var strMax = EffectToStringLib9c(
                    row.Id,
                    row.SkillType,
                    digest.PowerMax,
                    digest.StatPowerRatioMax,
                    digest.ReferencedStatType);
                desc = $"{VariableColorTag}{strMin}-{strMax}</color>";
            }

            return L10nManager.Localize(format, chanceText, desc);
        }

        /// <summary>
        /// Thin wrapper around <c>SkillExtensions.EffectToString</c> that hops client enums
        /// back to their lib9c ordinals. The lib9c helper still consumes <c>Nekoyume.Model.Skill.SkillType</c>
        /// + <c>Nekoyume.Model.Stat.StatType</c>, so until those helpers are ported the tooltip
        /// keeps this single re-cast boundary instead of scattering casts through every call.
        /// </summary>
        private static string EffectToStringLib9c(
            int skillId,
            ClientSkillType skillType,
            long power,
            int statPowerRatio,
            ClientStatType referencedStatType)
        {
            return SkillExtensions.EffectToString(
                skillId,
                (Nekoyume.Model.Skill.SkillType)(int)skillType,
                power,
                statPowerRatio,
                BuffViewMapper.MapStatType(referencedStatType));
        }
    }
}
