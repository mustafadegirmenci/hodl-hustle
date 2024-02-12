using System.Collections.Generic;
using UnityEngine;

namespace SunkCost.HH.Modules.EmployeeSystem
{
    [CreateAssetMenu(fileName = "Employee Registry", menuName = "SunkCost/Employee Management/Employee Registry")]
    public class EmployeeRegistry : ScriptableObject
    {
        public List<EmployeeData> entries = new();
    }
}
