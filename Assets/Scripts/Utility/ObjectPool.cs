using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Utility {
    public class ObjectPool<t> where t : MonoBehaviour {
        Queue<t> objectPool = new Queue<t>();
        t prefab;

        int startingPool = 5;
        
        public ObjectPool(t prefab,Transform parent = null) {
            this.prefab = prefab;
            for(int i = 0; i < startingPool; i++) objectPool.Enqueue(MonoBehaviour.Instantiate(prefab, parent));
        }

        public t GetObject() {
            if (objectPool.Count > 0) return objectPool.Dequeue();
            return MonoBehaviour.Instantiate(prefab);
        }

        public void ReturnToPool(t newObject) {
            objectPool.Enqueue(newObject);
        }
       
    }
}