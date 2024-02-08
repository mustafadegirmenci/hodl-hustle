using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.UiSystem
{
    public class SC_Button : 
        MonoBehaviour, 
        IPointerEnterHandler, 
        IPointerExitHandler, 
        IPointerDownHandler, 
        IPointerUpHandler
    {
        public UnityEvent onClick = new();
        public UnityEvent onHoverEnter = new();
        public UnityEvent onHoverExit = new();
        public bool interactable = true;

        [SerializeField] private float animationTime = 0.1f;
        [SerializeField] private float defaultImageAlpha = 0.8f;
        [SerializeField] private float defaultTextAlpha = 0f;
        [SerializeField] private float hoveredImageAlpha = 1f;
        [SerializeField] private float hoveredTextAlpha = 1f;
        [SerializeField] private float pressedScale = 1f;
        [SerializeField] private float hoveredScale = 1.1f;
        
        private Transform _transform;
        private List<Image> _images;
        private List<TMP_Text> _texts;

        private void Awake()
        {
            _transform = transform;
            _images = GetComponentsInChildren<Image>().ToList();
            _texts = GetComponentsInChildren<TMP_Text>().ToList();
        }

        private void Start()
        {
            _images.ForEach(i => i.DOFade(defaultImageAlpha, 0));
            _texts.ForEach(t => t.DOFade(defaultTextAlpha, 0));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!interactable) return;
            
            _images.ForEach(i => i.DOFade(hoveredImageAlpha, animationTime));
            _transform.DOScale(hoveredScale, animationTime);
            _texts.ForEach(t => t.DOFade(hoveredTextAlpha, animationTime));
            
            onHoverEnter.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _images.ForEach(i => i.DOFade(defaultImageAlpha, animationTime));
            _transform.DOScale(1, animationTime);
            _texts.ForEach(t => t.DOFade(defaultTextAlpha, animationTime));
            
            onHoverExit.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable) return;
            
            _transform.DOScale(pressedScale, animationTime);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _transform.DOScale(hoveredScale, animationTime);
            onClick.Invoke();
        }

        private void OnDisable()
        {
            OnPointerExit(null);
        }
    }
}
