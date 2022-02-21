using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chess.Core.Managers {
    public class ChessNetworkManager : NetworkManager {
        [SerializeField] Matrix matrix;
        [SerializeField] GlobalPointers globalPointers;

        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;
        public static event Action OnSceneChange;

        private bool isGameInProgress = false;

        public List<Player> players { get; } = new List<Player>();


        #region Server
        public override void OnServerAddPlayer(NetworkConnection conn) {
            base.OnServerAddPlayer(conn);

            Player player = conn.identity.GetComponent<Player>();
            player.playerType = (PlayerType)players.Count;
            players.Add(player);
            

            player.SetDisplayName("Player " + players.Count);
            player.SetPartyOwner(players.Count == 1);
        }

        public override void OnServerConnect(NetworkConnection conn) {
            if (!isGameInProgress) return;

            conn.Disconnect();
        }

        public override void OnServerDisconnect(NetworkConnection conn) {
            Player player = conn.identity.GetComponent<Player>();
            players.Remove(player);

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer() {
            players.Clear();

            isGameInProgress = false;
        }

        public void StartGame() {     
            if (players.Count < 2) return;

            //SceneManager.LoadScene("MainGame");
            //print("game scene = " + gameScene.name);
            isGameInProgress = true;

            ServerChangeScene("MainGame");
        }
        #endregion

        #region Client
        public override void OnStartClient() {
            // Scene gameScene = SceneManager.GetSceneAt(1);
            //  print("game scene = " + gameScene.name);
           // print("scene count " + SceneManager.sceneCount);
            //print("total scenes " + SceneManager.) 
            //globalPointers.SetVariables();
        }

        [Obsolete]
        public override void OnClientConnect(NetworkConnection conn) {
            base.OnClientConnect(conn);

            ClientOnConnected?.Invoke();
        //    print("on start client " + conn);
            //need to move this to object only in game scene
            //matrix.GetComponent<Matrix>().SetUpTiles();
        }

        [Obsolete]
        public override void OnClientDisconnect(NetworkConnection conn) {
            base.OnClientDisconnect(conn);

            ClientOnDisconnected?.Invoke();
        }

        public override void OnStopClient() {
            players.Clear();
        }

        public Player GetPlayer(PlayerType playerType) {
            return players.Find(p => p.playerType == playerType);
        }
        #endregion
    }
}
//hamachi info
//network unity test network
//