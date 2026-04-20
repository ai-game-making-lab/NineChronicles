#if LIB9C_RESTORED // stubbed out after lib9c deletion
using UnityEditor;
using UnityEngine;

namespace Nekoyume.Game.Util
{
    [CanEditMultipleObjects][CustomEditor(typeof(PositionConstraintToScreen))]
    public class PositionConstraintToScreenEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var comp = (PositionConstraintToScreen)target;
            if (GUILayout.Button("Constraint To Screen"))
            {
                comp.Constraint();
            }
        }
    }
}

#endif
