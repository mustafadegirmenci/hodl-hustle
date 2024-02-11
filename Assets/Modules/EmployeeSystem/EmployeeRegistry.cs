using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SunkCost.HH.Modules.CharacterSystem;
using SunkCost.HH.Modules.PhotographySystem;
using UnityEngine;

namespace SunkCost.HH.Modules.EmployeeSystem
{
    [CreateAssetMenu(fileName = "Employee Registry", menuName = "SunkCost/Employee Management/Employee Registry")]
    public class EmployeeRegistry : ScriptableObject
    {
        public List<EmployeeData> entries = new();
    }

    [Serializable]
    public class EmployeeData
    {
        private string _id;
        private string _customizationData;

        public EmployeeData(string customizationData)
        {
            _id = Guid.NewGuid().ToString();
            _customizationData = customizationData;
        }

        public string GetId() => _id;
        
        public string GetCustomizationData() => _customizationData;

        //TODO: Add caching mechanism
        public async UniTask<Sprite> GetSpriteAsync()
        {
            var texture2D = await PhotoTaker.instance.TakePhoto(
                CharacterManager.instance.dummyCharacter.gameObject,
                from: PhotoDirection.Front,
                width: 200f,
                height: 200f,
                distance: 0.8f,
                cameraOffset: Vector3.up * 0.8f,
                lookAtOffset: Vector3.up * 0.8f
            );
            var sprite = PhotoConverter.instance.Texture2DToSprite(texture2D);

            return sprite;
        }
    }
}
