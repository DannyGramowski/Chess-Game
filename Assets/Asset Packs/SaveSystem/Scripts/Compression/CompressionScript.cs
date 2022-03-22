using SaveSystem.Internal;
using System;

namespace SaveSystem.Compression
{
    public abstract class CompressionScript
    {
        private RawDataConverter rawConverter;

        #region CONSTRUCTOR
        public CompressionScript()
        {
            rawConverter = new RawDataConverter();
        }
        #endregion

        #region PROPERTY GZip
        public static CompressionScript GZip
        {
            get
            {
                return new GZipCompression();
            }
        }
        #endregion

        #region METHOD Compress
        public string Compress(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            return rawConverter.ConvertToBase64(
                CompressBytes(rawConverter.Encode(input))
            );
        }
        #endregion

        #region METHOD Decompress
        public string Decompress(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            try
            {
                return rawConverter.Decode(
                    DecompressBytes(rawConverter.ConvertFromBase64(input))
                );
            }
            catch
            {
                throw;
            }
        }
        #endregion

        protected abstract byte[] CompressBytes(byte[] input);
        protected abstract byte[] DecompressBytes(byte[] input);
    }
}