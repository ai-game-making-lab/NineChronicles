using Nekoyume.SingleClient.Blockchain;

namespace Nekoyume.SingleClient.Models.Market
{
    /// Projection of lib9c <c>FavProduct</c> — a FungibleAssetValue listing (e.g. Crystal).
    public sealed class FavProductSnapshot : ProductSnapshot
    {
        public FungibleAssetValue Asset { get; set; }
    }
}
