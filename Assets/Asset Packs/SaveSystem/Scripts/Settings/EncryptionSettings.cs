using SaveSystem.Encryption;

namespace SaveSystem.Settings
{
    public struct EncryptionSettings
    {
        public bool UseEncryption { get; set; }

        #region PROPERTY Encryption
        private EncryptionScript encryption;
        public EncryptionScript Encryption
        {
            get
            {
                if (encryption == null)
                    encryption = EncryptionScript.DES;
                return encryption;
            }
            set
            {
                encryption = value;
            }
        }
        #endregion

        #region PROPERTY EncryptionPassword
        private string encryptionPassword;
        public string EncryptionPassword
        {
            get
            {
                if (encryptionPassword == null)
                    encryptionPassword = "";
                return encryptionPassword;
            }
            set
            {
                encryptionPassword = value;
            }
        }
        #endregion
    }
}
