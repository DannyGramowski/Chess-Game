using SaveSystem.Settings;

namespace SaveSystem.Internal
{
    public class EncryptionHelper
    {
        private EncryptionSettings settings;

        #region CONSTRUCTOR
        public EncryptionHelper(EncryptionSettings settings)
        {
            this.settings = settings;
        }
        #endregion

        #region METHOD Encrypt
        public string Encrypt(string value)
        {
            return settings.Encryption.Encrypt(
                value, settings.EncryptionPassword
            );
        }
        #endregion

        #region METHOD Decrypt
        public string Decrypt(string value)
        {
            try
            {
                return settings.Encryption.Decrypt(
                    value, settings.EncryptionPassword
                );
            }
            catch { throw; }
        }
        #endregion
    }
}
