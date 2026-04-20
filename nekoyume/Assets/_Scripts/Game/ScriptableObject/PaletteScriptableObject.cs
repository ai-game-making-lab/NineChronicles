#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.EnumType;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Nekoyume
{
    [CreateAssetMenu(fileName = "UI_Palette", menuName = "Scriptable Object/Palette",
        order = int.MaxValue)]
    public class PaletteScriptableObject : ScriptableObjectIncludeEnum<ColorType>
    {
        [Serializable]
        public class ColorInfo
        {
            public ColorType colorType;
            public Color Color;
        }

        public List<ColorInfo> Palette;
    }
}

#endif
