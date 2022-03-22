using SaveSystem.Settings;
using System;
using System.Collections.Generic;

namespace SaveSystem
{
    public class SaveUtility : SaveScript
    {
        private SaveScript saveScript;

        private SaveLocation location;
        #region PROPERTY Location { get; set; }
        public SaveLocation Location
        {
            get { return location; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                location = value;
                try
                {
                    saveScript = GetSaveScript();
                }
                catch { throw; }
            }
        }
        #endregion

        private SerializerSettings settings;
        #region PROPERTY Settings { get; set; }
        public SerializerSettings Settings
        {
            get { return settings; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                settings = value;
                saveScript = GetSaveScript();
            }
        }
        #endregion

        #region CONSTRUCTOR
        public SaveUtility(SaveLocation location)
        {
            if (location == null)
                throw new ArgumentNullException("location");
            this.location = location;
            this.settings = SerializerSettings.Default;
            try
            {
                saveScript = GetSaveScript();
            }
            catch { throw; }
        }
        public SaveUtility(SaveLocation location, SerializerSettings settings)
        {
            if (location == null)
                throw new ArgumentNullException("location");
            if (settings == null)
                throw new ArgumentNullException("options");
            this.location = location;
            this.settings = settings;
            try
            {
                saveScript = GetSaveScript();
            }
            catch { throw; }
        }
        #endregion

        #region REGION SaveScript implementation
        #region METHOD SaveInternal
        protected override void SaveInternal<T>(string key, T value)
        {
            try
            {
                saveScript.Save(key, value);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD LoadInternal
        protected override T LoadInternal<T>(string key)
        {
            try
            {
                return saveScript.Load<T>(key);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD LoadIntoInternal
        protected override void LoadIntoInternal<T>(string key, T obj)
        {
            try
            {
                saveScript.LoadInto(key, obj);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD HasKeyInternal
        protected override bool HasKeyInternal(string key)
        {
            try
            {
                return saveScript.HasKey(key);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD DeleteKeyInternal
        protected override void DeleteKeyInternal(string key)
        {
            try
            {
                saveScript.DeleteKey(key);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD GetKeys
        public override List<string> GetKeys()
        {
            try
            {
                return saveScript.GetKeys();
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region PRIVATE METHOD GetSaveScript
        private SaveScript GetSaveScript()
        {
            if (location is PlayerPrefsLocation)
                return new PlayerPrefsSave(settings);
            if (location is FileLocation)
                return new FileSave((FileLocation)location, settings);
            if (location is WebLocation)
                return new WebSave((WebLocation)location, settings);
            throw new NotImplementedException("Saving at this location is not implemented. Property name: Location");
        }
        #endregion
    }
}
