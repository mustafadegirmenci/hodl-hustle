using UnityEngine;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.UiSystem
{
    public class SC_Popup : MonoBehaviour
    {
        private Transform _transform;
        private Image _image;

        private void Awake()
        {
            _transform = transform;
            _image = GetComponent<Image>();
        }
        
        public void Show()
        {
            
        }

        public void Hide()
        {
            
        }
    }
}
