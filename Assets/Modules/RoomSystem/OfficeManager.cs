using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SunkCost.HH.Modules.InputSystem;
using SunkCost.HH.Modules.RoomSystem.Core;
using SunkCost.HH.Modules.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.RoomSystem
{
    public class OfficeManager : MonoSingleton<OfficeManager>
    {
        [SerializeField] private Button startRoomConstructionButton;
        [SerializeField] private Button endRoomConstructionButton;
        [SerializeField] private Button cancelRoomConstructionButton;
        [SerializeField] private Button switchToAddTileStateButton;
        [SerializeField] private Button switchToRemoveTileStateButton;
        
        [SerializeField] private GameObject officeTileIndicator;
        [SerializeField] private Grid officeGrid;
        [SerializeField] private LayerMask officeLayerMask;
        
        [SerializeField] private Transform initialTilesParent;
        
        private readonly Dictionary<(float, float), OfficeTile> _tiles = new();
        private readonly List<Room> _rooms = new();

        private RoomConstructionState _constructionState;
        private readonly List<(float, float)> _selectedTilePositions = new();
        private readonly List<(float, float)> _tempSelectedTilePositions = new();
        private (float, float)? _selectionStartCorner;
        private (float, float)? _selectionEndCorner;

        private Camera _mainCam;
        private Coroutine _roomConstructionRoutine;

        private Vector3 _cachedPointerToOfficePosition;
        private Vector3 _indicatorPosition;

        private void Awake()
        {
            _mainCam = Camera.main;
        }

        private void Start()
        {
            foreach (Transform initialTile in initialTilesParent)
            {
                var gridPos = officeGrid.CellToWorld(officeGrid.WorldToCell(initialTile.transform.position));
                _tiles.Add((gridPos.x, gridPos.z), initialTile.GetComponent<OfficeTile>());
            }
            Debug.Log($"Initialized the office with {_tiles.Count} tiles.");
        }

        private void OnEnable()
        {
            InputManager.instance.onMouseDown.AddListener(HandleMouseDown);
            InputManager.instance.onMouseUp.AddListener(HandleMouseUp);
            
            startRoomConstructionButton.onClick.AddListener(StartRoomConstruction);
            endRoomConstructionButton.onClick.AddListener(EndRoomConstruction);
            cancelRoomConstructionButton.onClick.AddListener(CancelRoomConstruction);
            switchToAddTileStateButton.onClick.AddListener(SwitchToAddTileState);
            switchToRemoveTileStateButton.onClick.AddListener(SwitchToRemoveTileState);
        }

        private void OnDisable()
        {
            InputManager.instance.onMouseDown.RemoveListener(HandleMouseDown);
            InputManager.instance.onMouseUp.RemoveListener(HandleMouseUp);
            
            startRoomConstructionButton.onClick.RemoveListener(StartRoomConstruction);
            endRoomConstructionButton.onClick.RemoveListener(EndRoomConstruction);
            cancelRoomConstructionButton.onClick.RemoveListener(CancelRoomConstruction);
            switchToAddTileStateButton.onClick.RemoveListener(SwitchToAddTileState);
            switchToRemoveTileStateButton.onClick.RemoveListener(SwitchToRemoveTileState);
        }

        private void StartRoomConstruction()
        {
            if (_roomConstructionRoutine != null)
            {
                return;
            }

            SwitchToAddTileState();
            _selectedTilePositions.Clear();

            _roomConstructionRoutine = StartCoroutine(nameof(RoomConstruction));
        }

        private void EndRoomConstruction()
        {
            if (_roomConstructionRoutine == null)
            {
                return;
            }

            StopCoroutine(_roomConstructionRoutine);
        }
        
        private void CancelRoomConstruction()
        {
            if (_roomConstructionRoutine == null)
            {
                return;
            }

            StopCoroutine(_roomConstructionRoutine);
        }
        
        private void SwitchToAddTileState()
        {
            _constructionState = RoomConstructionState.ConstructionAddTile;
            _tempSelectedTilePositions.Clear();
            Debug.Log("Switched to Add Tile state.");
        }
        
        private void SwitchToRemoveTileState()
        {
            _constructionState = RoomConstructionState.ConstructionRemoveTile;
            _tempSelectedTilePositions.Clear();
            Debug.Log("Switched to Remove Tile state.");
        }

        private void HandleMouseDown()
        {
            if (_roomConstructionRoutine == null)
            {
                return;
            }
            
            if (_constructionState is RoomConstructionState.ConstructionAddTile or RoomConstructionState.ConstructionRemoveTile)
            {
                _selectionStartCorner = (_indicatorPosition.x, _indicatorPosition.z);
            }
        }
        
        private void HandleMouseUp()
        {
            if (_roomConstructionRoutine == null)
            {
                return;
            }
            
            if (_constructionState is RoomConstructionState.ConstructionAddTile)
            {
                _selectionStartCorner = null;
                _selectionEndCorner = (_indicatorPosition.x, _indicatorPosition.z);
                
                foreach (var tempSelectedTilePosition in _tempSelectedTilePositions)
                {
                    if (!_selectedTilePositions.Contains(tempSelectedTilePosition))
                    {
                        _selectedTilePositions.Add(tempSelectedTilePosition);
                    }
                }
                _tempSelectedTilePositions.Clear();
            }
            else if (_constructionState is RoomConstructionState.ConstructionRemoveTile)
            {
                _selectionStartCorner = null;
                _selectionEndCorner = (_indicatorPosition.x, _indicatorPosition.z);
                
                foreach (var tempSelectedTilePosition in _tempSelectedTilePositions)
                {
                    if (_selectedTilePositions.Contains(tempSelectedTilePosition))
                    {
                        _selectedTilePositions.Remove(tempSelectedTilePosition);
                    }
                }
                _tempSelectedTilePositions.Clear();
            }
        }

        private IEnumerator RoomConstruction()
        {
            while (true)
            {
                var ray = _mainCam.ScreenPointToRay(InputManager.instance.MousePosition);
                if (Physics.Raycast(ray: ray, hitInfo: out var hit, maxDistance: Mathf.Infinity, layerMask: officeLayerMask))
                {
                    _cachedPointerToOfficePosition = hit.point;
                }

                var newIndicatorPosition = officeGrid.CellToWorld(officeGrid.WorldToCell(_cachedPointerToOfficePosition));
                // if (newIndicatorPosition != _indicatorPosition)
                // {
                //     _indicatorPosition = newIndicatorPosition;
                // }
                officeTileIndicator.transform.DOMove(newIndicatorPosition, 0.1f);

                if (InputManager.instance.mouseDragging)
                {
                    foreach (var tempSelectedTilePosition in _tempSelectedTilePositions)
                    {
                        if (_tiles.ContainsKey(tempSelectedTilePosition))
                        {
                            if (_constructionState is RoomConstructionState.ConstructionAddTile)
                            {
                                if (!_selectedTilePositions.Contains(tempSelectedTilePosition))
                                {
                                    _tiles[tempSelectedTilePosition].DeselectForConstruction();
                                }
                            }
                            else if (_constructionState is RoomConstructionState.ConstructionRemoveTile)
                            {
                                if (_selectedTilePositions.Contains(tempSelectedTilePosition))
                                {
                                    _tiles[tempSelectedTilePosition].SelectForConstruction();
                                }
                            }
                        }
                    }
                    _tempSelectedTilePositions.Clear();
                    
                    if (_selectionStartCorner.HasValue)
                    {
                        var startCorner = _selectionStartCorner.Value;
                        var endCorner = _indicatorPosition;

                        var minX = Mathf.Min(startCorner.Item1, endCorner.x);
                        var maxX = Mathf.Max(startCorner.Item1, endCorner.x);
                        var minZ = Mathf.Min(startCorner.Item2, endCorner.z);
                        var maxZ = Mathf.Max(startCorner.Item2, endCorner.z);

                        for (var x = minX; x <= maxX; x += officeGrid.cellSize.x)
                        {
                            for (var z = minZ; z <= maxZ; z += officeGrid.cellSize.z)
                            {
                                var tilePosition = (x, z);

                                if (_tempSelectedTilePositions.Contains(tilePosition)) continue;
                                
                                _tempSelectedTilePositions.Add(tilePosition);
                                if (_tiles.ContainsKey(tilePosition))
                                {
                                    if (_constructionState is RoomConstructionState.ConstructionAddTile)
                                    {
                                        if (_selectedTilePositions.Contains(tilePosition)) continue;
                                        _tiles[tilePosition].SelectForConstruction();
                                    }
                                    else if (_constructionState is RoomConstructionState.ConstructionRemoveTile)
                                    {
                                        if (!_selectedTilePositions.Contains(tilePosition)) continue;
                                        _tiles[tilePosition].DeselectForConstruction();
                                    }
                                }
                                
                            }
                        }
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        
    }
}
