using Chess.UI;
using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Chess.Core {

    public class InputManager : NetworkBehaviour {
        ISelectable oldSelection;
        ISelectable selection;
        [SerializeField] MovementPattern test;
        [SerializeField]PatternSelectionManager patternSelectionManager;
        PlayerType playerType;

        public void Awake() {
        }

        public void SetPlayerType(PlayerType playerType) {
            this.playerType = playerType;
        }

        public void LeftClick(InputAction.CallbackContext context) {
            if (context.performed) {
                SelectObject();
            }
        }


        private bool SelectObject() {
            var ray = GlobalPointers.mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                selection = hit.transform.GetComponent<ISelectable>();

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
            if(selection.IsSelectable(playerType)) selection.OnSelect();
            Tile tile = selection as Tile;
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