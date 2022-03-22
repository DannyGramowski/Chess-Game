using Hson;
using Hson.Utility;

namespace SaveSystem.Settings {
    public class SerializerSettings
    {
        private DataModificationSettings dataModificationSettings;
        public DataModificationSettings DataModificationSettings {
            get { return dataModificationSettings; }
            set { dataModificationSettings = value; }
        }

        private CompressionSettings compressionSettings;
        public CompressionSettings CompressionSettings {
            get { return compressionSettings; }
            set { compressionSettings = value; }
        }

        private EncryptionSettings encryptionSettings;
        public EncryptionSettings EncryptionSettings {
            get { return encryptionSettings; }
            set { encryptionSettings = value; }
        }

        private HsonOptions hsonOptions;
        public HsonOptions HsonOptions {
            get { return hsonOptions; }
            set { hsonOptions = value; }
        }

        #region PROPERTY Default
        public static SerializerSettings Default
        {
            get
            {
                return new SerializerSettings(
                    new DataModificationSettings(),
                    new CompressionSettings(),
                    new EncryptionSettings(),
                    HsonOptions.Default);
            }
        }
        #endregion

        #region CONSTRUCTOR
        private SerializerSettings(
            DataModificationSettings dataModificationSettings,
            CompressionSettings compressionSettings,
            EncryptionSettings encryptionSettings,
            HsonOptions hsonOptions)
        {
            this.dataModificationSettings = dataModificationSettings;
            this.compressionSettings = compressionSettings;
            this.encryptionSettings = encryptionSettings;
            this.hsonOptions = hsonOptions;
        }
        #endregion
    }
}
