using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SunkCost.HH.Modules.DecorationSystem
{
    public class DecorationItem : MonoBehaviour, IPointerDownHandler
    {
        [HideInInspector] public UnityEvent onHeld = new();
        [HideInInspector] public UnityEvent onPlaced = new();
        [HideInInspector] public UnityEvent onClicked = new();
        [HideInInspector] public UnityEvent onMoved = new();
        [HideInInspector] public UnityEvent onRotated = new();
        
        public MeshRenderer decorationIndicator;
        public DecorationItemState CurrentState { get; private set; } = DecorationItemState.ToBePlaced;

        private Tween _rotationTween;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            onClicked.Invoke();
        }

        public bool Hold()
        {
            if (CurrentState != DecorationItemState.Placed && CurrentState != DecorationItemState.ToBePlaced)
            {
                return false;
            }
            
            CurrentState = DecorationItemState.Replacing;
            onHeld.Invoke();
            return true;
        }

        public bool Place()
        {
            if (CurrentState != DecorationItemState.ToBePlaced && CurrentState != DecorationItemState.Replacing)
            {
                return false;
            }

            onPlaced.Invoke();
            CurrentState = DecorationItemState.Placed;
            return true;
        }

        public void Move(Vector3 destination)
        {
            onMoved.Invoke();
            
            transform
                .DOMove(destination, 0.05f)
                .OnComplete(() => onMoved.Invoke());
        }

        public void RotateClockwise()
        {
            if (_rotationTween is { active: true })
            {
                return;
            }
            
            _rotationTween = transform
                .DORotate(transform.rotation.eulerAngles + new Vector3(0, 90, 0), 0.05f)
                .OnComplete(() => onRotated.Invoke());
        }

        public Bounds GetBounds()
        {
            return decorationIndicator.bounds;
        }
    }
}