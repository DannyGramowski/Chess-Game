using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.Core {
    public class Tile : MonoBehaviour, ISelectable {
        public IOnTile onTile;
        Vector3Int gridPos;
        [SerializeField] Text text;

        public void Setup(Vector3Int gridPos) {
            this.gridPos = gridPos;
            if (GlobalPointers.useDebug) SetText(gridPos);
        }

        public Vector3Int GetGridPos() {
            return gridPos;
        }

        public void SetColor(Color color) {
            GetComponentInChildren<MeshRenderer>().material.color = color;
        }

        public void AddIOnTile(IOnTile onTile) {
            this.onTile = onTile;
        }

        public bool IsSelectable(PlayerType playerType) => true;


        public void OnSelect() { }

        private void SetText(Vector3Int pos ) {
            text.text = pos.ToString();
        }
    }
}
