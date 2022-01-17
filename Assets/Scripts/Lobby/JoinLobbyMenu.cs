using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.Lobby {
    public class JoinLobbyMenu : MonoBehaviour {
        [SerializeField] private LobbyNetworkManager networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;
        [SerializeField] private TMP_InputField ipAddressInputField = null;
        [SerializeField] private Button joinButton = null;

        private void OnEnable() {
            LobbyNetworkManager.OnClientConnected += HandleClientConnected;
            LobbyNetworkManager.OnClientDisconnected += HandleClientDisconnected;
        }

        private void OnDisable() {
            LobbyNetworkManager.OnClientConnected -= HandleClientConnected;
            LobbyNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
        }

        public void JoinLobby() {
            string ipAddress = ipAddressInputField.text;

            networkManager.networkAddress = ipAddress;
            networkManager.StartClient();

            joinButton.interactable = false;
        }

        private void HandleClientConnected() {
            joinButton.interactable = true;

            gameObject.SetActive(false);
            landingPagePanel.SetActive(false);
        }

        private void HandleClientDisconnected() {
            joinButton.interactable = false;
        }

        
    }
}