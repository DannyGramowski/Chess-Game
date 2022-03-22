using Chess.Combat;
using Chess.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chess.Core {
    public class A_PlaceUnit : Ability {
        Unit placingUnit;
        

        private new void Start() {
            base.baseUnit = null;
        }

        private void Update() {
            if (placingUnit == null) return;
            if(!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) return;
            if (!hit.transform.TryGetComponent<Tile>(out Tile tile)) return;
            if(ValidSelection(tile.GetComponent<IsSelectable>())) placingUnit.transform.position = tile.transform.position;
                
                  
        }
        public bool HasUnit() => placingUnit != null;

        public void SetUnit(Unit newUnit) {
            placingUnit = newUnit;
        }


        public override void ActivateAbility(IsSelectable additionalData) {
            print("move");
            placingUnit.CmdMove(additionalData.GetComponent<Tile>());
            placingUnit = null;
        }



        public override void CancelAbility() {
        }

        public override Type GetAdditionSelectionType() => typeof(Tile);

        public override bool ValidSelection(IsSelectable data) {
            Tile tile = data.GetComponent<Tile>();
            if (placingUnit == null || tile == null) return false;



            return tile.IsEmpty() && GlobalPointers.matrix.WithinSetUpArea(tile.GetGridPos());
        }



    }
}