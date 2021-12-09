﻿using System.Collections;
using UnityEngine;
using Chess.Core;
using UnityEngine.UI;

namespace Chess.UI {
    public class PatternSelectionManager : MonoBehaviour {
        [SerializeField] Button[] buttons;
        Unit unit;
        MovementPattern activePattern;
        PlayerType playerType;

        public void SetPlayerType(PlayerType playerType) {
            this.playerType = playerType;
        }

        public void SetUnit(Unit newUnit) {
            if (unit == newUnit) return;
            unit = newUnit;
            activePattern = null;
            SetButtons();
        }

        public bool ValidMovement(Tile testTile) {
            return unit != null && activePattern != null && unit.ValidMovement(testTile, activePattern);
        }

        private void SetButtons() {
            for(int i = 0; i < 4; i++) {
                AddPatternToButton(i, unit.GetNextPattern());
            }
        }

        //add on hover show possible moves
        private void AddPatternToButton(int buttonNum, MovementPattern pattern) {
            Button button = buttons[buttonNum];
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SetActivePattern(buttonNum, pattern));
            button.GetComponentInChildren<Text>().text = pattern.GetMovement().ToString();
        }

        public void MoveUnit(Tile newTile) {
            if (unit == null) Debug.LogError("There is no unit");
            unit.GenerateValidMovements(activePattern);
            unit.CmdMove(newTile);
            activePattern = null;
        }

        private void SetActivePattern(int buttonNum, MovementPattern pattern) {
            if(activePattern != null)unit.GenerateValidMovements(activePattern);//reset previous patterns 
            activePattern = pattern;
            unit.GenerateValidMovements(activePattern, Color.green);// sets new pattern to green
            AddPatternToButton(buttonNum, pattern);
        }



    }
}