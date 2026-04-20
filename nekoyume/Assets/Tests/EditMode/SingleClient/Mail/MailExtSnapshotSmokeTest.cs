using System;
using System.Collections.Generic;
using Nekoyume.SingleClient.Blockchain;
using Nekoyume.SingleClient.Models.Mail;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient.Mail
{
    /// Smoke coverage for the 8 Mail mirror DTOs added in S9a-ext. Each test constructs the
    /// snapshot, round-trips the <c>Id</c> field through the base <see cref="MailSnapshot"/>
    /// setter, and asserts the subtype-specific <see cref="MailType"/> route so UI migration
    /// in S9b has a pinned contract to rely on.
    public class MailExtSnapshotSmokeTest
    {
        [Test]
        public void AdventureBossRaffleWinnerMailSnapshot_RoundTripsIdAndTypesAsSystem()
        {
            var id = Guid.NewGuid();
            var mail = new AdventureBossRaffleWinnerMailSnapshot
            {
                Id = id,
                Season = 7,
            };

            Assert.AreEqual(id, mail.Id);
            Assert.AreEqual(7, mail.Season);
            Assert.AreEqual(MailType.System, mail.MailType);
        }

        [Test]
        public void CancelOrderMailSnapshot_RoundTripsIdAndTypesAsAuction()
        {
            var id = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var mail = new CancelOrderMailSnapshot
            {
                Id = id,
                OrderId = orderId,
            };

            Assert.AreEqual(id, mail.Id);
            Assert.AreEqual(orderId, mail.OrderId);
            Assert.AreEqual(MailType.Auction, mail.MailType);
        }

        [Test]
        public void CustomCraftMailSnapshot_RoundTripsIdAndTypesAsCustomCraft()
        {
            var id = Guid.NewGuid();
            var mail = new CustomCraftMailSnapshot
            {
                Id = id,
            };

            Assert.AreEqual(id, mail.Id);
            Assert.AreEqual(MailType.CustomCraft, mail.MailType);
        }

        [Test]
        public void MaterialCraftMailSnapshot_RoundTripsIdAndTypesAsWorkshop()
        {
            var id = Guid.NewGuid();
            var mail = new MaterialCraftMailSnapshot
            {
                Id = id,
                ItemId = 500000,
                ItemCount = 3,
            };

            Assert.AreEqual(id, mail.Id);
            Assert.AreEqual(500000, mail.ItemId);
            Assert.AreEqual(3, mail.ItemCount);
            Assert.AreEqual(MailType.Workshop, mail.MailType);
        }

        [Test]
        public void OrderExpirationMailSnapshot_RoundTripsIdAndTypesAsAuction()
        {
            var id = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var mail = new OrderExpirationMailSnapshot
            {
                Id = id,
                OrderId = orderId,
            };

            Assert.AreEqual(id, mail.Id);
            Assert.AreEqual(orderId, mail.OrderId);
            Assert.AreEqual(MailType.Auction, mail.MailType);
        }

        [Test]
        public void PatrolRewardMailSnapshot_RoundTripsIdAndTypesAsAuction()
        {
            var id = Guid.NewGuid();
            var mail = new PatrolRewardMailSnapshot
            {
                Id = id,
                Items = new List<(int Id, int Count)> { (600000, 10) },
                FungibleAssetValues = Array.Empty<FungibleAssetValue>(),
            };

            Assert.AreEqual(id, mail.Id);
            Assert.AreEqual(1, mail.Items.Count);
            Assert.AreEqual(600000, mail.Items[0].Id);
            Assert.AreEqual(MailType.Auction, mail.MailType);
        }

        [Test]
        public void SellCancelMailSnapshot_RoundTripsIdAndTypesAsAuction()
        {
            var id = Guid.NewGuid();
            var mail = new SellCancelMailSnapshot
            {
                Id = id,
            };

            Assert.AreEqual(id, mail.Id);
            Assert.AreEqual(MailType.Auction, mail.MailType);
        }

        [Test]
        public void WorldBossRewardMailSnapshot_RoundTripsIdAndTypesAsSystem()
        {
            var id = Guid.NewGuid();
            var mail = new WorldBossRewardMailSnapshot
            {
                Id = id,
                Items = new List<(int Id, int Count)> { (700000, 1) },
                FungibleAssetValues = Array.Empty<FungibleAssetValue>(),
            };

            Assert.AreEqual(id, mail.Id);
            Assert.AreEqual(1, mail.Items.Count);
            Assert.AreEqual(700000, mail.Items[0].Id);
            Assert.AreEqual(MailType.System, mail.MailType);
        }
    }
}
