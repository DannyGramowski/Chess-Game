using Chess.UI;
using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Chess.Core {

    public class InputManager : NetworkBehaviour {
        IsSelectable oldSelection;
        IsSelectable selection;
        [SerializeField] MovementPattern test;
        [SerializeField] PlayerPointer playerPointer;
        PatternSelectionManager patternSelectionManager;
        PlayerType playerType;

        private void Start() {
            patternSelectionManager = playerPointer.patternSelectionManager;
            playerType = playerPointer.playerType;
        }

        private void Update() {
            if(Mouse.current.leftButton.wasPressedThisFrame) {
                SelectObject();
            }
        }
        public void SetPlayerType(PlayerType playerType) {
            this.playerType = playerType;
        }

        


        private bool SelectObject() {
            print("select object");
            var ray = GlobalPointers.mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                selection = hit.transform.GetComponent<IsSelectable>();

                if(oldSelection != null) {
                    Unselect();
                }

                if(selection != null) {
                    Select();
                    oldSelection = selection;
                }
            }
            return false;
        }

        private void Select() {
            if(selection.SelectionValid(playerType)) selection.OnSelect();
            Tile tile = selection.GetComponent<Tile>();
            if (tile != null && patternSelectionManager.ValidMovement(tile)) patternSelectionManager.MoveUnit(tile); 
            
        }

        private void Unselect() {

        }
    }

    public enum PlayerType{
        player1,
        player2,
        all
    }
}