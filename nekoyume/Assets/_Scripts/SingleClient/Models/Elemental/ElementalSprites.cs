using System;
using UnityEngine;

namespace Nekoyume.SingleClient.Models.Elemental
{
    /// <summary>
    /// Client-owned sprite lookup mirroring the lib9c-typed <c>ElementalTypeUIExtensions.GetSprite</c>
    /// so UI call sites can consume the client <see cref="ElementalType"/> without importing
    /// <c>Nekoyume.Model.Elemental</c>. Resource paths are kept identical to the lib9c-typed
    /// helper so the underlying icons are unchanged.
    /// </summary>
    public static class ElementalSprites
    {
        private const string FireIconResourcePath = "UI/Icons/ElementalType/icon_elemental_fire";
        private const string WaterIconResourcePath = "UI/Icons/ElementalType/icon_elemental_water";
        private const string LandIconResourcePath = "UI/Icons/ElementalType/icon_elemental_land";
        private const string WindIconResourcePath = "UI/Icons/ElementalType/icon_elemental_wind";
        private const string NormalIconResourcePath = "UI/Icons/ElementalType/icon_element_normal";

        public static Sprite GetSprite(this ElementalType type)
        {
            switch (type)
            {
                case ElementalType.Normal:
                    return Resources.Load<Sprite>(NormalIconResourcePath);
                case ElementalType.Fire:
                    return Resources.Load<Sprite>(FireIconResourcePath);
                case ElementalType.Water:
                    return Resources.Load<Sprite>(WaterIconResourcePath);
                case ElementalType.Land:
                    return Resources.Load<Sprite>(LandIconResourcePath);
                case ElementalType.Wind:
                    return Resources.Load<Sprite>(WindIconResourcePath);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
