using System;
using System.Text;

namespace SaveSystem.Internal
{
    public class RawDataConverter
    {
        private Encoding encoding;

        #region CONSTRUCTOR
        public RawDataConverter()
        {
            encoding = Encoding.UTF8;
        }
        #endregion

        #region METHOD Encode
        public byte[] Encode(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return encoding.GetBytes(value);
        }
        #endregion

        #region METHOD Decode
        public string Decode(byte[] value)
        {
            if(value == null)
                throw new ArgumentNullException("value");
            try
            {
                return encoding.GetString(value);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region METHOD ConvertToBase64
        public string ConvertToBase64(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return Convert.ToBase64String(value);
        }
        #endregion

        #region METHOD ConvertFromBase64
        public byte[] ConvertFromBase64(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            try
            {
                return Convert.FromBase64String(value);
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
