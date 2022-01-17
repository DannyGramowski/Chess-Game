using System.Collections;
using UnityEngine;
using Chess.UI;
using Chess.Combat;
using Mirror;
using System.Collections.Generic;

namespace Chess.Core{
    public class PlayerPointer : NetworkBehaviour {
        public PatternSelectionManager patternSelectionManager { get; private set; }
        public InputManager inputManager { get; private set; }
        public PlayerSquad playerSquad { get; private set; }
        [SyncVar]
        public PlayerType playerType;

        private Matrix matrix;

        [SerializeField] InputManager _inputManager;
        [SerializeField] PlayerSquad _playerSquad;
        #region Server
         //need to do SetPlayerPieces in start because NetworkIdentity is set after Awake
        public override void OnStartServer() {
            base.OnStartServer();
           // print(connectionToServer.identity + " on server start ");
          //  print("on server start, num players is " + (NetworkServer.connections.Count - 1));
            playerType = (PlayerType) (NetworkServer.connections.Count - 1);
            matrix = GlobalPointers.matrix;
            matrix.SetPlayerPieces(SpawnSquadUnits(), playerType);
        }

        [Server]
        public List<OnTile> SpawnSquadUnits() {
            List<OnTile> output = new List<OnTile>();
          //  print("squad length " + playerSquad.GetUnits().Count);
            foreach(Unit unit in playerSquad.GetUnits()) {
                var temp = Instantiate(unit.gameObject);
               // print("instantiate " + temp);
                NetworkServer.Spawn(temp, connectionToClient);
                output.Add(temp.GetComponent<OnTile>());
            }
          //  print("returned with length " + output.Count);
            return output;
        }
        #endregion

        #region Client
         public void Awake() {
            if (matrix != null) return;//prevents method being called twice

            patternSelectionManager = FindObjectOfType<PatternSelectionManager>(true);
            //patternSelectionManager.SetPlayerType(playerType);
            inputManager = _inputManager;
            inputManager.SetPlayerType(playerType);
            playerSquad = _playerSquad;
            matrix = FindObjectOfType<Matrix>();
         }
        #endregion
       

        

       
        
        
    }
}