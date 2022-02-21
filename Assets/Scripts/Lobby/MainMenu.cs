using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Lobby {
    public class MainMenu : MonoBehaviour {
        [SerializeField] GameObject landingPagePanel = null;
        [SerializeField] AudioClip lobbyMusic;

        private void Start() {
            //Core.AudioManager.Instance.SetMusic(lobbyMusic);
        }

        public void HostLobby() {
            landingPagePanel.SetActive(false);
            
            NetworkManager.singleton.StartHost();
        }
    }
}
