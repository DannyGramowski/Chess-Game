using SaveSystem.Internal;
using SaveSystem.Settings;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    public class PlayerPrefsSave : SaveScript
    {
        private Serializer serializer;
        private PlayerPrefsList keyList;

        #region CONSTRUCTOR
        public PlayerPrefsSave()
        {
            this.serializer = new Serializer();
            keyList = new PlayerPrefsList("playerprefs-keys");
        }
        public PlayerPrefsSave(SerializerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            this.serializer = new Serializer(settings);
            keyList = new PlayerPrefsList("playerprefs-keys");
        }
        #endregion

        #region REGION SaveScript implementation
        #region METHOD SaveInternal
        protected override void SaveInternal<T>(string key, T value)
        {
            if (!HasKey(key))
                keyList.Add(key);
            PlayerPrefs.SetString(key, serializer.Serialize(value));
        }
        #endregion

        #region METHOD LoadInternal
        protected override T LoadInternal<T>(string key)
        {
            try
            {
                return (T)serializer.Deserialize<T>(PlayerPrefs.GetString(key));
            }
            catch { throw; }
        }
        #endregion

        #region METHOD LoadIntoInternal
        protected override void LoadIntoInternal<T>(string key, T obj)
        {
            try
            {
                serializer.DeserializeInto(PlayerPrefs.GetString(key), obj);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD HasKeyInternal
        protected override bool HasKeyInternal(string key)
        {
            return keyList.Contains(key);
        }
        #endregion

        #region METHOD DeleteKeyInternal
        protected override void DeleteKeyInternal(string key)
        {
            keyList.Remove(key);
            PlayerPrefs.DeleteKey(key);
        }
        #endregion

        #region METHOD GetKeys
        public override List<string> GetKeys()
        {
            return keyList.GetElements();
        }
        #endregion
        #endregion

        #region PRIVATE CLASS PlayerPrefsList
        private class PlayerPrefsList
        {
            private string listKey;
            private int Count
            {
                get { return PlayerPrefs.GetInt(listKey + ".count", 0); }
                set { PlayerPrefs.SetInt(listKey + ".count", value); }
            }

            #region CONSTRUCTOR
            public PlayerPrefsList(string listKey)
            {
                this.listKey = listKey;
            }
            #endregion

            #region PRIVATE METHOD GetElement
            private string GetElement(int index)
            {
                return PlayerPrefs.GetString(listKey + "-" + index);
            }
            #endregion

            #region PRIVATE METHOD SetElement
            private void SetElement(int index, string value)
            {
                PlayerPrefs.SetString(listKey + "-" + index, value);
            }
            #endregion

            #region PRIVATE METHOD DeleteElement
            private void DeleteElement(int index)
            {
                PlayerPrefs.DeleteKey(listKey + "-" + index);
            }
            #endregion

            #region PRIVATE METHOD GetElementIndex
            private int GetElementIndex(string element)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (GetElement(i) == element)
                        return i;
                }
                return -1;
            }
            #endregion

            #region METHOD Contains
            public bool Contains(string element)
            {
                return GetElementIndex(element) >= 0;
            }
            #endregion

            #region METHOD GetElements
            public List<string> GetElements()
            {
                List<string> elements = new List<string>();
                for (int i = 0; i < Count; i++)
                    elements.Add(GetElement(i));
                return elements;
            }
            #endregion

            #region METHOD Clear
            public void Clear()
            {
                for (int i = Count - 1; i >= 0; i--)
                    DeleteElement(i);
                Count = 0;
            }
            #endregion

            #region METHOD Remove
            public void Remove(string element)
            {
                int elementIndex = GetElementIndex(element);
                if (elementIndex >= 0)
                {
                    for (int i = elementIndex; i < Count; i++)
                    {
                        if (i < Count - 1)
                            SetElement(i, GetElement(i + 1));
                    }
                    DeleteElement(Count - 1);
                    Count = Count - 1;
                }
            }
            #endregion

            #region METHOD Add
            public void Add(string element)
            {
                SetElement(Count, element);
                Count = Count + 1;
            }
            #endregion
        }
        #endregion
    }
}
