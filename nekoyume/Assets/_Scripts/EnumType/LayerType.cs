#if LIB9C_RESTORED // stubbed out after lib9c deletion
using UnityEngine;

namespace Nekoyume.EnumType
{
    public enum LayerType
    {
        Default,
        InGameBackground,
        InGameVFXBackground,
        Character,
        InGameVFXForeground,
        InGameForeground,
        UI,
        VFX
    }

    public static class LayerTypeExtensions
    {
        public static int ToLayerID(this LayerType layerType)
        {
            return LayerMask.NameToLayer(layerType.ToString());
        }

        public static string ToLayerName(this LayerType layerType)
        {
            return layerType.ToString();
        }
    }
}

#endif
