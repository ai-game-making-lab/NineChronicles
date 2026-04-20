using System;
using System.Security.Cryptography;
using System.Text;

namespace Nekoyume.SingleClient.Blockchain
{
    public readonly struct Address : IEquatable<Address>
    {
        public const int Size = 20;

        private readonly byte[] _bytes;

        public Address(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != Size)
            {
                throw new ArgumentException(
                    $"Address must be {Size} bytes, got {bytes.Length}.",
                    nameof(bytes));
            }

            _bytes = (byte[])bytes.Clone();
        }

        public Address(string hex)
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
                    $"Address hex must be {Size * 2} chars, got {trimmed.Length}.",
                    nameof(hex));
            }

            var buf = new byte[Size];
            for (var i = 0; i < Size; i++)
            {
                buf[i] = Convert.ToByte(trimmed.Substring(i * 2, 2), 16);
            }

            _bytes = buf;
        }

        public byte[] ToByteArray()
        {
            return _bytes is null ? new byte[Size] : (byte[])_bytes.Clone();
        }

        public string ToHex()
        {
            var buf = _bytes ?? new byte[Size];
            var sb = new StringBuilder(Size * 2);
            foreach (var b in buf)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public override string ToString() => "0x" + ToHex();

        public Address Derive(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var selfBytes = _bytes ?? new byte[Size];
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var input = new byte[selfBytes.Length + keyBytes.Length];
            Buffer.BlockCopy(selfBytes, 0, input, 0, selfBytes.Length);
            Buffer.BlockCopy(keyBytes, 0, input, selfBytes.Length, keyBytes.Length);

            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(input);
            var result = new byte[Size];
            Buffer.BlockCopy(hash, 0, result, 0, Size);
            return new Address(result);
        }

        public Address Derive(byte[] key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var selfBytes = _bytes ?? new byte[Size];
            var input = new byte[selfBytes.Length + key.Length];
            Buffer.BlockCopy(selfBytes, 0, input, 0, selfBytes.Length);
            Buffer.BlockCopy(key, 0, input, selfBytes.Length, key.Length);

            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(input);
            var result = new byte[Size];
            Buffer.BlockCopy(hash, 0, result, 0, Size);
            return new Address(result);
        }

        public bool Equals(Address other)
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

        public override bool Equals(object obj) => obj is Address a && Equals(a);

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

        public static bool operator ==(Address left, Address right) => left.Equals(right);

        public static bool operator !=(Address left, Address right) => !left.Equals(right);

        public static Address Random()
        {
            var buf = new byte[Size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buf);
            return new Address(buf);
        }
    }
}
