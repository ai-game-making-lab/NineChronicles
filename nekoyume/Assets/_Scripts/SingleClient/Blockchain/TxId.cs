using System;
using System.Security.Cryptography;
using System.Text;

namespace Nekoyume.SingleClient.Blockchain
{
    public readonly struct TxId : IEquatable<TxId>
    {
        public const int Size = 32;

        private readonly byte[] _bytes;

        public TxId(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != Size)
            {
                throw new ArgumentException(
                    $"TxId must be {Size} bytes, got {bytes.Length}.",
                    nameof(bytes));
            }

            _bytes = (byte[])bytes.Clone();
        }

        public byte[] ToByteArray()
        {
            return _bytes is null ? new byte[Size] : (byte[])_bytes.Clone();
        }

        public override string ToString()
        {
            var buf = _bytes ?? new byte[Size];
            var sb = new StringBuilder(Size * 2);
            foreach (var b in buf)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static TxId FromHex(string hex)
        {
            if (hex == null)
            {
                throw new ArgumentNullException(nameof(hex));
            }

            var trimmed = hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? hex.Substring(2)
                : hex;
            if (trimmed.Length != Size * 2)
            {
                throw new ArgumentException(
                    $"TxId hex length must be {Size * 2}, got {trimmed.Length}.",
                    nameof(hex));
            }

            var buf = new byte[Size];
            for (var i = 0; i < Size; i++)
            {
                buf[i] = Convert.ToByte(trimmed.Substring(i * 2, 2), 16);
            }
            return new TxId(buf);
        }

        public static TxId Random()
        {
            var buf = new byte[Size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buf);
            return new TxId(buf);
        }

        public bool Equals(TxId other)
        {
            var a = _bytes ?? new byte[Size];
            var b = other._bytes ?? new byte[Size];
            for (var i = 0; i < Size; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj) => obj is TxId t && Equals(t);

        public override int GetHashCode()
        {
            var buf = _bytes ?? new byte[Size];
            var hash = 17;
            foreach (var b in buf)
            {
                hash = unchecked(hash * 31 + b);
            }
            return hash;
        }

        public static bool operator ==(TxId left, TxId right) => left.Equals(right);

        public static bool operator !=(TxId left, TxId right) => !left.Equals(right);
    }
}
