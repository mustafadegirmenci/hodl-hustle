using SunkCost.HH.Modules.CharacterSystem;
using SunkCost.HH.Modules.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace SunkCost.HH.Modules.EmployeeSystem
{
    public class EmployeeManager : MonoSingleton<EmployeeManager>
    {
        public EmployeeRegistry employeeRegistry;

        [HideInInspector] public UnityEvent<EmployeeData> onEmployeeCreated = new();

        private void Start()
        {
            InvokeRepeating(nameof(CreateRandomEmployee), 2, Random.Range(5, 7));
        }

        private void CreateRandomEmployee()
        {
            var customizationData = CharacterManager.instance.CreateRandomCustomizationData();

            var employeeData = new EmployeeData(customizationData, Random.Range(10, 90));

            employeeRegistry.entries.Add(employeeData);
            onEmployeeCreated.Invoke(employeeData);
            
            Debug.Log("Employee Created");
        }
    }
}