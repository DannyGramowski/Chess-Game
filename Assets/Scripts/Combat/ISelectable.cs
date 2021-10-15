using System.Collections;
using UnityEngine;

namespace Chess.Core {
    public interface ISelectable {
        bool IsSelectable(PlayerType playerType);
        void OnSelect();
    }
}