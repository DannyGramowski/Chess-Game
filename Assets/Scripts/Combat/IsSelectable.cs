using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Chess.Core {
    public class IsSelectable: NetworkBehaviour{
        public UnityEvent OnSelectEvent;
        public UnityEvent OnDeselectEvent;
        Func<PlayerType, bool> selectionValid;

        public void AddSelectionValidParameters(Func<PlayerType, bool> methods) {
            selectionValid += methods;
        }

        public void AddOnSelectEvent(UnityAction onSelect) {
            OnSelectEvent.AddListener(onSelect);
        }  
        public void AddOnDeselectEvent(UnityAction onDeselect) {
            OnDeselectEvent.AddListener(onDeselect);
        } 

        public bool SelectionValid(PlayerType playerType) {
            return selectionValid(playerType);
        }

        public void OnSelect() => OnSelectEvent?.Invoke();
        public void OnDeselect() => OnSelectEvent?.Invoke();

    }
}