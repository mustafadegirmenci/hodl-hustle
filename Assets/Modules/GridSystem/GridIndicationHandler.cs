using SunkCost.HH.Modules.InputSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.GridSystem
{
    public class GridIndicationHandler : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<GridTile> onIndicatedTileChanged = new();
        
        [SerializeField] private GridInteractionHandler gridInteractionHandler;
        
        private GridTile _pointedGridTile;
        
        private void Start()
        {
            InputManager.instance.onMouseDelta.AddListener(RecalculatePointedGridTile);
        }

        private void RecalculatePointedGridTile()
        {
            if (!gridInteractionHandler.ScreenToTile(out var tileHit))
            {
                return;
            }

            if (tileHit == _pointedGridTile)
            {
                return;
            }

            _pointedGridTile = tileHit;
            onIndicatedTileChanged.Invoke(_pointedGridTile);
        }
    }
}
