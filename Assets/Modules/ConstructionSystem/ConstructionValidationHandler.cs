using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class ConstructionValidationHandler : MonoBehaviour
    {
        public bool ValidateSelection(List<GridTile> tilesToValidate)
        {
            foreach (var tile in tilesToValidate)
            {
                if (tile.Occupant != null)
                {
                    return false;
                }
                
                if (!AllTilesConnected())
                {
                    return false;
                }
            }
            
            return true;
            
            bool AllTilesConnected()
            {
                var tilePositionSet = new HashSet<Vector3Int>(tilesToValidate.Select(t => t.Coordinates));
                var visited = new HashSet<Vector3Int> { tilesToValidate[0].Coordinates };
                var stack = new Stack<Vector3Int>();
                
                stack.Push(tilesToValidate[0].Coordinates);
                
                while (stack.Count > 0)
                {
                    var currentTile = stack.Pop();

                    if (visited.Add(currentTile))
                    {
                        continue;
                    }
                    
                    foreach (var neighbor in GetNeighbors(currentTile))
                    {
                        if (tilePositionSet.Contains(neighbor) && visited.Add(neighbor))
                        {
                            stack.Push(neighbor);
                        }
                    }
                }
                
                return visited.Count == tilePositionSet.Count;
                
                IEnumerable<Vector3Int> GetNeighbors(Vector3Int coords)
                {
                    yield return coords + Vector3Int.left;
                    yield return coords + Vector3Int.right;
                    yield return coords + Vector3Int.back;
                    yield return coords + Vector3Int.forward;
                }
            }
        }
    }
}
