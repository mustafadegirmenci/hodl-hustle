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
                occupant: null
            );
            
            Tiles.Add(coords, newGridTile);
            onTileAdded.Invoke(newGridTile);
            return true;
        }
    }
}
