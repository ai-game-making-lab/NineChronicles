#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI
{
    public abstract class NewVerticalTooltipWidget : NewTooltipWidget
    {
        public VerticalLayoutGroup verticalLayoutGroup;

        protected override void UpdateAnchoredPosition(RectTransform target)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)verticalLayoutGroup.transform);
            base.UpdateAnchoredPosition(target);
        }
    }
}

#endif
