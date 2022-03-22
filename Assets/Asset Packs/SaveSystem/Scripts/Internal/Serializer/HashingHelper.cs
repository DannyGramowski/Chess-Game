using SaveSystem.Exceptions;
using SaveSystem.Settings;
using System.Text;

namespace SaveSystem.Internal
{
    public class HashingHelper
    {
        private DataModificationSettings settings;

        #region CONSTRUCTOR
        public HashingHelper(DataModificationSettings settings)
        {
            this.settings = settings;
        }
        #endregion

        #region METHOD ComputeHash
        public string ComputeHash(string value)
        {
            return settings.Hashing.ComputeHash(
                value,
                settings.HashSalt
            );
        }
        #endregion

        #region METHOD ExtractHash
        public string ExtractHash(string value)
        {
            StringBuilder hash = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '<')
                    return hash.ToString();
                hash.Append(value[i]);
            }
            return hash.ToString();
        }
        #endregion

        #region METHOD CheckHash 
        public void CheckHash(string hash, string existing)
        {
            if (hash != ComputeHash(existing))
                throw new DataModifiedException("The game data was modified.");
        }
        #endregion
    }
}
