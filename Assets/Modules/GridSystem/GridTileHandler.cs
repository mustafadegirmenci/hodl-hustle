using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.GridSystem
{
    public class GridTileHandler : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<GridTile> onTileAdded = new();
        
        public readonly Dictionary<Vector3Int, GridTile> Tiles = new();
        
        [SerializeField] private Grid grid;

        public bool TryAddTile(Vector3Int coords)
        {
            if (Tiles.ContainsKey(coords))
            {
                return false;
            }

            var newGridTile = new GridTile(
                coordinates: coords,
                worldPosition: grid.CellToWorld(coords),
                occupant: null,
                tileSize: grid.cellSize.x
            );
            
            Tiles.Add(coords, newGridTile);
            onTileAdded.Invoke(newGridTile);
            return true;
        }

        public bool TryWorldToTile(Vector3 worldPos, out GridTile tile)
        {
            return Tiles.TryGetValue(WorldToCellCoords(worldPos), out tile);
        }

        public Vector3Int WorldToCellCoords(Vector3 worldPos)
        {
            return grid.WorldToCell(worldPos);
        }
        
        public Vector3 CellCoordsToWorldPosition(Vector3Int cellCoords)
        {
            return grid.CellToWorld(cellCoords);
        }
        
        public List<Vector3Int> GetCellCoordsInBounds(List<Bounds> bounds)
        {
            var cellCoords = new List<Vector3Int>();

            foreach (var bound in bounds)
            {
                var minCoords = WorldToCellCoords(bound.min);
                var maxCoords = WorldToCellCoords(bound.max);

                for (var x = minCoords.x + 1; x <= maxCoords.x; x++)
                {
                    for (var z = minCoords.z + 1; z <= maxCoords.z; z++)
                    {
                        cellCoords.Add(new Vector3Int(x, 0, z));
                    }
                }
            }

            return cellCoords;
        }
    }
}
