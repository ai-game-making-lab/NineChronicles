#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Security.Cryptography;
using System.Text;

namespace Nekoyume.SingleClient.Blockchain
{
    public interface IHashAlgorithm
    {
        int Size { get; }
        byte[] Compute(byte[] input);
    }

    public sealed class SHA256Algorithm : IHashAlgorithm
    {
        public int Size => 32;
        public byte[] Compute(byte[] input)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(input);
        }
    }

    public readonly struct HashDigest<T> : IEquatable<HashDigest<T>>
        where T : IHashAlgorithm, new()
    {
        public static int Size => new T().Size;

        private readonly byte[] _bytes;

        public HashDigest(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            var expected = Size;
            if (bytes.Length != expected)
            {
                throw new ArgumentException(
                    $"HashDigest<{typeof(T).Name}> must be {expected} bytes, got {bytes.Length}.",
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
            var sb = new StringBuilder(buf.Length * 2);
            foreach (var b in buf)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static HashDigest<T> FromString(string hex)
        {
            if (hex == null)
            {
                throw new ArgumentNullException(nameof(hex));
            }

            var trimmed = hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? hex.Substring(2)
                : hex;
            var size = Size;
            if (trimmed.Length != size * 2)
            {
                throw new ArgumentException(
                    $"Hex length must be {size * 2}, got {trimmed.Length}.",
                    nameof(hex));
            }

            var buf = new byte[size];
            for (var i = 0; i < size; i++)
            {
                buf[i] = Convert.ToByte(trimmed.Substring(i * 2, 2), 16);
            }
            return new HashDigest<T>(buf);
        }

        public bool Equals(HashDigest<T> other)
        {
            var size = Size;
            var a = _bytes ?? new byte[size];
            var b = other._bytes ?? new byte[size];
            for (var i = 0; i < size; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj) => obj is HashDigest<T> h && Equals(h);

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

        public static bool operator ==(HashDigest<T> left, HashDigest<T> right) => left.Equals(right);

        public static bool operator !=(HashDigest<T> left, HashDigest<T> right) => !left.Equals(right);
    }
}

#endif
