using SunkCost.HH.Modules.UiSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class ConstructionUiHandler : MonoBehaviour
    {
        [SerializeField] private ConstructionStateHandler constructionStateHandler;
        [SerializeField] private ConstructionManager constructionManager;
        
        [SerializeField] private SC_Button addTilesButton;
        [SerializeField] private SC_Button removeTilesButton;
        [SerializeField] private SC_Button finishConstructionButton;
        
        private void Start()
        {
            addTilesButton.onClick.AddListener(constructionStateHandler.SwitchToAddTilesMode);
            removeTilesButton.onClick.AddListener(constructionStateHandler.SwitchToRemoveTilesMode);
            finishConstructionButton.onClick.AddListener(() => constructionManager.EndConstruction());
            
            finishConstructionButton.gameObject.SetActive(false);
            addTilesButton.gameObject.SetActive(false);
            removeTilesButton.gameObject.SetActive(false);
            
            constructionManager.onConstructionStarted.AddListener(_ =>
            {
                finishConstructionButton.gameObject.SetActive(true);
                addTilesButton.gameObject.SetActive(true);
                removeTilesButton.gameObject.SetActive(true);
            });
            
            constructionManager.onConstructionEnded.AddListener(_ =>
            {
                finishConstructionButton.gameObject.SetActive(false);
                addTilesButton.gameObject.SetActive(false);
                removeTilesButton.gameObject.SetActive(false);
            });
        }
    }
}
