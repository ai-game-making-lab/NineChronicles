#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nekoyume
{
    [CreateAssetMenu(fileName = "VFX_Aura", menuName = "Scriptable Object/Aura",
        order = int.MaxValue)]
    public class AuraScriptableObject : ScriptableObject
    {
        public List<AuraData> data;
    }

    [Serializable]
    public class AuraData
    {
        public int id;
        public List<GameObject> prefab;
    }
}

#endif
