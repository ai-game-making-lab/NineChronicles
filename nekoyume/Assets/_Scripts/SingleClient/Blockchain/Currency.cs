using System;
using System.Collections.Immutable;

namespace Nekoyume.SingleClient.Blockchain
{
    public readonly struct Currency : IEquatable<Currency>
    {
        public string Ticker { get; }

        public byte DecimalPlaces { get; }

        public IImmutableSet<Address> Minters { get; }

        public bool TotalSupplyTrackable { get; }

        public FungibleAssetValue? MaximumSupply { get; }

        private Currency(
            string ticker,
            byte decimalPlaces,
            IImmutableSet<Address> minters,
            bool totalSupplyTrackable,
            FungibleAssetValue? maximumSupply)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new ArgumentException("Ticker must be non-empty.", nameof(ticker));
            }

            Ticker = ticker;
            DecimalPlaces = decimalPlaces;
            Minters = minters ?? ImmutableHashSet<Address>.Empty;
            TotalSupplyTrackable = totalSupplyTrackable;
            MaximumSupply = maximumSupply;
        }

        public static Currency Legacy(
            string ticker,
            byte decimalPlaces,
            IImmutableSet<Address> minters)
        {
            return new Currency(ticker, decimalPlaces, minters, false, null);
        }

        public static Currency Legacy(
            string ticker,
            byte decimalPlaces,
            Address? minter)
        {
            var set = minter.HasValue
                ? ImmutableHashSet.Create(minter.Value)
                : ImmutableHashSet<Address>.Empty;
            return Legacy(ticker, decimalPlaces, set);
        }

        public static Currency Uncapped(
            string ticker,
            byte decimalPlaces,
            IImmutableSet<Address> minters)
        {
            return new Currency(ticker, decimalPlaces, minters, true, null);
        }

        public static Currency Uncapped(
            string ticker,
            byte decimalPlaces,
            Address? minter)
        {
            var set = minter.HasValue
                ? ImmutableHashSet.Create(minter.Value)
                : ImmutableHashSet<Address>.Empty;
            return Uncapped(ticker, decimalPlaces, set);
        }

        public static Currency Capped(
            string ticker,
            byte decimalPlaces,
            (System.Numerics.BigInteger Major, System.Numerics.BigInteger Minor) maximum,
            IImmutableSet<Address> minters)
        {
            var currency = new Currency(ticker, decimalPlaces, minters, true, null);
            var raw = maximum.Major * System.Numerics.BigInteger.Pow(10, decimalPlaces) + maximum.Minor;
            var max = FungibleAssetValue.FromRawValue(currency, raw);
            return new Currency(ticker, decimalPlaces, minters, true, max);
        }

        public FungibleAssetValue ZeroValue => FungibleAssetValue.FromRawValue(this, 0);

        public bool Equals(Currency other)
        {
            return Ticker == other.Ticker
                && DecimalPlaces == other.DecimalPlaces
                && TotalSupplyTrackable == other.TotalSupplyTrackable;
        }

        public override bool Equals(object obj) => obj is Currency c && Equals(c);

        public override int GetHashCode()
        {
            unchecked
            {
                var h = 17;
                h = h * 31 + (Ticker?.GetHashCode() ?? 0);
                h = h * 31 + DecimalPlaces;
                h = h * 31 + (TotalSupplyTrackable ? 1 : 0);
                return h;
            }
        }

        public static bool operator ==(Currency left, Currency right) => left.Equals(right);

        public static bool operator !=(Currency left, Currency right) => !left.Equals(right);

        public override string ToString() => $"{Ticker}:{DecimalPlaces}";
    }
}
