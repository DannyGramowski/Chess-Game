using Chess.Combat;
using Chess.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplay : MonoBehaviour {
    [SerializeField] TMP_Text abilityPointText;
    [SerializeField] TMP_Text abilityNameText;
    [SerializeField] Button button;

    Ability currAbility;
   public void SetDisplay(Ability ability) {
        currAbility = ability;
        abilityPointText.text = ability.ActionPointCost.ToString();
        abilityNameText.text = ability.AbilityName;
    }

    public void ActivateAbility() {
        print("activated " + currAbility.AbilityName);
        if (currAbility.ActionPointCost > currAbility.GetComponent<Unit>().currActionPoints) return;
        if(currAbility.GetAdditionSelectionType() != null) {
            currAbility.GetComponent<Unit>().player.playerPointer.inputManager.SetActiveAbility(currAbility);
        }
        currAbility.ActivateAbility(null);
    }
}
