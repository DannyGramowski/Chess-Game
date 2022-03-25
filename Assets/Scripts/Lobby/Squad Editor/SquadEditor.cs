using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Chess.Combat;
using System.Linq;
using Chess.Saving;

namespace Chess.Lobby {
    public class SquadEditor : Singleton<SquadEditor> {
        [SerializeField] TMP_Text lobbyText;
        [SerializeField] MovementPattern[] movementPatterns;
        [SerializeField] Ability[] abilities;
        [SerializeField] Weapon[] weapons;

        FileDisplay selectedFileDisplay;
        PlayerSquad activeSquad;
        SavingSystem savingSystem;

        private void Start() {
            savingSystem = SavingSystem.Instance;
        }

        private void OnEnable() {
            lobbyText.gameObject.SetActive(false);
            CheckResources();
            
        }

        private void OnDisable() {
            lobbyText.gameObject.SetActive(true);
        }


        //make sure all movement patterns and abilities are in the list
        private void CheckResources() {
            var loadedPatterns = Resources.LoadAll<MovementPattern>("");
            Debug.Assert(loadedPatterns.Length == movementPatterns.Length, "incorrect patterns in array");
            var loadedAbilites = Resources.LoadAll<Ability>("");
            Debug.Assert(loadedAbilites.Length == abilities.Length, "incorrect abilites in array");
            var loadedWeapons = Resources.LoadAll<Weapon>("");
            Debug.Assert(loadedWeapons.Length == weapons.Length, "incorrect weapons in array");
        }

        public void SaveSquad() {
            savingSystem.Save();
        }
    }
}