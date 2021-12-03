using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
//using System;

namespace Chess.Core {
    public class Grid : Singleton<Grid> {
        [Min(1)]
        [SerializeField] Vector3Int gridSize ;
        [SerializeField] Vector3Int playerStartLoc;
        [SerializeField] Tile tilePrefab;
        [SerializeField] Obstacle obstaclePrefab;
        [SerializeField] Transform parentPrefab; //parent container for the tiles in the grid
        [SerializeField] Color[] colorCycle;
        [SerializeField] Unit unit;
        [SerializeField] Transform unitParent;
        [SerializeField][Range(0, 1)] float obstacleDensity;
        [SerializeField] int areaWithoutObstacles;//width on each side without obstacle spawn
        [Min(0.001f)]
        [SerializeField] Vector3 tileOffset = Vector3.one; //the offset of the tiles in the grid
        Tile[ , , ] tileGrid;

        private void Start() {
            Task<IEnumerable<Vector3Int>> task = Task<IEnumerable<Vector3Int>>.Factory.StartNew(() => GenerateObstaclePositions());
            if (GetComponentInChildren<Tile>() == null) CreateGrid();
            else SetGridPointers();
            SetPlayerPieces();
            task.Wait();

            GenerateObstacles(task.Result);
        }

        [Button("Create Grid")]
        public void CreateGrid() {
            ClearChildren();
            print("create grid");
            tileGrid = new Tile[gridSize.x, gridSize.y, gridSize.z];
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
                        temp.Setup(new Vector3Int(x, y, z));
                        temp.GetComponentInChildren<MeshRenderer>().material.color = GetColor(temp.GetGridPos());
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

        public Tile GetTile(Vector3Int pos) {
            return tileGrid[pos.x, pos.y, pos.z];
        }

        public Tile GetTile(int x, int y, int z) {
            return tileGrid[x, y, z];
        }

        public Vector3Int GetGridSize() {
            return new Vector3Int(tileGrid.GetLength(0), tileGrid.GetLength(1), tileGrid.GetLength(2));
        }

        public Color GetColor(Vector3Int pos ) {
            return colorCycle[pos.y % colorCycle.Length];
        }

        private void SetGridPointers() {
            tileGrid = new Tile[gridSize.x, gridSize.y, gridSize.z];
            print("set grid pointers");
            var tiles = GetComponentsInChildren<Tile>();
            foreach(Tile tile in tiles) {
                Vector3Int temp = tile.GetGridPos();
                tileGrid[temp.x, temp.y, temp.z] = tile;
            }
        }

        private IEnumerable<Vector3Int> GenerateObstaclePositions() {
            Dictionary<int, Vector3Int> positions = new Dictionary<int, Vector3Int>();
            int totalTiles = (int) ((gridSize.x - (2 * areaWithoutObstacles)) * (gridSize.y) * (gridSize.z - (areaWithoutObstacles)) * obstacleDensity);
            int i = 0, a = 0;
            while(i < totalTiles) {
                a++;
                if(a > totalTiles * 2) {                
                    Debug.LogError("stopped due to infinite loop");
                }
            //for (int a = 0; a < 10; a++) {   
                var temp = new Vector3Int(Utils.GetRandomNumber(0, gridSize.x), Utils.GetRandomNumber(0, gridSize.y), Utils.GetRandomNumber(0 + areaWithoutObstacles, gridSize.z - areaWithoutObstacles));
                if(!positions.ContainsKey(temp.GetHashCode())) {
                    i++;
                    positions[temp.GetHashCode()] = temp;
                }
            }
            return positions.Values;
        }
        
        private void SetPlayerPieces() {
            Tile pos = GetTile(playerStartLoc);
            var temp = Instantiate(unit, unitParent);
            temp.Move(pos);
            pos.AddIOnTile(temp);
        }

        private void GenerateObstacles(IEnumerable<Vector3Int> positions) {
            foreach(var pos in positions) {
                Tile tile = GetTile(pos);
                var obstacle = Instantiate(obstaclePrefab, tile.transform);
                tile.AddIOnTile(obstacle);
            }

        }
    }
}
//player piieces
//obstacles
// generate tiles
//powerups