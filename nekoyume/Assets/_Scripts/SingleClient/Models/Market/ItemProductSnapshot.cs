using Nekoyume.SingleClient.Models.Items;

namespace Nekoyume.SingleClient.Models.Market
{
    /// Projection of lib9c <c>ItemProduct</c> — a tradable-item listing. The original lib9c
    /// <c>ITradableItem</c> reference collapses to <see cref="IItemSnapshot"/> so UI resolves
    /// item metadata through the same mirror path as the inventory.
    public sealed class ItemProductSnapshot : ProductSnapshot
    {
        public IItemSnapshot TradableItem { get; set; }
        public int ItemCount { get; set; }
    }
}
