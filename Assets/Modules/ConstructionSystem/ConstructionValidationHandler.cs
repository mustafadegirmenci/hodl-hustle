using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class ConstructionValidationHandler : MonoBehaviour
    {
        [SerializeField] private ConstructionSelectionHandler constructionSelectionHandler;
        [SerializeField] private ConstructionStateHandler constructionStateHandler;
        
        public bool IsSelectionValid { get; private set; }

        private void Start()
        {
            constructionSelectionHandler.onVolatileSelectionChanged.AddListener(ValidateNewSelection);
        }

        private void ValidateNewSelection(List<GridTile> volatileSelection)
        {
            if (constructionStateHandler.CurrentState is ConstructionState.SelectingTilesToBeAdded)
            {
                var tilesToValidate = volatileSelection
                    .Concat(constructionSelectionHandler.PersistentSelection)
                    .ToList();
                ValidateRoomIntegrity(tilesToValidate);
            }
            else if (constructionStateHandler.CurrentState is ConstructionState.SelectingTilesToBeRemoved)
            {
                var tilesToValidate = constructionSelectionHandler.PersistentSelection
                    .Except(volatileSelection)
                    .ToList();
                ValidateRoomIntegrity(tilesToValidate);
            }
        }

        private void ValidateRoomIntegrity(List<GridTile> tilesToValidate)
        {
            foreach (var tile in tilesToValidate)
            {
                if (tile.Occupant != null)
                {
                    IsSelectionValid = false;
                    return;
                }
                
                if (!AllTilesConnected())
                {
                    IsSelectionValid = false;
                    return;
                }
            }

            IsSelectionValid = true;

            bool AllTilesConnected()
            {
                var tilePositionSet = new HashSet<Vector3Int>(tilesToValidate.Select(t => t.Coordinates));

                var visited = new HashSet<Vector3Int>();
                var stack = new Stack<Vector3Int>();

                stack.Push(tilesToValidate[0].Coordinates);
                visited.Add(tilesToValidate[0].Coordinates);

                while (stack.Count > 0)
                {
                    var currentTile = stack.Pop();

                    foreach (var neighbor in GetNeighbors(currentTile))
                    {
                        if (tilePositionSet.Contains(neighbor) && !visited.Contains(neighbor))
                        {
                            stack.Push(neighbor);
                            visited.Add(neighbor);
                        }
                    }
                }

                return visited.Count == tilePositionSet.Count;

                IEnumerable<Vector3Int> GetNeighbors(Vector3Int tile)
                {
                    int[] dx = { -1, 1, 0, 0 };
                    int[] dz = { 0, 0, -1, 1 };

                    for (var i = 0; i < dx.Length; i++)
                    {
                        var nx = tile.x + dx[i];
                        var nz = tile.z + dz[i];

                        yield return new Vector3Int(nx, 0, nz);
                    }
                }
            }
        }
    }
}
