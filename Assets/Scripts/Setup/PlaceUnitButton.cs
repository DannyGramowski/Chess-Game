using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess.Combat;
using UnityEngine.UI;
using Chess.Core;
using TMPro;

namespace Chess.UI { 
public class PlaceUnitButton : MonoBehaviour {
        [SerializeField] Button button;
        [SerializeField] TMP_Text text;

        SetupManager setupManager;
        Unit unit;

        public Unit Unit => unit;

        public void Setup(Unit newUnit, SetupManager setupManager) {
            unit = newUnit;
            button.onClick.AddListener(OnPress);
            button.name = unit.name;
            text.text = unit.name;
            this.setupManager = setupManager;
        }

        private void OnPress() {
            unit.gameObject.SetActive(true);
            setupManager.StartPlaceUnit(unit);
        }
    }
}
