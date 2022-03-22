using SaveSystem.Cloud;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

namespace SaveSystem.Examples
{
    public class CloudExample : MonoBehaviour
    {
        //Change these values to make it work
        private const string SERVER = "www.myserver.com";
        private const string SERVER_USERNAME = "my-user";
        private const string SERVER_PASSWORD = "my-password";

        private string localFilePath;

        private FtpManager ftpManager;
        private FtpManager.ProgressAction progressAction;
        private bool isConnected = false;

        private Thread action;

        private void Start()
        {
            //Create local file which will be uploaded later
            CreateLocalFile();

            NetworkCredential networkCredential = new NetworkCredential(SERVER_USERNAME, SERVER_PASSWORD, SERVER);
            ftpManager = new FtpManager(networkCredential);

            //In case your FTP server does not support the standard way of getting the file size.
            ftpManager.UseAlternativeFileSizeMethod = true;

            //Everytime the upload or download progress changes the function OnProgress(float progress) is called.  
            progressAction = OnProgress;

            //StartCoroutine to prevent lagging
            StartCoroutine(CheckIfConnected());
        }

        #region CREATE LOCAL FILE
        private void CreateLocalFile()
        {
            FileLocation location = new FileLocation(Application.persistentDataPath);
            Debug.Log("Created local file at: " + location.Path);

            FileSave fileSave = new FileSave(location);

            //Create a 2.5MB file to make the upload progress recognizable
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 2500000; i++)
                stringBuilder.Append('A');

            string fileName = "file.txt";
            localFilePath = Path.Combine(location.Path, fileName);

            fileSave.Save(fileName, stringBuilder.ToString());
        }
        #endregion

        #region CHECK IF CONNECTED
        private IEnumerator CheckIfConnected()
        {
            //Wait a bit to prevent lagging
            yield return new WaitForSeconds(0.1f);

            //Is the server able to respond within the timeout limit?
            isConnected = ftpManager.IsConnected();
            Debug.Log("Connected to server: " + isConnected);
        }
        #endregion

        #region UPLOAD FILE
        public void UploadFile()
        {
            //StartCoroutine to prevent lagging
            StartCoroutine(CoroutineUploadFile());
        }
        #endregion

        #region COROUTINE UPLOAD FILE
        private IEnumerator CoroutineUploadFile()
        {
            //Wait a bit to prevent lagging
            yield return new WaitForSeconds(0.1f);

            if (isConnected)
            {
                //Create a new directory
                if (!ftpManager.DirectoryExists("Example"))
                {
                    Debug.Log("Creating directory on the server...");
                    ftpManager.CreateDirectory("Example", "");
                }

                //Upload the local file
                Debug.Log("Uploading File...");
                currentProgress = 0;

                //Run in a new thread to cancel it at a later time
                action = new Thread(() =>
                {
                    try
                    {
                        ftpManager.UploadFile(localFilePath, "Example/uploaded.txt", progressAction);
                    }
                    catch (Exception e) { Debug.LogError(e); }
                });
                action.Start();
            }
            else
                Debug.Log("The file could not be uploaded because the connection to the server failed.");
        }
        #endregion

        #region DOWNLOAD FILE
        public void DownloadFile()
        {
            //StartCoroutine to prevent lagging
            StartCoroutine(CoroutineDownloadFile());
        }
        #endregion

        #region COROUTINE DOWNLOAD FILE
        private IEnumerator CoroutineDownloadFile()
        {
            //Wait a bit to prevent lagging
            yield return new WaitForSeconds(0.1f);

            if (isConnected)
            {
                //Does the file exist on the server?
                bool fileExists = ftpManager.FileExists("Example/uploaded.txt");

                if (fileExists)
                {
                    Debug.Log("Downloading File...");
                    currentProgress = 0;

                    //Run in a new thread to cancel it at a later time
                    action = new Thread(() =>
                    {
                        try
                        {
                            ftpManager.DowloadFile(localFilePath, "Example/uploaded.txt", progressAction);
                        }
                        catch (Exception e) { Debug.LogError(e); }
                    });
                    action.Start();
                }
                else
                    Debug.Log("The file could not be downloaded because it does not exist on the server.");
            }
            else
                Debug.Log("The file could not be downloaded because the connection to the server failed.");
        }
        #endregion

        #region PROGRESS
        private int currentProgress;
        private void OnProgress(float progress)
        {
            int progressPercent = (int)(progress * 100);
            //Update every 10%
            if (progressPercent >= currentProgress + 10)
            {
                currentProgress = (int)(progress * 100);
                Debug.Log("Progress: " + currentProgress + "%");
            }
            if (progress == 1)
                Debug.Log("Progress 100% - Done!");
        }
        #endregion

        #region CANCEL
        public void Cancel()
        {
            if (action != null)
                action.Abort();
        }
        #endregion
    }
}