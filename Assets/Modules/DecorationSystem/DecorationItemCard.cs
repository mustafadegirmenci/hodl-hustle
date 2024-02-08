using SunkCost.HH.Modules.UiSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.DecorationSystem
{
    public class DecorationItemCard : SC_Button
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Image image;
        
        private DecorationItem _prefab;

        public void Init(DecorationItemRegistryEntry entry)
        {
            nameText.text = entry.name;
            _prefab = entry.prefab;
            priceText.text = entry.price.ToString();
            image.sprite = entry.image;
            
            onClick.AddListener(HandleClick);
        }

        private void HandleClick()
        {
            DecorationManager.instance.SpawnDecorationItem(_prefab);
        }
    }
}
