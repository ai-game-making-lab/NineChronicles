#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Libplanet.Crypto;
using Libplanet.Types.Assets;
using Nekoyume.Model.Item;

namespace Nekoyume.SingleClient
{
    public sealed class ClientOrder
    {
        public const long ExpirationInterval = 36000;
        public const int TaxRate = 8;

        public ClientOrder(
            Guid orderId,
            Guid tradableId,
            long expiredBlockIndex,
            Address sellerAgentAddress,
            Address sellerAvatarAddress,
            ItemSubType itemSubType,
            FungibleAssetValue price,
            int itemCount)
        {
            OrderId = orderId;
            TradableId = tradableId;
            ExpiredBlockIndex = expiredBlockIndex;
            SellerAgentAddress = sellerAgentAddress;
            SellerAvatarAddress = sellerAvatarAddress;
            ItemSubType = itemSubType;
            Price = price;
            ItemCount = itemCount;
        }

        public Guid OrderId { get; }
        public Guid TradableId { get; }
        public long ExpiredBlockIndex { get; }
        public Address SellerAgentAddress { get; }
        public Address SellerAvatarAddress { get; }
        public ItemSubType ItemSubType { get; }
        public FungibleAssetValue Price { get; }
        public int ItemCount { get; }

        public FungibleAssetValue Tax => Price.DivRem(100, out _) * TaxRate;

        public FungibleAssetValue TaxedPrice => Price - Tax;
    }
}

#endif
