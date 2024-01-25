using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class ConstructionStateHandler : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<ConstructionState> onStateChanged = new();

        [SerializeField] private ConstructionManager constructionManager;
        
        public ConstructionState CurrentState
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
        
        private void Start()
        {
            CurrentState = ConstructionState.Passive;
            constructionManager.onConstructionStarted.AddListener(_ =>
            {
                CurrentState = ConstructionState.WaitingToSelectTilesToBeAdded;
            });
        }
        
        public void SwitchToAddTilesMode()
        {
            if (CurrentState is ConstructionState.WaitingToSelectTilesToBeRemoved)
            {
                CurrentState = ConstructionState.WaitingToSelectTilesToBeAdded;
            }
        }

        public void SwitchToRemoveTilesMode()
        {
            if (CurrentState is ConstructionState.WaitingToSelectTilesToBeAdded)
            {
                CurrentState = ConstructionState.WaitingToSelectTilesToBeRemoved;
            }
        }
    }
}
