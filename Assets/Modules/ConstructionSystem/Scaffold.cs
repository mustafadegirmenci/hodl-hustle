using UnityEngine;

namespace SunkCost.HH.Modules.ConstructionSystem
{
    public class Scaffold : MonoBehaviour
    {
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();
        }

        public void SetState(ScaffoldState state)
        {
            var material = _renderer.material;
            
            if (state is ScaffoldState.Persistent)
            {
                material.color = Color.black;
            }
            else if (state is ScaffoldState.SelectionToAdd)
            {
                material.color = Color.green;
            }
            else if (state is ScaffoldState.SelectionToRemove)
            {
                material.color = Color.red;
            }
        }
    }
}
