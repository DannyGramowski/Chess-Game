using Chess.Combat;
using Chess.Utility;
using UnityEngine;

namespace Chess.UI {
    public class AbilityDisplayManager : IUI {
        [SerializeField] AbilityDisplay abilityDisplayPrefab;
        [SerializeField] RectTransform layoutGroup;

        ObjectPool<AbilityDisplay> abilityDisplayPool;
        Unit currUnit;
        // Start is called before the first frame update
        void Start() {
            abilityDisplayPool = new ObjectPool<AbilityDisplay>(abilityDisplayPrefab, layoutGroup);
            foreach (var display in GetComponentsInChildren<AbilityDisplay>()) display.gameObject.SetActive(false);
        }

        void AddAbilityDisplay(Ability ability) {
            AbilityDisplay display = abilityDisplayPool.GetObject();
            display.gameObject.SetActive(true);
            display.SetDisplay(ability);
        }

        void DisableDisplay(AbilityDisplay display) {
            display.gameObject.SetActive(false);
            abilityDisplayPool.ReturnToPool(display);
        }

        public override void SetDisplay(object data) {
            currUnit = (Unit) data;
            var activeDisplays = GetComponentsInChildren<AbilityDisplay>();
            foreach (var display in activeDisplays) DisableDisplay(display);
            foreach (var ability in currUnit.GetAbilities()) AddAbilityDisplay(ability);
        }

        public override UIType GetUIType() => UIType.abilityDisplayManager;

    }
}
