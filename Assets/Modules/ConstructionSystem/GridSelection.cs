using System.Collections.Generic;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class GridRect
    {
        public bool AllTilesConnected { get; private set; } = false;
        
        private readonly Dictionary<Vector3Int, GridTile> _elements = new();
        
        public void Add(GridTile tile)
        {
            var coords = tile.Coordinates;

            if (_elements.Count == 0)
            {
                AllTilesConnected = true;
                _elements.Add(coords, tile);
                return;
            }
            
            if (_elements.ContainsKey(coords))
            {
                return;
            }

            AllTilesConnected = 
                _elements.ContainsKey(coords + Vector3Int.left) ||
                _elements.ContainsKey(coords + Vector3Int.right) ||
                _elements.ContainsKey(coords + Vector3Int.back) ||
                _elements.ContainsKey(coords + Vector3Int.forward);
        }

        public void Remove(Vector3Int coords)
        {
            
        }
    }
}
