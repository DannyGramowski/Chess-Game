using Chess.Combat;
using Chess.Core;
using Chess.Core.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.UI {
    public class SetupManager :  IUI {
        [SerializeField] PlaceUnitButton placeUnitButton;
        [SerializeField] Transform buttonParent;
        [SerializeField] bool useDefaultSpawnPositions;

        //public static event Action OnPlaceUnit;

        [SerializeField] List<PlaceUnitButton> buttons = new List<PlaceUnitButton>();
        A_PlaceUnit placeUnitAbility;
        InputManager inputManager;
        [SerializeField] List<Unit> units = new List<Unit>();

        
        private void SetReferences() {
            placeUnitAbility = GetComponent<A_PlaceUnit>();
            inputManager = GlobalPointers.chessNetworkManager.GetPlayer(GlobalPointers.GetPlayerType()).GetComponent<InputManager>();
           // OnPlaceUnit += SetReadyFlag;

        }

        public override UIType GetUIType() => UIType.setupManager;

        public override void SetDisplay(object data) {
            List<OnTile> onTiles = (List<OnTile>)data;
            if (placeUnitAbility == null) SetReferences();
            units.Clear();

            inputManager.SetActiveAbility(placeUnitAbility);
                
            foreach (var onTile in onTiles) units.Add(onTile.GetComponent<Unit>());

            if (useDefaultSpawnPositions) {
                SetDefaultSpawnLocations();
                SetReadyFlag();
            } else {
                foreach (Unit unit in units) {
                    var unitButton = Instantiate(placeUnitButton, buttonParent);
                    unitButton.Setup(unit, this);
                    buttons.Add(unitButton);
                }
            }
        }

        public void StartPlaceUnit(Unit unit) {
            if (placeUnitAbility.HasUnit()) return;

            placeUnitAbility.SetUnit(unit);
            foreach (var meshes in unit.GetComponentsInChildren<MeshRenderer>()) meshes.enabled = true;
            units.Remove(unit);
            unit.gameObject.SetActive(unit);
            foreach(var button in buttons) {
                if (button.Unit == unit) {
                    button.gameObject.SetActive(false);
                }
            }
        }
         


        public void SetReadyFlag() {
            print("set ready flag");
            if(units.Count == 0) {

                print("units count valid");
                GlobalPointers.gameManager.CmdEndSetup((int) GlobalPointers.GetPlayerType());
            }
        }

        private void SetDefaultSpawnLocations() {
            for(int i = 0; i < units.Count; i++) {
                var matrix = GlobalPointers.matrix;
                int xCoord = GlobalPointers.GetPlayerType() == PlayerType.player1 ? 0 : matrix.GetMatrixSize().x - 1;
                print("unit " + i + " " + units[i]);
                print("matrix " + matrix);
                print($"{xCoord},{matrix.GetMatrixSize().y - 1},{i}");
                var tile = matrix.GetTile(xCoord, matrix.GetMatrixSize().y - 1, i);
                print("tile " + tile);
                units[i].CmdMove(tile);
                foreach (var meshes in units[i].GetComponentsInChildren<MeshRenderer>()) meshes.enabled = true;
                units[i].gameObject.SetActive(true);
            }

            units.Clear();
        }
    }
}
