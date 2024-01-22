using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class RoomManager : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<List<Room>> onRoomsChanged = new();
        
        [SerializeField] private Room roomPrefab;
        [SerializeField] private Transform roomsContainer;

        [SerializeField] private Button startRoomCreationButton;

        public readonly List<Room> Rooms = new();
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
            Rooms.Add(newRoom);
            
            newRoom.StartEditing();
            newRoom.gameObject.name = (_roomCount++).ToString();
            newRoom.onRoomChanged.AddListener(tiles =>
            {
                if (tiles.Count == 0)
                {
                    Rooms.Remove(newRoom);
                    Destroy(newRoom.gameObject);
                }
                onRoomsChanged.Invoke(Rooms);
            });
        }
    }
}
