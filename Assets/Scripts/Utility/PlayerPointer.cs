using System.Collections;
using UnityEngine;
using Chess.UI;
using Chess.Combat;
using Mirror;
using System.Collections.Generic;

namespace Chess.Core{
    public class PlayerPointer : MonoBehaviour {
        public PatternSelectionManager patternSelectionManager { get; private set; }
        public InputManager inputManager { get; private set; }
        public PlayerType playerType => player.playerType;

        public Player player { get; private set; }

        private Matrix matrix;

        [SerializeField] InputManager _inputManager;
        #region Server
         //need to do SetPlayerPieces in start because NetworkIdentity is set after Awake
        public void Start() {
            player = GetComponent<Player>();
            matrix = GlobalPointers.matrix;
        }

       
        #endregion

        #region Client
         public void Awake() {
            if (matrix != null) return;//prevents method being called twice

            patternSelectionManager = FindObjectOfType<PatternSelectionManager>(true);
            //patternSelectionManager.SetPlayerType(playerType);
            inputManager = _inputManager;
            //inputManager.SetPlayerType(playerType);
            matrix = FindObjectOfType<Matrix>();
         }
        #endregion
    }
}