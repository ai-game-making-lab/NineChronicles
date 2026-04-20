#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Generic;

namespace Nekoyume.UI.Model
{
    [Serializable]
    public class ShopEquipments
    {
        public List<ShopEquipment> shopEquipments;
    }

    [Serializable]
    public class ShopEquipment
    {
        public string orderId;
        public string tradableId;
        public long sellStartedBlockIndex;
        public long sellExpiredBlockIndex;
        public string sellerAgentAddress;
        public string sellerAvatarAddress;
        public decimal price;
        public long combatPoint;
        public int level;
        public int id;
        public int itemCount;
        public string itemSubType;

    }

    [Serializable]
    public class ShopResponse
    {
        public ShopEquipments shopQuery;
    }
}

#endif
