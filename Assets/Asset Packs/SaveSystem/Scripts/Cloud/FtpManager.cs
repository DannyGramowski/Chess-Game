using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SaveSystem.Cloud
{
    /// <summary>
    /// Manages content on a server
    /// </summary>
    public class FtpManager
    {
        public delegate void ProgressAction(float progress);

        #region PROPERTIES
        private ushort bufferSize = 2048;
        /// <summary>
        /// A file is uploaded and downloaded in x byte parts
        /// </summary>
        public ushort BufferSize
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }

        private NetworkCredential loginCredentials;
        private NetworkCredential networkCredential;
        /// <summary>
        /// Contains url and login credentials for the FTP server
        /// </summary>
        public NetworkCredential NetworkCredential
        {
            get { return networkCredential; }
            set
            {
                try { ValidateNetworkCredential(value); }
                catch (Exception ex) { throw ex; }

                networkCredential = value;
            }
        }

        private int timeoutMilliseconds = 8000;
        /// <summary>
        /// An operation will be interrupted when there is no response for x milliseconds
        /// </summary>
        public int TimeoutMilliseconds
        {
            get { return timeoutMilliseconds; }
            set { timeoutMilliseconds = value; }
        }

        private bool useAlternativeFileSizeMethod = false;
        /// <summary>
        /// Some FTP servers do not support the standard way of getting the size of a file.
        /// This property specifies if alternative (slower) methods should be used.
        /// </summary>
        public bool UseAlternativeFileSizeMethod
        {
            get { return useAlternativeFileSizeMethod; }
            set { useAlternativeFileSizeMethod = value; }
        }
        #endregion

        #region CONSTRUCTOR
        public FtpManager(NetworkCredential credential)
        {
            try { ValidateNetworkCredential(credential); }
            catch (Exception ex) { throw ex; }

            this.networkCredential = credential;
            loginCredentials = new NetworkCredential(credential.UserName, credential.Password);
        }
        #endregion

        #region PRIVATE METHOD ValidateNetworkCredential
        private void ValidateNetworkCredential(NetworkCredential credential)
        {
            if (credential == null)
                throw new ArgumentNullException("credential");
            if (credential.Domain == null)
                throw new NullReferenceException("Property cannot be null. Property name: NetworkCredential.Domain");
            if (credential.UserName == null)
                throw new NullReferenceException("Property cannot be null. Property name: NetworkCredential.UserName");
            if (credential.Password == null)
                throw new NullReferenceException("Property cannot be null. Property name: NetworkCredential.Password");
        }
        #endregion

        #region PRIVATE METHOD GetUrl
        private string GetUrl(string path)
        {
            return "ftp://" + networkCredential.Domain + "/" + path;
        }
        #endregion

        #region PRIVATE METHOD GetWebRequest
        private FtpWebRequest GetWebRequest(string path, string requestMethod)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(GetUrl(path));
            request.Method = requestMethod;
            request.Credentials = loginCredentials;
            request.Timeout = timeoutMilliseconds;
            return request;
        }
        #endregion

        #region PRIVATE METHOD ListContent
        /// <summary>
        /// Gets the content (files and directories) located in directory on the server
        /// </summary>
        private List<string> ListContent(string path)
        {
            List<string> directoryContents = new List<string>();

            try
            {
                FtpWebRequest request = GetWebRequest(path + "/", WebRequestMethods.Ftp.ListDirectory);

                using (var response = (FtpWebResponse)request.GetResponse())
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    while (!streamReader.EndOfStream)
                        directoryContents.Add(streamReader.ReadLine());
                }
            }
            catch
            {
                throw;
            }

            directoryContents.Remove(".");
            directoryContents.Remove("..");

            return directoryContents;
        }
        #endregion

        #region METHOD ListDirectory
        /// <summary>
        /// Gets the content of a directory on the server
        /// </summary>
        public List<string> ListDirectory(string path, FtpSearchOption searchOption)
        {
            try
            {
                List<string> contents = ListContent(path);
                if (searchOption == FtpSearchOption.FilesOrDirectories)
                    return contents;

                List<string> directories = new List<string>();
                List<string> files = new List<string>();

                foreach (string content in contents)
                {
                    if (FileExists(path + "/" + content))
                        files.Add(content);
                    else
                        directories.Add(content);
                }

                if (searchOption == FtpSearchOption.Directories)
                    return directories;
                return files;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region REGION Network Connection
        #region METHOD IsConnected
        /// <summary>
        /// Returns true if the server is able to respond within the timeout limit.
        /// </summary>
        public bool IsConnected()
        {
            FtpWebRequest request;
            try
            {
                request = GetWebRequest("", WebRequestMethods.Ftp.ListDirectory);
            }
            catch { throw; }

            try
            {
                WebResponse response = request.GetResponse();
                response.Close();
            }
            catch { return false; }
            return true;
        }
        #endregion

        #region METHOD GetPing
        /// <summary>
        /// Returns the time in milliseconds it takes the server to respond
        /// </summary>
        public int GetPing()
        {
            try
            {
                FtpWebRequest request = GetWebRequest("", WebRequestMethods.Ftp.ListDirectory);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                WebResponse response = request.GetResponse();

                stopwatch.Stop();
                response.Close();

                return (int)stopwatch.ElapsedMilliseconds;
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #endregion

        #region METHOD GetLastModifiedDate
        /// <summary>
        /// Gets the date when the last modification of a file happened.
        /// </summary>
        public DateTime GetLastModifiedDate(string path)
        {
            try
            {
                FtpWebRequest request = GetWebRequest(path, WebRequestMethods.Ftp.GetDateTimestamp);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                DateTime lastModified = response.LastModified;
                response.Close();
                return lastModified;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region METHOD UploadFile
        /// <summary>
        /// Uploads a file to an FTP server.
        /// </summary>
        /// <param name="progressAction">Is called when the upload progress changes.</param>
        public void UploadFile(string localPath, string serverPath, ProgressAction progressAction = null)
        {
            try
            {
                FileInfo localFile = new FileInfo(localPath);
                FtpWebRequest request = GetWebRequest(serverPath, WebRequestMethods.Ftp.UploadFile);
                request.KeepAlive = false;
                request.UseBinary = true;
                // Notify server about size of uploaded file
                request.ContentLength = localFile.Length;

                byte[] buffer = new byte[bufferSize];
                int contentLength = 0;

                FileStream localStream = localFile.OpenRead();
                Stream serverStream = request.GetRequestStream();

                // Loop until local stream content ends.
                while ((contentLength = localStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    if (progressAction != null)
                        progressAction.Invoke(localStream.Position / (float)localStream.Length);

                    // Write content from local stream to the server.
                    serverStream.Write(buffer, 0, contentLength);
                }

                serverStream.Close();
                localStream.Close();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region METHOD DowloadFile
        /// <summary>
        /// Downloads a file from an FTP server.
        /// </summary>
        /// <param name="progressAction">Is called when the download progress changes.</param>
        public void DowloadFile(string localPath, string serverPath, ProgressAction progressAction = null)
        {
            try
            {
                byte[] buffer = new byte[bufferSize];
                long fileSize = 0;
                if (progressAction != null)
                    fileSize = GetFileSize(serverPath);

                FtpWebRequest request = GetWebRequest(serverPath, WebRequestMethods.Ftp.DownloadFile);
                request.UseBinary = true;

                WebResponse response = request.GetResponse();
                Stream serverStream = response.GetResponseStream();
                FileStream localStream = new FileStream(localPath, FileMode.Create);

                int bytesRead;
                while ((bytesRead = serverStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    localStream.Write(buffer, 0, bytesRead);
                    if (progressAction != null)
                        progressAction.Invoke(localStream.Position / (float)fileSize);
                }
                localStream.Close();
                serverStream.Close();
                response.Close();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region METHOD DeleteFile
        /// <summary>
        /// Deletes a file on the server.
        /// </summary>
        public void DeleteFile(string path)
        {
            try
            {
                FtpWebRequest request = GetWebRequest(path, WebRequestMethods.Ftp.DeleteFile);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region METHOD DeleteDirectoryRecursively
        /// <summary>
        /// Deletes a directory on the server recursively.
        /// </summary>
        public void DeleteDirectoryRecursively(string path)
        {
            List<string> files;
            List<string> subDirectories;
            try
            {
                files = ListDirectory(path, FtpSearchOption.Files);
                subDirectories = ListDirectory(path, FtpSearchOption.Directories);

                foreach (string file in files)
                    DeleteFile(path + "/" + file);
                foreach (string directory in subDirectories)
                    DeleteDirectoryRecursively(path + "/" + directory);

                FtpWebRequest request = GetWebRequest(path, WebRequestMethods.Ftp.RemoveDirectory);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region METHOD GetFileSize
        /// <summary>
        /// Gets the size (in byte) of a file on the server.
        /// </summary>
        public long GetFileSize(string path)
        {
            try
            {
                if (useAlternativeFileSizeMethod)
                {
                    FtpWebRequest request = GetWebRequest(Path.GetDirectoryName(path) + "/", WebRequestMethods.Ftp.ListDirectoryDetails);
                    string fileName = Path.GetFileName(path);

                    if (!FileExists(path))
                        throw new FileNotFoundException("File " + fileName + " was not found at the given path on the server.");

                    using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                    {
                        Regex regex = new Regex(
                            @"^([\w-]+)\s+(\d+)\s+(\w+)\s+(\w+)\s+(\d+)\s+" +
                            @"(\w+\s+\d+\s+\d+|\w+\s+\d+\s+\d+:\d+)\s+(.+)$");

                        string line;
                        while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                        {
                            Match match = regex.Match(line);
                            string name = match.Groups[7].Value;
                            if (name != fileName)
                                continue;

                            return long.Parse(match.Groups[5].Value);
                        }
                        throw new FileNotFoundException("File " + fileName + " was not found at the given path on the server.");
                    }
                }
                else
                {
                    FtpWebRequest request = GetWebRequest(path, WebRequestMethods.Ftp.GetFileSize);
                    request.UseBinary = false;

                    WebResponse response = request.GetResponse();
                    int fileSize = (int)response.ContentLength;
                    response.Close();
                    return fileSize;
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region METHOD FileExists
        /// <summary>
        /// Returns true if there exists a file at the given path on the server
        /// </summary>
        public bool FileExists(string path)
        {
            try
            {
                FtpWebRequest request = GetWebRequest(path, WebRequestMethods.Ftp.GetDateTimestamp);
                WebResponse response = request.GetResponse();
                response.Close();
            }
            catch (WebException exc)
            {
                FtpWebResponse response = (FtpWebResponse)exc.Response;
                if (response == null)
                    throw;

                response.Close();

                //The exception was thrown because the file does not exist
                if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                    throw;

                return false;
            }
            catch
            {
                throw;
            }
            return true;
        }
        #endregion

        #region METHOD DirectoryExists
        /// <summary>
        /// Returns true if there exists a directory at the given path on the server
        /// </summary>
        public bool DirectoryExists(string path)
        {
            try
            {
                WebRequest request = GetWebRequest(path + "/", WebRequestMethods.Ftp.ListDirectory);
                WebResponse response = request.GetResponse();
                response.Close();
            }
            catch (WebException exc)
            {
                FtpWebResponse response = (FtpWebResponse)exc.Response;
                if (response == null)
                    throw;
                response.Close();

                if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailableOrBusy &&
                    response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                    throw;

                return false;
            }
            catch
            {
                throw;
            }
            return true;
        }
        #endregion

        #region METHOD CreateDirectory
        /// <summary>
        /// Creates a directory on the server
        /// </summary>
        public void CreateDirectory(string directoryName, string path)
        {
            try
            {
                FtpWebRequest request = GetWebRequest(path + "/" + directoryName, WebRequestMethods.Ftp.MakeDirectory);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region METHOD Rename
        /// <summary>
        /// Renames a file or directory on a server.
        /// </summary>
        public void Rename(string path, string newName)
        {
            try
            {
                FtpWebRequest request = GetWebRequest(path, WebRequestMethods.Ftp.Rename);
                request.RenameTo = newName;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
