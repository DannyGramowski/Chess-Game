using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Chess.Core {
    public class Grid : MonoBehaviour {
        [Min(1)][AssetsOnly]
        [SerializeField] Vector3Int gridSize ;
        [SerializeField] Tile tilePrefab;
        [SerializeField] Transform parentPrefab; //parent container for the tiles in the grid
        [SerializeField] Material[] colorCycle;
        [Min(0.001f)]
        [SerializeField] Vector3 tileOffset = Vector3.one; //the offset of the tiles in the grid
        Tile[ , , ] tileGrid;

        private void Start() {
            if(transform.childCount == 0) CreateGrid();
        }
        [Button("Create Grid")]
        public void CreateGrid() {
            ClearChildren();
            tileGrid = new Tile[gridSize.x, gridSize.y, gridSize.z];
            //float x=0, y=0,z=0
            Vector3 previousPos = Vector3.zero;
            for(int y = 0; y < tileGrid.GetLength(1);  y++) {
                Transform parentY = Instantiate(parentPrefab, transform);
                parentY.name = "y parent " + y;
                for(int z = 0; z < tileGrid.GetLength(2); z++) {
                    Transform parentZ = Instantiate(parentPrefab, parentY);
                    parentZ.name = "z parent " + z;
                    for(int x = 0; x < tileGrid.GetLength(0); x++) {
                        var temp = Instantiate(tilePrefab, new Vector3(x * tileOffset.x, y * tileOffset.y, z * tileOffset.z), Quaternion.identity, parentZ  );
                        temp.name = $"tile({x}, {y}, {z}";
                        temp.GetComponentInChildren<MeshRenderer>().material = colorCycle[y % colorCycle.Length];
                    }
                }
            }       
        }
        [Button("Clear Children")]
        public void ClearChildren() {
            /*Transform[] children = transform.GetComponentsInChildren<Transform>();
            
            foreach(var c in children) {
                if (transform == c) continue;
                print("destroying " + c.name);
                //DestroyImmediate(c.gameObject);
            }*/
            for(int i = transform.childCount - 1; i >= 0; i--) {
                print("child count " + transform.childCount + " i " + i);
                Transform child = transform.GetChild(i);
                print(child.name);
                DestroyImmediate(child.gameObject);
            }
        }    

        public Tile GetTile(Vector3Int pos) {
            return tileGrid[pos.x, pos.y, pos.z];
        }
    }
}
