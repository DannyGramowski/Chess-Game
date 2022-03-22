using System.Net;

namespace SaveSystem
{
    public class WebLocation : SaveLocation
    {
        private string localPath;
        public string LocalPath { get { return localPath; } }

        private string serverPath;
        public string ServerPath { get { return serverPath; } }

        private NetworkCredential networkCredential;
        public NetworkCredential NetworkCredential
        {
            get { return networkCredential; }
            set { networkCredential = value; }
        }

        private ushort bufferSize = 2048;
        public ushort BufferSize
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }

        private int timeoutMilliseconds = 8000;
        public int TimeoutMilliseconds {
            get { return timeoutMilliseconds; }
            set { timeoutMilliseconds = value; }
        }

        #region CONSTRUCTOR
        public WebLocation(NetworkCredential credentials, string localPath, string serverPath)
        {
            this.networkCredential = credentials;
            this.localPath = localPath;
            this.serverPath = serverPath;
        }
        #endregion
    }
}
