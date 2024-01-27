using System;
using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.ConstructionSystem;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.RoomSystem
{
    public class Room : MonoBehaviour
    {
        public RoomWallHandler roomWallHandler;
        
        public readonly Dictionary<Vector3Int, (GridTile, RoomTile)> Tiles = new();
        
        [HideInInspector] public UnityEvent<List<GridTile>> onRoomChanged = new();
        
        [SerializeField] private Button startEditingRoomButton;
        [SerializeField] private Canvas roomCanvas;
        [SerializeField] private RoomTile roomTilePrefab;
        [SerializeField] private Transform roomTilesContainer;

        private void Start()
        {
            startEditingRoomButton.onClick.AddListener(StartEditing);
        }

        public void StartEditing()
        {
            if (!ConstructionManager.instance.StartConstruction(Tiles.Values.Select(kvp => kvp.Item1).ToList()))
            {
                return;
            }

            startEditingRoomButton.gameObject.SetActive(false);
            
            ConstructionManager.instance.onConstructionEnded.AddListener(UpdateTiles);
            ConstructionManager.instance.onConstructionEnded.AddListener(_ => FinishEditing());

            foreach (var (key, (gridTile, roomTile)) in Tiles)
            {
                roomTile.SetState(RoomTileState.UnderConstruction);
                gridTile.Occupant = null;
            }
        }

        private void FinishEditing()
        {
            ConstructionManager.instance.onConstructionEnded.RemoveListener(UpdateTiles);
            ConstructionManager.instance.onConstructionEnded.RemoveListener(_ => FinishEditing());
            
            startEditingRoomButton.gameObject.SetActive(true);
            
            ConstructionManager.instance.onConstructionEnded.RemoveListener(UpdateTiles);
            
            foreach (var (key, (gridTile, roomTile)) in Tiles)
            {
                roomTile.SetState(RoomTileState.Normal);
                gridTile.Occupant = roomTile;
            }
            
            onRoomChanged.Invoke(Tiles.Values.Select(kvp => kvp.Item1).ToList());
        }

        private void UpdateTiles(List<GridTile> gridTiles)
        {
            var removedTiles = Tiles.Values.Select(kvp => kvp.Item1).Except(gridTiles).ToList();
            var addedTiles = gridTiles.Except(Tiles.Values.Select(kvp => kvp.Item1)).ToList();
    
            foreach (var gridTile in removedTiles)
            {
                Destroy(Tiles[gridTile.Coordinates].Item2.gameObject);
                gridTile.Occupant = null;
                Tiles.Remove(gridTile.Coordinates);
            }
    
            foreach (var gridTile in addedTiles)
            {
                var newRoomTile = Instantiate(
                    original: roomTilePrefab,
                    position: new Vector3(gridTile.WorldPosition.x, 0,gridTile.WorldPosition.z),
                    rotation: Quaternion.identity,
                    parent: roomTilesContainer
                );

                newRoomTile.room = this;
                Tiles.Add(gridTile.Coordinates, (gridTile, newRoomTile));
                gridTile.Occupant = newRoomTile;
            }

            var canvasRect = roomCanvas.GetComponent<RectTransform>();

            if (Tiles.Count == 0)
            {
                canvasRect.gameObject.SetActive(false);
            }
            else
            {
                canvasRect.gameObject.SetActive(true);
                
                var averagePosition = Vector3.zero;
                foreach (var tile in Tiles.Values.Select(kvp => kvp.Item1))
                {
                    averagePosition += tile.WorldPosition;
                }
                averagePosition /= Tiles.Count;
                averagePosition.y += 3.0f;

                roomCanvas.GetComponent<RectTransform>().position = averagePosition;
            }
        }

        private void OnDestroy()
        {
            roomWallHandler.ClearWalls();
        }
    }
}
