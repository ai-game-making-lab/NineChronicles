using Nekoyume.SingleClient.Blockchain;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>GrindingMail</c> — equipment grind payout (crystal FAV + material count).
    public sealed class GrindingMailSnapshot : MailSnapshot
    {
        public int ItemCount { get; set; }
        public FungibleAssetValue Asset { get; set; }
        public int RewardMaterialCount { get; set; }

        public override MailType MailType => MailType.Grinding;
    }
}
