using SaveSystem.Cloud;
using SaveSystem.Settings;
using System;
using System.Collections.Generic;

namespace SaveSystem
{
    public class WebSave : SaveScript, ISaveRaw
    {
        private WebLocation location;
        public WebLocation Location
        {
            get { return location; }
            set
            {
                try
                {
                    ValidateLocation(value);
                }
                catch (Exception ex) { throw ex; }

                location = value;
            }
        }

        private FtpManager ftpManager;
        private FileSave fileSave;

        #region CONSTRUCTOR
        public WebSave(WebLocation location)
        {
            try
            {
                ValidateLocation(location);
            }
            catch (Exception ex) { throw ex; }

            this.location = location;
            this.ftpManager = new FtpManager(location.NetworkCredential);
            this.fileSave = new FileSave(new FileLocation(location.LocalPath));
        }
        public WebSave(WebLocation location, SerializerSettings settings)
        {
            try
            {
                ValidateLocation(location);
            }
            catch (Exception ex) { throw ex; }

            if (settings == null)
                throw new ArgumentNullException("options");
            this.location = location;
            this.ftpManager = new FtpManager(location.NetworkCredential);
            this.fileSave = new FileSave(new FileLocation(location.LocalPath), settings);
        }
        #endregion

        #region PRIVATE METHOD ValidateLocation
        private void ValidateLocation(WebLocation location)
        {
            if (location == null)
                throw new ArgumentNullException("location");
            if (location.ServerPath == null)
                throw new NullReferenceException("Property cannot be null. Property name: Location.ServerPath");
            if (location.LocalPath == null)
                throw new NullReferenceException("Property cannot be null. Property name: Location.LocalPath");
            if (location.NetworkCredential == null)
                throw new NullReferenceException("Property cannot be null. Property name: Location.NetworkCredential");
            if (location.NetworkCredential.Domain == null)
                throw new NullReferenceException("Property cannot be null. Property name: Location.NetworkCredential.Domain");
            if (location.NetworkCredential.Password == null)
                throw new NullReferenceException("Property cannot be null. Property name: Location.NetworkCredential.Password");
            if (location.NetworkCredential.UserName == null)
                throw new NullReferenceException("Property cannot be null. Property name: Location.NetworkCredential.UserName");
        }
        #endregion

        #region PRIVATE METHOD RefreshFtpManager
        private void RefreshFtpManager()
        {
            ftpManager.TimeoutMilliseconds = location.TimeoutMilliseconds;
            ftpManager.BufferSize = location.BufferSize;
            ftpManager.NetworkCredential = location.NetworkCredential;
        }
        #endregion

        #region PRIVATE METHOD GetAbsoluteFilePath
        private string GetAbsoluteFilePath(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            return location.ServerPath + "/" + fileName;
        }
        #endregion

        #region REGION SaveScript implementation
        #region METHOD SaveInternal
        protected override void SaveInternal<T>(string key, T value)
        {
            RefreshFtpManager();
            try
            {
                fileSave.Save(key, value);
                ftpManager.UploadFile(
                    fileSave.GetAbsoluteFilePath(key),
                    GetAbsoluteFilePath(key));
            }
            catch { throw; }
        }
        #endregion

        #region METHOD LoadInternal
        protected override T LoadInternal<T>(string key)
        {
            RefreshFtpManager();
            try
            {
                ftpManager.DowloadFile(
                    fileSave.GetAbsoluteFilePath(key),
                    GetAbsoluteFilePath(key));
                return fileSave.Load<T>(key);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD LoadIntoInternal
        protected override void LoadIntoInternal<T>(string key, T obj)
        {
            RefreshFtpManager();
            try
            {
                ftpManager.DowloadFile(
                    fileSave.GetAbsoluteFilePath(key),
                    GetAbsoluteFilePath(key));
                fileSave.Load<T>(key);
            }
            catch { throw; }
        }
        #endregion

        #region METHOD HasKeyInternal
        protected override bool HasKeyInternal(string key)
        {
            RefreshFtpManager();
            try
            {
                return ftpManager.FileExists(GetAbsoluteFilePath(key));
            }
            catch { throw; }
        }
        #endregion

        #region METHOD DeleteKeyInternal
        protected override void DeleteKeyInternal(string key)
        {
            RefreshFtpManager();
            try
            {
                ftpManager.DeleteFile(GetAbsoluteFilePath(key));
            }
            catch { throw; }
        }
        #endregion

        #region METHOD DeleteAll
        public new void DeleteAll()
        {
            RefreshFtpManager();
            try
            {
                base.DeleteAll();
            }
            catch { throw; }
        }
        #endregion

        #region METHOD GetKeys
        public override List<string> GetKeys()
        {
            RefreshFtpManager();
            try
            {
                return ftpManager.ListDirectory(location.ServerPath, FtpSearchOption.Files);
            }
            catch { throw; }
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
            RefreshFtpManager();
            try
            {
                fileSave.SaveRawString(key, value);
                ftpManager.UploadFile(
                    fileSave.GetAbsoluteFilePath(key),
                    location.ServerPath + "/" + key);
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
            RefreshFtpManager();
            try
            {
                fileSave.SaveRawBytes(key, value);
                ftpManager.UploadFile(
                    fileSave.GetAbsoluteFilePath(key),
                    location.ServerPath + "/" + key);
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
            RefreshFtpManager();

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
                ftpManager.DowloadFile(
                    fileSave.GetAbsoluteFilePath(key),
                    GetAbsoluteFilePath(key));
                return fileSave.LoadRawString(key);
            }
            catch
            {
                throw;
            }
        }

        public byte[] LoadRawBytes(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            RefreshFtpManager();

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
                ftpManager.DowloadFile(
                    fileSave.GetAbsoluteFilePath(key),
                    GetAbsoluteFilePath(key));
                return fileSave.LoadRawBytes(key);
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
