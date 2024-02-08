using System;
using Cysharp.Threading.Tasks.Triggers;
using Sunbox.Avatars;
using UnityEngine;

namespace SunkCost.HH.Modules.EmployeeSystem
{
    public class EmployeeCreationManager : MonoBehaviour
    {
        [SerializeField] private GameObject avatarPrefab;

        private GameObject avatarInstance;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SpawnEmployee();
            }
        }

        public void SpawnEmployee()
        {
            if (avatarInstance == null)
            {
                avatarInstance = Instantiate(avatarPrefab);
            }
            
            var avatar = avatarInstance.GetComponent<AvatarCustomization>();
            avatar.RandomizeBodyParameters(
                ignoreHeight: true,
                unifiedHairColors: true
            );
            avatar.RandomizeClothing();
        }
    }
}