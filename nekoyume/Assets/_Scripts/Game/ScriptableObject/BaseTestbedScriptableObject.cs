#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿#if LIB9C_TOOLS || LIB9C_DEV_EXTENSIONS
using Lib9c.DevExtensions.Model;

namespace Nekoyume.Game.ScriptableObject
{
    public class BaseTestbedScriptableObject<T> : UnityEngine.ScriptableObject where T : BaseTestbedModel
    {
        public T Data;
    }
}
#endif

#endif
