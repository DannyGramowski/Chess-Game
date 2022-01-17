using Chess.Core;
using System.Collections;
using UnityEngine;

namespace Chess.Combat {
    public abstract class Ability : MonoBehaviour {
        public int ActionPointCost => actionPointCost;
        public string AbilityName => abilityName;
        public Sprite AbilityImage => abilityImage;     
        
        [SerializeField]  int actionPointCost;
        [SerializeField]  string abilityName;
        [SerializeField]  Sprite abilityImage;

        public abstract void ActivateAbility();
       
    }
}