using Chess.UI;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Chess.Core {
    public class GameManager : NetworkBehaviour {
       [SyncVar]public int playerTurn = 0; //the player number whose turn it is
        public bool isTurn => playerTurn == (int)GlobalPointers.playerType; 

        #region Server
       
        [ClientRpc]
        public void RpcSetTurnText(int playerTurn) {
         //   print("rpc set text");
            GlobalPointers.UI_Manager.SetTurnText(playerTurn);
        }
        #endregion

        #region Client
        [Command(requiresAuthority = false)]
        public void CmdEndTurn(PlayerType playerType) {
            print("cmd end turn for turn " + playerTurn + ": player type " + GlobalPointers.playerType);
            if (playerTurn != (int)playerType) return;
            print("ended turn for " + playerTurn);
            playerTurn = (playerTurn + 1) % 2;
           // isTurn = !isTurn;
            RpcSetTurnText(playerTurn);
        }

        public void EndTurn() {
        //    print("game manager end turn");
           
            CmdEndTurn(GlobalPointers.playerType);
           
        }

        private void Start() {
            SetInitialTurn();
        }

        void SetInitialTurn() {
            GlobalPointers.UI_Manager.SetTurnText(playerTurn);
            //isTurn = (int)GlobalPointers.playerType == playerTurn;
        }
        
        #endregion

    }
}