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
            InvokeRepeating(nameof(CreateRandomEmployee), 0f, 5f);
        }

        private void CreateRandomEmployee()
        {
            var customizationData = CharacterManager.instance.CreateRandomCustomizationData();

            var employeeData = new EmployeeData(customizationData);

            employeeRegistry.entries.Add(employeeData);
            onEmployeeCreated.Invoke(employeeData);
            
            Debug.Log("Employee Created");
        }
    }
}