using SaveSystem.Hashing;

namespace SaveSystem.Settings {
    public struct DataModificationSettings
    {
        public bool MakeDataImmutable { get; set; }

        #region PROPERTY Hashing
        private HashingScript hashing;
        public HashingScript Hashing
        {
            get
            {
                if (hashing == null)
                    hashing = HashingScript.MD5;
                return hashing;
            }
            set
            {
                hashing = value;
            }
        }
        #endregion

        #region PROPERTY HashSalt
        private string hashSalt;
        public string HashSalt
        {
            get
            {
                if (hashSalt == null)
                    hashSalt = "";
                return hashSalt;
            }
            set
            {
                hashSalt = value;
            }
        }
        #endregion
    }
}
