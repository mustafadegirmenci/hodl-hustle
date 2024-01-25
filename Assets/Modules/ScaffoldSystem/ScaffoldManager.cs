using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.ConstructionSystem;
using SunkCost.HH.Modules.GridSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.ScaffoldSystem
{
    public class ScaffoldManager : MonoBehaviour
    {
        [SerializeField] private ConstructionSelectionHandler constructionSelectionHandler;
        [SerializeField] private ConstructionStateHandler constructionStateHandler;
        
        [SerializeField] private ConstructionManager constructionManager;
        [SerializeField] private Scaffold scaffoldPrefab;
        [SerializeField] private Transform scaffoldsContainer;

        private readonly Dictionary<GridTile, Scaffold> _persistentScaffolds = new();
        private readonly Dictionary<GridTile, Scaffold> _volatileScaffolds = new();
        private ConstructionState _state;
        
        private void Start()
        {
            constructionSelectionHandler.onPersistentSelectionChanged.AddListener(UpdatePersistentScaffolds);
            constructionSelectionHandler.onVolatileSelectionChanged.AddListener(UpdateVolatileScaffolds);
            constructionStateHandler.onStateChanged.AddListener(state => _state = state);
            constructionManager.onConstructionEnded.AddListener(_ => ClearScaffolds());
        }

        private void UpdateVolatileScaffolds(List<GridTile> tiles)
        {
            var removedTiles = _volatileScaffolds.Keys.Where(t => !tiles.Contains(t));
            var addedTiles = tiles.Where(t => !_volatileScaffolds.Keys.Contains(t));
            
            foreach (var removed in removedTiles.ToList())
            {
                Destroy(_volatileScaffolds[removed].gameObject);
                _volatileScaffolds.Remove(removed);
            }
            
            foreach (var added in addedTiles.ToList())
            {
                var newScaffold = Instantiate(
                    original: scaffoldPrefab,
                    parent: scaffoldsContainer,
                    position: added.WorldPosition,
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
            
            var removedTiles = _persistentScaffolds.Keys.Where(t => !tiles.Contains(t));
            var addedTiles = tiles.Where(t => !_persistentScaffolds.Keys.Contains(t));
            
            foreach (var removed in removedTiles.ToList())
            {
                Destroy(_persistentScaffolds[removed].gameObject);
                _persistentScaffolds.Remove(removed);
            }
            
            foreach (var added in addedTiles.ToList())
            {
                var newScaffold = Instantiate(
                    original: scaffoldPrefab,
                    parent: scaffoldsContainer,
                    position: added.WorldPosition,
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
