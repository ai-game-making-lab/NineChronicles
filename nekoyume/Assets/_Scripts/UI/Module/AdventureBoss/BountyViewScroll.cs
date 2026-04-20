#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.UI.Scroller;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Nekoyume
{
    public class BountyItemData
    {
        public int Rank;
        public string Name;
        public int Count;
        public BigInteger Ncg;
        public float Bonus;
    }

    public class BountyViewScroll : RectScroll<BountyItemData, BountyViewScroll.ContextModel>
    {
        public class ContextModel : RectScrollDefaultContext
        {
        }
    }
}

#endif
