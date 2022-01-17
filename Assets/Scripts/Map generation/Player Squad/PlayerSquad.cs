using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Combat {
    [CreateAssetMenu(fileName = "New Player Squad", menuName = "Create Player Squad")]
    public class PlayerSquad : ScriptableObject {
        
        
        [SerializeField] int numUnits;
        [SerializeField] List<Unit> squad;

        private void Awake() {
            if (squad.Count > numUnits) Debug.LogError("squad length is larger than size");
        }

        public List<Unit> GetUnits() => squad;
        public Unit GetUnit(int index) => squad[index];

        public void SetUnit(Unit unit, int index) {
            if (index >= numUnits) Debug.LogError(index + " is greater than " + numUnits);
            squad[index] = unit;
        }
    }
}