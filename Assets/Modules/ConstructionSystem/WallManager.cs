using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.ConstructionSystem.Walls;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class WallManager : MonoBehaviour
    {
        [SerializeField] private Grid wallGrid;
        [SerializeField] private RoomManager roomManager;
        [SerializeField] private GridManager gridManager;

        [SerializeField] private GameObject tester;
        [SerializeField] private GameObject farLeftPrefab;
        [SerializeField] private GameObject farLeftRightPrefab;
        [SerializeField] private GameObject farNearPrefab;
        [SerializeField] private GameObject farNearLeftPrefab;
        [SerializeField] private GameObject farNearRightPrefab;
        [SerializeField] private GameObject farRightPrefab;
        [SerializeField] private GameObject leftRightPrefab;
        [SerializeField] private GameObject nearLeftPrefab;
        [SerializeField] private GameObject nearLeftRightPrefab;
        [SerializeField] private GameObject nearRightPrefab;
        
        private readonly Dictionary<WallCode, GameObject> _wallPrefabs = new();
        private readonly List<GameObject> _walls = new();

        private void Start()
        {
            roomManager.onRoomsChanged.AddListener(UpdateWalls);
            
            _wallPrefabs[WallCode.NearLeft] = nearLeftPrefab;
            _wallPrefabs[WallCode.FarRight | WallCode.NearRight | WallCode.FarLeft] = nearLeftPrefab;
            
            _wallPrefabs[WallCode.NearRight] = nearRightPrefab;
            _wallPrefabs[WallCode.NearLeft | WallCode.FarLeft | WallCode.FarRight] = nearRightPrefab;
            
            _wallPrefabs[WallCode.FarLeft] = farLeftPrefab;
            _wallPrefabs[WallCode.NearLeft | WallCode.NearRight | WallCode.FarRight] = farLeftPrefab;
            
            _wallPrefabs[WallCode.FarRight] = farRightPrefab;
            _wallPrefabs[WallCode.NearLeft | WallCode.NearRight | WallCode.FarLeft] = farRightPrefab;
            
            _wallPrefabs[WallCode.NearLeft | WallCode.NearRight] = leftRightPrefab;
            _wallPrefabs[WallCode.FarLeft | WallCode.FarRight] = leftRightPrefab;
            
            _wallPrefabs[WallCode.NearLeft | WallCode.FarLeft] = farNearPrefab;
            _wallPrefabs[WallCode.NearRight | WallCode.FarRight] = farNearPrefab;
        }

        private void UpdateWalls(List<Room> rooms)
        {
            ClearWalls();
            
            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minZ = int.MaxValue;
            var maxZ = int.MinValue;
            
            foreach (var room in rooms)
            {
                foreach (var (gridTile, roomTile) in room.Tiles)
                {
                    var coords = gridTile.Coordinates;

                    if (coords.X < minX) minX = coords.X;
                    if (coords.X > maxX) maxX = coords.X;
                    if (coords.Z < minZ) minZ = coords.Z;
                    if (coords.Z > maxZ) maxZ = coords.Z;
                }
            }

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
                            wallCode |= WallCode.NearLeft;
                        }
                    }
                    
                    var nearRightCoord = new GridCoordinate(x + 1, z);
                    if (gridManager.Tiles.ContainsKey(nearRightCoord))
                    {
                        if (gridManager.Tiles[nearRightCoord].Occupant is RoomTile)
                        {
                            wallCode |= WallCode.NearRight;
                        }
                    }
                    var farLeftCoord = new GridCoordinate(x, z + 1);
                    if (gridManager.Tiles.ContainsKey(farLeftCoord))
                    {
                        if (gridManager.Tiles[farLeftCoord].Occupant is RoomTile)
                        {
                            wallCode |= WallCode.FarLeft;
                        }
                    }
                    
                    var farRightCoord = new GridCoordinate(x + 1, z + 1);
                    if (gridManager.Tiles.ContainsKey(farRightCoord))
                    {
                        if (gridManager.Tiles[farRightCoord].Occupant is RoomTile)
                        {
                            wallCode |= WallCode.FarRight;
                        }
                    }
                    
                    SpawnSingleWall(wallCode, x, z);
                }
            }
        }

        private void SpawnSingleWall(WallCode code, int x, int z)
        {
            var worldPos = wallGrid.CellToWorld(new Vector3Int(x, 0, z));

            if (_wallPrefabs.TryGetValue(code, out var prefab))
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
