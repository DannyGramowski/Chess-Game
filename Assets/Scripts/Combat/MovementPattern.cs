using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Core {
    [CreateAssetMenu(fileName = "new movement pattern", menuName = "Create Movement Pattern")]
    public class MovementPattern : ScriptableObject {
        [SerializeField] Vector3Int movement;

        private const int X_Z_HASH_CONST= 691;
        private const int Y_HASH_CONST = 2011;
        private int checkHash; //multiplies the movement values by prime numbers to determine if the tested movement is a variation of the pattern
        
        private void OnEnable() {
            checkHash = movement.x * X_Z_HASH_CONST + movement.y * Y_HASH_CONST + movement.z * X_Z_HASH_CONST;
        }
        public Vector3Int GetMovement() {
            return movement;
        }

        public bool ValidMovement(Tile currentTile, Tile toTile) {
            int distX = Mathf.Abs(toTile.GetGridPos().x - currentTile.GetGridPos().x);
            int distY = Mathf.Abs(toTile.GetGridPos().y - currentTile.GetGridPos().y);
            int distZ = Mathf.Abs(toTile.GetGridPos().z - currentTile.GetGridPos().z);
            int testHash = distX * X_Z_HASH_CONST + distY * Y_HASH_CONST + distZ * X_Z_HASH_CONST;
            return testHash == checkHash;
        }

        public IEnumerable<Vector3Int> GetValidOffsets(Vector3Int unitPos) {
            List<Vector3Int> output = new List<Vector3Int>();
            //int a = 0;
            //loops through y values. if there is movement in the y it is 2 iterations otherwise it is only one
            for(int y = 1+NormailizeInt(movement.y); y > 0; y--) {
               // for(int i = 4 * NormailizeInt(movement.x) + 4 * NormailizeInt(movement.z); i >= 0; i--) {
                for(int i = 0; i < 4; i++) {
                    //a++;
                  //  Debug.Log("a is " + a);
                    //x part
                    //y part gets a positve version of movemnt.y and a negative version of movemnt.y if movement.y does not equal 0. it uses the Mathf.pow to change the sign to get the different cases
                    Vector2Int layerCoordsX = new Vector2Int((int)(Mathf.Sin(i * 1.57075f) * movement.x),(int)( Mathf.Cos(i * 1.57075f) * movement.x));
                    Vector2Int layerCoordsZ = new Vector2Int((int)(Mathf.Cos(i * 1.57075f) * movement.z),(int)( Mathf.Sin(i * 1.57075f) * movement.z));
                    int yCoord = (int)Mathf.Pow(-1, y) * movement.y;
                    Vector3Int offset1 = new Vector3Int(layerCoordsX.x + layerCoordsZ.x + unitPos.x, yCoord + unitPos.y, layerCoordsX.y + layerCoordsZ.y + unitPos.z);
                    Vector3Int offset2 = new Vector3Int(layerCoordsX.x - layerCoordsZ.x + unitPos.x, yCoord + unitPos.y, layerCoordsX.y - layerCoordsZ.y + unitPos.z);
                //    Debug.Log("test " + offset1 + " with result " + CheckBounds(offset1)); 
                    if(CheckBounds(offset1))output.Add(offset1);
                    //almost working might be a bounds problem
                    //add logic to prevent out of bounds
                    //  var otherCoord = new Vector3Int(layerCoordsX.x - layerCoordsZ.x, yCoord, layerCoordsX.y - layerCoordsZ.y);
               //     Debug.Log("test " + offset2 + " with result " + CheckBounds(offset2) + " multiplication of " + (offset2.x * offset2.z != 0));

                    if (offset2.x * offset2.z != 0 && CheckBounds(offset2))output.Add(offset2);
                }
            }
            return output;
        }

        private bool CheckBounds(Vector3Int testPos) {
            Vector3Int gridSize = Grid.Instance.GetGridSize();
            return testPos.x >= 0 && testPos.y > 0 && testPos.z > 0 && testPos.x < gridSize.x && testPos.y < gridSize.y &&testPos.z < gridSize.z;
        }

        //if 0 return zero otherwise returns one
        private int NormailizeInt(int i) {
            return i > 0 ? i / i : 0;
        }
    }
}