#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿using TMPro;
using UnityEngine;

namespace Nekoyume.UI.Scroller
{
    public class CustomCraftStatCell : RectCell<CustomCraftStatCell.Model, RectScrollDefaultContext>
    {
        public class Model
        {
            public string CompositionString;
            public string SubStatTotalString;
        }

        [SerializeField]
        private TextMeshProUGUI compositionText;

        [SerializeField]
        private TextMeshProUGUI subStatTotalText;

        public override void UpdateContent(Model itemData)
        {
            compositionText.SetText(itemData.CompositionString);
            subStatTotalText.SetText(itemData.SubStatTotalString);
        }
    }
}

#endif
