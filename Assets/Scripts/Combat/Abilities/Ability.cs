using Chess.Core;
using System.Collections;
using UnityEngine;
using System;

namespace Chess.Combat {
    public abstract class Ability : MonoBehaviour {
        public int ActionPointCost => actionPointCost;
        public string AbilityName => abilityName;
        public Sprite AbilityImage => abilityImage;
        public int EquiptmentPointCost => equiptmentPointCost;
        
        [SerializeField] protected int actionPointCost;
        [SerializeField] protected string abilityName;
        [SerializeField] protected Sprite abilityImage;
        [SerializeField] protected int equiptmentPointCost;

        protected Unit baseUnit;

        protected void Start() {
            baseUnit = GetComponent<Unit>();            
        }

        public virtual void ActivateAbility(IsSelectable additionalData) {
            print("decreased AP");
            UI.AbilityDisplayManager displayManager = GlobalPointers.UI_Manager.GetUI(UI.UIType.abilityDisplayManager) as UI.AbilityDisplayManager;
            GetComponent<Unit>().CmdDecreaseActionPoints(ActionPointCost); 
            if (displayManager.gameObject.activeSelf) displayManager.UpdateAPDisplay();
        }

        public virtual Type GetAdditionSelectionType() => null;

        public virtual bool ValidSelection(IsSelectable data) => true;

        public virtual void CancelAbility() { }
        
       
    }
}