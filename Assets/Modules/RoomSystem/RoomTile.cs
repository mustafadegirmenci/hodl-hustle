using SunkCost.HH.Modules.GridSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.RoomSystem
{
    public class RoomTile : MonoBehaviour, IGridOccupant
    {
        public Room room;
        
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
