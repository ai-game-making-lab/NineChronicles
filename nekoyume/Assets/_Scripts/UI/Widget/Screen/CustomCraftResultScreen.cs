#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿using Nekoyume.Model.Item;
using Nekoyume.UI.Module;
using UnityEngine;

namespace Nekoyume.UI
{
    public class CustomCraftResultScreen : ScreenWidget
    {
        [SerializeField]
        private CustomCraftResultView view;

        public void Show(Equipment resultEquipment)
        {
            view.Show(resultEquipment);
            base.Show();
        }
    }
}

#endif
