using System.Collections;
using UnityEngine;

namespace Chess.Lobby {
    public class MainMenu : MonoBehaviour {
        [SerializeField] private LobbyNetworkManager networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;
        
        public void HostLobby() {
            networkManager.StartHost();

            landingPagePanel.SetActive(false);
        }
      
    }
}