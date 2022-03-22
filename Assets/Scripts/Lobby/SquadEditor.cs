using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Chess.Lobby {
    public class SquadEditor : MonoBehaviour {
        [SerializeField] TMP_Text lobbyText;
        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        private void OnEnable() {
            lobbyText.gameObject.SetActive(false);
        }

        private void OnDisable() {
            lobbyText.gameObject.SetActive(true);
        }
    }
}