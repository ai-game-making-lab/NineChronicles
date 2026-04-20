#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Globalization;
using System.Numerics;

namespace Nekoyume.SingleClient.Blockchain
{
    public readonly struct FungibleAssetValue :
        IEquatable<FungibleAssetValue>, IComparable<FungibleAssetValue>
    {
        public Currency Currency { get; }

        public BigInteger RawValue { get; }

        public BigInteger MajorUnit => RawValue / BigInteger.Pow(10, Currency.DecimalPlaces);

        public BigInteger MinorUnit =>
            BigInteger.Abs(RawValue % BigInteger.Pow(10, Currency.DecimalPlaces));

        public int Sign => RawValue.Sign;

        private FungibleAssetValue(Currency currency, BigInteger rawValue)
        {
            Currency = currency;
            RawValue = rawValue;
        }

        public static FungibleAssetValue FromRawValue(Currency currency, BigInteger rawValue)
        {
            return new FungibleAssetValue(currency, rawValue);
        }

        public static FungibleAssetValue Parse(Currency currency, string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var s = value.Trim();
            var negative = false;
            if (s.StartsWith("-"))
            {
                negative = true;
                s = s.Substring(1);
            }

            var dot = s.IndexOf('.');
            string intPart;
            string fracPart;
            if (dot < 0)
            {
                intPart = s;
                fracPart = string.Empty;
            }
            else
            {
                intPart = s.Substring(0, dot);
                fracPart = s.Substring(dot + 1);
            }

            if (fracPart.Length > currency.DecimalPlaces)
            {
                throw new FormatException(
                    $"Fractional part exceeds currency decimal places ({currency.DecimalPlaces}).");
            }

            fracPart = fracPart.PadRight(currency.DecimalPlaces, '0');
            var combined = (string.IsNullOrEmpty(intPart) ? "0" : intPart) + fracPart;
            var raw = BigInteger.Parse(combined, CultureInfo.InvariantCulture);
            if (negative)
            {
                raw = -raw;
            }

            return new FungibleAssetValue(currency, raw);
        }

        public string GetQuantityString(bool minorUnit = false)
        {
            var dp = Currency.DecimalPlaces;
            var abs = BigInteger.Abs(RawValue);
            var divisor = BigInteger.Pow(10, dp);
            var major = abs / divisor;
            var minor = abs % divisor;
            var sign = RawValue.Sign < 0 ? "-" : string.Empty;
            if (dp == 0 || (!minorUnit && minor.IsZero))
            {
                return sign + major.ToString(CultureInfo.InvariantCulture);
            }

            var minorStr = minor.ToString(CultureInfo.InvariantCulture).PadLeft(dp, '0');
            if (!minorUnit)
            {
                minorStr = minorStr.TrimEnd('0');
            }
            return sign + major.ToString(CultureInfo.InvariantCulture) + "." + minorStr;
        }

        public override string ToString() =>
            $"{GetQuantityString(true)} {Currency.Ticker}";

        public bool Equals(FungibleAssetValue other) =>
            Currency.Equals(other.Currency) && RawValue == other.RawValue;

        public override bool Equals(object obj) => obj is FungibleAssetValue v && Equals(v);

        public override int GetHashCode()
        {
            unchecked
            {
                var h = 17;
                h = h * 31 + Currency.GetHashCode();
                h = h * 31 + RawValue.GetHashCode();
                return h;
            }
        }

        public int CompareTo(FungibleAssetValue other)
        {
            AssertSameCurrency(other);
            return RawValue.CompareTo(other.RawValue);
        }

        private void AssertSameCurrency(FungibleAssetValue other)
        {
            if (!Currency.Equals(other.Currency))
            {
                throw new InvalidOperationException(
                    $"Currency mismatch: {Currency} vs {other.Currency}.");
            }
        }

        public static FungibleAssetValue operator +(FungibleAssetValue a, FungibleAssetValue b)
        {
            a.AssertSameCurrency(b);
            return FromRawValue(a.Currency, a.RawValue + b.RawValue);
        }

        public static FungibleAssetValue operator -(FungibleAssetValue a, FungibleAssetValue b)
        {
            a.AssertSameCurrency(b);
            return FromRawValue(a.Currency, a.RawValue - b.RawValue);
        }

        public static FungibleAssetValue operator -(FungibleAssetValue a) =>
            FromRawValue(a.Currency, -a.RawValue);

        public static FungibleAssetValue operator *(FungibleAssetValue a, BigInteger b) =>
            FromRawValue(a.Currency, a.RawValue * b);

        public static FungibleAssetValue operator *(BigInteger a, FungibleAssetValue b) =>
            FromRawValue(b.Currency, a * b.RawValue);

        public static FungibleAssetValue operator /(FungibleAssetValue a, BigInteger b) =>
            FromRawValue(a.Currency, a.RawValue / b);

        public static bool operator ==(FungibleAssetValue a, FungibleAssetValue b) => a.Equals(b);
        public static bool operator !=(FungibleAssetValue a, FungibleAssetValue b) => !a.Equals(b);
        public static bool operator <(FungibleAssetValue a, FungibleAssetValue b) => a.CompareTo(b) < 0;
        public static bool operator >(FungibleAssetValue a, FungibleAssetValue b) => a.CompareTo(b) > 0;
        public static bool operator <=(FungibleAssetValue a, FungibleAssetValue b) => a.CompareTo(b) <= 0;
        public static bool operator >=(FungibleAssetValue a, FungibleAssetValue b) => a.CompareTo(b) >= 0;
    }
}

#endif
