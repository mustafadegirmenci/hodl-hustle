using System.Collections.Generic;
using SunkCost.HH.Modules.InputSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.GridSystem
{
    public class GridManager : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<GridCoordinate> onIndicatorCoordsChanged = new();
        [HideInInspector] public UnityEvent<GridTile> onGridTileClicked = new();

        [SerializeField] private Camera gridSelectionCamera;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private LayerMask gridSelectionLayerMask;
        [SerializeField] private Grid grid;

        [SerializeField] private Transform initialTilesContainer;

        public GridCoordinate IndicatorGridCoord { get; private set; }

        private readonly Dictionary<GridCoordinate, GridTile> _tiles = new();

        public void AddTile(GridCoordinate coordinate, GridTile tile)
        {
            _tiles.Add(coordinate, tile);
            tile.Coordinates = coordinate;
            inputManager.onMouseDown.AddListener(CheckGridTileClick);
        }

        private void Start()
        {
            foreach (Transform initialTile in initialTilesContainer)
            {
                var cellPos = grid.WorldToCell(initialTile.transform.position);
                AddTile(new GridCoordinate(cellPos.x, cellPos.z), initialTile.GetComponent<GridTile>());
            }

            inputManager.onMouseDelta.AddListener(RecalculateGridIndicator);
        }

        public List<GridTile> GetTilesInSelectionBox(GridCoordinate startCorner, GridCoordinate endCorner)
        {
            var coords = new List<GridTile>();
            
            var minX = Mathf.Min(startCorner.X, endCorner.X);
            var maxX = Mathf.Max(startCorner.X, endCorner.X);
            var minZ = Mathf.Min(startCorner.Z, endCorner.Z);
            var maxZ = Mathf.Max(startCorner.Z, endCorner.Z);

            for (var x = minX; x <= maxX; x++)
            {
                for (var z = minZ; z <= maxZ; z++)
                {
                    var coord = new GridCoordinate(x, z);
                    if (_tiles.TryGetValue(coord, out var tile))
                    {
                        coords.Add(tile);
                    }
                }
            }

            return coords;
        }

        public Vector3 GridToWorld(GridCoordinate gridCoordinate)
        {
            return grid.CellToWorld(new Vector3Int(gridCoordinate.X, 0, gridCoordinate.Z));
        }

        private void RecalculateGridIndicator()
        {
            var ray = gridSelectionCamera.ScreenPointToRay(inputManager.MousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, gridSelectionLayerMask))
            {
                return;
            }
            
            var cellPos = grid.WorldToCell(hit.transform.position);
            
            var newIndicatorGridCoord = new GridCoordinate(cellPos.x, cellPos.z);
            if (newIndicatorGridCoord == IndicatorGridCoord)
            {
                return;
            }
                
            IndicatorGridCoord = newIndicatorGridCoord;
            onIndicatorCoordsChanged.Invoke(newIndicatorGridCoord);
        }

        private void CheckGridTileClick()
        {
            var ray = gridSelectionCamera.ScreenPointToRay(inputManager.MousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, gridSelectionLayerMask))
            {
                return;
            }

            if (!hit.transform.TryGetComponent<GridTile>(out var tile))
            {
                return;
            }
            
            onGridTileClicked.Invoke(tile);
        }
    }
}
