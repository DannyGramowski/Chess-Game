using System.Collections;
using UnityEngine;

namespace Chess.Combat {
    [CreateAssetMenu(fileName = "new weapon", menuName = "Create Weapon")]

    public class Weapon : ScriptableObject {
        public int Damage => damage;
        public int Range => range;
        public int ActionPointCost => actionPointCost;
        public int EquitmentPointCost => equitmentPointCost;

        public string WeaponName => weaponName;

        [SerializeField] int damage = 1;
        [SerializeField] int range = 2;
        [SerializeField] int actionPointCost = 2;
        [SerializeField] int equitmentPointCost = 5;
        [SerializeField] string weaponName = "default";
    }
}