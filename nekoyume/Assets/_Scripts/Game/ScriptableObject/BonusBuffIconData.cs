#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.Model.Skill;
using System;
using Nekoyume.Editor;
using UnityEngine;

namespace Nekoyume.Game.ScriptableObject
{
    [Serializable]
    public class BonusBuffIconData
    {
        [SerializeField]
        [EnumToString(typeof(SkillCategory))]
        private string skillCategory;

        [SerializeField]
        private Sprite iconSprite;

        public SkillCategory SkillCategory =>
            Enum.TryParse(typeof(SkillCategory), skillCategory, true, out var skillCategoryEnum)
                ? (SkillCategory)skillCategoryEnum
                : SkillCategory.Buff;

        public Sprite IconSprite => iconSprite;
    }
}

#endif
