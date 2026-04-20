#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.UI;
using UnityEngine;

namespace Nekoyume
{
    [CreateAssetMenu(fileName = "TutorialPreset", menuName = "Scriptable Object/Tutorial/TutorialPreset", order = int.MaxValue)]
    public class TutorialPresetScriptableObject : ScriptableObject
    {
        public TutorialPreset tutorialPreset;

        public TextAsset json;
    }
}

#endif
