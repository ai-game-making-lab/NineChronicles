#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Libplanet.Crypto;
using Libplanet.Types.Assets;
using Nekoyume.Model.Item;
using Nekoyume.SingleClient;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient
{
    public class ClientCurrenciesTest
    {
        [Test]
        public void GetMinterlessCurrencySupportsClientRewardTickers()
        {
            Assert.AreEqual(ClientCurrencies.Crystal, ClientCurrencies.GetMinterlessCurrency("CRYSTAL"));
            Assert.AreEqual(ClientCurrencies.Garage, ClientCurrencies.GetMinterlessCurrency("GARAGE"));
            Assert.AreEqual("RUNE_TEST", ClientCurrencies.GetMinterlessCurrency("RUNE_TEST").Ticker);
            Assert.AreEqual("RUNESTONE_TEST", ClientCurrencies.GetMinterlessCurrency("RUNESTONE_TEST").Ticker);
            Assert.AreEqual("SOULSTONE_TEST", ClientCurrencies.GetMinterlessCurrency("SOULSTONE_TEST").Ticker);
        }

        [Test]
        public void GetMinterlessCurrencyRejectsUnsupportedTicker()
        {
            Assert.Throws<ArgumentException>(() => ClientCurrencies.GetMinterlessCurrency("NCG"));
        }

        [Test]
        public void PickAddressUsesAgentAddressForAgentCurrencies()
        {
            var agentAddress = new PrivateKey().Address;
            var avatarAddress = new PrivateKey().Address;

            Assert.AreEqual(
                agentAddress,
                ClientCurrencies.PickAddress(ClientCurrencies.Crystal, agentAddress, avatarAddress));
            Assert.AreEqual(
                agentAddress,
                ClientCurrencies.PickAddress(ClientCurrencies.Garage, agentAddress, avatarAddress));
            Assert.AreEqual(
                avatarAddress,
                ClientCurrencies.PickAddress(
                    ClientCurrencies.GetRune("RUNE_TEST"),
                    agentAddress,
                    avatarAddress));
        }

        [Test]
        public void WrappedCurrencyRoundTrips()
        {
            var wrapped = ClientCurrencies.GetWrappedCurrency(ClientCurrencies.Crystal);

            Assert.IsTrue(ClientCurrencies.IsWrappedCurrency(wrapped));
            Assert.AreEqual(ClientCurrencies.Crystal, ClientCurrencies.GetUnwrappedCurrency(wrapped));
        }

        [Test]
        public void ClientOrderUsesLocalMarketConstants()
        {
            var currency = Currency.Legacy("TEST", 2, null);
            var price = 1000 * currency;
            var order = new ClientOrder(
                Guid.NewGuid(),
                Guid.NewGuid(),
                100,
                new PrivateKey().Address,
                new PrivateKey().Address,
                ItemSubType.Weapon,
                price,
                3);

            Assert.AreEqual(36000, ClientOrder.ExpirationInterval);
            Assert.AreEqual(8, ClientOrder.TaxRate);
            Assert.AreEqual(80 * currency, order.Tax);
            Assert.AreEqual(920 * currency, order.TaxedPrice);
            Assert.AreEqual(3, order.ItemCount);
        }
    }
}

#endif
