using System.Collections;
using UnityEngine;

namespace Chess.Lobby {
    public class FileDisplay : MonoBehaviour {
        string filePath;
       
        public void SetFilePath(string newFilePath) {
            filePath = newFilePath;
        }

        public string GetFilePath() => filePath;

    }
}