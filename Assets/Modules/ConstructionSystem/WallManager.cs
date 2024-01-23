using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class WallManager : MonoBehaviour
    {
        [SerializeField] private Grid wallGrid;
        [SerializeField] private RoomManager roomManager;
        [SerializeField] private GridManager gridManager;
        
        private readonly List<GameObject> _walls = new();

        private void Start()
        {
            roomManager.onRoomsChanged.AddListener(UpdateWalls);
        }

        private void UpdateWalls(List<Room> rooms)
        {
            ClearWalls();
            
            foreach (var room in rooms)
            {
                foreach (var (gridTile, roomTile) in room.Tiles)
                {
            
                    var minX = int.MaxValue;
                    var maxX = int.MinValue;
                    var minZ = int.MaxValue;
                    var maxZ = int.MinValue;
                    
                    var coords = gridTile.Coordinates;

                    if (coords.X < minX) minX = coords.X;
                    if (coords.X > maxX) maxX = coords.X;
                    if (coords.Z < minZ) minZ = coords.Z;
                    if (coords.Z > maxZ) maxZ = coords.Z;
                    
                    for (var x = minX - 1; x <= maxX; x++)
                    {
                        for (var z = minZ - 1; z <= maxZ; z++)
                        {
                            var wallCode = WallCode.None;
                    
                            var nearLeftCoord = new GridCoordinate(x, z);
                            if (gridManager.Tiles.ContainsKey(nearLeftCoord))
                            {
                                if (gridManager.Tiles[nearLeftCoord].Occupant is RoomTile occupant)
                                {
                                    if (occupant.Room == room)
                                    {
                                        wallCode |= WallCode.NearLeft;
                                    }
                                }
                            }
                    
                            var nearRightCoord = new GridCoordinate(x + 1, z);
                            if (gridManager.Tiles.ContainsKey(nearRightCoord))
                            {
                                if (gridManager.Tiles[nearRightCoord].Occupant is RoomTile occupant)
                                {
                                    if (occupant.Room == room)
                                    {
                                        wallCode |= WallCode.NearRight;
                                    }
                                }
                            }
                            var farLeftCoord = new GridCoordinate(x, z + 1);
                            if (gridManager.Tiles.ContainsKey(farLeftCoord))
                            {
                                if (gridManager.Tiles[farLeftCoord].Occupant is RoomTile occupant)
                                {
                                    if (occupant.Room == room)
                                    {
                                        wallCode |= WallCode.FarLeft;
                                    }
                                }
                            }
                    
                            var farRightCoord = new GridCoordinate(x + 1, z + 1);
                            if (gridManager.Tiles.ContainsKey(farRightCoord))
                            {
                                if (gridManager.Tiles[farRightCoord].Occupant is RoomTile occupant)
                                {
                                    if (occupant.Room == room)
                                    {
                                        wallCode |= WallCode.FarRight;
                                    }
                                }
                            }
                    
                            SpawnSingleWall(room, wallCode, x, z);
                        }
                    }
                }
            }
        }

        private void SpawnSingleWall(Room room, WallCode code, int x, int z)
        {
            var worldPos = wallGrid.CellToWorld(new Vector3Int(x, 0, z));
            
            if (room.WallPrefabs.TryGetValue(code, out var prefab))
            {
                _walls.Add(Instantiate(prefab, worldPos, Quaternion.identity));
            }
        }

        private void ClearWalls()
        {
            foreach (var wall in _walls.ToList())
            {
                Destroy(wall);
                _walls.Remove(wall);
            }
        }
    }
}
