using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Neme.Utils
{
    internal class AesEncryption
    {
        private readonly byte[] key;

        /// <summary>
        /// Initializes the AES encryption utility using the provided system name or key.
        /// </summary>
        /// <param name="keySource">The source for the encryption key. If null, the system name is used by default.</param>
        public AesEncryption(string keySource = null)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Use system name as default key source
                keySource ??= Environment.MachineName;

                // Hash the source to generate a 32-byte key
                key = sha256.ComputeHash(Encoding.UTF8.GetBytes(keySource));
            }
        }

        /// <summary>
        /// Encrypts the given plain text using AES encryption.
        /// </summary>
        /// <param name="plainText">The plain text to encrypt.</param>
        /// <returns>The encrypted text as a Base64 string.</returns>
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Plain text cannot be null or empty.", nameof(plainText));

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length); // Prepend IV

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        /// <summary>
        /// Decrypts the given cipher text using AES encryption.
        /// </summary>
        /// <param name="cipherText">The encrypted text as a Base64 string.</param>
        /// <returns>The decrypted plain text.</returns>
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("Cipher text cannot be null or empty.", nameof(cipherText));

            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    // Extract IV
                    byte[] iv = new byte[16]; // AES block size
                    msDecrypt.Read(iv, 0, iv.Length);
                    aesAlg.IV = iv;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// Validates whether the provided key source is valid.
        /// </summary>
        /// <param name="keySource">The source for the encryption key.</param>
        /// <returns>True if valid, otherwise false.</returns>
        public static bool IsValidKeySource(string keySource)
        {
            return !string.IsNullOrEmpty(keySource);
        }

        /// <summary>
        /// Gets metadata about the current encryption setup.
        /// </summary>
        /// <returns>A string containing key length and other details.</returns>
        public string GetMetadata()
        {
            return $"Key Length: {key.Length * 8} bits";
        }
    }
}
