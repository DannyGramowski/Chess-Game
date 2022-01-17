using Chess.Combat;
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
     //   button.onClick.AddListener(ability.ActivateAbility);
        //print(ability.AbilityName + " button was set to " + button.onClick.);
    }

    public void ActivateAbility() {
        print("activated " + currAbility.AbilityName);
        currAbility.ActivateAbility();
    }

  /*  public void Test() {
        print("test button worked");
    }*/
}
