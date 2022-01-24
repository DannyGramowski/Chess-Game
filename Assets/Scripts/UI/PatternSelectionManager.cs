using System.Collections;
using UnityEngine;
using Chess.Core;
using UnityEngine.UI;
using Chess.Combat;

namespace Chess.UI {
    public class PatternSelectionManager : IUI {
        [SerializeField] Button[] buttons;
        Unit unit;
        AMove activeMoveAbility;
        MovementPattern activePattern;
        //PlayerType playerType;

      /*  public void SetPlayerType(PlayerType playerType) {
            this.playerType = playerType;
        }*/

        public void SetUnit(Unit newUnit) {
            if (unit == newUnit) return;
            unit = newUnit;
            activeMoveAbility = unit.GetAbility<AMove>();

            activePattern = null;
            SetButtons();
        }

        public bool ValidMovement(Tile testTile) {
            return unit != null && activePattern != null && activeMoveAbility.ValidMovement(testTile, activePattern);
        }

        private void SetButtons() {
            for(int i = 0; i < 4; i++) {
                AddPatternToButton(i, unit.GetNextPattern());
            }
        }

        //add on hover show possible moves
        private void AddPatternToButton(int buttonNum, MovementPattern pattern) {
            Button button = buttons[buttonNum];
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SetActivePattern(buttonNum, pattern));
            button.GetComponentInChildren<Text>().text = pattern.GetMovement().ToString();
        }

        public void MoveUnit(Tile newTile) {
            if (unit == null) Debug.LogError("There is no unit");
            activeMoveAbility.SetMovementOptionColors(activePattern);
            unit.CmdMove(newTile);
            activePattern = null;
        }

        private void SetActivePattern(int buttonNum, MovementPattern pattern) {
            if(activePattern != null) activeMoveAbility.SetMovementOptionColors(activePattern);//reset previous patterns 
            activePattern = pattern;
            activeMoveAbility.SetCurrentPattern(pattern);
           activeMoveAbility.SetMovementOptionColors(activePattern, Color.green);// sets new pattern to green
            AddPatternToButton(buttonNum, pattern);
        }

        public override void SetDisplay(object data) {
            print("set display for pattern selection manager");
            SetUnit((Unit)data);
        }

        public override UIType GetUIType() => UIType.patternSelectionManager;
    }
}