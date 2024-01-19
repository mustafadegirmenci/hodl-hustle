using System.Collections.Generic;
using UnityEngine;

namespace SunkCost.HH.Modules.RoomSystem.Core
{
    public class Room : MonoBehaviour
    {
        private readonly List<OfficeTile> _tiles = new();

        public void AddTile(OfficeTile tile)
        {
            _tiles.Add(tile);
        }

        public void RemoveTile(OfficeTile tile)
        {
            _tiles.Remove(tile);
        }
    }
}
