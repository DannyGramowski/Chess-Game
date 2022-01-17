using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.Core {
    [SelectionBase]
    public class Tile : NetworkBehaviour {
        [SyncVar] public OnTile onTile;
        [SyncVar] Vector3Int mapPos;
        // try moving to a different component to see if this is causeing unity not to be able to sync it
        IsSelectable isSelectable;

        #region Server
        public void Setup(Vector3Int mapPos) {
           // print(name + " setup " + connectionToServer);
            this.mapPos = mapPos;
            if (GlobalPointers.useDebug) SetText(mapPos);
           
        }
        #endregion

        #region Client

        public override void OnStartClient() {
            isSelectable = GetComponent<IsSelectable>();
            isSelectable.AddSelectionValidParameters(SelectionValid);
        }

        #endregion

        #region Helper
        public Vector3Int GetGridPos() {
            return mapPos;
        }

        public void SetColor(Color color) {
            GetComponentInChildren<MeshRenderer>().material.color = color;
        }

        public void ResetColor() {
            SetColor(GlobalPointers.matrix.GetColor(GetGridPos()));
        }
        #endregion

      

        public void AddOnTile(OnTile onTile) {
           // print(name + " added on tile " + onTile);
            //setting on tile in setup but it is getting cleared propably becasue syncvar
            this.onTile = onTile;
        }

        public bool IsEmpty() => onTile == null;

        public bool SelectionValid(PlayerType playerType) => true;


        public void OnSelect() { }

        private void SetText(Vector3Int pos ) {
            GetComponent<TextDrawer>().SetText(pos.ToString());
        }
    }
}
