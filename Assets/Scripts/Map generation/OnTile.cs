using Mirror;
using System.Collections;
using UnityEngine;

namespace Chess.Core {
    public class OnTile: NetworkBehaviour  {
        Tile tile;
        public void SetTile(Tile newtile) {
            tile = newtile;
        }

        public Tile GetTile() => tile;
    }
}