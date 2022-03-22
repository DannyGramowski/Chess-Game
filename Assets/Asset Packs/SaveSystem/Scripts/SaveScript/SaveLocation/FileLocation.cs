using UnityEngine;

namespace SaveSystem
{
    public class FileLocation : SaveLocation
    {
        private string path;
        public string Path { get { return path; } }

        #region CONSTRUCTOR
        public FileLocation(string path)
        {
            this.path = path;
        }
        #endregion
    }
}
