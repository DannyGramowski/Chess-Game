using Chess.Combat;
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
            if (!tile.IsEmpty()) return;
                placingUnit.transform.position = tile.transform.position;
                
                  
        }
        public bool HasUnit() => placingUnit != null;

        public void SetUnit(Unit newUnit) {
            placingUnit = newUnit;
        }


        public override void ActivateAbility(IsSelectable additionalData) {
            print("move");
            placingUnit.Move(additionalData.GetComponent<Tile>());
            placingUnit = null;
        }



        public override void CancelAbility() {
        }

        public override Type GetAdditionSelectionType() => typeof(Tile);

        public override bool ValidSelection(IsSelectable data) {
            Tile tile = data.GetComponent<Tile>();

            return placingUnit != null && tile != null && tile.IsEmpty();
        }



    }
}