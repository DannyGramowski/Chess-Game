using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Chess.Core;
using Chess.UI;
using Mirror;

namespace Chess.Combat {
    public class Unit : NetworkBehaviour {
        [SerializeField] MovementPattern[] movementPatterns;
        [SerializeField] Ability[] abilities; //movement and attacking will be considered abilities


        [SyncVar] private Tile currTile;
        private Queue<MovementPattern> patternQueue;
        private PlayerType playerType;
        private PlayerPointer playerPointer;
        private IsSelectable selectable;
        private OnTile onTile;
        private MovementPattern currentPattern;

        #region Server

        #endregion

        #region Client
        public void Awake() {
            //print("start");
            SetQueue();
            playerPointer = FindObjectsOfType<PlayerPointer>().Where(x => x.playerType == playerType).First();
            selectable = GetComponent<IsSelectable>();
            selectable.AddSelectionValidParameters(IsSelectable);
            selectable.AddOnSelectEvent(OnSelect);
            selectable.AddOnDeselectEvent(OnDeselect);
            onTile = GetComponent<OnTile>();

            //     not able to move units and units are able to be spawned on top of each other. I think that OnTile is not getting set.
        }

        [Command]
        public void CmdMove(Tile newTile) {
            Move(newTile);
        }
        #endregion

        public bool IsSelectable(PlayerType playerType) => this.playerType == playerType;

        public void OnSelect() => GlobalPointers.UI_Manager.SetUI(UIType.abilityDisplayManager, this);
        public void OnDeselect() { 
            GenerateValidMovements(currentPattern);
            currentPattern = null;
        }
            
        
        [Server]
        public void Move(Tile newTile) {
            currTile?.AddOnTile(null);
            currTile = newTile;
            transform.position = currTile.transform.position;
            currTile.AddOnTile(onTile);
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
           // print("pattern " + pattern);
            //print("curr tile " + currTile);
            Matrix matrix = GlobalPointers.matrix;
            currentPattern = pattern;
           // print("player pointer " + matrix);
            foreach(var pos in pattern.GetValidOffsets(currTile.GetGridPos(), matrix.GetMatrixSize())) {
                Tile tile = matrix.GetTile(pos);
                if (ValidMovement(tile, pattern)) { 
                    if (newColor == default(Color)) tile.ResetColor();//if there is no color specified it resets the color
                    else tile.SetColor(newColor);
                }
            }
        }

        public bool ValidMovement(Tile tile, MovementPattern pattern) {
            return tile.onTile == null && pattern.ValidMovement(currTile,tile);
        }

        public void PlacedOnTile(Tile tile) {
        }

        public Ability[] GetAbilities() => abilities;
    }
}