#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Security.Cryptography;
using System.Text;

namespace Nekoyume.SingleClient.Blockchain
{
    public sealed class PrivateKey : IEquatable<PrivateKey>
    {
        public const int Size = 32;

        private readonly byte[] _bytes;

        public PrivateKey()
        {
            var buf = new byte[Size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buf);
            _bytes = buf;
        }

        public PrivateKey(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != Size)
            {
                throw new ArgumentException(
                    $"PrivateKey must be {Size} bytes, got {bytes.Length}.",
                    nameof(bytes));
            }

            _bytes = (byte[])bytes.Clone();
        }

        public PrivateKey(string hex)
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
                    $"PrivateKey hex must be {Size * 2} chars, got {trimmed.Length}.",
                    nameof(hex));
            }

            var buf = new byte[Size];
            for (var i = 0; i < Size; i++)
            {
                buf[i] = Convert.ToByte(trimmed.Substring(i * 2, 2), 16);
            }
            _bytes = buf;
        }

        public byte[] ByteArray => (byte[])_bytes.Clone();

        public byte[] ToByteArray() => (byte[])_bytes.Clone();

        public string ToHex()
        {
            var sb = new StringBuilder(Size * 2);
            foreach (var b in _bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public PublicKey PublicKey
        {
            get
            {
                using var sha = SHA256.Create();
                var hash = sha.ComputeHash(_bytes);
                var pub = new byte[Nekoyume.SingleClient.Blockchain.PublicKey.CompressedSize];
                pub[0] = 0x02;
                Buffer.BlockCopy(hash, 0, pub, 1, 32);
                return new PublicKey(pub);
            }
        }

        public Address Address
        {
            get
            {
                using var sha = SHA256.Create();
                var hash = sha.ComputeHash(_bytes);
                var size = Nekoyume.SingleClient.Blockchain.Address.Size;
                var addr = new byte[size];
                Buffer.BlockCopy(hash, 12, addr, 0, size);
                return new Address(addr);
            }
        }

        public bool Equals(PrivateKey other)
        {
            if (other is null)
            {
                return false;
            }
            for (var i = 0; i < Size; i++)
            {
                if (_bytes[i] != other._bytes[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj) => obj is PrivateKey p && Equals(p);

        public override int GetHashCode()
        {
            var h = 17;
            foreach (var b in _bytes)
            {
                h = unchecked(h * 31 + b);
            }
            return h;
        }
    }
}

#endif
