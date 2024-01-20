using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using SunkCost.HH.Modules.InputSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class ConstructionManager : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<Vector3> onIndicatorPositionChanged = new();
        [HideInInspector] public UnityEvent<ConstructionState> onStateChanged = new();
        [HideInInspector] public UnityEvent<List<GridTile>> onVolatileSelectionChanged = new();
        [HideInInspector] public UnityEvent<List<GridTile>> onPersistentSelectionChanged = new();
        [HideInInspector] public UnityEvent<List<GridTile>> onConstructionEnded = new();

        [SerializeField] private Button addTilesButton;
        [SerializeField] private Button removeTilesButton;
        
        [SerializeField] private GridManager gridManager;
        [SerializeField] private InputManager inputManager;
        
        private readonly List<GridTile> _persistentSelection = new();
        private List<GridTile> _volatileSelection = new();

        private bool _isSelectionIntegrityValid;

        private ConstructionState CurrentState
        {
            get => _currentState;
            set
            {
                var oldValue = _currentState;
                _currentState = value;

                if (oldValue != value)
                {
                    onStateChanged.Invoke(value);
                }
            }
        }
        private ConstructionState _currentState;
        
        private GridTile _selectionStartCornerTile;

        private void Start()
        {
            addTilesButton.onClick.AddListener(SwitchToAddTilesMode);
            removeTilesButton.onClick.AddListener(SwitchToRemoveTilesMode);

            CurrentState = ConstructionState.Passive;
        }

        public void StartConstruction()
        {
            if (CurrentState is not ConstructionState.Passive)
            {
                return;
            }
            
            CurrentState = ConstructionState.WaitingToSelectTilesToBeAdded;
            gridManager.onIndicatorCoordsChanged.AddListener(HandleIndicatorCoordsChanged);
            gridManager.onGridTileClicked.AddListener(HandleGridTileClicked);
            inputManager.onMouseUp.AddListener(HandleMouseUp);
        }

        public void EndConstruction()
        {
            if (CurrentState is ConstructionState.Passive)
            {
                return;
            }
            
            CurrentState = ConstructionState.Passive;
            gridManager.onIndicatorCoordsChanged.RemoveListener(HandleIndicatorCoordsChanged);
            onConstructionEnded.Invoke(_persistentSelection);
            _persistentSelection.Clear();
        }

        private void SwitchToAddTilesMode()
        {
            if (CurrentState is ConstructionState.WaitingToSelectTilesToBeRemoved)
            {
                CurrentState = ConstructionState.WaitingToSelectTilesToBeAdded;
            }
        }

        private void SwitchToRemoveTilesMode()
        {
            if (CurrentState is ConstructionState.WaitingToSelectTilesToBeAdded)
            {
                CurrentState = ConstructionState.WaitingToSelectTilesToBeRemoved;
            }
        }

        private void HandleIndicatorCoordsChanged(GridCoordinate indicatorCoords)
        {
            var newIndicatorPosition = gridManager.GridToWorld(indicatorCoords);
            onIndicatorPositionChanged.Invoke(newIndicatorPosition);
            
            if (CurrentState is ConstructionState.SelectingTilesToBeAdded
                     or ConstructionState.SelectingTilesToBeRemoved)
            {
                var newCurrentSelection = gridManager.GetTilesInSelectionBox(
                    startCorner: _selectionStartCornerTile.Coordinates,
                    endCorner: indicatorCoords
                );

                UpdateVolatileSelection(newCurrentSelection);
            }
        }

        private void HandleGridTileClicked(GridTile tile)
        {
            if (CurrentState == ConstructionState.WaitingToSelectTilesToBeAdded)
            {
                _selectionStartCornerTile = tile;
                CurrentState = ConstructionState.SelectingTilesToBeAdded;
            }
            else if (CurrentState == ConstructionState.WaitingToSelectTilesToBeRemoved)
            {
                _selectionStartCornerTile = tile;
                CurrentState = ConstructionState.SelectingTilesToBeRemoved;
            }
        }

        private void HandleMouseUp()
        {
            if (CurrentState == ConstructionState.SelectingTilesToBeAdded)
            {
                CurrentState = ConstructionState.WaitingToSelectTilesToBeAdded;

                if (_isSelectionIntegrityValid)
                {
                    AddVolatileSelectionToPersistent();
                }
                else
                {
                    UpdateVolatileSelection(new List<GridTile>());
                }
            }
            else if (CurrentState == ConstructionState.SelectingTilesToBeRemoved)
            {
                CurrentState = ConstructionState.WaitingToSelectTilesToBeRemoved;
                
                if (_isSelectionIntegrityValid)
                {
                    RemoveVolatileSelectionFromPersistent();
                }
                else
                {
                    UpdateVolatileSelection(new List<GridTile>());
                }
            }
        }

        private void UpdateVolatileSelection(List<GridTile> newValue)
        {
            _volatileSelection = newValue;
            if (CurrentState is ConstructionState.SelectingTilesToBeAdded)
            {
                _isSelectionIntegrityValid = ValidateRoomIntegrity(_volatileSelection.Concat(_persistentSelection).ToList());
            }
            else if (CurrentState is ConstructionState.SelectingTilesToBeRemoved)
            {
                _isSelectionIntegrityValid = ValidateRoomIntegrity(_persistentSelection.Except(_volatileSelection).ToList());
            }
            
            onVolatileSelectionChanged.Invoke(newValue);
        }

        private void AddVolatileSelectionToPersistent()
        {
            foreach (var add in _volatileSelection.Where(add => !_persistentSelection.Contains(add)))
            {
                _persistentSelection.Add(add);
            }
            _volatileSelection.Clear();

            onPersistentSelectionChanged.Invoke(_persistentSelection);
        }

        private void RemoveVolatileSelectionFromPersistent()
        {
            foreach (var add in _volatileSelection.Where(add => _persistentSelection.Contains(add)))
            {
                _persistentSelection.Remove(add);
            }
            _volatileSelection.Clear();

            onPersistentSelectionChanged.Invoke(_persistentSelection);
        }
        
        private bool ValidateRoomIntegrity(List<GridTile> tilesToValidate)
        {
            foreach (var tile in tilesToValidate)
            {
                if (tile.Occupant != null) return false;
                if (!AllTilesConnected()) return false;
            }

            return true;

            bool AllTilesConnected()
            {
                var tilePositionSet = new HashSet<GridCoordinate>(tilesToValidate.Select(t => t.Coordinates));

                var visited = new HashSet<GridCoordinate>();
                var stack = new Stack<GridCoordinate>();

                stack.Push(tilesToValidate[0].Coordinates);
                visited.Add(tilesToValidate[0].Coordinates);

                while (stack.Count > 0)
                {
                    var currentTile = stack.Pop();

                    foreach (var neighbor in GetNeighbors(currentTile))
                    {
                        if (tilePositionSet.Contains(neighbor) && !visited.Contains(neighbor))
                        {
                            stack.Push(neighbor);
                            visited.Add(neighbor);
                        }
                    }
                }

                return visited.Count == tilePositionSet.Count;

                IEnumerable<GridCoordinate> GetNeighbors(GridCoordinate tile)
                {
                    int[] dx = { -1, 1, 0, 0 };
                    int[] dz = { 0, 0, -1, 1 };

                    for (var i = 0; i < dx.Length; i++)
                    {
                        var nx = tile.X + dx[i];
                        var nz = tile.Z + dz[i];

                        yield return new GridCoordinate(nx, nz);
                    }
                }
            }
        }
    }
}
