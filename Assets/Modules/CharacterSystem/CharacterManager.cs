using AdvancedPeopleSystem;
using Cysharp.Threading.Tasks;
using SunkCost.HH.Modules.PrefabSystem;
using SunkCost.HH.Modules.Utility;
using UnityEngine;

namespace SunkCost.HH.Modules.CharacterSystem
{
    public class CharacterManager : MonoSingleton<CharacterManager>
    {
        public CustomizableCharacter dummyCharacter;
        
        [SerializeField] private Transform charactersContainer;

        public string CreateRandomCustomizationData()
        {
            return dummyCharacter
                .Randomize()
                .GetSetup()
                .Serialize(CharacterCustomizationSetup.CharacterFileSaveFormat.Json);
        }
        
        public async UniTask<CustomizableCharacter> SpawnCharacterFromDataAsync(string characterData)
        {
            var setup = CharacterCustomizationSetup.Deserialize(
                data: characterData, 
                format: CharacterCustomizationSetup.CharacterFileSaveFormat.Json);
            
            var (success, spawnedCharacterGo) = await PrefabManager.instance.prefabRegistry.SpawnPrefab(
                prefabId: "BaseCharacter", 
                parent: charactersContainer);
            
            var spawnedCharacter = spawnedCharacterGo.GetComponent<CustomizableCharacter>();
            setup.ApplyToCharacter(spawnedCharacter);
            
            return spawnedCharacter;
        }
    }
}
