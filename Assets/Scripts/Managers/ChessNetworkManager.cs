using Mirror;
using System.Collections;
using UnityEngine;

namespace Chess.Core.Managers {
    public class ChessNetworkManager : NetworkManager {
        [SerializeField] Matrix matrix;
        [SerializeField] GlobalPointers globalPointers;

        #region Server
        public override void OnStartServer() {
         
        }

        public override void OnStartClient() {
            globalPointers.SetVariables();
        }

        public override void OnClientConnect(NetworkConnection conn) {
            base.OnClientConnect(conn);
        //    print("on start client " + conn);
            matrix.GetComponent<Matrix>().SetUpTiles();
        }

        #endregion
    }
}