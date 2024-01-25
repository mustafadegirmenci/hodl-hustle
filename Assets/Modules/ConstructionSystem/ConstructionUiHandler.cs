using UnityEngine;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class ConstructionUiHandler : MonoBehaviour
    {
        [SerializeField] private ConstructionStateHandler constructionStateHandler;
        [SerializeField] private ConstructionManager constructionManager;
        
        [SerializeField] private Button addTilesButton;
        [SerializeField] private Button removeTilesButton;
        [SerializeField] private Button finishConstructionButton;
        
        private void Start()
        {
            addTilesButton.onClick.AddListener(constructionStateHandler.SwitchToAddTilesMode);
            removeTilesButton.onClick.AddListener(constructionStateHandler.SwitchToRemoveTilesMode);
            finishConstructionButton.onClick.AddListener(() => constructionManager.EndConstruction());
            
            finishConstructionButton.gameObject.SetActive(false);
            
            constructionManager.onConstructionStarted.AddListener(_ =>
            {
                finishConstructionButton.gameObject.SetActive(true);
            });
            
            constructionManager.onConstructionEnded.AddListener(_ =>
            {
                finishConstructionButton.gameObject.SetActive(false);
            });
        }
    }
}
