#if LIB9C_RESTORED // stubbed out after lib9c deletion
namespace Nekoyume.SingleClient.Models.Market
{
    /// Mirror of <c>Nekoyume.Model.Market.ProductType</c>. Ordering matches lib9c.
    public enum ProductType
    {
        Fungible,
        FungibleAssetValue,
        NonFungible,
    }

    /// Scaffold placeholder mirroring the client-side <c>MarketFilter</c> spec reference.
    /// lib9c does not expose a <c>MarketFilter</c> type directly; the enum captures the two
    /// filter categories the market UI switches on today.
    public enum MarketFilter
    {
        All,
        Fungible,
        NonFungible,
        FungibleAssetValue,
    }
}

#endif
