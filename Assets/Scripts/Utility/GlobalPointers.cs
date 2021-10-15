using System.Collections;
using UnityEngine;

namespace Chess.Core {
    public class GlobalPointers : Singleton<GlobalPointers> {
        public static Camera mainCamera;

        private static PlayerPointer[] playerPointers;
        [SerializeField] PlayerPointer[] _playerPointers;
        
        private void Awake() {
            mainCamera = Camera.main;
            playerPointers = _playerPointers;
        }

        public static PlayerPointer GetPlayerPointer(PlayerType playerType) {
            int num = (int)playerType;
            if (num >= playerPointers.Length) Debug.LogError("there is not a player pointer for the selected player type");
            return playerPointers[num];
        }
    }
}