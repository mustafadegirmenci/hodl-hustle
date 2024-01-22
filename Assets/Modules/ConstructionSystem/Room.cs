using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private RoomTile roomTilePrefab;
        [SerializeField] private Transform roomTilesContainer;
        [SerializeField] private Button startEditingRoomButton;
        [SerializeField] private Canvas roomCanvas;

        private readonly Dictionary<GridTile, RoomTile> _roomTiles = new();

        private void Start()
        {
            startEditingRoomButton.onClick.AddListener(StartEditing);
        }

        public void StartEditing()
        {
            if (!ConstructionManager.instance.StartConstruction(_roomTiles.Keys.ToList()))
            {
                return;
            }
            ConstructionManager.instance.finishConstructionButton.onClick.AddListener(FinishEditing);
            Debug.Log($"Started editing room {name}.");

            startEditingRoomButton.gameObject.SetActive(false);
            
            ConstructionManager.instance.onConstructionEnded.AddListener(UpdateTiles);
            
            foreach (var (gridTile, roomTile) in _roomTiles)
            {
                roomTile.SetState(RoomTileState.UnderConstruction);
                gridTile.Occupant = null;
            }
        }

        private void FinishEditing()
        {
            if (!ConstructionManager.instance.EndConstruction())
            {
                return;
            }
            ConstructionManager.instance.finishConstructionButton.onClick.RemoveListener(FinishEditing);
            Debug.Log($"Finished editing room {name}.");
            
            startEditingRoomButton.gameObject.SetActive(true);
            
            ConstructionManager.instance.onConstructionEnded.RemoveListener(UpdateTiles);
            
            foreach (var (gridTile, roomTile) in _roomTiles)
            {
                roomTile.SetState(RoomTileState.Normal);
                gridTile.Occupant = roomTile;
            }
        }

        private void UpdateTiles(List<GridTile> gridTiles)
        {
            Debug.Log($"New tile count for {name} is {gridTiles.Count}.");
            var removedTiles = _roomTiles.Keys.Except(gridTiles).ToList();
            var addedTiles = gridTiles.Except(_roomTiles.Keys).ToList();
    
            foreach (var gridTile in removedTiles)
            {
                Destroy(_roomTiles[gridTile]);
                gridTile.Occupant = null;
                _roomTiles.Remove(gridTile);
            }
    
            foreach (var gridTile in addedTiles)
            {
                var newRoomTile = Instantiate(
                    original: roomTilePrefab,
                    position: gridTile.transform.position,
                    rotation: Quaternion.identity,
                    parent: roomTilesContainer
                );
        
                _roomTiles.Add(gridTile, newRoomTile);
                gridTile.Occupant = newRoomTile;
            }
            Debug.Log($"New tile count for {name} is {_roomTiles.Count}.");

            var canvasRect = roomCanvas.GetComponent<RectTransform>();

            if (_roomTiles.Count == 0)
            {
                Debug.Log("_roomTiles.Count == 0");
                canvasRect.gameObject.SetActive(false);
            }
            else
            {
                canvasRect.gameObject.SetActive(true);
                
                var averagePosition = Vector3.zero;
                foreach (var tile in _roomTiles.Keys)
                {
                    averagePosition += tile.transform.position;
                }
                averagePosition /= _roomTiles.Count;
                averagePosition.y += 3.0f;

                roomCanvas.GetComponent<RectTransform>().position = averagePosition;
            }
        }

    }
}
