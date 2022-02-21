using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess.Core;

namespace Chess.Combat {
    [CreateAssetMenu(fileName = "new movement pattern", menuName = "Create Movement Pattern")]
    public class MovementPattern : ScriptableObject {
        [SerializeField] Vector3Int movement;

        private const int X_Z_HASH= 691;
        private const int Y_HASH = 2011;
        private const float HALF_PIE = 1.57075f;
        private int checkHash; //multiplies the movement values by prime numbers to determine if the tested movement is a variation of the pattern
        
        private void OnEnable() {
            checkHash = movement.x * X_Z_HASH + movement.y * Y_HASH + movement.z * X_Z_HASH;
        }

        public Vector3Int GetMovement() =>movement;

        public int GetActionPointCost() => (int)(Mathf.Sqrt(movement.x * movement.x + movement.z * movement.z) + 2 * movement.y);

        public bool ValidMovement(Tile currentTile, Tile toTile) {
            int distX = Mathf.Abs(toTile.GetGridPos().x - currentTile.GetGridPos().x);
            int distY = Mathf.Abs(toTile.GetGridPos().y - currentTile.GetGridPos().y);
            int distZ = Mathf.Abs(toTile.GetGridPos().z - currentTile.GetGridPos().z);
            int testHash = distX * X_Z_HASH + distY * Y_HASH + distZ * X_Z_HASH;
            return testHash == checkHash;
        }

        //generates the possible offsets off of the point given based on this movement pattern
        public IEnumerable<Vector3Int> GetValidOffsets(Vector3Int unitPos, Vector3Int matrixSize) {
            List<Vector3Int> output = new List<Vector3Int>();
            //loops through y values. if there is movement in the y it is 2 iterations otherwise it is only one
            for(int y = 1+NormailizeInt(movement.y); y > 0; y--) {
                for(int i = 0; i < 4; i++) {
                    //x part of of coordsX gives the offset off of the origin and the x part of corrdsZ gives the offset off the corrdsX
                    //y part of of coordsZ gives the offset off of the origin and the y part of corrdsZ gives the offset off the corrdsX
                    Vector2Int layerCoordsX = new Vector2Int((int)(Mathf.Sin(i * HALF_PIE) * movement.x),(int)( Mathf.Cos(i * HALF_PIE) * movement.x));
                    Vector2Int layerCoordsZ = new Vector2Int((int)(Mathf.Cos(i * HALF_PIE) * movement.z),(int)( Mathf.Sin(i * HALF_PIE) * movement.z));
                    //y part gets a positve version of movemnt.y and a negative version of movemnt.y if movement.y does not equal 0. it uses the Mathf.pow to change the sign to get the different cases
                    int yCoord = (int)Mathf.Pow(-1, y) * movement.y;

                    Vector3Int offset1 = new Vector3Int(layerCoordsX.x + layerCoordsZ.x + unitPos.x, yCoord + unitPos.y, layerCoordsX.y + layerCoordsZ.y + unitPos.z);
                    //calculates the possible offset for offset 2 so you dont have to calculate it twice
                    var offset2Change = new Vector2Int(layerCoordsX.x - layerCoordsZ.x, layerCoordsX.y - layerCoordsZ.y);
                    Vector3Int offset2 = new Vector3Int(offset2Change.x + unitPos.x, yCoord + unitPos.y, offset2Change.y + unitPos.z);

                    if (CheckBounds(offset1, matrixSize)) {
                        output.Add(offset1);
                    }
                    //if there is movement in both the x and z axis it will add an different offset
                    if (offset2Change.x * offset2Change.y != 0 && CheckBounds(offset2, matrixSize)) {
                        output.Add(offset2);
                    }
                }
            }
            return output;
        }

        private bool CheckBounds(Vector3Int testPos, Vector3Int matrixSize) {
            return testPos.x >= 0 && testPos.y >= 0 && testPos.z >= 0 && testPos.x < matrixSize.x && testPos.y < matrixSize.y &&testPos.z < matrixSize.z;
        }

        //if 0 return zero otherwise returns one
        private int NormailizeInt(int i) {
            return i > 0 ? i / i : 0;
        }
    }
}