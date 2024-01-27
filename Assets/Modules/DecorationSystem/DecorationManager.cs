using DG.Tweening;
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
        [HideInInspector] public UnityEvent<DecorationItem> onDecorationItemHeld = new();
        [HideInInspector] public UnityEvent<DecorationItem> onDecorationItemRotated = new();
        [HideInInspector] public UnityEvent onDecorationItemPlaced = new();
        
        [SerializeField] private GridIndicationHandler decorationGridIndicationHandler;
        [SerializeField] private RoomManager roomManager;
        [SerializeField] private DecorationItem test;

        private DecorationItem _currentDecorationItem;
        private bool _held;
        private bool _canBePlaced;
        private Vector3 _cachedPosition;
        private Tween _rotationTween;
        
        private void Start()
        {
            InputManager.instance.onMouseUp.AddListener(PlaceItem);
            InputManager.instance.onRotateItem.AddListener(RotateHeldItem);
            AddDecorationItem(test);
        }

        public void AddDecorationItem(DecorationItem decorationItem)
        {
            decorationItem.onClicked.AddListener(() => HoldItem(decorationItem));
        }

        public void HoldItem(DecorationItem item)
        {
            if (_currentDecorationItem != null && _held)
            {
                return;
            }

            _held = true;
            _currentDecorationItem = item;
            _cachedPosition = _currentDecorationItem.transform.position;
            decorationGridIndicationHandler.onIndicatedTileChanged.AddListener(MoveItemWithIndicator);
            onDecorationItemHeld.Invoke(_currentDecorationItem);
        }
        
        private void PlaceItem()
        {
            if (_currentDecorationItem == null)
            {
                return;
            }
            
            _held = false;
            
            if (_canBePlaced)
            {
                onDecorationItemPlaced.Invoke();
                _currentDecorationItem = null;
                decorationGridIndicationHandler.onIndicatedTileChanged.RemoveListener(MoveItemWithIndicator);
            }
        }
        
        private void MoveItemWithIndicator(GridTile indicator)
        {
            if (_currentDecorationItem == null)
            {
                return;
            }
            
            if (!_held)
            {
                return;
            }
            
            _currentDecorationItem.transform.DOMove(indicator.WorldPosition, 0.2f);
            ValidatePlacability();
        }

        private void RotateHeldItem()
        {
            if (_currentDecorationItem == null)
            {
                return;
            }

            if (_rotationTween is { active: true })
            {
                _rotationTween.Kill();
            }
            
            _rotationTween = _currentDecorationItem.transform.DORotate(
                _currentDecorationItem.transform.rotation.eulerAngles + new Vector3(0, 90, 0),
                0.2f
            );
        }

        private void ValidatePlacability()
        {
            if (!roomManager.TryWorldPointToRoom(_currentDecorationItem.transform.position, out var room))
            {
                _canBePlaced = false;
                return;
            }

            _canBePlaced = true;
        }
    }
}
