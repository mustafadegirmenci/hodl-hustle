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
        
        public readonly List<Transform> PlacementIndicators = new();

        private void Start()
        {
            decorationManager.onDecorationItemHeld.AddListener(UpdatePlacementIndicators);
            decorationManager.onDecorationItemRotated.AddListener(UpdatePlacementIndicators);
            decorationManager.onDecorationItemPlaced.AddListener(ClearPlacementIndicators);
            decorationManager.onPlaceabilityChanged.AddListener(UpdateIndicatorColors);
        }

        private void UpdatePlacementIndicators(DecorationItem item)
        {
            ClearPlacementIndicators();
            
            var indicatorWorldPositions = decorationGridTileHandler
                .GetCellCoordsInBounds(item.GetBounds())
                .Select(ic => decorationGridTileHandler.CellCoordsToWorldPosition(ic));
            
            foreach (var indicatorPos in indicatorWorldPositions)
            {
                PlacementIndicators.Add(Instantiate(placementIndicatorPrefab, 
                    indicatorPos,
                    Quaternion.identity,
                    item.transform)
                );
            }
        }

        private void UpdateIndicatorColors(bool validity)
        {
            foreach (var placementIndicator in PlacementIndicators)
            {
                placementIndicator.GetComponentsInChildren<Renderer>()
                    .ToList()
                    .ForEach(r => r.material.color = validity ? Color.green : Color.red);
            }
        }

        private void ClearPlacementIndicators()
        {
            foreach (var placementIndicator in PlacementIndicators.ToList())
            {
                PlacementIndicators.Remove(placementIndicator);
                Destroy(placementIndicator.gameObject);
            }
        }
    }
}
