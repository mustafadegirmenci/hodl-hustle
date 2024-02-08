using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace SunkCost.HH.Modules.UiSystem
{
    public class SC_Popup : MonoBehaviour
    {
        [SerializeField] private float animationTime = 0.1f;
        
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public async void ShowAsync()
        {
            gameObject.SetActive(true);
            await _canvasGroup.DOFade(1, animationTime);
        }

        public async void HideAsync()
        {
            await _canvasGroup.DOFade(0, animationTime);
            gameObject.SetActive(false);
        }
    }
}
