using System.Collections;
using DG.Tweening;
using SunkCost.HH.Modules.InputSystem;
using SunkCost.HH.Modules.RoomSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunkCost.HH.Modules.DecorationSystem
{
    public class DecorationItem : MonoBehaviour , IPointerDownHandler
    {
        private Room _ownerRoom;
        private Coroutine _placementRoutine;

        private bool CanBePlaced
        {
            get => _canBePlaced;
            set
            {
                if (value == _canBePlaced)
                {
                    return;
                }

                _canBePlaced = value;
                OnPlaceabilityChanged(value);
            }
        }
        private bool _canBePlaced;
        
        private void Start()
        {
            InputManager.instance.onMouseUp.AddListener(HandleMouseUp);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_placementRoutine != null)
            {
                return;
            }
            
            _placementRoutine = StartCoroutine(nameof(Placement));
        }
        
        private void HandleMouseUp()
        {
            if (_placementRoutine == null)
            {
                return;
            }
            
            StopCoroutine(_placementRoutine);
        }
        
        private IEnumerator Placement()
        {
            while (true)
            {
                if (DecorationManager.instance.GetWorldHit(out var point))
                {
                    transform.DOMove(point, 0.2f);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        
        private void OnPlaceabilityChanged(bool newState)
        {
            var ren = GetComponent<Renderer>();
            var mat = ren.material;
            
            if (newState)
            {
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1f);
            }
            else
            {
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.5f);
            }
        }
    }
}
