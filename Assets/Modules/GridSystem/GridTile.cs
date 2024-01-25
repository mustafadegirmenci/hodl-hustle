using JetBrains.Annotations;
using UnityEngine;

namespace SunkCost.HH.Modules.GridSystem
{
    public class GridTile
    {
        public Vector3Int Coordinates;
        public Vector3 WorldPosition;
        [CanBeNull] public IGridOccupant Occupant;

        public GridTile(Vector3Int coordinates, Vector3 worldPosition, [CanBeNull] IGridOccupant occupant)
        {
            Coordinates = coordinates;
            WorldPosition = worldPosition;
            Occupant = occupant;
        }
    }
}
