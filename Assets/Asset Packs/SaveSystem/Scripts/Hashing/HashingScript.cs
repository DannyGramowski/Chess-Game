using SaveSystem.Internal;
using System;
using System.Security.Cryptography;

namespace SaveSystem.Hashing
{
    public abstract class HashingScript
    {
        private RawDataConverter rawConverter;
        protected HashAlgorithm hashAlgorithm;

        #region PROPERTY SHA512
        public static HashingScript SHA512
        {
            get
            {
                return new SHA512HashComputation();
            }
        }
        #endregion

        #region PROPERTY MD5
        public static HashingScript MD5
        {
            get
            {
                return new MD5HashComputation();
            }
        }
        #endregion

        #region CONSTRUCTOR
        public HashingScript()
        {
            rawConverter = new RawDataConverter();
        }
        #endregion

        #region METHOD ComputeHashBytes
        public byte[] ComputeHashBytes(string input, string salt)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (salt == null)
                throw new ArgumentNullException("salt");
            return hashAlgorithm.ComputeHash(
                rawConverter.Encode(input + salt)
            );
        }
        #endregion

        #region METHOD ComputeHash
        public string ComputeHash(string input, string salt)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (salt == null)
                throw new ArgumentNullException("salt");
            return rawConverter.ConvertToBase64(
                ComputeHashBytes(input, salt)
            );
        }
        #endregion
    }
}
