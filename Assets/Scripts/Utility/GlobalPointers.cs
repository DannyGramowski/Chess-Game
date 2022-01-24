using Chess.UI;
using System.Collections;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Core {
    public class GlobalPointers : Singleton<GlobalPointers> {
        public static Camera mainCamera;
        public static bool useDebug;
        public static UIManager UI_Manager;
        public static GameManager gameManager;
        public static PlayerType playerType;
        public static Matrix matrix;

        [SerializeField] bool _useDebug;
        [SerializeField] UIManager _UIManager;
        [SerializeField] Matrix _matrix;

        List<PlayerPointer> playerPointers = new List<PlayerPointer>();
        
        public void SetVariables() {
            mainCamera = Camera.main;
            useDebug = _useDebug;
            UI_Manager = _UIManager;
            matrix = _matrix;
            gameManager = GetComponent<GameManager>();
            playerType = GetComponent<NetworkIdentity>().isServer ? PlayerType.player1 : PlayerType.player2;
/*            foreach (var cons in NetworkServer.connections.Values) {
                print(cons.ToString());
            }
            print("set player type to " + playerType);*/
        }

        public void AddPlayerPointer(PlayerPointer playerPointer) {
            playerPointers.Add(playerPointer);
        }

        public PlayerPointer GetPlayerPointer(PlayerType playerType) {
            return playerPointers.Where(a => a.playerType == playerType).First();
        }
    }
}