using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.ConstructionSystem;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.RoomSystem
{
    public class Room : MonoBehaviour
    {
        public RoomWallHandler roomWallHandler;
        
        public readonly Dictionary<Vector3Int, (GridTile, RoomTile)> Tiles = new();
        
        [HideInInspector] public UnityEvent<List<GridTile>> onRoomChanged = new();
        [HideInInspector] public UnityEvent onEditStarted = new();
        [HideInInspector] public UnityEvent onEditFinished = new();
        
        [SerializeField] private RoomTile roomTilePrefab;
        [SerializeField] private Transform roomTilesContainer;
        
        private void OnDrawGizmos()
        {
            foreach (var (key, (gridTile, roomTile)) in Tiles)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(gridTile.WorldPosition, Vector3.one * 0.8f);

                if (roomTile.room == this)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(gridTile.WorldPosition, gridTile.WorldPosition + Vector3.up);
                }
            }
        }
        
        public void StartEditing()
        {
            if (!ConstructionManager.instance.StartConstruction(Tiles.Values.Select(kvp => kvp.Item1).ToList()))
            {
                return;
            }
            
            ConstructionManager.instance.onConstructionEnded.AddListener(UpdateTiles);
            ConstructionManager.instance.onConstructionEnded.AddListener(_ => FinishEditing());

            foreach (var (key, (gridTile, roomTile)) in Tiles)
            {
                roomTile.SetState(RoomTileState.UnderConstruction);
                gridTile.Occupant = null;
            }
            onEditStarted.Invoke();
        }

        private void FinishEditing()
        {
            ConstructionManager.instance.onConstructionEnded.RemoveListener(UpdateTiles);
            ConstructionManager.instance.onConstructionEnded.RemoveListener(_ => FinishEditing());
            
            ConstructionManager.instance.onConstructionEnded.RemoveListener(UpdateTiles);
            
            foreach (var (key, (gridTile, roomTile)) in Tiles)
            {
                roomTile.SetState(RoomTileState.Normal);
                gridTile.Occupant = roomTile;
            }
            
            onRoomChanged.Invoke(Tiles.Values.Select(kvp => kvp.Item1).ToList());
            onEditFinished.Invoke();
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
        }

        private void OnDestroy()
        {
            roomWallHandler.ClearWalls();
        }
    }
}
