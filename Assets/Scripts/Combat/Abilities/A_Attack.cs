using Chess.Core;
using System;
using System.Collections;
using UnityEngine;

namespace Chess.Combat {
    public class A_Attack : Ability {
        [SerializeField] Weapon weapon;
        Unit targetUnit;

        private new void Start() {
            base.Start();
            actionPointCost = weapon.ActionPointCost;
            equiptmentPointCost = weapon.EquitmentPointCost;
        }

        public override void ActivateAbility(IsSelectable additionalData) {
            print("activate attack");
            if(additionalData == null) {
                print("additional data null");
            } else {
                print(name + " attacking " + additionalData);
                targetUnit = additionalData.GetComponent<Unit>();
                baseUnit.CmdDealDamage(weapon.Damage, targetUnit);
                base.ActivateAbility(additionalData);
            //print("attack");
            }
        }

        Ray displayRay = default;
        bool debugRay = false;
        public override bool ValidSelection(IsSelectable data) {
            targetUnit = data.GetComponent<Unit>();
            if (targetUnit == null) return false;

            Vector3 basePos = baseUnit.GetCurrentTile().GetGridPos();
            Vector3 targetPos = targetUnit.GetCurrentTile().GetGridPos();
            bool output = true;
            output &= Vector3.Distance(basePos, targetPos) < weapon.Range;
            Ray ray = new Ray(baseUnit.transform.position + new Vector3(0,0.2f,0), (targetUnit.transform.position - baseUnit.transform.position).normalized * 20);//added offset so raycast does not colide with floor
            displayRay = ray;
            debugRay = true;
            //Gizmos.DrawRay(ray);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 50)) return false;
            Unit unit;
            output &= hit.transform.TryGetComponent<Unit>(out unit);
            output &= unit?.player.playerType != baseUnit.player.playerType;
            return output;
        }

        private void OnDrawGizmos() {
            if (debugRay) {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(displayRay);
            }
        }

        public override Type GetAdditionSelectionType() => typeof(Unit);
    }
}