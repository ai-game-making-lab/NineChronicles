using System;
using System.Collections.Generic;
using Nekoyume.SingleClient.Models.Mail;
using Nekoyume.SingleClient.Models.Market;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient.Mail
{
    /// Scaffold-level smoke coverage for the Mail mirror DTOs: instantiate each concrete type,
    /// round-trip field set/get, and confirm <c>MailType</c> routing per subtype. These tests
    /// pin the shape so S6c can migrate UI references with a typed reference to rely on.
    public class MailSnapshotSmokeTest
    {
        [Test]
        public void BaseFieldsAreSetAndReadable()
        {
            var id = Guid.NewGuid();
            var mail = new CombinationMailSnapshot
            {
                Id = id,
                BlockIndex = 42,
                RequiredBlockIndex = 100,
                New = true,
            };

            Assert.AreEqual(id, mail.Id);
            Assert.AreEqual(42, mail.BlockIndex);
            Assert.AreEqual(100, mail.RequiredBlockIndex);
            Assert.IsTrue(mail.New);
            Assert.AreEqual(MailType.Workshop, mail.MailType);
        }

        [Test]
        public void MailTypeRoutingMatchesLib9c()
        {
            Assert.AreEqual(MailType.Workshop, new CombinationMailSnapshot().MailType);
            Assert.AreEqual(MailType.Workshop, new ItemEnhanceMailSnapshot().MailType);
            Assert.AreEqual(MailType.Auction, new BuyerMailSnapshot().MailType);
            Assert.AreEqual(MailType.Auction, new SellerMailSnapshot().MailType);
            Assert.AreEqual(MailType.Auction, new OrderBuyerMailSnapshot().MailType);
            Assert.AreEqual(MailType.Auction, new OrderSellerMailSnapshot().MailType);
            Assert.AreEqual(MailType.Auction, new ProductBuyerMailSnapshot().MailType);
            Assert.AreEqual(MailType.Auction, new ProductSellerMailSnapshot().MailType);
            Assert.AreEqual(MailType.Auction, new ProductCancelMailSnapshot().MailType);
            Assert.AreEqual(MailType.Auction, new UnloadFromMyGaragesRecipientMailSnapshot().MailType);
            Assert.AreEqual(MailType.Auction, new ClaimItemsMailSnapshot().MailType);
            Assert.AreEqual(MailType.System, new DailyRewardMailSnapshot().MailType);
            Assert.AreEqual(MailType.System, new MonsterCollectionMailSnapshot().MailType);
            Assert.AreEqual(MailType.Grinding, new GrindingMailSnapshot().MailType);
        }

        [Test]
        public void ProductBuyerMailDiscriminatesProductSubtype()
        {
            // Reproduces the `is ItemProduct` / `is FavProduct` branches MailPopup.cs runs today.
            var withItem = new ProductBuyerMailSnapshot
            {
                ProductId = Guid.NewGuid(),
                Product = new ItemProductSnapshot { ItemCount = 3 },
            };
            var withFav = new ProductBuyerMailSnapshot
            {
                ProductId = Guid.NewGuid(),
                Product = new FavProductSnapshot(),
            };

            Assert.IsTrue(withItem.Product is ItemProductSnapshot);
            Assert.IsTrue(withFav.Product is FavProductSnapshot);
        }

        [Test]
        public void MailBoxEnumeratesByBlockIndexDescending()
        {
            var older = new CombinationMailSnapshot { BlockIndex = 1 };
            var newer = new CombinationMailSnapshot { BlockIndex = 9 };
            var mailBox = new MailBoxSnapshot(new List<MailSnapshot> { older, newer });

            using var e = mailBox.GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreSame(newer, e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreSame(older, e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void ClaimItemsMailCarriesMemoAndItemPairs()
        {
            var mail = new ClaimItemsMailSnapshot
            {
                Memo = "season_pass",
                Items = new List<(int Id, int Count)> { (500000, 2), (500001, 7) },
            };

            Assert.AreEqual("season_pass", mail.Memo);
            Assert.AreEqual(2, mail.Items.Count);
            Assert.AreEqual(500001, mail.Items[1].Id);
            Assert.AreEqual(7, mail.Items[1].Count);
        }
    }
}
