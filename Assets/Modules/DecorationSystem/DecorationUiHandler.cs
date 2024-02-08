using SunkCost.HH.Modules.UiSystem;
using SunkCost.HH.Modules.Utility;
using UnityEngine;

namespace SunkCost.HH.Modules.DecorationSystem
{
    public class DecorationUiHandler : MonoSingleton<DecorationUiHandler>
    {
        [SerializeField] private DecorationItemRegistry itemRegistry;
        [SerializeField] private Transform cardContainer;
        [SerializeField] private DecorationItemCard cardPrefab;
        [SerializeField] private SC_Popup decorationItemsList;

        private void Start()
        {
            InitializeItemCards();
        }

        public void ShowDecorationItemsList()
        {
            decorationItemsList.ShowAsync();
        }

        public void HideDecorationItemsList()
        {
            decorationItemsList.HideAsync();
        }

        private void InitializeItemCards()
        {
            foreach (var entry in itemRegistry.entries)
            {
                var newCard = Instantiate(original: cardPrefab, parent: cardContainer);
                newCard.Init(entry);
            }
        }
    }
}
