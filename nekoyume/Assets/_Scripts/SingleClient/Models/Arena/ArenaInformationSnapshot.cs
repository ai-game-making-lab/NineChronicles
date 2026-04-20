using Nekoyume.SingleClient.Blockchain;

namespace Nekoyume.SingleClient.Models.Arena
{
    /// Projection of <c>ArenaInformation</c> — per-championship ticket / record counters. Holds
    /// only the scalar fields the arena preparation + board UI reads.
    public sealed class ArenaInformationSnapshot
    {
        public const int MaxTicketCount = 8;

        public Address Address { get; set; }
        public int Win { get; set; }
        public int Lose { get; set; }
        public int Ticket { get; set; }
        public int TicketResetCount { get; set; }
        public int PurchasedTicketCount { get; set; }
    }
}
