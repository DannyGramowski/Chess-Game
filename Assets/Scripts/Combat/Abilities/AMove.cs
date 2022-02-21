using Chess.Core;
using System;
using System.Collections;
using UnityEngine;

namespace Chess.Combat{
    public class AMove : Ability {
        private MovementPattern currentPattern;
        private Tile currTile => baseUnit.GetCurrentTile();

        public override void ActivateAbility(IsSelectable additionalData) {
         //   print("activated move: data is " + additionalData);
            if(additionalData == null) {
           //     print("set ui from move ability");
                GlobalPointers.UI_Manager.SetUI(UI.UIType.patternSelectionManager, baseUnit);
            } else if(currentPattern != null){
             //   print("current pattern not null");
                Tile selectedTile = additionalData.GetComponent<Tile>();
                if( currentPattern.ValidMovement(currTile, selectedTile)) {
                    SetMovementOptionColors(currentPattern);
                    baseUnit.CmdMove(selectedTile);
                    GlobalPointers.UI_Manager.SetUI(UI.UIType.abilityDisplayManager, baseUnit);
                    actionPointCost = currentPattern.GetActionPointCost();
                    base.ActivateAbility(additionalData);
                }
            }
        }

        public override Type GetAdditionSelectionType() => typeof(Tile);

        public override bool ValidSelection(IsSelectable data) {
           data.TryGetComponent(out Tile tile);
            print("valid selection tile " + tile);
            return tile != null && ValidMovement(tile, currentPattern);
        }
        public override void CancelAbility() {
            if(currentPattern != null) SetMovementOptionColors(currentPattern);
            currentPattern = null;
        }

        public void SetCurrentPattern(MovementPattern pattern) {
            currentPattern = pattern;
        }

        public void SetMovementOptionColors(MovementPattern pattern, Color newColor = default(Color)) {
            Matrix matrix = GlobalPointers.matrix;
            currentPattern = pattern;
            foreach (var pos in pattern.GetValidOffsets(currTile.GetGridPos(), matrix.GetMatrixSize())) {
                Tile tile = matrix.GetTile(pos);
                if (ValidMovement(tile, pattern)) {
                    if (newColor == default(Color)) tile.ResetColor();//if there is no color specified it resets the color
                    else tile.SetColor(newColor);
                }
            }
        }

        public bool ValidMovement(Tile tile, MovementPattern pattern) {
            return tile.onTile == null && pattern != null && pattern.ValidMovement(currTile, tile);
        }


    }

}
