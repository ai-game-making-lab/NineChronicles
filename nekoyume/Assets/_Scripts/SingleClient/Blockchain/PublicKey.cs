using System;
using System.Text;

namespace Nekoyume.SingleClient.Blockchain
{
    public readonly struct PublicKey : IEquatable<PublicKey>
    {
        public const int CompressedSize = 33;
        public const int UncompressedSize = 65;

        private readonly byte[] _bytes;

        public PublicKey(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != CompressedSize && bytes.Length != UncompressedSize)
            {
                throw new ArgumentException(
                    $"PublicKey must be {CompressedSize} or {UncompressedSize} bytes, got {bytes.Length}.",
                    nameof(bytes));
            }

            _bytes = (byte[])bytes.Clone();
        }

        public byte[] ToByteArray(bool compress = true)
        {
            var buf = _bytes ?? new byte[CompressedSize];
            if (!compress && buf.Length == CompressedSize)
            {
                var padded = new byte[UncompressedSize];
                Buffer.BlockCopy(buf, 0, padded, 0, buf.Length);
                return padded;
            }

            if (compress && buf.Length == UncompressedSize)
            {
                var trimmed = new byte[CompressedSize];
                Buffer.BlockCopy(buf, 0, trimmed, 0, CompressedSize);
                return trimmed;
            }

            return (byte[])buf.Clone();
        }

        public string ToHex(bool compress = true)
        {
            var buf = ToByteArray(compress);
            var sb = new StringBuilder(buf.Length * 2);
            foreach (var b in buf)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public override string ToString() => ToHex(true);

        public static PublicKey FromHex(string hex)
        {
            if (hex == null)
            {
                throw new ArgumentNullException(nameof(hex));
            }

            var trimmed = hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? hex.Substring(2)
                : hex;
            if (trimmed.Length % 2 != 0)
            {
                throw new ArgumentException("PublicKey hex length must be even.", nameof(hex));
            }

            var buf = new byte[trimmed.Length / 2];
            for (var i = 0; i < buf.Length; i++)
            {
                buf[i] = Convert.ToByte(trimmed.Substring(i * 2, 2), 16);
            }
            return new PublicKey(buf);
        }

        public bool Equals(PublicKey other)
        {
            var a = _bytes ?? Array.Empty<byte>();
            var b = other._bytes ?? Array.Empty<byte>();
            if (a.Length != b.Length)
            {
                return false;
            }
            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj) => obj is PublicKey p && Equals(p);

        public override int GetHashCode()
        {
            var buf = _bytes ?? Array.Empty<byte>();
            var h = 17;
            foreach (var b in buf)
            {
                h = unchecked(h * 31 + b);
            }
            return h;
        }

        public static bool operator ==(PublicKey left, PublicKey right) => left.Equals(right);

        public static bool operator !=(PublicKey left, PublicKey right) => !left.Equals(right);
    }
}
