using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using SunkCost.HH.Modules.RoomSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.DecorationSystem
{
    public class DecorationValidationHandler : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<bool> onPlacabilityChanged = new();
        
        [SerializeField] private GridTileHandler decorationGridTileHandler;
        [SerializeField] private DecorationManager decorationManager;
        [SerializeField] private RoomManager roomManager;
        
        public bool CanBePlaced
        {
            get => _canBePlaced;
            private set
            {
                if (value == _canBePlaced)
                {
                    return;
                }

                _canBePlaced = value;
                onPlacabilityChanged.Invoke(value);
                Debug.Log(value);
            } 
        }

        private bool _canBePlaced;
        private DecorationItem _itemToValidate;
        
        private void OnEnable()
        {
            decorationManager.onCurrentDecorationItemChanged.AddListener(HandleDecorationItemToValidateChanged);
        }
        
        private void OnDisable()
        {
            decorationManager.onCurrentDecorationItemChanged.RemoveListener(HandleDecorationItemToValidateChanged);
            if (_itemToValidate != null)
            {
                _itemToValidate.onMoved.RemoveListener(ValidatePlacability);
                _itemToValidate.onRotated.RemoveListener(ValidatePlacability);
            }
        }

        private void HandleDecorationItemToValidateChanged(DecorationItem newItem)
        {
            if (_itemToValidate != null)
            {
                _itemToValidate.onMoved.RemoveListener(ValidatePlacability);
                _itemToValidate.onRotated.RemoveListener(ValidatePlacability);
            }
            
            _itemToValidate = newItem;
            if (_itemToValidate != null)
            {
                _itemToValidate.onMoved.AddListener(ValidatePlacability);
                _itemToValidate.onRotated.AddListener(ValidatePlacability);
            }
        }

        private void ValidatePlacability()
        {
            if (_itemToValidate == null)
            {
                CanBePlaced = false;
                return;
            }

            var occupiedDecorationTileWorldPoints = decorationGridTileHandler
                .GetCellCoordsInBounds(_itemToValidate.GetBounds())
                .Select(decorationGridTileHandler.CellCoordsToWorldPosition).ToList();
            
            foreach (var worldPoint in occupiedDecorationTileWorldPoints)
            {
                if (!roomManager.TryWorldPointToRoom(worldPoint, out var room))
                {
                    CanBePlaced = false;
                    Debug.Log("Not in a room");
                    return;
                }

                if (decorationManager.TryWorldPointToDecorationItem(worldPoint, out var occupantItem))
                {
                    CanBePlaced = false;
                    return;
                }
            }

            CanBePlaced = true;
        }
    }
}
