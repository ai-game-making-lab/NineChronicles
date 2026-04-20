using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Nekoyume.SingleClient.Blockchain
{
    public sealed class ProtectedPrivateKey
    {
        private const int SaltSize = 16;
        private const int NonceSize = 16;
        private const int MacSize = 32;
        private const int PbkdfIterations = 100_000;
        private const int DerivedKeySize = 32;

        public Address Address { get; }

        public byte[] Salt { get; }

        public byte[] Nonce { get; }

        public byte[] Ciphertext { get; }

        public byte[] Mac { get; }

        public int Iterations { get; }

        private ProtectedPrivateKey(
            Address address,
            byte[] salt,
            byte[] nonce,
            byte[] ciphertext,
            byte[] mac,
            int iterations)
        {
            Address = address;
            Salt = salt;
            Nonce = nonce;
            Ciphertext = ciphertext;
            Mac = mac;
            Iterations = iterations;
        }

        public static ProtectedPrivateKey Protect(PrivateKey key, string passphrase)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (passphrase == null)
            {
                throw new ArgumentNullException(nameof(passphrase));
            }

            var salt = new byte[SaltSize];
            var nonce = new byte[NonceSize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            rng.GetBytes(nonce);

            var derived = DeriveKey(passphrase, salt, PbkdfIterations);
            var ciphertext = Crypt(key.ToByteArray(), derived.EncryptKey, nonce);
            var mac = ComputeMac(derived.MacKey, ciphertext);

            return new ProtectedPrivateKey(
                key.Address,
                salt,
                nonce,
                ciphertext,
                mac,
                PbkdfIterations);
        }

        public PrivateKey Unprotect(string passphrase)
        {
            if (passphrase == null)
            {
                throw new ArgumentNullException(nameof(passphrase));
            }

            var derived = DeriveKey(passphrase, Salt, Iterations);
            var mac = ComputeMac(derived.MacKey, Ciphertext);
            if (!ConstantTimeEquals(mac, Mac))
            {
                throw new CryptographicException("MAC verification failed (wrong passphrase?).");
            }

            var plain = Crypt(Ciphertext, derived.EncryptKey, Nonce);
            return new PrivateKey(plain);
        }

        public string ToJson()
        {
            var sb = new StringBuilder();
            sb.Append('{');
            sb.Append("\"address\":\"").Append(Address.ToHex()).Append('"');
            sb.Append(",\"iter\":").Append(Iterations);
            sb.Append(",\"salt\":\"").Append(ToHex(Salt)).Append('"');
            sb.Append(",\"nonce\":\"").Append(ToHex(Nonce)).Append('"');
            sb.Append(",\"ciphertext\":\"").Append(ToHex(Ciphertext)).Append('"');
            sb.Append(",\"mac\":\"").Append(ToHex(Mac)).Append('"');
            sb.Append('}');
            return sb.ToString();
        }

        public static ProtectedPrivateKey FromJson(string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            string ReadField(string name)
            {
                var needle = "\"" + name + "\":";
                var idx = json.IndexOf(needle, StringComparison.Ordinal);
                if (idx < 0)
                {
                    throw new FormatException($"Missing field '{name}' in ProtectedPrivateKey JSON.");
                }
                idx += needle.Length;
                while (idx < json.Length && char.IsWhiteSpace(json[idx]))
                {
                    idx++;
                }

                if (idx < json.Length && json[idx] == '"')
                {
                    var end = json.IndexOf('"', idx + 1);
                    if (end < 0)
                    {
                        throw new FormatException($"Unterminated string for '{name}'.");
                    }
                    return json.Substring(idx + 1, end - idx - 1);
                }

                var stop = idx;
                while (stop < json.Length && json[stop] != ',' && json[stop] != '}')
                {
                    stop++;
                }
                return json.Substring(idx, stop - idx).Trim();
            }

            var address = new Address(ReadField("address"));
            var iter = int.Parse(ReadField("iter"));
            var salt = FromHex(ReadField("salt"));
            var nonce = FromHex(ReadField("nonce"));
            var ciphertext = FromHex(ReadField("ciphertext"));
            var mac = FromHex(ReadField("mac"));

            return new ProtectedPrivateKey(address, salt, nonce, ciphertext, mac, iter);
        }

        private struct DerivedKeys
        {
            public byte[] EncryptKey;
            public byte[] MacKey;
        }

        private static DerivedKeys DeriveKey(string passphrase, byte[] salt, int iterations)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
                passphrase,
                salt,
                iterations,
                HashAlgorithmName.SHA256);
            var full = pbkdf2.GetBytes(DerivedKeySize * 2);
            var enc = new byte[DerivedKeySize];
            var mac = new byte[DerivedKeySize];
            Buffer.BlockCopy(full, 0, enc, 0, DerivedKeySize);
            Buffer.BlockCopy(full, DerivedKeySize, mac, 0, DerivedKeySize);
            return new DerivedKeys { EncryptKey = enc, MacKey = mac };
        }

        private static byte[] Crypt(byte[] input, byte[] key, byte[] nonce)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            aes.Key = key;

            var output = new byte[input.Length];
            var counter = (byte[])nonce.Clone();
            var keystream = new byte[16];
            using var enc = aes.CreateEncryptor();
            for (var offset = 0; offset < input.Length; offset += 16)
            {
                enc.TransformBlock(counter, 0, 16, keystream, 0);
                var remaining = Math.Min(16, input.Length - offset);
                for (var i = 0; i < remaining; i++)
                {
                    output[offset + i] = (byte)(input[offset + i] ^ keystream[i]);
                }

                for (var i = 15; i >= 0; i--)
                {
                    counter[i]++;
                    if (counter[i] != 0)
                    {
                        break;
                    }
                }
            }

            return output;
        }

        private static byte[] ComputeMac(byte[] macKey, byte[] ciphertext)
        {
            using var hmac = new HMACSHA256(macKey);
            return hmac.ComputeHash(ciphertext);
        }

        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            var diff = 0;
            for (var i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        private static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private static byte[] FromHex(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                throw new FormatException("Hex length must be even.");
            }
            var buf = new byte[hex.Length / 2];
            for (var i = 0; i < buf.Length; i++)
            {
                buf[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return buf;
        }
    }
}
