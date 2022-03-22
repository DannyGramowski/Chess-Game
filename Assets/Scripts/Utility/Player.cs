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

        public readonly SyncList<Guid> squadGuids = new SyncList<Guid>();//need to store guids of network identitys to sync b/c unit is a network behaviour and mirror is stupid

        [SerializeField] [Range(0, 1)] float deathPerecentage;//perecentage of points lost for game to end

        private List<Unit> squad = new List<Unit>();

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
            print("spawn squad units");
            foreach (Unit unit in playerSquad.GetUnits()) {
                var temp = Instantiate(unit.gameObject);
                temp.GetComponent<Unit>().player = this;
                NetworkServer.Spawn(temp, connectionToClient);
                squadGuids.Add(temp.GetComponent<NetworkIdentity>().assetId);

            }
            MaxEquitmentPoints = GetSquadEquitmentCost();
            Debug.Assert(squadGuids.Count != 0);
        }

       [Server]
       public void CheckIfLost() {
            print("check if lost");
            print("curr equiptment points " + GetSquadEquitmentCost() + " max equitpment points " + MaxEquitmentPoints);
            if(GetSquadEquitmentCost() <= MaxEquitmentPoints * deathPerecentage) {

                PlayerType winner = playerType == PlayerType.player1 ? PlayerType.player2 : PlayerType.player1;//sets winner to other playertype 
                
                GlobalPointers.gameManager.EndGame(winner);
            }
        }

        [Server] 
        public void UnitDeathCleanup(Unit unit) {
            squadGuids.Remove(unit.GetComponent<NetworkIdentity>().assetId);

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

        private void Update() {
            if(setPlaceDisplay == 1 && squadGuids.Count != 0) {
                RpcSetPlaceDisplay();
                setPlaceDisplay = 2;
            }
        }

        public void ClientUnitDeathCleanup(Unit unit) {
            squad.Remove(unit);
        }

        public void OnGameStart() {
            GetComponent<InputManager>().enabled = true;
            GetComponent<PlayerPointer>().enabled = true;
            setPlaceDisplay = 1;
            // GlobalPointers.matrix.SetPlayerPieces(SpawnSquadUnits(), playerType);
        }

        int setPlaceDisplay = 0;//0 before game start, 1 after game start, 2 after setPlaceDisplay is called
       // [ClientRpc]
        public void RpcSetPlaceDisplay() {
            if (playerType != GlobalPointers.GetPlayerType()) {
                return;//only calls method 
            }

            List<OnTile> dataSquad = new List<OnTile>();
            var units = FindObjectsOfType<Unit>();
            foreach(var unit in units) {
                var unitId = unit.GetComponent<NetworkIdentity>();
                int index = squadGuids.IndexOf(unitId.assetId);
                if(index >= 0 && unit.player.playerType == GlobalPointers.GetPlayerType()) {
                    dataSquad.Add(unitId.GetComponent<OnTile>());
                    squad.Add(unit);
                }
            }
             
            GlobalPointers.UI_Manager.SetUI(UIType.setupManager, dataSquad);
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

        private int GetSquadEquitmentCost() {
            int output = 0;
            foreach (var unit in squad) output += unit.GetUnitEquiptmentPoints();
            return output;
        }

        #endregion
    }
}