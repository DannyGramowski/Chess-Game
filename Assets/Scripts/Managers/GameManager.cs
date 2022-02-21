using Chess.UI;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Chess.Core {

    public class GameManager : NetworkBehaviour {
        public static event Action OnFinishSetup;

       [SyncVar]public int playerTurn = 0; //the player number whose turn it is
        [SyncVar] public GamePhase gamePhase = GamePhase.Setup;
        public bool isTurn => playerTurn == (int)GlobalPointers.playerType;

        bool[] EndSetupFlags = new bool[2];//index 0 for player 1, index 1 for player 2;
        #region Server
       
        [ClientRpc]
        public void RpcSetTurnText(int playerTurn) {
         //   print("rpc set text");
            GlobalPointers.UI_Manager.SetTurnText(playerTurn);
        }

        [Server]
        public void EndSetupPhase() {
            print("end setup phase");
            if (!(EndSetupFlags[0] & EndSetupFlags[1] == true)) return;

            gamePhase = GamePhase.Player1Turn;
            OnFinishSetup?.Invoke();
            GlobalPointers.UI_Manager.SetUI(UIType.abilityDisplayManager, null);
        }
        #endregion

        #region Client
        [Command(requiresAuthority = false)]
        public void CmdEndTurn(PlayerType playerType) {
            if (playerTurn != (int)playerType) return;
            playerTurn = (playerTurn + 1) % 2;
           // isTurn = !isTurn;
            RpcSetTurnText(playerTurn);
        }

        [Command(requiresAuthority = false)]
        public void CmdEndSetup(int playerNum) {
            print("end setup command for player " + playerNum);
            EndSetupFlags[playerNum] = true;
            EndSetupPhase();
        }

        public void EndTurn() {
        //    print("game manager end turn");
           
            CmdEndTurn(GlobalPointers.playerType);
           
        }

        private void Start() {
            SetInitialTurn();
        }

        void SetInitialTurn() {
           // GlobalPointers.UI_Manager.SetTurnText(playerTurn);
            //isTurn = (int)GlobalPointers.playerType == playerTurn;
        }
        
        #endregion

    }

    public enum GamePhase {
        Setup,
        Player1Turn,
        Player2Turn,
        GameOver
    }
}