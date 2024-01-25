using UnityEngine;

namespace SunkCost.HH.Modules.GridSystem
{
    public class GridInitializer : MonoBehaviour
    {
        [SerializeField] private GridTileHandler gridTileHandler;

        private void Awake()
        {
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    gridTileHandler.TryAddTile(new Vector3Int(i, 0, j));
                }
            }
        }
    }
}
