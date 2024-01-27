using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SunkCost.HH.Modules.DecorationSystem
{
    public class DecorationItem : MonoBehaviour, IPointerDownHandler
    {
        [HideInInspector] public UnityEvent onClicked = new();

        private Renderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onClicked.Invoke();
        }

        public List<Bounds> GetBounds()
        {
            return GetComponentsInChildren<Renderer>()
                .Select(r => r.bounds)
                .ToList();
        }
    }
}