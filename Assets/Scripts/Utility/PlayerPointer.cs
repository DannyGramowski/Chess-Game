using System.Collections;
using UnityEngine;
using Chess.UI;
using Mirror;
using System.Collections.Generic;

namespace Chess.Core{
    public class PlayerPointer : NetworkBehaviour {
        public PatternSelectionManager patternSelectionManager { get; private set; }
        public InputManager inputManager { get; private set; }
        public PlayerSquad playerSquad { get; private set; }
        public PlayerType playerType { get; private set; }
        public Matrix matrix { get; private set; }

 
        //[SerializeField] PatternSelectionManager _patternSelectionManager;
        [SerializeField] InputManager _inputManager;
        [SerializeField] PlayerSquad _playerSquad;
        [SerializeField] PlayerType _playerType = PlayerType.player1;
       
        // Use this for initialization
        public void Awake() {
            playerType = _playerType;
            patternSelectionManager = FindObjectOfType<PatternSelectionManager>();
            patternSelectionManager.SetPlayerType(playerType);
            inputManager = _inputManager;
            inputManager.SetPlayerType(playerType);
            playerSquad = _playerSquad;
            matrix = FindObjectOfType<Matrix>();
        }

        //need to do SetPlayerPieces in start because NetworkIdentity is set after Awake
        public override void OnStartServer() {
            base.OnStartServer();
            matrix.SetPlayerPieces(SpawnPlayerSquad(), playerType);
        }

        public List<Unit> SpawnPlayerSquad() {
            List<Unit> output = new List<Unit>();
            foreach(Unit unit in playerSquad.GetUnits()) {
                var temp = Instantiate(unit.gameObject);
                NetworkServer.Spawn(temp, connectionToClient);
                output.Add(temp.GetComponent<Unit>());
            }
            return output;
        }
        
        
    }
}