using System.Collections.Generic;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.RoomSystem
{
    public class RoomManager : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<List<Room>> onRoomsChanged = new();
        
        [SerializeField] private GridTileHandler gridTileHandler;
        [SerializeField] private Room roomPrefab;
        [SerializeField] private Transform roomsContainer;

        [SerializeField] private Button startRoomCreationButton;

        public readonly List<Room> Rooms = new();
        private static int _roomCount = 0;

        private void Start()
        {
            startRoomCreationButton.onClick.AddListener(CreateRoom);
        }

        public bool TryWorldPointToRoom(Vector3 point, out Room room)
        {
            if (!gridTileHandler.TryWorldToTile(point, out var gridTile))
            {
                room = null;
                return false;
            }

            foreach (var r in Rooms)
            {
                if (r.Tiles.TryGetValue(gridTile.Coordinates, out var tuple))
                {
                    room = tuple.Item2.room;
                    return true;
                }
            }

            room = null;
            return false;
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
