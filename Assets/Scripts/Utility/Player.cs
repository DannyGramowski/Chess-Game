using Chess.Combat;
using Chess.Core.Managers;
using Chess.UI;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chess.Core {
    public class Player : NetworkBehaviour {
        public PlayerPointer playerPointer;
        [SyncVar]
        public PlayerType playerType;

        [SyncVar]
        public int MaxEquitmentPoints;

        public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
        public static event Action ClientOnInfoUpdated;

        readonly SyncList<OnTile> squad = new SyncList<OnTile>();
        [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
        private bool isPartyOwner = false;

        [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
        private string displayName;
        [SerializeField] PlayerSquad playerSquad;

        public string DisplayName => displayName;

        public bool IsPartyOwner => isPartyOwner;

        #region Server
              

        public override void OnStartServer() {
            DontDestroyOnLoad(gameObject);
        }

        [Server]
        public void SpawnSquadUnits() {

            foreach (Unit unit in playerSquad.GetUnits()) {
                var temp = Instantiate(unit.gameObject);
                temp.GetComponent<Unit>().player = this;
                NetworkServer.Spawn(temp, connectionToClient);
                squad.Add(temp.GetComponent<OnTile>());
                print("spawn unit " + unit + " for player " + playerType);

            }
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
            //GlobalPointers.Instance.AddPlayer(this);
        }

        public void OnGameStart() {
            GetComponent<InputManager>().enabled = true;
            GetComponent<PlayerPointer>().enabled = true;

           // GlobalPointers.matrix.SetPlayerPieces(SpawnSquadUnits(), playerType);
        }

        [TargetRpc]
        public void RpcSetPlaceDisplay() {
            print("squad length " + squad.Count);
            GlobalPointers.UI_Manager.SetUI(UIType.setupManager, squad.ToList());
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