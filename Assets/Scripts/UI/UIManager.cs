using Chess.Core;
using Chess.Combat;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Chess.UI {
    public class UIManager : MonoBehaviour {
        [SerializeField] IUI[] UIs;
/*        [SerializeField] PatternSelectionManager patternSelectionManager;
        [SerializeField] AbilityDisplayManager abilityDisplayManager;*/
        [SerializeField] TMP_Text turnText;

        public IUI SetUI(UIType _UIType, object data) {
            IUI output = null;
            foreach(IUI ui in UIs) {
                if(ui.GetUIType() == _UIType) {
                    output = ui;
                    (ui as MonoBehaviour).gameObject.SetActive(true);
                    ui.SetDisplay(data);
                } else {
                    (ui as MonoBehaviour).gameObject.SetActive(false);

                }
            } 

            return output;
        }

        public IUI SetUI(int uiNum, object data) {
            IUI output = null;
            if (uiNum >= UIs.Length) Debug.LogError("number is higher than number of UI's");
            for(int i = 0; i < UIs.Length; i++) {
                if(i == uiNum) {
                    output = UIs[i];
                    (output as MonoBehaviour).gameObject.SetActive(true);
                    output.SetDisplay(data);
                } else {
                    (output as MonoBehaviour).gameObject.SetActive(false);
                }
            }
            return output;
        }
       /* public void SetPatterns(Unit unit) {
            patternSelectionManager.SetUnit(unit);
        }

        public void SetAbilityDisplays(Unit unit) {
            abilityDisplayManager.SetDisplay(unit);
        }*/

        public void EndTurn() {
           // print("ui manager end turn");
            GlobalPointers.gameManager.EndTurn();
        }

        public void SetTurnText(int turnNumber) {
          //  print("player type " + GlobalPointers.playerType + ": turn num " + turnNumber);
            if(turnNumber == (int) GlobalPointers.playerType) {
                turnText.text = "your turn";
            } else {
                turnText.text = "opponents turn";
            }
        }
      
    }

    public enum UIType {
        patternSelectionManager,
        abilityDisplayManager
    }
}