using SaveSystem.Internal;
using SaveSystem.Settings;
using System;
using System.Collections.Generic;
using System.IO;

namespace SaveSystem
{
    public class FileSave : SaveScript, ISaveRaw
    {
        private FileLocation location;
        public FileLocation Location
        {
            get { return location; }
            set
            {
                try { ValidateLocation(value); }
                catch (Exception ex) { throw ex; }

                location = value;
            }
        }

        private Serializer serializer;

        #region CONSTRUCTOR
        public FileSave(FileLocation location)
        {
            try { ValidateLocation(location); }
            catch (Exception ex) { throw ex; }

            this.location = location;
            this.serializer = new Serializer();
        }
        public FileSave(FileLocation location, SerializerSettings settings)
        {
            try { ValidateLocation(location); }
            catch (Exception ex) { throw ex; }

            if (settings == null)
                throw new ArgumentNullException("settings");
            this.location = location;
            this.serializer = new Serializer(settings);
        }
        #endregion

        #region PRIVATE METHOD ValidateLocation
        private void ValidateLocation(FileLocation location)
        {
            if (location == null)
                throw new ArgumentNullException("location");
            if (location.Path == null)
                throw new NullReferenceException("Property cannot be null. Property name: Location.Path");
        }
        #endregion

        #region PRIVATE METHOD WriteFileContent
        private void WriteFileContent(string key, string content)
        {
            try
            {
                File.WriteAllText(GetAbsoluteFilePath(key), content);
            }
            catch { throw; }
        }
        #endregion

        #region PRIVATE METHOD ReadFileContent
        private string ReadFileContent(string key)
        {
            try
            {
                return File.ReadAllText(GetAbsoluteFilePath(key));
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
                WriteFileContent(key, serializer.Serialize(value));
            }
            catch { throw; }
        }
        #endregion

        #region METHOD LoadInternal
        protected override T LoadInternal<T>(string key)
        {
            try
            {
                return (T)serializer.Deserialize<T>(ReadFileContent(key));
            }
            catch { throw; }
        }
        #endregion

        #region METHOD LoadIntoInternal
        protected override void LoadIntoInternal<T>(string key, T obj)
        {
            try
            {
                serializer.DeserializeInto(ReadFileContent(key), obj);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD HasKeyInternal
        protected override bool HasKeyInternal(string key)
        {
            try
            {
                return File.Exists(GetAbsoluteFilePath(key));
            }
            catch { throw; }
        }
        #endregion

        #region METHOD DeleteKeyInternal
        protected override void DeleteKeyInternal(string key)
        {
            try
            {
                File.Delete(GetAbsoluteFilePath(key));
            }
            catch { throw; }
        }
        #endregion

        #region METHOD GetKeys
        public override List<string> GetKeys()
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(location.Path);
                FileInfo[] files = directory.GetFiles();

                List<string> keys = new List<string>();
                foreach (FileInfo file in files)
                    keys.Add(file.Name);

                return keys;
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #endregion

        #region REGION ISaveRaw implementation
        public void SaveRawString(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");
            try
            {
                WriteFileContent(key, value);
            }
            catch
            {
                throw;
            }
        }

        public void SaveRawBytes(string key, byte[] value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            try
            {
                File.WriteAllBytes(GetAbsoluteFilePath(key), value);
            }
            catch
            {
                throw;
            }
        }

        public string LoadRawString(string key)
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
                throw new KeyNotFoundException("key");

            try
            {
                return ReadFileContent(key);
            }
            catch { throw; }
        }

        public byte[] LoadRawBytes(string key)
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
                return File.ReadAllBytes(GetAbsoluteFilePath(key));
            }
            catch { throw; }
        }
        #endregion

        #region METHOD GetAbsoluteFilePath
        public string GetAbsoluteFilePath(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            try
            {
                return Path.Combine(location.Path, fileName);
            }
            catch { throw; }
        }
        #endregion
    }
}