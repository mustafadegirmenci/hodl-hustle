using UnityEngine;

namespace SunkCost.HH.Modules.EmployeeSystem
{
    public class EmployeeHiringUiHandler : MonoBehaviour
    {
        [SerializeField] private Transform cardContainer;
        [SerializeField] private EmployeeItemCard cardPrefab;
        
        private void Start()
        {
            EmployeeManager.instance.onEmployeeCreated.AddListener(HandleEmployeeCreated);
        }

        private void HandleEmployeeCreated(EmployeeData employeeData)
        {
            var card = Instantiate(cardPrefab, cardContainer);
            card.InitAsync(employeeData);
        }
    }
}
