using Chess.Core;
using Chess.Combat;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Chess.UI {
    public class UIManager : MonoBehaviour {
        [SerializeField] IUI[] UIs;
        [SerializeField] TMP_Text turnText;
        public IUI ActiveUI => _activeUI;

        IUI _activeUI;

        public IUI SetUI(UIType _UIType, object data) {
            if (ActiveUI != null && ActiveUI.GetUIType() == _UIType) return ActiveUI;
            IUI output = null;
            foreach(IUI ui in UIs) {
                if(ui.GetUIType() == _UIType) {
                    output = ui;
                    (ui as MonoBehaviour).gameObject.SetActive(true);
                    ui.SetDisplay(data);
                    _activeUI = ui;
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

        public IUI GetUI(UIType _UIType) => UIs.Where(ui => ui.GetUIType() == _UIType).First();
        

        public void EndTurn() {
            GlobalPointers.gameManager.EndTurn();
        }

        public void SetTurnText(int turnNumber) {
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