#if LIB9C_RESTORED // stubbed out after lib9c deletion
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI
{
    public abstract class VerticalTooltipWidget<T> : TooltipWidget<T> where T : Model.Tooltip
    {
        public VerticalLayoutGroup verticalLayoutGroup;

        protected override void SubscribeTarget(RectTransform target)
        {
            LayoutRebuild();
            base.SubscribeTarget(target);
        }

        protected void LayoutRebuild()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)verticalLayoutGroup.transform);
        }
    }
}

#endif
