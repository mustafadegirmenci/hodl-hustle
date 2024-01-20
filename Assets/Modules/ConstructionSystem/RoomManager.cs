using System.Collections.Generic;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private Room roomPrefab;
        [SerializeField] private Transform roomsContainer;
        
        [SerializeField] private ConstructionManager constructionManager;

        [SerializeField] private Button startRoomCreationButton;
        [SerializeField] private Button finishRoomCreationButton;

        private readonly List<Room> _rooms = new();

        private void Start()
        {
            startRoomCreationButton.onClick.AddListener(() =>
            {
                constructionManager.StartConstruction();
                constructionManager.onConstructionEnded.AddListener(AddRoom);
            });
            
            finishRoomCreationButton.onClick.AddListener(() =>
            {
                constructionManager.EndConstruction();
            });
        }

        public void AddRoom(List<GridTile> tiles)
        {
            var newRoom = Instantiate(
                original: roomPrefab,
                parent: roomsContainer
            );
            newRoom.SetTiles(tiles);
            _rooms.Add(newRoom);
            Debug.Log($"Added a new room with size: {tiles.Count}");
        }
    }
}