using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunkCost.HH.Modules.DecorationSystem
{
    [CreateAssetMenu(fileName = "Decoration Item Registry", menuName = "SunkCost/Decoration Item Registry")]
    public class DecorationItemRegistry : ScriptableObject
    {
        public List<DecorationItemRegistryEntry> entries = new();
    }

    [Serializable]
    public class DecorationItemRegistryEntry
    {
        public string name;
        public int price;
        public Sprite image;
        public DecorationItem prefab;
    }
}
