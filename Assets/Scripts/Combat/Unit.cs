using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Chess.UI;

namespace Chess.Core {
    public class Unit : MonoBehaviour, ISelectable, IOnTile {
        [SerializeField] MovementPattern[] movementPatterns;
        
        private Tile currTile;
        private Queue<MovementPattern> patternQueue;
        private PlayerType playerType = PlayerType.player1;
        void Start() {
            SetQueue();
        }

        public bool IsSelectable(PlayerType playerType) {
            return true;
        }

        public void OnSelect() {
            print("selected " + name);
            PatternSelectionManager patternSelection = GlobalPointers.GetPlayerPointer(playerType).patternSelectionManager;
            patternSelection.SetUnit(this);
        }

        public void Move(Tile newTile) {
            currTile?.AddIOnTile(null);
            currTile = newTile;
            transform.position = currTile.transform.position;
            currTile.AddIOnTile(this);
        }

        public MovementPattern GetNextPattern() {
            var temp = patternQueue.Dequeue();
            patternQueue.Enqueue(temp);
            return temp;
        }
        private void SetQueue() {
            System.Random rnd = new System.Random();
            patternQueue = new Queue<MovementPattern>(movementPatterns.OrderBy(pat => rnd.Next()));
        }

        public void GenerateValidMovements(MovementPattern pattern, Color newColor = default(Color)) {
            foreach(var pos in pattern.GetValidOffsets(currTile.GetGridPos())) {
                Tile tile = Grid.Instance.GetTile(pos);
                if (ValidMovement(tile, pattern)) { 
                    if (newColor == default(Color)) tile.SetColor(Grid.Instance.GetColor(tile.GetGridPos()));//if there is no color specified it resets the color
                    else tile.SetColor(newColor);
                }
            }
        }

        public bool ValidMovement(Tile tile, MovementPattern pattern) {
            return tile.onTile == null && pattern.ValidMovement(currTile,tile);
        }
    }
}