using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SunkCost.HH.Modules.PhotographySystem
{
    [FilePath("Modules/PhotographySystem", FilePathAttribute.Location.PreferencesFolder)]
    public class PhotoRegistry : ScriptableSingleton<PhotoRegistry> 
    {
        private readonly Dictionary<string, Texture2D> _entries = new();

        public bool TryGetEntry(string key, out Texture2D texture)
        {
            return _entries.TryGetValue(key, out texture);
        }

        public bool TryAddEntry(string key, Texture2D texture)
        {
            return _entries.TryAdd(key, texture);
        }
    }
}
