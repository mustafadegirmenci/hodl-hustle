using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunkCost.HH.Modules.PrefabSystem
{
    [CreateAssetMenu(fileName = "Prefab Registry", menuName = "SunkCost/Prefab Management")]
    public class PrefabRegistry : ScriptableObject
    {
        public List<PrefabEntry> prefabs = new();

        public async UniTask<(bool, GameObject)> SpawnPrefab(string prefabId, Transform parent = null)
        {
            var assetReference = GetPrefab(prefabId);

            if (assetReference == null)
            {
                Debug.LogError($"Prefab with ID {prefabId} not found.");
                return (false, null);
            }

            var loadedAsset = await assetReference.InstantiateAsync(parent: parent).Task;
            return (true, loadedAsset.gameObject);
        }

        private AssetReferenceGameObject GetPrefab(string prefabId)
        {
            var entry = prefabs.Find(x => x.prefabId == prefabId);

            return entry?.assetReference;
        }
    }
}