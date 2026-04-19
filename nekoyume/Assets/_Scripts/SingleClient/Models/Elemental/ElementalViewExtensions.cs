using Nekoyume.EnumType;
using Nekoyume.Game.Controller;
using Nekoyume.L10n;
using UnityEngine;

namespace Nekoyume.SingleClient.Models.Elemental
{
    /// <summary>
    /// Client-owned localization + palette lookups mirroring the lib9c-typed versions in
    /// <c>LocalizationExtensions</c>. Keeps the same L10n keys (<c>ELEMENTAL_TYPE_*</c>) and
    /// palette slots (<c>ColorType.TextElement0*</c>) so migrated UI sites render identically to
    /// the lib9c-typed callers they replace.
    /// </summary>
    public static class ElementalViewExtensions
    {
        public static string GetLocalizedString(this ElementalType value)
        {
            return L10nManager.Localize($"ELEMENTAL_TYPE_{value.ToString().ToUpper()}");
        }

        public static Color GetElementalTypeColor(this ElementalType elementalType)
        {
            return elementalType switch
            {
                ElementalType.Normal => Palette.GetColor(ColorType.TextElement00),
                ElementalType.Fire => Palette.GetColor(ColorType.TextElement01),
                ElementalType.Land => Palette.GetColor(ColorType.TextElement02),
                ElementalType.Water => Palette.GetColor(ColorType.TextElement04),
                ElementalType.Wind => Palette.GetColor(ColorType.TextElement05),
                _ => Color.white
            };
        }
    }
}
