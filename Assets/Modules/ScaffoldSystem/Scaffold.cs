using UnityEngine;

namespace SunkCost.HH.Modules.ScaffoldSystem
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
                material.color = new Color(0, 0, 0, 0.2f);
            }
            else if (state is ScaffoldState.SelectionToAdd)
            {
                material.color = new Color(0, 200, 0, 0.2f);
            }
            else if (state is ScaffoldState.SelectionToRemove)
            {
                material.color = new Color(200, 0, 0, 0.2f);
            }
        }
    }
}
