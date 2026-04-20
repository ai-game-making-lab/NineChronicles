#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿using Nekoyume.UI.Model;
using UnityEngine;

namespace Nekoyume.UI.Scroller
{
    public class RuneStoneInventoryScroll : RectScroll<RuneStoneInventoryItem, RuneStoneInventoryScroll.ContextModel>
    {
        public class ContextModel : RectScrollDefaultContext
        {
        }

        [SerializeField]
        private RuneStoneInventoryCell cellTemplate = null;
    }
}

#endif
