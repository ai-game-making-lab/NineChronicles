namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>MaterialCraftMail</c> — workshop material-delivery mail. Carries the
    /// item id and count; the concrete lib9c <c>Material</c> instance is resolved by the UI via
    /// the item sheet using <see cref="ItemId"/>.
    public sealed class MaterialCraftMailSnapshot : MailSnapshot
    {
        public int ItemId { get; set; }
        public int ItemCount { get; set; }

        public override MailType MailType => MailType.Workshop;
    }
}
