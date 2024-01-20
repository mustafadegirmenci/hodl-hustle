using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class ScaffoldManager : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private ConstructionManager constructionManager;
        [SerializeField] private Scaffold scaffoldPrefab;
        [SerializeField] private Transform scaffoldsContainer;

        private readonly Dictionary<GridCoordinate, Scaffold> _persistentScaffolds = new();
        private readonly Dictionary<GridCoordinate, Scaffold> _volatileScaffolds = new();
        private ConstructionState _state;
        
        private void Start()
        {
            constructionManager.onPersistentSelectionChanged.AddListener(UpdatePersistentScaffolds);
            constructionManager.onVolatileSelectionChanged.AddListener(UpdateVolatileScaffolds);
            constructionManager.onStateChanged.AddListener(state => _state = state);
            constructionManager.onConstructionEnded.AddListener(_ => ClearScaffolds());
        }

        private void UpdateVolatileScaffolds(List<GridTile> tiles)
        {
            var removedTiles = _volatileScaffolds.Keys.Except(tiles.Select(t => t.Coordinates)).ToList();
            var addedTiles = tiles.Select(t => t.Coordinates).Except(_volatileScaffolds.Keys).ToList();
            
            foreach (var removed in removedTiles)
            {
                Destroy(_volatileScaffolds[removed].gameObject);
                _volatileScaffolds.Remove(removed);
            }
            
            foreach (var added in addedTiles)
            {
                var newScaffold = Instantiate(
                    original: scaffoldPrefab,
                    parent: scaffoldsContainer,
                    position: gridManager.GridToWorld(added),
                    rotation: Quaternion.identity
                );

                if (_state is ConstructionState.SelectingTilesToBeAdded)
                {
                    newScaffold.SetState(ScaffoldState.SelectionToAdd);
                }
                else if (_state is ConstructionState.SelectingTilesToBeRemoved)
                {
                    newScaffold.SetState(ScaffoldState.SelectionToRemove);
                }
                _volatileScaffolds.Add(added, newScaffold);
            }
        }
        
        private void UpdatePersistentScaffolds(List<GridTile> tiles)
        {
            foreach (var (key, value) in _volatileScaffolds)
            {
                Destroy(value.gameObject);
            }
            _volatileScaffolds.Clear();
            
            var removedTiles = _persistentScaffolds.Keys.Except(tiles.Select(t => t.Coordinates)).ToList();
            var addedTiles = tiles.Select(t => t.Coordinates).Except(_persistentScaffolds.Keys).ToList();
            
            foreach (var removed in removedTiles)
            {
                Destroy(_persistentScaffolds[removed].gameObject);
                _persistentScaffolds.Remove(removed);
            }
            
            foreach (var added in addedTiles)
            {
                var newScaffold = Instantiate(
                    original: scaffoldPrefab,
                    parent: scaffoldsContainer,
                    position: gridManager.GridToWorld(added),
                    rotation: Quaternion.identity
                );
                newScaffold.SetState(ScaffoldState.Persistent);
                _persistentScaffolds.Add(added, newScaffold);
            }
        }

        private void ClearScaffolds()
        {
            foreach (var (key, value) in _persistentScaffolds)
            {
                Destroy(value.gameObject);
            }
            
            foreach (var (key, value) in _volatileScaffolds)
            {
                Destroy(value.gameObject);
            }
            
            _persistentScaffolds.Clear();
            _volatileScaffolds.Clear();
        }
    }
}
