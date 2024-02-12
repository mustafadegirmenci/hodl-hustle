using System;
using SunkCost.HH.Modules.Utility;
using UnityEngine;
using Random = System.Random;

namespace SunkCost.HH.Modules.EmployeeSystem
{
    public class EmployeeNameGenerator : MonoSingleton<EmployeeNameGenerator>
    {
        private string[] _names;
        private string[] _surnames;

        public override void Init()
        {
            _names = Resources.Load<TextAsset>("names").text.Split('\n');
            _surnames = Resources.Load<TextAsset>("surnames").text.Split('\n');

            if (_names.Length == 0 || _surnames.Length == 0)
            {
                Console.WriteLine("Error: Empty CSV files.");
            }
        }
        
        public string GenerateRandomName()
        {
            var rand = new Random();
            var randomName = _names[rand.Next(_names.Length)].Trim();

            Debug.Log(randomName);
            Debug.Log(randomName.Length);
            return randomName;
        }

        public string GenerateRandomSurname()
        {
            var rand = new Random();
            var randomSurname = _surnames[rand.Next(_surnames.Length)].Trim();

            return randomSurname;
        }

    }
}
