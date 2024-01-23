using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class Room : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<List<GridTile>> onRoomChanged = new();
        
        [SerializeField] private RoomTile roomTilePrefab;
        [SerializeField] private Transform roomTilesContainer;
        [SerializeField] private Button startEditingRoomButton;
        [SerializeField] private Canvas roomCanvas;
        
        [SerializeField] private GameObject farLeftInnerPrefab;
        [SerializeField] private GameObject farLeftOuterPrefab;
        [SerializeField] private GameObject farNearLeftBiasedPrefab;
        [SerializeField] private GameObject farNearRightBiasedPrefab;
        [SerializeField] private GameObject farRightInnerPrefab;
        [SerializeField] private GameObject farRightOuterPrefab;
        [SerializeField] private GameObject leftRightFarBiasedPrefab;
        [SerializeField] private GameObject leftRightNearBiasedPrefab;
        [SerializeField] private GameObject nearLeftInnerPrefab;
        [SerializeField] private GameObject nearLeftOuterPrefab;
        [SerializeField] private GameObject nearRightInnerPrefab;
        [SerializeField] private GameObject nearRightOuterPrefab;
        
        public readonly Dictionary<WallCode, GameObject> WallPrefabs = new();

        public readonly Dictionary<GridTile, RoomTile> Tiles = new();

        private void Start()
        {
            startEditingRoomButton.onClick.AddListener(StartEditing);

            InitializeWallPrefabs();
        }

        public void StartEditing()
        {
            if (!ConstructionManager.instance.StartConstruction(Tiles.Keys.ToList()))
            {
                return;
            }
            ConstructionManager.instance.finishConstructionButton.onClick.AddListener(FinishEditing);
            Debug.Log($"Started editing room {name}.");

            startEditingRoomButton.gameObject.SetActive(false);
            
            ConstructionManager.instance.onConstructionEnded.AddListener(UpdateTiles);
            
            foreach (var (gridTile, roomTile) in Tiles)
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
            
            foreach (var (gridTile, roomTile) in Tiles)
            {
                roomTile.SetState(RoomTileState.Normal);
                gridTile.Occupant = roomTile;
            }
            
            onRoomChanged.Invoke(Tiles.Keys.ToList());
        }

        private void UpdateTiles(List<GridTile> gridTiles)
        {
            Debug.Log($"New tile count for {name} is {gridTiles.Count}.");
            var removedTiles = Tiles.Keys.Except(gridTiles).ToList();
            var addedTiles = gridTiles.Except(Tiles.Keys).ToList();
    
            foreach (var gridTile in removedTiles)
            {
                Destroy(Tiles[gridTile]);
                gridTile.Occupant = null;
                Tiles.Remove(gridTile);
            }
    
            foreach (var gridTile in addedTiles)
            {
                var newRoomTile = Instantiate(
                    original: roomTilePrefab,
                    position: new Vector3(gridTile.transform.position.x, 0,gridTile.transform.position.z),
                    rotation: Quaternion.identity,
                    parent: roomTilesContainer
                );

                newRoomTile.Room = this;
                Tiles.Add(gridTile, newRoomTile);
                gridTile.Occupant = newRoomTile;
            }
            Debug.Log($"New tile count for {name} is {Tiles.Count}.");

            var canvasRect = roomCanvas.GetComponent<RectTransform>();

            if (Tiles.Count == 0)
            {
                Debug.Log("_roomTiles.Count == 0");
                canvasRect.gameObject.SetActive(false);
            }
            else
            {
                canvasRect.gameObject.SetActive(true);
                
                var averagePosition = Vector3.zero;
                foreach (var tile in Tiles.Keys)
                {
                    averagePosition += tile.transform.position;
                }
                averagePosition /= Tiles.Count;
                averagePosition.y += 3.0f;

                roomCanvas.GetComponent<RectTransform>().position = averagePosition;
            }
        }

        private void InitializeWallPrefabs()
        {
            WallPrefabs[WallCode.NearLeft] = nearLeftInnerPrefab;
            WallPrefabs[WallCode.FarRight | WallCode.NearRight | WallCode.FarLeft] = nearLeftOuterPrefab;
            
            WallPrefabs[WallCode.NearRight] = nearRightInnerPrefab;
            WallPrefabs[WallCode.NearLeft | WallCode.FarLeft | WallCode.FarRight] = nearRightOuterPrefab;
            
            WallPrefabs[WallCode.FarLeft] = farLeftInnerPrefab;
            WallPrefabs[WallCode.NearLeft | WallCode.NearRight | WallCode.FarRight] = farLeftOuterPrefab;
            
            WallPrefabs[WallCode.FarRight] = farRightInnerPrefab;
            WallPrefabs[WallCode.NearLeft | WallCode.NearRight | WallCode.FarLeft] = farRightOuterPrefab;
            
            WallPrefabs[WallCode.NearLeft | WallCode.NearRight] = leftRightNearBiasedPrefab;
            WallPrefabs[WallCode.FarLeft | WallCode.FarRight] = leftRightFarBiasedPrefab;
            
            WallPrefabs[WallCode.NearLeft | WallCode.FarLeft] = farNearLeftBiasedPrefab;
            WallPrefabs[WallCode.NearRight | WallCode.FarRight] = farNearRightBiasedPrefab;
        }
    }
}
