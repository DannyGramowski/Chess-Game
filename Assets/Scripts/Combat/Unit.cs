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
           // GenerateValidMovements(movementPatterns[0], Color.green);
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

        public void SetTile(Tile newTile) {
            currTile = newTile;
            transform.position = currTile.transform.position;
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

        public void GenerateValidMovements(MovementPattern pattern, Color newColor) {
            foreach(var pos in pattern.GetValidOffsets(currTile.GetGridPos())) {
                Tile tile = Grid.Instance.GetTile(pos);
                if(ValidMovement(tile, pattern))tile.GetComponentInChildren<MeshRenderer>().material.color = newColor;
            }
        }

        public bool ValidMovement(Tile tile, MovementPattern pattern) {
       //     Debug.Log($"test valid movement for {tile} and returned {tile.onTile == null} and{pattern.ValidMovement(currTile, tile)}");
            return tile.onTile == null && pattern.ValidMovement(currTile,tile);
        }
    }
}