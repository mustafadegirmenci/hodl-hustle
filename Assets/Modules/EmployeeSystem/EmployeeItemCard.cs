using System.Collections.Generic;
using SunkCost.HH.Modules.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.EmployeeSystem
{
    public class EmployeeItemCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text ageText;
        [SerializeField] private TMP_Text roleText;
        [SerializeField] private Image image;

        [SerializeField] private List<GameObject> starFillers = new();

        public async void InitAsync(EmployeeData employeeData)
        {
            nameText.text = $"{employeeData.GetName()} {employeeData.GetSurname()}";
            ageText.text = employeeData.GetAge().ToString();
            roleText.text = employeeData.GetRole().ToString().SeparatePascalCaseWithSpaces();
            
            var startCount = Mathf.Clamp(employeeData.GetOverallQualityLevel() / 20, 0, 5);
            for (var i = 0; i < startCount; i++)
            {
                starFillers[i].SetActive(true);
            }
            
            image.sprite = await employeeData.GetSpriteAsync();
        }
    }
}