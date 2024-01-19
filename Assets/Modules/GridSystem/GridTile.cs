using JetBrains.Annotations;
using UnityEngine;

namespace SunkCost.HH.Modules.GridSystem
{
    public class GridTile : MonoBehaviour
    {
        public GridCoordinate Coordinates;
        
        [CanBeNull] public IGridOccupant Occupant;
    }
}
