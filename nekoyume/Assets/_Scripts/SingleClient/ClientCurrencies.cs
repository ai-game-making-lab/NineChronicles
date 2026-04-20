using System;
using System.Globalization;
using System.Linq;
using Libplanet.Crypto;
using Libplanet.Types.Assets;

namespace Nekoyume.SingleClient
{
    public static class ClientCurrencies
    {
        public static readonly Currency Crystal = Currency.Legacy(
            "CRYSTAL",
            18,
            minters: null);

        public static readonly Currency Garage = Currency.Uncapped(
            "GARAGE",
            18,
            minters: null);

        public static readonly Currency Mead = Currency.Legacy(
            "Mead",
            18,
            minters: null);

        public static Currency GetMinterlessCurrency(string ticker)
        {
            if (string.IsNullOrEmpty(ticker))
            {
                throw new ArgumentNullException(nameof(ticker));
            }

            switch (ticker)
            {
                case "CRYSTAL":
                    return Crystal;
                case "GARAGE":
                    return Garage;
            }

            if (IsRuneTicker(ticker))
            {
                return GetRune(ticker);
            }

            if (IsSoulStoneTicker(ticker))
            {
                return GetSoulStone(ticker);
            }

            throw new ArgumentException($"Unsupported minterless ticker: {ticker}", nameof(ticker));
        }

        public static bool IsRuneTicker(string ticker)
        {
            if (string.IsNullOrEmpty(ticker))
            {
                return false;
            }

            ticker = ticker.ToLower(CultureInfo.InvariantCulture);
            return ticker.StartsWith("rune_") || ticker.StartsWith("runestone_");
        }

        public static Currency GetRune(string ticker)
        {
            if (string.IsNullOrEmpty(ticker))
            {
                throw new ArgumentNullException(nameof(ticker));
            }

            return Currency.Legacy(ticker, 0, minters: null);
        }

        public static bool IsSoulStoneTicker(string ticker) =>
            !string.IsNullOrEmpty(ticker) &&
            ticker.ToLower(CultureInfo.InvariantCulture).StartsWith("soulstone_");

        public static Currency GetSoulStone(string ticker)
        {
            if (string.IsNullOrEmpty(ticker))
            {
                throw new ArgumentNullException(nameof(ticker));
            }

            return Currency.Legacy(ticker, 0, minters: null);
        }

        public static Currency GetCurrencyByTicker(string ticker)
        {
            if (ticker == "NCG")
            {
                throw new ArgumentException(
                    "NCG requires the runtime gold currency instance.",
                    nameof(ticker));
            }

            return GetMinterlessCurrency(ticker);
        }

        public static Currency GetWrappedCurrency(Currency currency) =>
            Currency.Legacy(
                $"FAV__{currency.Ticker}",
                currency.DecimalPlaces,
                minters: null);

        public static bool IsWrappedCurrency(Currency currency) =>
            currency.Ticker.StartsWith("FAV", StringComparison.Ordinal);

        public static Currency GetUnwrappedCurrency(Currency currency)
        {
            if (!IsWrappedCurrency(currency))
            {
                throw new ArgumentException(
                    $"{currency.Ticker} is not a wrapped currency.",
                    nameof(currency));
            }

            var parsedTicker = currency.Ticker.Split("__");
            if (parsedTicker.Length != 2)
            {
                throw new ArgumentException(
                    $"{currency.Ticker} is not a supported wrapped currency ticker.",
                    nameof(currency));
            }

            return GetMinterlessCurrency(parsedTicker[1]);
        }

        public static Address PickAddress(
            Currency currency,
            Address agentAddress,
            Address avatarAddress)
        {
            var agentCurrencies = new[]
            {
                Crystal,
                Garage,
                Mead,
            };

            return agentCurrencies.Contains(currency) || currency.Ticker == "NCG"
                ? agentAddress
                : avatarAddress;
        }
    }
}
