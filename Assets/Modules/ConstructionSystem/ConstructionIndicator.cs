using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class ConstructionIndicator : MonoBehaviour
    {
        [SerializeField] private ConstructionSelectionHandler constructionSelectionHandler;
        [SerializeField] private ConstructionStateHandler constructionStateHandler;
        [SerializeField] private Color addColor;
        [SerializeField] private Color removeColor;

        private List<Renderer> _renderers;
        private Tween _scaleTween;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>().ToList();
        }

        private void OnEnable()
        {
            _scaleTween = transform.DOScale(0.8f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
            _scaleTween.Kill();
            transform.localScale = new Vector3(1, 1, 1);
        }

        private void Start()
        {
            constructionSelectionHandler.onSelectionIndicatorChanged.AddListener(indicatorTile =>
            {
                transform.DOMove(indicatorTile.WorldPosition, 0.2f);
            });
            
            constructionStateHandler.onStateChanged.AddListener(constructionState =>
            {
                if (constructionState == ConstructionState.Passive)
                {
                    gameObject.SetActive(false);
                }
                else if (constructionState == ConstructionState.WaitingToSelectTilesToBeAdded)
                {
                    gameObject.SetActive(true);
                    _renderers.ForEach(r => r.material.color = addColor);
                }
                else if (constructionState == ConstructionState.WaitingToSelectTilesToBeRemoved)
                {
                    gameObject.SetActive(true);
                    _renderers.ForEach(r => r.material.color = removeColor);
                }
                else if (constructionState == ConstructionState.SelectingTilesToBeAdded)
                {
                    gameObject.SetActive(true);
                    _renderers.ForEach(r => r.material.color = addColor);
                }
                else if (constructionState == ConstructionState.SelectingTilesToBeRemoved)
                {
                    gameObject.SetActive(true);
                    _renderers.ForEach(r => r.material.color = removeColor);
                }
            });
            
            gameObject.SetActive(false);
        }
    }
}
