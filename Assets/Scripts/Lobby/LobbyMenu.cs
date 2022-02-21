using Chess.Core;
using Chess.Core.Managers;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] GameObject lobbyUI = null;
    [SerializeField] Button startGameButton = null;
    [SerializeField] TMP_Text[] playerNameTexts = new TMP_Text[2];

    private void Start() {
        ChessNetworkManager.ClientOnConnected += HandleClientConnected;
        Player.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        Player.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void OnDestroy() {
        ChessNetworkManager.ClientOnConnected -= HandleClientConnected;
        
        Player.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

     private void ClientHandleInfoUpdated() {
        var players = (NetworkManager.singleton as ChessNetworkManager).players;

        for(int i = 0; i <playerNameTexts.Length; i++) {
            playerNameTexts[i].text = i < players.Count ? players[i].DisplayName : "Waiting For Player...";
        }
        startGameButton.interactable = players.Count == 2;
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state) {
        startGameButton.gameObject.SetActive(state);
    }

    [Client]
    public void StartGame() {
        print("network client " + NetworkClient.connection.identity);
        NetworkClient.connection.identity.GetComponent<Player>().CmdStartGame();
        startGameButton.interactable = false;
    }

    private void HandleClientConnected() {
        lobbyUI.SetActive(true);
    }

    public void LeaveLobby() {
        if(NetworkServer.active && NetworkClient.isConnected) {
            NetworkManager.singleton.StopHost();
        } else {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }

    public void LogPress() {
        print("button pressed");
    }
 
}
