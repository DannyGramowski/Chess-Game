using Chess.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Chess.Core {

    public class InputManager : MonoBehaviour {
        ISelectable oldSelection;
        ISelectable selection;
        [SerializeField] MovementPattern test;
        [SerializeField] PatternSelectionManager patternSelectionManager;
        PlayerType playerType;

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
        }

        private void Unselect() {

        }

        private void TryMoveUnit(Unit unit, Tile tile) {
          //  if(patternSelectionManager.)
        }
    }

    public enum PlayerType{
        player1,
        player2,
        all
    }
}