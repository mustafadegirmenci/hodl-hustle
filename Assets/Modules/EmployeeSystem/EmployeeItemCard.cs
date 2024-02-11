using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunkCost.HH.Modules.EmployeeSystem
{
    public class EmployeeItemCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image image;

        public async void InitAsync(EmployeeData employeeData)
        {
            nameText.text = $"Employee {Random.Range(1000, 9999)}";
            image.sprite = await employeeData.GetSpriteAsync();
        }
    }
}