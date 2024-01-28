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

        [SerializeField] private float animationTime = 0.1f;
        [SerializeField] private float defaultAlpha = 0.8f;
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
            _images.ForEach(i => i.DOFade(defaultAlpha, 0));
            _texts.ForEach(t => t.DOFade(0, 0));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _images.ForEach(i => i.DOFade(1, animationTime));
            _transform.DOScale(hoveredScale, animationTime);
            _texts.ForEach(t => t.DOFade(1, animationTime));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _images.ForEach(i => i.DOFade(defaultAlpha, animationTime));
            _transform.DOScale(1, animationTime);
            _texts.ForEach(t => t.DOFade(0, animationTime));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
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
