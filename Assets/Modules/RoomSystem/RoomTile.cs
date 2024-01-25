using SunkCost.HH.Modules.DecorationSystem;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.RoomSystem
{
    public class RoomTile : MonoBehaviour, IGridOccupant
    {
        public Room room;
        public DecorationItem farLeftDecorationItem;
        public DecorationItem farRightDecorationItem;
        public DecorationItem nearLeftDecorationItem;
        public DecorationItem nearRightDecorationItem;
        
        private RoomTileState _currentState;

        public void SetState(RoomTileState newState)
        {
            if (newState == RoomTileState.Normal)
            {
                gameObject.SetActive(true);
            }
            else if (newState == RoomTileState.UnderConstruction)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
