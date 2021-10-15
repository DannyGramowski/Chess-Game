using System.Collections;
using UnityEngine;
using System;
namespace Chess.Core {
    public static class Utils {
        private static System.Random randomGenerator = new System.Random();

        //returns float between 0 and 1
        public static float GetRandomNumber() {
            int ran = randomGenerator.Next(1000);
            return ran / 1000f;
        }

        public static int GetRandomNumber(int min, int max) {
            return randomGenerator.Next(min, max);
        }
    }
}