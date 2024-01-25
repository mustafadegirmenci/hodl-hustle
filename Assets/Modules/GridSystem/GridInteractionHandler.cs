using SunkCost.HH.Modules.InputSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.GridSystem
{
    public class GridInteractionHandler : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<GridTile> onTileClicked = new();
        
        [SerializeField] private GridTileHandler gridTileHandler;
        [SerializeField] private Grid grid;
        [SerializeField] private LayerMask layerMask;
        
        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
        }

        private void Start()
        {
            InputManager.instance.onMouseDown.AddListener(CheckGridTileClick);
        }
        
        public bool ScreenToTileCoordinates(out Vector3Int tileHitCoords)
        {
            var ray = _cam.ScreenPointToRay(InputManager.instance.MousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
            {
                tileHitCoords = Vector3Int.zero;
                return false;
            }

            tileHitCoords = grid.WorldToCell(hit.point + grid.cellSize / 2);
            return true;
        }

        public bool ScreenToTilePosition(out Vector3 tileHitPosition)
        {
            tileHitPosition = default;
            if (!ScreenToTileCoordinates(out var coords))
            {
                return false;
            }
            
            tileHitPosition = grid.CellToWorld(coords);
            return true;
        }

        public bool ScreenToTile(out GridTile tileHit)
        {
            tileHit = default;
            
            if (!ScreenToTileCoordinates(out var tileHitCoords))
            {
                return false;
            }
            
            if (!gridTileHandler.Tiles.TryGetValue(tileHitCoords, out tileHit))
            {
                return false;
            }

            return true;
        }

        private void CheckGridTileClick()
        {
            if (!ScreenToTile(out var tileHit))
            {
                return;
            }
            
            onTileClicked.Invoke(tileHit);
        }
    }
}
