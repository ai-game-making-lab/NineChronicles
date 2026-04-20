#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Immutable;
using System.Numerics;

namespace Nekoyume.SingleClient.Blockchain
{
    public readonly struct Currency : IEquatable<Currency>
    {
        public string Ticker { get; }

        public byte DecimalPlaces { get; }

        public IImmutableSet<Address> Minters { get; }

        public bool TotalSupplyTrackable { get; }

        public BigInteger? MaximumSupplyRawValue { get; }

        public FungibleAssetValue? MaximumSupply =>
            MaximumSupplyRawValue.HasValue
                ? FungibleAssetValue.FromRawValue(this, MaximumSupplyRawValue.Value)
                : (FungibleAssetValue?)null;

        private Currency(
            string ticker,
            byte decimalPlaces,
            IImmutableSet<Address> minters,
            bool totalSupplyTrackable,
            BigInteger? maximumSupplyRawValue)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new ArgumentException("Ticker must be non-empty.", nameof(ticker));
            }

            Ticker = ticker;
            DecimalPlaces = decimalPlaces;
            Minters = minters ?? ImmutableHashSet<Address>.Empty;
            TotalSupplyTrackable = totalSupplyTrackable;
            MaximumSupplyRawValue = maximumSupplyRawValue;
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
            (BigInteger Major, BigInteger Minor) maximum,
            IImmutableSet<Address> minters)
        {
            var raw = maximum.Major * BigInteger.Pow(10, decimalPlaces) + maximum.Minor;
            return new Currency(ticker, decimalPlaces, minters, true, raw);
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

#endif
