using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Chess.Core {
    public class IsSelectable: NetworkBehaviour{
        Func<PlayerType, bool> selectionValid;
        [SerializeField] UnityEvent OnSelectEvent;

        public void AddSelectionValidParameters(Func<PlayerType, bool> methods) {
            selectionValid += methods;
        }

        public bool SelectionValid(PlayerType playerType) {
            bool result = selectionValid(playerType);
            
            return result;
        }

        public void OnSelect() => OnSelectEvent?.Invoke();
/*        bool IsSelectable(PlayerType playerType);
        void OnSelect();*/
    }
}