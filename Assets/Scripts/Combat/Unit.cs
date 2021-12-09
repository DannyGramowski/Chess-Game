using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Chess.UI;
using Mirror;

namespace Chess.Core {
    public class Unit : NetworkBehaviour {
        [SerializeField] MovementPattern[] movementPatterns;
        

        [SyncVar] private Tile currTile;
        private Queue<MovementPattern> patternQueue;
        private PlayerType playerType = PlayerType.player1;
        private PlayerPointer playerPointer;
        IsSelectable selectable;
        OnTile onTile;
        public void Awake() {
            print("start");
            SetQueue();
            playerPointer = GetComponent<PlayerPointer>();
            selectable = GetComponent<IsSelectable>();
            selectable.AddSelectionValidParameters(IsSelectable);
            onTile = GetComponent<OnTile>();
       //     not able to move units and units are able to be spawned on top of each other. I think that OnTile is not getting set.
        }

        public bool IsSelectable(PlayerType playerType) {
            return true;
        }

        public void OnSelect() {
            print("selected " + name);
         //   PatternSelectionManager patternSelection = GlobalPointers.GetPlayerPointer(playerType).patternSelectionManager;
          //  patternSelection.SetUnit(this);
        }

        [Command]
        public void CmdMove(Tile newTile) {
            print("move");
            currTile?.AddOnTile(null);
            currTile = newTile;
            transform.position = currTile.transform.position;
            currTile.AddOnTile(onTile);
            print("on tile is " + onTile);
            print("tile on tile is " + onTile);
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
            foreach(var pos in pattern.GetValidOffsets(currTile.GetGridPos(), playerPointer.matrix.GetMatrixSize())) {
                Tile tile = playerPointer.matrix.GetTile(pos);
                if (ValidMovement(tile, pattern)) { 
                    if (newColor == default(Color)) tile.SetColor(playerPointer.matrix.GetColor(tile.GetGridPos()));//if there is no color specified it resets the color
                    else tile.SetColor(newColor);
                }
            }
        }

        public bool ValidMovement(Tile tile, MovementPattern pattern) {
            return tile.onTile == null && pattern.ValidMovement(currTile,tile);
        }

        public void PlacedOnTile(Tile tile) {
        }
    }
}