using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using Mirror;
//using System;

namespace Chess.Core {
    public class Matrix : NetworkBehaviour {
        [Min(1)]
        [SerializeField] Vector3Int matrixSize ;
        //  [SerializeField] Vector3Int playerStartLoc;
        [SerializeField] int unitSpawnLevel;
        [SerializeField] Tile tilePrefab;
        [SerializeField] Obstacle obstaclePrefab;
        [SerializeField] Transform parentPrefab; //parent container for the tiles in the grid
        [SerializeField] Color[] colorCycle;
       // [SerializeField] Unit unit;
        [SerializeField] Transform unitParent;
        [SerializeField][Range(0, 1)] float obstacleDensity;
        [SerializeField] int areaWithoutObstacles;//width on each side without obstacle spawn
        [Min(0.001f)]
        [SerializeField] Vector3 tileOffset = Vector3.one; //the offset of the tiles in the grid
        Tile[ , , ] tileMatrix;

        private void Start() {
            Task<IEnumerable<Vector3Int>> task = Task<IEnumerable<Vector3Int>>.Factory.StartNew(() => GenerateObstaclePositions());
            if (GetComponentInChildren<Tile>() == null) CreateMatrix();
            else SetMatrixPointers();
            task.Wait();

            GenerateObstacles(task.Result);
        }

        #region Server

        #endregion

        #region Client

        #endregion

        [Button("Create Grid")]
        public void CreateMatrix() {
            ClearChildren();
            tileMatrix = new Tile[matrixSize.x, matrixSize.y, matrixSize.z];
            Vector3 previousPos = Vector3.zero;
            for(int y = 0; y < tileMatrix.GetLength(1);  y++) {
                Transform parentY = Instantiate(parentPrefab, transform);
                parentY.name = "y parent " + y;
                for(int z = 0; z < tileMatrix.GetLength(2); z++) {
                    Transform parentZ = Instantiate(parentPrefab, parentY);
                    parentZ.name = "z parent " + z;
                    for(int x = 0; x < tileMatrix.GetLength(0); x++) {
                        var temp = Instantiate(tilePrefab, new Vector3(x * tileOffset.x, y * tileOffset.y, z * tileOffset.z), Quaternion.identity, parentZ);
                        NetworkServer.Spawn(temp.gameObject);
                        temp.name = $"tile({x}, {y}, {z}";
                        temp.Setup(new Vector3Int(x, y, z));
                        temp.GetComponentInChildren<MeshRenderer>().material.color = GetColor(temp.GetGridPos());
                        if (Application.isPlaying) tileMatrix[x, y, z] = temp;
                    }
                }
            }       
        }

        [Button("Clear Children")]
        public void ClearChildren() {
            for(int i = transform.childCount - 1; i >= 0; i--) {
              //  print("child count " + transform.childCount + " i " + i);
                Transform child = transform.GetChild(i);
              //  print(child.name);
                DestroyImmediate(child.gameObject);
            }
        }    

        public Tile GetTile(Vector3Int pos) {
            return tileMatrix[pos.x, pos.y, pos.z];
        }

        public Tile GetTile(int x, int y, int z) {
            return tileMatrix[x, y, z];
        }

        public Vector3Int GetMatrixSize() {
            return new Vector3Int(tileMatrix.GetLength(0), tileMatrix.GetLength(1), tileMatrix.GetLength(2));
        }

        public Color GetColor(Vector3Int pos ) {
            return colorCycle[pos.y % colorCycle.Length];
        }

        private void SetMatrixPointers() {
            tileMatrix = new Tile[matrixSize.x, matrixSize.y, matrixSize.z];
            //print("set grid pointers");
            var tiles = GetComponentsInChildren<Tile>();
            foreach(Tile tile in tiles) {
                Vector3Int temp = tile.GetGridPos();
                tileMatrix[temp.x, temp.y, temp.z] = tile;
            }
        }

        private IEnumerable<Vector3Int> GenerateObstaclePositions() {
            Dictionary<int, Vector3Int> positions = new Dictionary<int, Vector3Int>();
            int totalTiles = (int) ((matrixSize.x - (2 * areaWithoutObstacles)) * (matrixSize.y) * (matrixSize.z - (areaWithoutObstacles)) * obstacleDensity);
            int i = 0, a = 0;
            while(i < totalTiles) {
                a++;
                if(a > totalTiles * 3) {                
                    Debug.LogError("stopped due to infinite loop");
                }
            //for (int a = 0; a < 10; a++) {   
                var temp = new Vector3Int(Utils.GetRandomNumber(0, matrixSize.x), Utils.GetRandomNumber(0, matrixSize.y), Utils.GetRandomNumber(0 + areaWithoutObstacles, matrixSize.z - areaWithoutObstacles));
                if(!positions.ContainsKey(temp.GetHashCode())) {
                    i++;
                    positions[temp.GetHashCode()] = temp;
                }
            }
            return positions.Values;
        }
        
        public void SetPlayerPieces(List<Unit> units, PlayerType playerType) {

            int min = 0;
            int max = areaWithoutObstacles;
            if(playerType == PlayerType.player1) {
                min = matrixSize.z - areaWithoutObstacles;
                max = matrixSize.z;
            }

            foreach (Unit unit in units) {
                print("start place unit " + unit);
                Tile tile = null;
                while (true) {
                    Vector3Int pos = new Vector3Int(Utils.GetRandomNumber(0, matrixSize.x), unitSpawnLevel, Utils.GetRandomNumber(min, max));
                    print("new test pos " + pos + " for " + unit);
                    tile = GetTile(pos);
                    if (tile.IsEmpty()) break;
                }
                print("set " + unit + " to tile " + tile);
                unit.CmdMove(tile);
            }     
            /*Tile pos = GetTile(playerStartLoc);
            var temp = Instantiate(unit, unitParent);
            temp.CmdMove(pos);
            pos.AddIOnTile(temp);*/
        }

        private void GenerateObstacles(IEnumerable<Vector3Int> positions) {
            foreach(var pos in positions) {
                Tile tile = GetTile(pos);
                var obstacle = Instantiate(obstaclePrefab, tile.transform);
                NetworkServer.Spawn(obstacle.gameObject);
                tile.AddOnTile(obstacle.GetComponent<OnTile>());
            }

        }
    }
}
//player piieces
//obstacles
// generate tiles
//powerups