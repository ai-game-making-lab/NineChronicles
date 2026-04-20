using System;
using Nekoyume.SingleClient.Blockchain;

namespace Nekoyume.SingleClient.Models.Market
{
    /// Base projection of lib9c <c>Product</c> — the shared identity, price, and seller fields
    /// every <c>ProductBuyer/Seller/CancelMail</c> payload exposes. Concrete subtypes
    /// (<see cref="ItemProductSnapshot"/> / <see cref="FavProductSnapshot"/>) layer the
    /// kind-specific payload on top so UI can <c>is</c>-discriminate as today.
    public abstract class ProductSnapshot
    {
        public Guid ProductId { get; set; }
        public ProductType Type { get; set; }

        /// Listing price. Carried as the blockchain-shim <see cref="FungibleAssetValue"/> so the
        /// mail settlement UI can format it without re-entering lib9c types.
        public FungibleAssetValue Price { get; set; }

        public long RegisteredBlockIndex { get; set; }
        public Address SellerAvatarAddress { get; set; }
        public Address SellerAgentAddress { get; set; }
    }
}
