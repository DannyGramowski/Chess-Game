using System.Collections;
using UnityEngine;

namespace Chess.Core {
    public class GlobalPointers : Singleton<GlobalPointers> {
        public static Camera mainCamera;
        public static bool useDebug;
  
        [SerializeField] bool _useDebug;
        
        private void Awake() {
            mainCamera = Camera.main;
            useDebug = _useDebug;
        }
    }
}