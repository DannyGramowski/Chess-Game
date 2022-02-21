using Chess.UI;
using System.Collections;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using Chess.Core.Managers;

namespace Chess.Core {
    public class GlobalPointers : Singleton<GlobalPointers> {
        public static Camera mainCamera;
        public static bool useDebug;
        public static UIManager UI_Manager;
        public static GameManager gameManager;
        public static PlayerType playerType;
        public static Matrix matrix;
        public static ChessNetworkManager chessNetworkManager;

        [SerializeField] bool _useDebug;
        [SerializeField] UIManager _UIManager;
        [SerializeField] Matrix _matrix;

        public void Awake() {
            mainCamera = Camera.main;
            useDebug = _useDebug;
            UI_Manager = _UIManager;
            matrix = _matrix;
            gameManager = GetComponent<GameManager>();
            chessNetworkManager = FindObjectOfType<ChessNetworkManager>();
            foreach (var player in chessNetworkManager.players) {
                player.OnGameStart();
            }
        }

        public static PlayerType GetPlayerType() {
            return Mirror.NetworkClient.isHostClient ? PlayerType.player1 : PlayerType.player2;
        }
    }
}