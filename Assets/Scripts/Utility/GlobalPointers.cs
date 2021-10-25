using System.Collections;
using UnityEngine;

namespace Chess.Core {
    public class GlobalPointers : Singleton<GlobalPointers> {
        public static Camera mainCamera;
        public static bool useDebug;

        private static PlayerPointer[] playerPointers;
        
        [SerializeField] PlayerPointer[] _playerPointers;
        [SerializeField] bool _useDebug;
        
        private void Awake() {
            mainCamera = Camera.main;
            playerPointers = _playerPointers;
            useDebug = _useDebug;
        }

        public static PlayerPointer GetPlayerPointer(PlayerType playerType) {
            int num = (int)playerType;
            if (num >= playerPointers.Length) Debug.LogError("there is not a player pointer for the selected player type");
            return playerPointers[num];
        }
    }
}