using SaveSystem.Compression;

namespace SaveSystem.Settings
{
    public struct CompressionSettings {
        
        public bool UseCompression { get; set; }

        #region PROPERTY Compression
        private CompressionScript compression;
        public CompressionScript Compression
        {
            get
            {
                if (compression == null)
                    compression = CompressionScript.GZip;
                return compression;
            }
            set
            {
                compression = value;
            }
        }
        #endregion
    }
}
