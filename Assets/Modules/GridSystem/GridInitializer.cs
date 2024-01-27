using UnityEngine;

namespace SunkCost.HH.Modules.GridSystem
{
    public class GridInitializer : MonoBehaviour
    {
        [SerializeField] private GridTileHandler gridTileHandler;
        [SerializeField] private float cellCount = 10;

        private void Awake()
        {
            for (var i = 0; i < cellCount; i++)
            {
                for (var j = 0; j < cellCount; j++)
                {
                    gridTileHandler.TryAddTile(new Vector3Int(i, 0, j));
                }
            }
        }
    }
}
