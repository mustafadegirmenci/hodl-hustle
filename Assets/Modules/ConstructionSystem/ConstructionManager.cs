using System;
using System.Collections.Generic;
using SunkCost.HH.Modules.GridSystem;
using SunkCost.HH.Modules.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class ConstructionManager : MonoSingleton<ConstructionManager>
    {
        [HideInInspector] public UnityEvent<List<GridTile>> onConstructionStarted = new();
        [HideInInspector] public UnityEvent<List<GridTile>> onConstructionEnded = new();
        
        [SerializeField] private ConstructionStateHandler constructionStateHandler;
        [SerializeField] private ConstructionSelectionHandler constructionSelectionHandler;
        
        public bool StartConstruction(List<GridTile> initialTiles)
        {
            if (constructionStateHandler.CurrentState is not ConstructionState.Passive)
            {
                return false;
            }
            
            constructionStateHandler.CurrentState = ConstructionState.WaitingToSelectTilesToBeAdded;
            onConstructionStarted.Invoke(initialTiles);

            return true;
        }

        public bool EndConstruction()
        {
            if (constructionStateHandler.CurrentState is ConstructionState.Passive)
            {
                return false;
            }
            
            constructionStateHandler.CurrentState = ConstructionState.Passive;
            onConstructionEnded.Invoke(constructionSelectionHandler.PersistentSelection);
            
            return true;
        }
    }
}
