using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private Room roomPrefab;
        [SerializeField] private Transform roomsContainer;

        [SerializeField] private Button startRoomCreationButton;

        private readonly List<Room> _rooms = new();
        private static int _roomCount = 0;

        private void Start()
        {
            startRoomCreationButton.onClick.AddListener(CreateRoom);
        }

        private void CreateRoom()
        {
            var newRoom = Instantiate(
                original: roomPrefab,
                parent: roomsContainer
            );
            newRoom.StartEditing();
            newRoom.gameObject.name = (_roomCount++).ToString();
            _rooms.Add(newRoom);
        }
    }
}
