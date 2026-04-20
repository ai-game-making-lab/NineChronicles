#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿using Nekoyume.UI.Module;
using UnityEngine;

namespace Nekoyume.UI.Scroller
{
    public class EnhancementInventoryCell : GridCell<Model.EnhancementInventoryItem,
        EnhancementInventoryScroll.ContextModel>
    {
        [SerializeField]
        private EquipmentInventoryItemView view;

        public override void UpdateContent(Model.EnhancementInventoryItem viewModel)
        {
            view.Set(viewModel, Context);
            Context.CellDictionary[Index] = this;
        }
    }
}

#endif
