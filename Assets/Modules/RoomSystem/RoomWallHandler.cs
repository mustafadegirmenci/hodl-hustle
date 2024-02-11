using System.Collections.Generic;
using System.Linq;
using SunkCost.HH.Modules.WallSystem;
using UnityEngine;

namespace SunkCost.HH.Modules.RoomSystem
{
    public class RoomWallHandler : MonoBehaviour
    {
        public readonly List<GameObject> WallObjects = new();
        public readonly Dictionary<WallCode, GameObject> WallPrefabs = new();
        
        [SerializeField] private Transform wallsContainer;
        [SerializeField] private GameObject farLeftInnerPrefab;
        [SerializeField] private GameObject farLeftOuterPrefab;
        [SerializeField] private GameObject farNearLeftBiasedPrefab;
        [SerializeField] private GameObject farNearRightBiasedPrefab;
        [SerializeField] private GameObject farRightInnerPrefab;
        [SerializeField] private GameObject farRightOuterPrefab;
        [SerializeField] private GameObject leftRightFarBiasedPrefab;
        [SerializeField] private GameObject leftRightNearBiasedPrefab;
        [SerializeField] private GameObject nearLeftInnerPrefab;
        [SerializeField] private GameObject nearLeftOuterPrefab;
        [SerializeField] private GameObject nearRightInnerPrefab;
        [SerializeField] private GameObject nearRightOuterPrefab;

        private void Awake()
        {
            InitializeWallPrefabs();
        }

        public void SpawnWall(WallCode code, Vector3 worldPosition)
        {
            if (WallPrefabs.TryGetValue(code, out var prefab))
            {
                WallObjects.Add(Instantiate(prefab, worldPosition, Quaternion.identity, wallsContainer));
            }
        }

        public void ClearWalls()
        {
            foreach (var wall in WallObjects.ToList())
            {
                Destroy(wall);
                WallObjects.Remove(wall);
            }
        }
        
        private void InitializeWallPrefabs()
        {
            WallPrefabs[WallCode.NearLeft] = nearLeftInnerPrefab;
            WallPrefabs[WallCode.FarRight | WallCode.NearRight | WallCode.FarLeft] = nearLeftOuterPrefab;
            
            WallPrefabs[WallCode.NearRight] = nearRightInnerPrefab;
            WallPrefabs[WallCode.NearLeft | WallCode.FarLeft | WallCode.FarRight] = nearRightOuterPrefab;
            
            WallPrefabs[WallCode.FarLeft] = farLeftInnerPrefab;
            WallPrefabs[WallCode.NearLeft | WallCode.NearRight | WallCode.FarRight] = farLeftOuterPrefab;
            
            WallPrefabs[WallCode.FarRight] = farRightInnerPrefab;
            WallPrefabs[WallCode.NearLeft | WallCode.NearRight | WallCode.FarLeft] = farRightOuterPrefab;
            
            WallPrefabs[WallCode.NearLeft | WallCode.NearRight] = leftRightNearBiasedPrefab;
            WallPrefabs[WallCode.FarLeft | WallCode.FarRight] = leftRightFarBiasedPrefab;
            
            WallPrefabs[WallCode.NearLeft | WallCode.FarLeft] = farNearLeftBiasedPrefab;
            WallPrefabs[WallCode.NearRight | WallCode.FarRight] = farNearRightBiasedPrefab;
        }
    }
}
