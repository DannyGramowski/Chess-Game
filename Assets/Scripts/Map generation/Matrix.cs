using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Mirror;
using Chess.UI;
//using System;

namespace Chess.Core {
    public class Matrix : NetworkBehaviour {
        [Min(1)]
        [SerializeField] Vector3Int matrixSize;
        //  [SerializeField] Vector3Int playerStartLoc;
        [SerializeField] int unitSpawnLevel;
        [SerializeField] GameObject tilePrefab;
        [SerializeField] GameObject obstaclePrefab;
        [SerializeField] Transform parentPrefab; //parent container for the tiles in the grid
        [SerializeField] Color[] colorCycle;
        // [SerializeField] Unit unit;
        [SerializeField] Transform unitParent;
        [SerializeField] [Range(0, 1)] float obstacleDensity;
        [SerializeField] int areaWithoutObstacles;//width on each side without obstacle spawn
        [Min(0.001f)]
        [SerializeField] Vector3 tileOffset = Vector3.one; //the offset of the tiles in the grid
        [SerializeField] Tile[,,] tileMatrix;

        #region Server

        public override void OnStartServer() {
            CreateMatrix();
            GenerateObstacles();
            foreach(var player in GlobalPointers.chessNetworkManager.players) {
                player.SpawnSquadUnits();
                player.RpcSetPlaceDisplay();//prevents it being called twice
            }
            
        }

        

        [Server]
        public void CreateMatrix() {
            if (!NetworkServer.active) {
                print("server not active");
                return;
            }

            for (int y = 0; y < matrixSize.y; y++) {
                for (int x = 0; x < matrixSize.x; x++) {
                    for (int z = 0; z < matrixSize.z; z++) {
                        Vector3Int pos = new Vector3Int(x, y, z);
                        GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                        tile.GetComponent<Tile>().Setup(pos);
                        tile.GetComponentInChildren<MeshRenderer>().material.color = colorCycle[0];
                        NetworkServer.Spawn(tile);
                    }
                }
            }

            SetUpTiles();
        }


        [Server]
        public void SetUpPlayerPieces(List<OnTile> units, PlayerType playerType) {
           // print("set player pieces");
         //  not setting pieces to different sides
            int min = 0;
            int max = areaWithoutObstacles;
            if (playerType == PlayerType.player1) {
                min = matrixSize.z - areaWithoutObstacles;
                max = matrixSize.z;
            }

            foreach (OnTile unit in units) {
              //  print("start place unit " + unit);
                Tile tile = null;
                while (true) {
                    Vector3Int pos = new Vector3Int(Utils.GetRandomNumber(0, matrixSize.x), unitSpawnLevel, Utils.GetRandomNumber(min, max));
                   // print("new test pos " + pos + " for " + unit);
                    tile = GetTile(pos);
                  //  print("test tile " + tile);
                    if (tile.IsEmpty()) break;
                }
               // print("set " + unit + " to tile " + tile);
                unit.SetTile(tile);
            }
        }

        private void GenerateObstacles() {
            IEnumerable<Vector3Int> positions = GenerateObstaclePositions();
            foreach (var pos in positions) {
               // print("obstacle at pos " + pos);
                Tile tile = GetTile(pos);
                var obstacle = Instantiate(obstaclePrefab, tile.transform);
                NetworkServer.Spawn(obstacle);
                tile.AddOnTile(obstacle.GetComponent<OnTile>());
            }
            SetUpObstacles();
        }
        #endregion

        #region Client
        public override void OnStartClient() {
            if (isClientOnly) {
                SetUpTiles();
                SetUpObstacles();
            }
        }

        public void SetUpTiles() {
            tileMatrix = new Tile[matrixSize.x, matrixSize.y, matrixSize.z];
            var tiles = FindObjectsOfType<Tile>().Reverse();

            //holds all the x parents b/c we dont need to know y parents
            Transform[] xParents = new Transform[matrixSize.y * matrixSize.x];
            for(int y = 0; y < matrixSize.y; y++) {
                Transform yParent = Instantiate(parentPrefab, transform);
                yParent.name = "Y Parent " + y.ToString();
                for(int x = 0; x < matrixSize.x; x++) {
                    Transform xParent = Instantiate(parentPrefab, yParent);
                   // print("created " + xParent);
                    xParent.name = $"X Parent ({x},{y})";
                    xParents[y * matrixSize.x + x] = xParent;
                }
            }

                int parentIndex = 0;
            int zCapacity = 0;
            foreach (var tile in tiles) {
                Vector3Int gridPos = tile.GetGridPos();
                 tile.name = "tile " + gridPos.ToString();
                tile.GetComponentInChildren<MeshRenderer>().material.color = GetColor(gridPos);
                    tile.transform.parent = xParents[parentIndex];
                    tileMatrix[gridPos.x, gridPos.y, gridPos.z] = tile;
                    zCapacity++;
                    if(zCapacity == matrixSize.z) {
                        zCapacity = 0;
                        parentIndex++;
                    }
                }
            }
        
        public void SetUpObstacles() {
            var obstacles = FindObjectsOfType<Obstacle>();
            Transform obstacleParent = Instantiate(parentPrefab, transform);
            obstacleParent.name = "obstacle parent";

            foreach(var obstacle in obstacles) {
                obstacle.transform.parent = obstacleParent;
            }
        }

        #endregion

        #region HelperFunctions
        public Color GetColor(Vector3Int pos) {
            return colorCycle[pos.y % colorCycle.Length];
        }
        public Tile GetTile(Vector3Int pos) {
            //print($"get tile {pos.ToString()}");
            return GetTile(pos.x, pos.y, pos.z);
        }

        public Tile GetTile(int x, int y, int z) {
            return tileMatrix[x, y, z];
        }

        public Vector3Int GetMatrixSize() {
            return new Vector3Int(tileMatrix.GetLength(0), tileMatrix.GetLength(1), tileMatrix.GetLength(2));
        }

        private IEnumerable<Vector3Int> GenerateObstaclePositions() {
        //    print("generate obstacle positions");
            Dictionary<int, Vector3Int> positions = new Dictionary<int, Vector3Int>();
            int totalTiles = (int)((matrixSize.x - (2 * areaWithoutObstacles)) * (matrixSize.y) * (matrixSize.z - (areaWithoutObstacles)) * obstacleDensity);
    //        print("total tiles " + totalTiles);
            int i = 0, a = 0;
            while (i < totalTiles) {
                a++;
                if (a > totalTiles * 3) {
                    Debug.LogError("stopped due to infinite loop");
                }
                //for (int a = 0; a < 10; a++) {   
                var temp = new Vector3Int(Utils.GetRandomNumber(0, matrixSize.x), Utils.GetRandomNumber(0, matrixSize.y), Utils.GetRandomNumber(0 + areaWithoutObstacles, matrixSize.z - areaWithoutObstacles));
                if (!positions.ContainsKey(temp.GetHashCode())) {
                    i++;
                    positions[temp.GetHashCode()] = temp;
                }
            }
          //  print("end generate obstacle positions");
            return positions.Values;
        }

       
            /*Tile pos = GetTile(playerStartLoc);
            var temp = Instantiate(unit, unitParent);
            temp.CmdMove(pos);
            pos.AddIOnTile(temp); */
}

        #endregion

    }


//player piieces
//obstacles
// generate tiles
//powerups

