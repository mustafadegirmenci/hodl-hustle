using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private RoomTile roomTilePrefab;
        [SerializeField] private Transform roomTilesContainer;

        private readonly Dictionary<GridTile, RoomTile> _roomTiles = new();

        public void SetTiles(List<GridTile> tiles)
        {
            UpdateTiles(tiles);
        }

        private void UpdateTiles(List<GridTile> gridTiles)
        {
            var removedTiles = _roomTiles.Keys.Except(gridTiles);
            
            foreach (var gridTile in removedTiles)
            {
                Destroy(_roomTiles[gridTile]);
                gridTile.Occupant = null;
            }
            
            var addedTiles = gridTiles.Except(_roomTiles.Keys);
            
            foreach (var gridTile in addedTiles)
            {
                var newRoomTile = Instantiate(
                    original: roomTilePrefab,
                    position: gridTile.transform.position,
                    rotation: Quaternion.identity,
                    parent: roomTilesContainer
                );
                
                _roomTiles.Add(gridTile, newRoomTile);
                gridTile.Occupant = newRoomTile;
            }
        }
    }
}
