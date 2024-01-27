using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.DecorationSystem
{
    public class DecorationIndicationHandler : MonoBehaviour
    {
        [SerializeField] private Transform placementIndicatorPrefab;
        [SerializeField] private DecorationManager decorationManager;
        [SerializeField] private GridTileHandler decorationGridTileHandler;
        
        private readonly List<Transform> _placementIndicators = new();

        private void Start()
        {
            decorationManager.onDecorationItemHeld.AddListener(UpdatePlacementIndicators);
            decorationManager.onDecorationItemRotated.AddListener(UpdatePlacementIndicators);
            decorationManager.onDecorationItemPlaced.AddListener(ClearPlacementIndicators);
        }

        private void UpdatePlacementIndicators(DecorationItem item)
        {
            ClearPlacementIndicators();
            
            var indicatorWorldPositions = decorationGridTileHandler
                .GetCellCoordsInBounds(item.GetBounds())
                .Select(ic => decorationGridTileHandler.CellCoordsToWorldPosition(ic));
            
            foreach (var indicatorPos in indicatorWorldPositions)
            {
                _placementIndicators.Add(Instantiate(placementIndicatorPrefab, 
                    indicatorPos,
                    Quaternion.identity,
                    item.transform)
                );
            }
        }

        private void ClearPlacementIndicators()
        {
            foreach (var placementIndicator in _placementIndicators.ToList())
            {
                _placementIndicators.Remove(placementIndicator);
                Destroy(placementIndicator.gameObject);
            }
        }
    }
}
