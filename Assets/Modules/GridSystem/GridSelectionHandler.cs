using System.Collections.Generic;
using UnityEngine;

namespace SunkCost.HH.Modules.GridSystem
{
    public class GridSelectionHandler : MonoBehaviour
    {
        [SerializeField] private GridTileHandler gridTileHandler;
        
        public List<GridTile> GetTilesInSelectionBox(Vector3Int startCorner, Vector3Int endCorner)
        {
            var tiles = new List<GridTile>();
            
            var minX = Mathf.Min(startCorner.x, endCorner.x);
            var maxX = Mathf.Max(startCorner.x, endCorner.x);
            var minZ = Mathf.Min(startCorner.z, endCorner.z);
            var maxZ = Mathf.Max(startCorner.z, endCorner.z);

            for (var x = minX; x <= maxX; x++)
            {
                for (var z = minZ; z <= maxZ; z++)
                {
                    var coord = new Vector3Int(x, 0, z);
                    if (gridTileHandler.Tiles.TryGetValue(coord, out var tile))
                    {
                        tiles.Add(tile);
                    }
                }
            }

            return tiles;
        }
    }
}