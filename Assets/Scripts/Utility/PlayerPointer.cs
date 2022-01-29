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
        public PlayerSquad playerSquad { get; private set; }

        public PlayerType playerType => player.playerType;

        public Player player { get; private set; }

        private Matrix matrix;

        [SerializeField] InputManager _inputManager;
        [SerializeField] PlayerSquad _playerSquad;
        #region Server
         //need to do SetPlayerPieces in start because NetworkIdentity is set after Awake
        public void Start() {
            // print(connectionToServer.identity + " on server start ");
            //  print("on server start, num players is " + (NetworkServer.connections.Count - 1));
            player = GetComponent<Player>();
            matrix = GlobalPointers.matrix;
            //playerType = (PlayerType) player.playerNum;
            //matrix.SetPlayerPieces(SpawnSquadUnits(), playerType);
        }

       
        #endregion

        #region Client
         public void Awake() {
            if (matrix != null) return;//prevents method being called twice

            patternSelectionManager = FindObjectOfType<PatternSelectionManager>(true);
            //patternSelectionManager.SetPlayerType(playerType);
            inputManager = _inputManager;
            //inputManager.SetPlayerType(playerType);
            //disable then enable in game scene
            playerSquad = _playerSquad;
            matrix = FindObjectOfType<Matrix>();
         }
        #endregion
       

        

       
        
        
    }
}