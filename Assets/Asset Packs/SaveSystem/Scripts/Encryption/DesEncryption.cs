using System.Security.Cryptography;

namespace SaveSystem.Encryption
{
    public class DesEncryption : EncryptionScript
    {
        #region CONSTRUCTOR
        public DesEncryption() : base()
        {
            cryptoService = new TripleDESCryptoServiceProvider();
        }
        #endregion
    }
}
