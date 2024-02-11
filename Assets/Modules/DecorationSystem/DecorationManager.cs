using System.Linq;
using System.Collections.Generic;
using SunkCost.HH.Modules.GridSystem;
using SunkCost.HH.Modules.InputSystem;
using SunkCost.HH.Modules.RoomSystem;
using SunkCost.HH.Modules.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.DecorationSystem
{
    public class DecorationManager : MonoSingleton<DecorationManager>
    {
        [HideInInspector] public UnityEvent<DecorationItem> onCurrentDecorationItemChanged = new();
        public readonly Dictionary<Vector3Int, DecorationItem> PlacedDecorationItems = new();
        
        [SerializeField] private RoomManager roomManager;
        [SerializeField] private GridTileHandler decorationGridTileHandler;
        [SerializeField] private GridIndicationHandler decorationGridIndicationHandler;
        [SerializeField] private DecorationValidationHandler decorationValidationHandler;
        [SerializeField] private Transform decorationItemsContainer;

        private DecorationItem currentDecorationItem
        {
            get => _currentDecorationItem;
            set
            {
                if (value == _currentDecorationItem)
                {
                    return;
                }

                _currentDecorationItem = value;
                onCurrentDecorationItemChanged.Invoke(_currentDecorationItem);
            }
        }
        private DecorationItem _currentDecorationItem;

        private void Start()
        {
            InputManager.instance.onRotateItem.AddListener(RotateHeldItem);
        }

        public void SpawnDecorationItem(DecorationItem prefab)
        {
            var newItem = Instantiate(original: prefab, parent: decorationItemsContainer);
            newItem.onClicked.AddListener(() => HoldItem(newItem));
            HoldItem(newItem);
        }
        
        public bool TryWorldPointToDecorationItem(Vector3 point, out DecorationItem item)
        {
            item = null;
            
            if (!roomManager.TryWorldPointToRoom(point, out var room))
            {
                return false;
            }

            if (!decorationGridTileHandler.TryWorldToTile(point, out var decorationTile))
            {
                return false;
            }

            if (!PlacedDecorationItems.TryGetValue(decorationTile.Coordinates, out item))
            {
                return false;
            }

            return true;
        }

        private void HoldItem(DecorationItem item)
        {
            if (currentDecorationItem != null)
            {
                return;
            }

            if (!item.Hold())
            {
                return;
            }

            currentDecorationItem = item;
            decorationGridIndicationHandler.onIndicatedTileChanged.AddListener(MoveItemWithIndicator);
            InputManager.instance.onMouseUp.AddListener(PlaceItem);
        }
        
        private void PlaceItem()
        {
            if (currentDecorationItem == null)
            {
                Debug.Log("currentDecorationItem == null");
                return;
            }
            
            if (!decorationValidationHandler.CanBePlaced)
            {
                Debug.Log("!decorationValidationHandler.CanBePlaced");
                return;
            }

            if (!currentDecorationItem.Place())
            {
                Debug.Log("!currentDecorationItem.Place()");
                return;
            }
            
            var occupiedDecorationTileWorldPoints = decorationGridTileHandler
                .GetCellCoordsInBounds(currentDecorationItem.GetBounds())
                .Select(decorationGridTileHandler.CellCoordsToWorldPosition);
            
            foreach (var worldPoint in occupiedDecorationTileWorldPoints)
            {
                if (!decorationGridTileHandler.TryWorldToTile(worldPoint, out var occupiedDecorationTile))
                {
                    continue;
                }
                
                PlacedDecorationItems.Add(occupiedDecorationTile.Coordinates, currentDecorationItem);
            }
            Debug.Log("Placed");
            currentDecorationItem = null;
            decorationGridIndicationHandler.onIndicatedTileChanged.RemoveListener(MoveItemWithIndicator);
            InputManager.instance.onMouseUp.RemoveListener(PlaceItem);
        }
        
        private void MoveItemWithIndicator(GridTile indicator)
        {
            if (currentDecorationItem == null)
            {
                return;
            }
            
            currentDecorationItem.Move(indicator.WorldPosition);
        }

        private void RotateHeldItem()
        {
            if (currentDecorationItem == null)
            {
                return;
            }
            
            currentDecorationItem.RotateClockwise();
        }
    }
}
