using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Chess.Core {
    public class OnTile: NetworkBehaviour  {
        public UnityEvent<Tile> OnSetTile;
        Tile tile;
        public void SetTile(Tile newTile) {
            OnSetTile?.Invoke(newTile);
            tile = newTile;
        }

        public Tile GetTile() => tile;
    }
}