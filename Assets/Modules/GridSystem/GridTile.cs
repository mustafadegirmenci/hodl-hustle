using JetBrains.Annotations;
using UnityEngine;

namespace SunkCost.HH.Modules.GridSystem
{
    public class GridTile
    {
        public Vector3Int Coordinates;
        public Vector3 WorldPosition;
        public float TileSize;
        
        [CanBeNull] public IGridOccupant Occupant;

        public GridTile(Vector3Int coordinates, Vector3 worldPosition, [CanBeNull] IGridOccupant occupant, float tileSize)
        {
            Coordinates = coordinates;
            WorldPosition = worldPosition;
            Occupant = occupant;
            TileSize = tileSize;
        }

        public bool IsWorldPositionInside(Vector3 positionToCheck)
        {
            Debug.Log($"{positionToCheck}, {WorldPosition}");
            
            var halfTileSize = TileSize / 2f;

            var minX = WorldPosition.x - halfTileSize;
            var maxX = WorldPosition.x + halfTileSize;
            var minZ = WorldPosition.z - halfTileSize;
            var maxZ = WorldPosition.z + halfTileSize;

            return (positionToCheck.x >= minX && positionToCheck.x <= maxX &&
                    positionToCheck.y >= minZ && positionToCheck.y <= maxZ);
        }
    }
}
