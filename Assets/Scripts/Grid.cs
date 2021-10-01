using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
//using System;

namespace Chess.Core {
    public class Grid : MonoBehaviour {
       // System.Random randomGenerator = new System.Random();
        [Min(1)]
        [SerializeField] Vector3Int gridSize ;
        [SerializeField] Tile tilePrefab;
        [SerializeField] Obstacle obstaclePrefab;
        [SerializeField] Material selectColor;
        [SerializeField] Transform parentPrefab; //parent container for the tiles in the grid
        [SerializeField] Material[] colorCycle;
        [SerializeField][Range(0, 1)] float obstacleDensity;
        [SerializeField] int areaWithoutObstacles;//width on each side without obstacle spawn
        [Min(0.001f)]
        [SerializeField] Vector3 tileOffset = Vector3.one; //the offset of the tiles in the grid
        Tile[ , , ] tileGrid;

        private void Start() {
            Task<IEnumerable<Vector3Int>> task = Task<IEnumerable<Vector3Int>>.Factory.StartNew(() => GenerateObstaclePositions());
            if (transform.childCount == 0) CreateGrid();
            else SetGridPointers();
            SetPlayerPieces();
            task.Wait();

            GenerateObstacles(task.Result);

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
                        var temp = Instantiate(tilePrefab, new Vector3(x * tileOffset.x, y * tileOffset.y, z * tileOffset.z), Quaternion.identity, parentZ);
                        temp.name = $"tile({x}, {y}, {z}";
                        temp.GetComponentInChildren<MeshRenderer>().material = colorCycle[y % colorCycle.Length];
                        temp.Setup(new Vector3Int(x, y, z));
                        if (Application.isPlaying) tileGrid[x, y, z] = temp;
                    }
                }
            }       
        }

        [Button("Clear Children")]
        public void ClearChildren() {
            for(int i = transform.childCount - 1; i >= 0; i--) {
                print("child count " + transform.childCount + " i " + i);
                Transform child = transform.GetChild(i);
                print(child.name);
                DestroyImmediate(child.gameObject);
            }
        }    

       /* [Button("test hash")]
        public void Test() {
            for(int x = 0; x < 4; x++) {
                for (int y = 0; y < 4; y++) {
                    for (int z = 0; z < 4; z++) {
                        Vector3 temp = new Vector3(x,y,z);
                        print(temp + " " + temp.GetHashCode());
                    }
                }
            }
        }*/

        public Tile GetTile(Vector3Int pos) {
            return tileGrid[pos.x, pos.y, pos.z];
        }

        private void SetGridPointers() {
            var tiles = GetComponentsInChildren<Tile>();
            foreach(Tile tile in tiles) {
                Vector3Int temp = tile.GetGridPos();
                tileGrid[temp.x, temp.y, temp.z] = tile;
            }
        }

        private IEnumerable<Vector3Int> GenerateObstaclePositions() {
            Dictionary<int, Vector3Int> positions = new Dictionary<int, Vector3Int>();
            int totalTiles = (int) ((gridSize.x - (2 * areaWithoutObstacles)) * (gridSize.y) * (gridSize.z - (areaWithoutObstacles)) * obstacleDensity);
            print("total tiles " + totalTiles);
            int i = 0, a = 0;
            while(i < totalTiles) {
                a++;
                if(a > totalTiles * 2) {
                    Debug.LogWarning("stopped due to infinite loop");
                }
            //for (int a = 0; a < 10; a++) {   
                print($"{i} i of {totalTiles} total tiles");
                var temp = new Vector3Int(Utils.GetRandomNumber(0 + areaWithoutObstacles, gridSize.x - areaWithoutObstacles), Utils.GetRandomNumber(0, gridSize.y), Utils.GetRandomNumber(0, gridSize.z));
                if(!positions.ContainsKey(temp.GetHashCode())) {
                    i++;
                    print("added obstacle to " + temp);
                    positions[temp.GetHashCode()] = temp;
                }
            }
            foreach(var output in positions.Values) {
                print("ouptut " + output);
            }
            return positions.Values;
        }
        
        private void SetPlayerPieces() {

        }

        private void GenerateObstacles(IEnumerable<Vector3Int> positions) {
            foreach(var pos in positions) {
                Tile tile = GetTile(pos);
                Instantiate(obstaclePrefab, tile.transform.position, Quaternion.identity, tile.transform);
                tile.GetComponentInChildren<MeshRenderer>().material = selectColor;
            }

        }
    }
}
//player piieces
//obstacles
// generate tiles
//powerups