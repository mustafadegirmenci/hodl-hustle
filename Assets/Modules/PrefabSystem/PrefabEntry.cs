using UnityEngine.AddressableAssets;

namespace SunkCost.HH.Modules.PrefabSystem
{
    [System.Serializable]
    public class PrefabEntry
    {
        public string prefabId;
        public AssetReferenceGameObject assetReference;

        public PrefabEntry(string prefabId, AssetReferenceGameObject assetReference)
        {
            this.prefabId = prefabId;
            this.assetReference = assetReference;
        }
    }
}