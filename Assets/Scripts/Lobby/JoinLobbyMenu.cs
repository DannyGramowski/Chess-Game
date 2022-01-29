using Chess.Core.Managers;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.Lobby {
    public class JoinLobbyMenu : MonoBehaviour {
        [SerializeField] GameObject landingPagePanel = null;
        [SerializeField] TMP_InputField addressInput = null;
        [SerializeField] Button joinButton = null;

        private void OnEnable() {
            ChessNetworkManager.ClientOnConnected += HandleClientConnected;
            ChessNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
        }

        private void OnDisable() {
            ChessNetworkManager.ClientOnConnected -= HandleClientConnected;
            ChessNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
        }

        public void Join() {
            string address = addressInput.text;

            NetworkManager.singleton.networkAddress = address;
            NetworkManager.singleton.StartClient();

            joinButton.interactable = false;
        }

        private void HandleClientConnected() {
            joinButton.interactable = true;

            gameObject.SetActive(false);
            landingPagePanel.SetActive(false);
        }

        private void HandleClientDisconnected() {
            joinButton.interactable = true;
        }
    }
}
