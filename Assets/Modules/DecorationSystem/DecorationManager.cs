using SunkCost.HH.Modules.InputSystem;
using SunkCost.HH.Modules.Utility;
using UnityEngine;

namespace SunkCost.HH.Modules.DecorationSystem
{
    public class DecorationManager : MonoSingleton<DecorationManager>
    {
        [SerializeField] private Transform tester;
        [SerializeField] private Grid decorationGrid;
        [SerializeField] private Camera decorationCamera;
        [SerializeField] private LayerMask decorationLayerMask;

        public bool GetWorldHit(out Vector3 point)
        {
            var ray = decorationCamera.ScreenPointToRay(InputManager.instance.MousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, decorationLayerMask))
            {
                point = Vector3.zero;
                return false;
            }

            tester.position = hit.point;
            var offset = new Vector3(decorationGrid.cellSize.x / 2, 0, decorationGrid.cellSize.z / 2);
            var cell = decorationGrid.WorldToCell(hit.point + offset);
            point = decorationGrid.CellToWorld(cell);
            return true;
        }
    }
}