using System;
using System.Security.Cryptography;
using System.Text;

namespace Neme.Utils
{
    public static class RSAKeyManager
    {
        private static readonly RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

        /// <summary>
        /// Returns the Base64-encoded RSA Public Key for broadcasting.
        /// </summary>
        public static string GetPublicKey() => Convert.ToBase64String(rsa.ExportRSAPublicKey());

        /// <summary>
        /// Encrypts data using a given Base64-encoded public key.
        /// </summary>
        public static string EncryptWithPublicKey(string data, string publicKey)
        {
            using (var rsaEncryptor = new RSACryptoServiceProvider(2048))
            {
                rsaEncryptor.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                byte[] encryptedData = rsaEncryptor.Encrypt(Encoding.UTF8.GetBytes(data), false);
                return Convert.ToBase64String(encryptedData);
            }
        }

        /// <summary>
        /// Decrypts data using this peer's private key.
        /// </summary>
        public static string DecryptWithPrivateKey(string encryptedData)
        {
            byte[] decryptedData = rsa.Decrypt(Convert.FromBase64String(encryptedData), false);
            return Encoding.UTF8.GetString(decryptedData);
        }

        /// <summary>
        /// Generates a new RSA key pair (for when the app restarts).
        /// </summary>
        public static void RegenerateKeyPair()
        {
            rsa.PersistKeyInCsp = false; // Do not store keys in Windows CNG
            rsa.Clear();
            //rsa = new RSACryptoServiceProvider(2048);
        }
    }
}
