using System.Collections;
using UnityEngine;
using Chess.UI;

namespace Chess.Core{
    public class PlayerPointer : MonoBehaviour {
        public PatternSelectionManager patternSelectionManager { get; private set; }
        public InputManager inputManager { get; private set; }
        public PlayerSquad playerSquad { get; private set; }
        public PlayerType playerType { get; private set; }
 
        [SerializeField] PatternSelectionManager _patternSelectionManager;
        [SerializeField] InputManager _inputManager;
        [SerializeField] PlayerSquad _playerSquad;
        [SerializeField] PlayerType _playerType = PlayerType.player1;
        // Use this for initialization
        void Awake() {
            playerType = _playerType;
            patternSelectionManager = _patternSelectionManager;
            patternSelectionManager.SetPlayerType(playerType);
            inputManager = _inputManager;
            inputManager.SetPlayerType(playerType);

        }
    }
}