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
        
        public List<Vector3Int> GetCellCoordsInBounds(Bounds bounds)
        {
            var cellCoords = new List<Vector3Int>();

            var cellSize = grid.cellSize;
            var minCoords = WorldToCellCoords(bounds.min + new Vector3(cellSize.x, 0, cellSize.z) / 2);
            var maxCoords = WorldToCellCoords(bounds.max - new Vector3(cellSize.x, 0, cellSize.z) / 2);

            for (var x = minCoords.x; x <= maxCoords.x; x++)
            {
                for (var z = minCoords.z; z <= maxCoords.z; z++)
                {
                    cellCoords.Add(new Vector3Int(x, 0, z));
                }
            }

            return cellCoords;
        }
    }
}
