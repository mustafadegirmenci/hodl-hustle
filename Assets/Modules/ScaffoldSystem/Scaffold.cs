using UnityEngine;

namespace SunkCost.HH.Modules.ScaffoldSystem
{
    public class Scaffold : MonoBehaviour
    {
        [SerializeField] private GameObject scaffold;
        [SerializeField] private GameObject selector;
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = selector.GetComponentInChildren<Renderer>();
        }

        public void SetState(ScaffoldState state)
        {
            var material = _renderer.material;
            
            if (state is ScaffoldState.Persistent)
            {
                scaffold.SetActive(true);
                selector.SetActive(false);
            }
            else if (state is ScaffoldState.SelectionToAdd)
            {
                scaffold.SetActive(false);
                selector.SetActive(true);
                material.color = new Color(0, 1f, 0, 1f);
            }
            else if (state is ScaffoldState.SelectionToRemove)
            {
                scaffold.SetActive(false);
                selector.SetActive(true);
                material.color = new Color(1f, 0, 0, 1f);
            }
        }
    }
}
