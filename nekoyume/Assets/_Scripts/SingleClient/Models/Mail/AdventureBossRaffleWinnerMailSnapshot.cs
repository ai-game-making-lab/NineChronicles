using Nekoyume.SingleClient.Blockchain;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>AdventureBossRaffleWinnerMail</c> — adventure-boss season raffle win
    /// delivering a single FAV reward. UI surfaces the season index and the FAV amount; lib9c's
    /// Bencodex envelope is not mirrored here because it is a serialization concern.
    public sealed class AdventureBossRaffleWinnerMailSnapshot : MailSnapshot
    {
        public long Season { get; set; }
        public FungibleAssetValue Reward { get; set; }

        public override MailType MailType => MailType.System;
    }
}
