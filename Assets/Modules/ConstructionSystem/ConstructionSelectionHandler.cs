using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using SunkCost.HH.Modules.InputSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class ConstructionSelectionHandler : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<GridTile> onSelectionIndicatorChanged = new();
        [HideInInspector] public UnityEvent<List<GridTile>> onVolatileSelectionChanged = new();
        [HideInInspector] public UnityEvent<List<GridTile>> onPersistentSelectionChanged = new();
        
        public readonly List<GridTile> PersistentSelection = new();
        public readonly List<GridTile> VolatileSelection = new();
        
        [SerializeField] private ConstructionStateHandler constructionStateHandler;
        [SerializeField] private ConstructionManager constructionManager;
        [SerializeField] private ConstructionValidationHandler constructionValidationHandler;
        
        [SerializeField] private GridIndicationHandler gridIndicationHandler;
        [SerializeField] private GridSelectionHandler gridSelectionHandler;
        [SerializeField] private GridInteractionHandler gridInteractionHandler;
        
        private GridTile _selectionStartCornerTile;

        private void Start()
        {
            constructionManager.onConstructionStarted.AddListener(initialTiles =>
            {
                PersistentSelection.Clear();
                
                UpdateVolatileSelection(initialTiles);
                AddVolatileSelectionToPersistent();
            
                gridIndicationHandler.onIndicatedTileChanged.AddListener(onSelectionIndicatorChanged.Invoke);
                gridIndicationHandler.onIndicatedTileChanged.AddListener(HandleIndicatorChanged);
                gridInteractionHandler.onTileClicked.AddListener(HandleTileClicked);
                InputManager.instance.onMouseUp.AddListener(HandleMouseUp);
            });
            
            constructionManager.onConstructionEnded.AddListener(finalTiles =>
            {
                gridIndicationHandler.onIndicatedTileChanged.RemoveListener(onSelectionIndicatorChanged.Invoke);
                gridIndicationHandler.onIndicatedTileChanged.RemoveListener(HandleIndicatorChanged);
                gridInteractionHandler.onTileClicked.RemoveListener(HandleTileClicked);
                InputManager.instance.onMouseUp.RemoveListener(HandleMouseUp);
            });
        }

        private void UpdateVolatileSelection(List<GridTile> newValue)
        {
            VolatileSelection.Clear();
            VolatileSelection.AddRange(newValue);
            onVolatileSelectionChanged.Invoke(newValue);
        }

        private void AddVolatileSelectionToPersistent()
        {
            foreach (var add in VolatileSelection.Where(add => !PersistentSelection.Contains(add)))
            {
                PersistentSelection.Add(add);
            }
            VolatileSelection.Clear();

            onPersistentSelectionChanged.Invoke(PersistentSelection);
        }

        private void RemoveVolatileSelectionFromPersistent()
        {
            foreach (var add in VolatileSelection.Where(add => PersistentSelection.Contains(add)))
            {
                PersistentSelection.Remove(add);
            }
            VolatileSelection.Clear();

            onPersistentSelectionChanged.Invoke(PersistentSelection);
        }
        
        private void HandleIndicatorChanged(GridTile indicator)
        {
            if (constructionStateHandler.CurrentState is ConstructionState.SelectingTilesToBeAdded
                     or ConstructionState.SelectingTilesToBeRemoved)
            {
                var newCurrentSelection = gridSelectionHandler.GetTilesInSelectionBox(
                    startCorner: _selectionStartCornerTile.Coordinates,
                    endCorner: indicator.Coordinates
                );

                UpdateVolatileSelection(newCurrentSelection);
            }
        }

        private void HandleTileClicked(GridTile tile)
        {
            if (constructionStateHandler.CurrentState == ConstructionState.WaitingToSelectTilesToBeAdded)
            {
                _selectionStartCornerTile = tile;
                constructionStateHandler.CurrentState = ConstructionState.SelectingTilesToBeAdded;
            }
            else if (constructionStateHandler.CurrentState == ConstructionState.WaitingToSelectTilesToBeRemoved)
            {
                _selectionStartCornerTile = tile;
                constructionStateHandler.CurrentState = ConstructionState.SelectingTilesToBeRemoved;
            }
        }

        private void HandleMouseUp()
        {
            if (constructionStateHandler.CurrentState == ConstructionState.SelectingTilesToBeAdded)
            {
                constructionStateHandler.CurrentState = ConstructionState.WaitingToSelectTilesToBeAdded;

                var tilesToValidate = PersistentSelection.Concat(VolatileSelection).ToList();
                if (constructionValidationHandler.ValidateSelection(tilesToValidate))
                {
                    AddVolatileSelectionToPersistent();
                }
                else
                {
                    UpdateVolatileSelection(new List<GridTile>());
                }
            }
            else if (constructionStateHandler.CurrentState == ConstructionState.SelectingTilesToBeRemoved)
            {
                constructionStateHandler.CurrentState = ConstructionState.WaitingToSelectTilesToBeRemoved;
                
                var tilesToValidate = PersistentSelection.Except(VolatileSelection).ToList();
                if (constructionValidationHandler.ValidateSelection(tilesToValidate))
                {
                    RemoveVolatileSelectionFromPersistent();
                }
                else
                {
                    UpdateVolatileSelection(new List<GridTile>());
                }
            }
        }
    }
}
