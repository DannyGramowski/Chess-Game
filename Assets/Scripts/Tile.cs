using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Core {
    public class Tile : MonoBehaviour {
        public IOnTile onTile;
        Vector3Int gridPos;
        public void Setup(Vector3Int gridPos) {
            this.gridPos = gridPos;
        }

        public Vector3Int GetGridPos() {
            return gridPos;
        }
    }
}
