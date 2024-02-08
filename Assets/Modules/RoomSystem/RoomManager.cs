using System.Collections.Generic;
using SunkCost.HH.Modules.ConstructionSystem;
using SunkCost.HH.Modules.GridSystem;
using SunkCost.HH.Modules.UiSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.RoomSystem
{
    public class RoomManager : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<List<Room>> onRoomsChanged = new();
        
        [SerializeField] private GridTileHandler gridTileHandler;
        [SerializeField] private Room roomPrefab;
        [SerializeField] private Transform roomsContainer;

        [SerializeField] private SC_Button startRoomCreationButton;

        public readonly List<Room> Rooms = new();
        private static int _roomCount;

        private void Start()
        {
            startRoomCreationButton.onClick.AddListener(CreateRoom);

            ConstructionManager.instance.onConstructionStarted.AddListener(
                _ => startRoomCreationButton.gameObject.SetActive(false)
            );
            ConstructionManager.instance.onConstructionEnded.AddListener(
                _ => startRoomCreationButton.gameObject.SetActive(true)
            );
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
                    _roomCount--;
                }
                onRoomsChanged.Invoke(Rooms);
            });
        }
    }
}
