using System;
using System.Collections.Generic;

namespace SaveSystem
{
    public abstract class SaveScript
    {
        protected abstract void SaveInternal<T>(string key, T value);
        #region METHOD Save
        public void Save<T>(string key, T value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            try
            {
                SaveInternal(key, value);
            }
            catch { throw; }
        }
        #endregion

        protected abstract T LoadInternal<T>(string key);
        #region METHOD Load
        public T Load<T>(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            bool hasKey;
            try
            {
                hasKey = HasKey(key);
            }
            catch { throw; }
            if (!hasKey)
                throw new KeyNotFoundException("Key does not exist. Parameter name: key");

            try
            {
                return LoadInternal<T>(key);
            }
            catch { throw; }
        }

        public T Load<T>(string key, T defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            try
            {
                if (!HasKey(key))
                    return defaultValue;
            }
            catch { throw; }

            try
            {
                return LoadInternal<T>(key);
            }
            catch { throw; }
        }
        #endregion

        protected abstract void LoadIntoInternal<T>(string key, T obj) where T : class;
        #region METHOD LoadInto
        public void LoadInto<T>(string key, T obj) where T : class
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (obj == null)
                throw new ArgumentNullException("obj");

            bool hasKey;
            try
            {
                hasKey = HasKey(key);
            }
            catch { throw; }
            if (!hasKey)
                throw new KeyNotFoundException("Key does not exist. Parameter name: key");

            try
            {
                LoadIntoInternal(key, obj);
            }
            catch { throw; }
        }
        #endregion

        protected abstract bool HasKeyInternal(string key);
        #region METHOD HasKey
        public bool HasKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            try
            {
                return HasKeyInternal(key);
            }
            catch { throw; }
        }
        #endregion

        protected abstract void DeleteKeyInternal(string key);
        #region METHOD DeleteKey
        public void DeleteKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            bool hasKey;
            try
            {
                hasKey = HasKey(key);
            }
            catch { throw; }
            if (!hasKey)
                throw new KeyNotFoundException("Key does not exist. Parameter name: key");

            try
            {
                DeleteKeyInternal(key);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD DeleteAll
        public void DeleteAll()
        {
            try
            {
                foreach (string key in GetKeys())
                    DeleteKeyInternal(key);
            }
            catch { throw; }
        }
        #endregion

        public abstract List<string> GetKeys();
    }
}
