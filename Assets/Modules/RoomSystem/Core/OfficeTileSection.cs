using UnityEngine;

namespace SunkCost.HH.Modules.RoomSystem.Core
{
    public class OfficeTileSection : MonoBehaviour
    {
        private Decoration _occupier;

        public bool TryOccupy(Decoration occupier)
        {
            if (_occupier != null)
            {
                return false;
            }

            _occupier = occupier;
            return true;
        }

        public bool IsOccupied()
        {
            return _occupier != null;
        }
        
        public void Clear()
        {
            _occupier = null;
        }
    }
}
