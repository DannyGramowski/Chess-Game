using SaveSystem.Hashing;
using SaveSystem.Internal;
using System;
using System.IO;
using System.Security.Cryptography;

namespace SaveSystem.Encryption
{
    public abstract class EncryptionScript
    {
        private RawDataConverter rawConverter;
        private HashingScript hashingScript;

        protected SymmetricAlgorithm cryptoService;

        #region CONSTRUCTOR
        public EncryptionScript()
        {
            this.rawConverter = new RawDataConverter();
            this.hashingScript = HashingScript.MD5;
        }
        #endregion

        #region PROPERTY DES
        public static EncryptionScript DES
        {
            get
            {
                return new DesEncryption();
            }
        }
        #endregion

        #region METHOD Encrypt
        public string Encrypt(string input, string password)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (password == null)
                throw new ArgumentNullException("password");

            return rawConverter.ConvertToBase64(
                EncryptBytes(rawConverter.Encode(input), password)
            );
        }
        #endregion

        #region METHOD Decrypt
        public string Decrypt(string input, string password)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (password == null)
                throw new ArgumentNullException("password");

            try
            {
                return rawConverter.Decode(
                    DecryptBytes(rawConverter.ConvertFromBase64(input), password)
                );
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region PRIVATE METHOD EncryptBytes
        private byte[] EncryptBytes(byte[] input, string password)
        {
            byte[] key = hashingScript.ComputeHashBytes(password, "salt");
            byte[] iv = hashingScript.ComputeHashBytes(password, "salt");

            return Transform(input, cryptoService.CreateEncryptor(key, iv));
        }
        #endregion

        #region PRIVATE METHOD DecryptBytes
        private byte[] DecryptBytes(byte[] input, string password)
        {
            try
            {
                byte[] key = hashingScript.ComputeHashBytes(password, "salt");
                byte[] iv = hashingScript.ComputeHashBytes(password, "salt");

                return Transform(input, cryptoService.CreateDecryptor(key, iv));
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region PRIVATE METHOD Transform
        private byte[] Transform(byte[] input, ICryptoTransform CryptoTransform)
        {
            try
            {
                MemoryStream memStream = new MemoryStream();
                CryptoStream cryptStream = new CryptoStream(memStream, CryptoTransform, CryptoStreamMode.Write);

                cryptStream.Write(input, 0, input.Length);
                cryptStream.FlushFinalBlock();

                memStream.Position = 0;
                byte[] result = new byte[Convert.ToInt32(memStream.Length)];
                memStream.Read(result, 0, Convert.ToInt32(result.Length));

                memStream.Close();
                cryptStream.Close();

                return result;
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
