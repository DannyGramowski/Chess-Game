using System.Collections;
using UnityEngine;

namespace Chess.UI {
    public abstract class IUI : MonoBehaviour {
        public abstract void SetDisplay(object data);

        public abstract UIType GetUIType();
      
    }
}