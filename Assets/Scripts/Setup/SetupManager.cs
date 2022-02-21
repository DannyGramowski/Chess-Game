using Chess.Combat;
using Chess.Core;
using Chess.Core.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.UI {
    public class SetupManager :  IUI {
        [SerializeField] PlaceUnitButton placeUnitButton;
        [SerializeField] Transform buttonParent;

        A_PlaceUnit placeUnitAbility;
        [SerializeField] List<PlaceUnitButton> buttons = new List<PlaceUnitButton>();
        InputManager inputManager;
        [SerializeField] List<Unit> units = new List<Unit>();
        private void Start() {
            placeUnitAbility = GetComponent<A_PlaceUnit>();
            inputManager = GlobalPointers.chessNetworkManager.GetPlayer(GlobalPointers.GetPlayerType()).GetComponent<InputManager>();
        }

        public override UIType GetUIType() => UIType.setupManager;

        public override void SetDisplay(object data) {
            List<OnTile> onTiles = (List<OnTile>)data;
            print("units length " + onTiles.Count);
            units.Clear();

            inputManager.SetActiveAbility(placeUnitAbility);
                
            foreach (var onTile in onTiles) units.Add(onTile.GetComponent<Unit>());

            foreach(Unit unit in units) {
                var unitButton = Instantiate(placeUnitButton,buttonParent);
                unitButton.Setup(unit, this);
                buttons.Add(unitButton);
            }
        }

        public void StartPlaceUnit(Unit unit) {
            if (placeUnitAbility.HasUnit()) return;

            placeUnitAbility.SetUnit(unit);
            units.Remove(unit);
            foreach(var button in buttons) {
                if (button.Unit == unit) {
                    button.gameObject.SetActive(false);
                }
            }
        }

        public void SetReadyFlag() {
            if(units.Count == 0) {
                GlobalPointers.gameManager.CmdEndSetup((int) GlobalPointers.GetPlayerType());
            }
        }
    }
}
