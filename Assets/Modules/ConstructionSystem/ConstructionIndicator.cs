using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class ConstructionIndicator : MonoBehaviour
    {
        [SerializeField] private ConstructionManager constructionManager;

        private List<Renderer> _renderers;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>().ToList();
        }

        private void Start()
        {
            constructionManager.onIndicatorPositionChanged.AddListener(pos =>
            {
                transform.DOMove(pos, 0.2f);
            });
            
            constructionManager.onStateChanged.AddListener(constructionState =>
            {
                if (constructionState == ConstructionState.Passive)
                {
                    gameObject.SetActive(false);
                }
                else if (constructionState == ConstructionState.WaitingToSelectTilesToBeAdded)
                {
                    gameObject.SetActive(true);
                    _renderers.ForEach(r => r.material.color = Color.green);
                }
                else if (constructionState == ConstructionState.WaitingToSelectTilesToBeRemoved)
                {
                    gameObject.SetActive(true);
                    _renderers.ForEach(r => r.material.color = Color.red);
                }
                else if (constructionState == ConstructionState.SelectingTilesToBeAdded)
                {
                    gameObject.SetActive(true);
                    _renderers.ForEach(r => r.material.color = Color.green);
                }
                else if (constructionState == ConstructionState.SelectingTilesToBeRemoved)
                {
                    gameObject.SetActive(true);
                    _renderers.ForEach(r => r.material.color = Color.red);
                }
            });
            
            gameObject.SetActive(false);
        }
    }
}
