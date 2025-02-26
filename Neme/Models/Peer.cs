using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace Neme.Models
{
    public class Peer : INotifyPropertyChanged
    {
        private PeerStatus _status;

        public string Address { get; set; } // Network address
        public string Avatar { get; set; } // Optional avatar path
        public List<string> Groups { get; set; } = new List<string>();


        public string Name { get; set; }
        public string PublicKey { get; private set; }
        public string AesKey { get; private set; }

        private RSACryptoServiceProvider rsa;

        public Peer()
        {
            rsa = new RSACryptoServiceProvider(2048);
            PublicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
        }

        public string EncryptAESKey(string aesKey, string recipientPublicKey)
        {
            using (var rsaEncrypt = new RSACryptoServiceProvider(2048))
            {
                rsaEncrypt.ImportRSAPublicKey(Convert.FromBase64String(recipientPublicKey), out _);
                byte[] encryptedKey = rsaEncrypt.Encrypt(Encoding.UTF8.GetBytes(aesKey), false);
                return Convert.ToBase64String(encryptedKey);
            }
        }

        public string DecryptAESKey(string encryptedAesKey)
        {
            byte[] decryptedBytes = rsa.Decrypt(Convert.FromBase64String(encryptedAesKey), false);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public void UpdateAESKey(string newAesKey)
        {
            AesKey = newAesKey;
        }
    


    public PeerStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum PeerStatus
    {
        Online,
        Offline,
        Unresponsive
    }
}
