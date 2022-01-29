using Chess.Combat;
using Chess.Core.Managers;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Core {
    public class Player : NetworkBehaviour {
        [SyncVar]
        public int playerNum = -1;

        public PlayerPointer playerPointer;
        public PlayerType playerType;

        public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
        public static event Action ClientOnInfoUpdated;

        [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
        private bool isPartyOwner = false;

        [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
        private string displayName;
        PlayerSquad playerSquad;

        public string DisplayName => displayName;

        public bool IsPartyOwner => isPartyOwner;

        #region Server
        /*      private void Start() {
                  //GlobalPointers.matrix.SetPlayerPieces(SpawnSquadUnits(), playerType);
              }*/

        public override void OnStartServer() {
            DontDestroyOnLoad(gameObject);
        }

        [Server]
        public List<OnTile> SpawnSquadUnits() {
            List<OnTile> output = new List<OnTile>();

            foreach (Unit unit in playerSquad.GetUnits()) {
                var temp = Instantiate(unit.gameObject);
                temp.GetComponent<Unit>().player = this;
                NetworkServer.Spawn(temp, connectionToClient);
                output.Add(temp.GetComponent<OnTile>());
            }
            return output;
        }

        [Server]
        public void SetPartyOwner(bool state) {
            isPartyOwner = state;
        }

        [Server]
        public void SetDisplayName(string newName) {
            displayName = newName;
        }
        #endregion

        #region Client
        private void Awake() {
            playerPointer = GetComponent<PlayerPointer>();
           // GlobalPointers.Instance.AddPlayer(this);

        }

        [Command]
        public void CmdStartGame() {
            if (!isPartyOwner) return;

            (NetworkManager.singleton as ChessNetworkManager).StartGame();
        }

        public override void OnStartClient() {
            if (NetworkServer.active) return;

            (NetworkManager.singleton as ChessNetworkManager).players.Add(this);
            DontDestroyOnLoad(gameObject);
        }

        public override void OnStopClient() {
            ClientOnInfoUpdated?.Invoke();

            if (!isClientOnly) return;

            (NetworkManager.singleton as ChessNetworkManager).players.Add(this);
        }

        private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState) {
            if (!hasAuthority) return;
            AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
        }

        private void ClientHandleDisplayNameUpdated(string oldName, string newName) {
            ClientOnInfoUpdated?.Invoke();
        }
        #endregion
    }
}