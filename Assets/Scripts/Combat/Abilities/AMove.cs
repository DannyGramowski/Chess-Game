using Chess.Core;
using System.Collections;
using UnityEngine;

namespace Chess.Combat{
    public class AMove : Ability {
        public override void ActivateAbility() {
            //clicking on buttons is wierd
            print("activate move");
            GlobalPointers.UI_Manager.SetUI(UI.UIType.patternSelectionManager, GetComponent<Unit>());
        }
        
    }
}