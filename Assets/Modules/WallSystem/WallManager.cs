using System.Collections.Generic;
using SunkCost.HH.Modules.ConstructionSystem;
using SunkCost.HH.Modules.GridSystem;
using SunkCost.HH.Modules.RoomSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.WallSystem
{
    public class WallManager : MonoBehaviour
    {
        [SerializeField] private Grid wallGrid;
        [SerializeField] private RoomManager roomManager;
        [SerializeField] private GridTileHandler gridTileHandler;

        private void Start()
        {
            roomManager.onRoomsChanged.AddListener(UpdateWalls);
            ConstructionManager.instance.onConstructionStarted.AddListener(_ => ClearWalls());
        }

        private void ClearWalls()
        {
            foreach (var room in roomManager.Rooms)
            {
                room.roomWallHandler.ClearWalls();
            }
        }

        private void UpdateWalls(List<Room> rooms)
        {
            ClearWalls();
            
            foreach (var room in rooms)
            {
                foreach (var (key, (gridTile, roomTile)) in room.Tiles)
                {
                    var minX = int.MaxValue;
                    var maxX = int.MinValue;
                    var minZ = int.MaxValue;
                    var maxZ = int.MinValue;
                    
                    var coords = gridTile.Coordinates;

                    if (coords.x < minX) minX = coords.x;
                    if (coords.x > maxX) maxX = coords.x;
                    if (coords.z < minZ) minZ = coords.z;
                    if (coords.z > maxZ) maxZ = coords.z;
                    
                    for (var x = minX - 1; x <= maxX; x++)
                    {
                        for (var z = minZ - 1; z <= maxZ; z++)
                        {
                            var wallCode = WallCode.None;
                    
                            var nearLeftCoord = new Vector3Int(x, 0, z);
                            if (gridTileHandler.Tiles.ContainsKey(nearLeftCoord))
                            {
                                if (gridTileHandler.Tiles[nearLeftCoord].Occupant is RoomTile occupant)
                                {
                                    if (occupant.room == room)
                                    {
                                        wallCode |= WallCode.NearLeft;
                                    }
                                }
                            }
                    
                            var nearRightCoord = new Vector3Int(x + 1, 0, z);
                            if (gridTileHandler.Tiles.ContainsKey(nearRightCoord))
                            {
                                if (gridTileHandler.Tiles[nearRightCoord].Occupant is RoomTile occupant)
                                {
                                    if (occupant.room == room)
                                    {
                                        wallCode |= WallCode.NearRight;
                                    }
                                }
                            }
                            var farLeftCoord = new Vector3Int(x, 0, z + 1);
                            if (gridTileHandler.Tiles.ContainsKey(farLeftCoord))
                            {
                                if (gridTileHandler.Tiles[farLeftCoord].Occupant is RoomTile occupant)
                                {
                                    if (occupant.room == room)
                                    {
                                        wallCode |= WallCode.FarLeft;
                                    }
                                }
                            }
                    
                            var farRightCoord = new Vector3Int(x + 1, 0, z + 1);
                            if (gridTileHandler.Tiles.ContainsKey(farRightCoord))
                            {
                                if (gridTileHandler.Tiles[farRightCoord].Occupant is RoomTile occupant)
                                {
                                    if (occupant.room == room)
                                    {
                                        wallCode |= WallCode.FarRight;
                                    }
                                }
                            }
                    
                            var worldPos = wallGrid.CellToWorld(new Vector3Int(x, 0, z));
                            room.roomWallHandler.SpawnWall(wallCode, worldPos);
                        }
                    }
                }
            }
        }
    }
}
