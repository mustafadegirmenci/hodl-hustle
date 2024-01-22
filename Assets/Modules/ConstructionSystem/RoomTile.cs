using SunkCost.HH.Modules.GridSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class RoomTile : MonoBehaviour, IGridOccupant
    {
        [HideInInspector] public UnityEvent<RoomTileState> onStateChanged = new();

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
