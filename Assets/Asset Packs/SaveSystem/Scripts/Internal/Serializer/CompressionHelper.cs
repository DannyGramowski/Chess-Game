using SaveSystem.Settings;

namespace SaveSystem.Internal
{
    public class CompressionHelper
    {
        private CompressionSettings settings;

        #region CONSTRUCTOR
        public CompressionHelper(CompressionSettings settings)
        {
            this.settings = settings;
        }
        #endregion

        #region METHOD Compress
        public string Compress(string value)
        {
            return settings.Compression.Compress(value);
        }
        #endregion

        #region METHOD Decompress
        public string Decompress(string value)
        {
            try
            {
                return settings.Compression.Decompress(value);
            }
            catch { throw; }
        }
        #endregion
    }
}