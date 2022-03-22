using Chess.Combat;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.UI {
    public class HealthBar : MonoBehaviour {
        [SerializeField] Slider slider;
        //[SerializeField] Unit unit;

        // Update is called once per frame
        void Update() {
            //slider.value = unit.HealthPercentage;
            transform.LookAt(Camera.main.transform.position);
        }

        public void SetHealth(float healthPercentage) {
            slider.value = healthPercentage;
        }
    }
}